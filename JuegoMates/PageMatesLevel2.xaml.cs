using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace AprendeJugando.JuegoMates
{
    public partial class PageMatesLevel2 : Page
    {
        private Popup dragPopup;
        private TextBlock dragGhost;
        private DispatcherTimer ghostTimer;
        private int aciertos = 0;
        private int nivelActual = 1;


        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public PageMatesLevel2()
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoMatesNivel2.mp4");
            MostrarVentanaJuego(ventana);
            InitializeComponent();
        }

        private void Numero_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb && e.LeftButton == MouseButtonState.Pressed)
            {
                ghostTimer ??= new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
                ghostTimer.Tick += GhostTimer_Tick;

                ShowDragGhost(tb.Text, e.GetPosition(this));
                ghostTimer.Start();

                DragDrop.DoDragDrop(tb, tb.Text, DragDropEffects.Move);

                ghostTimer.Stop();
                HideDragGhost();
            }
        }

        private void GhostTimer_Tick(object sender, EventArgs e)
        {
            if (dragPopup?.IsOpen == true)
            {
                GetCursorPos(out POINT point);
                Point wpfPoint = PointFromScreen(new Point(point.X, point.Y));

                dragPopup.HorizontalOffset = wpfPoint.X - 50;
                dragPopup.VerticalOffset = wpfPoint.Y - 50;
            }
        }

        private void ShowDragGhost(string text, Point position)
        {
            dragGhost ??= new TextBlock
            {
                FontSize = 100,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Assets/#LazyDog"),
                Foreground = new SolidColorBrush(Color.FromRgb(244, 202, 68)),
                Opacity = 0.8
            };

            dragPopup ??= new Popup
            {
                AllowsTransparency = true,
                Placement = PlacementMode.Absolute,
                IsHitTestVisible = false,
                Child = dragGhost
            };

            dragGhost.Text = text;
            dragPopup.HorizontalOffset = position.X;
            dragPopup.VerticalOffset = position.Y;
            dragPopup.IsOpen = true;
        }

        private void HideDragGhost()
        {
            if (dragPopup != null)
                dragPopup.IsOpen = false;
        }

        private void ZonaDrop_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.Text) ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        private void ZonaDrop_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string numero = (string)e.Data.GetData(DataFormats.Text);
                Border zona = sender as Border;

                if (zona != null)
                {
                    // Detectar zona esperada a partir del nombre
                    string zonaEsperada = zona.Name.Replace("dropZona", "");

                    if (numero == zonaEsperada)
                    {
                        // Mostrar imagen correcta en la zona
                        zona.Child = new Image
                        {
                            Source = new BitmapImage(new Uri($"/Images/Imagenes/JuegoMates/imagenesmanos/{numero}.png",
                                UriKind.Relative)),
                            Stretch = Stretch.Uniform
                        };


                        foreach (var child in fondo.Children.OfType<TextBlock>())
                        {
                            if (child.Text == numero &&
                                Canvas.GetTop(child) > 500) // solo los que están en la parte baja del canvas
                            {
                                child.Visibility = Visibility.Collapsed;
                                break;
                            }
                        }


                        SonidoManager.Instance.ReproducirSonido("Sounds/sonidoPunto.wav");
                        aciertos++;

                        // Si se completan 5 aciertos en el grupo 1, pasamos al grupo 2
                        if (aciertos == 5 && nivelActual == 1)
                        {
                            MostrarSegundoGrupo();
                        }
                        // Si se completan 5 aciertos en el grupo 2, finalizamos el nivel
                        else if (aciertos == 5 && nivelActual == 2)
                        {
                            MostrarNivelCompletado();
                        }
                    }
                    else
                    {
                        SonidoManager.Instance.ReproducirSonido("Sounds/intentalo.wav");
                        SonidoManager.Instance.ReproducirSonido("Sounds/sonidoError.mp3");

                        var ventana = new ventanaNotificacionNivel();
                        ventana.CambiarTexto("¡Incorrecto! Intenta de nuevo.");
                        ventana.CambiarImagen("/Images/Imagenes/Personaje/duda.png", 200, 200);
                        ventana.Owner = Window.GetWindow(this);
                        ventana.ShowDialog();
                    }
                }

                e.Handled = true;
            }
        }

        private void MostrarSegundoGrupo()
        {
            nivelActual = 2;
            aciertos = 0;

            // Ocultar del grupo 1
            foreach (var text in fondo.Children.OfType<TextBlock>()
                         .Where(tb => int.TryParse(tb.Text, out int n) && n >= 1 && n <= 5))
                text.Visibility = Visibility.Collapsed;

            foreach (var zona in fondo.Children.OfType<Border>().Where(b =>
                         b.Name.StartsWith("dropZona") && int.TryParse(b.Name.Substring(8), out int n) && n >= 1 &&
                         n <= 5))
                zona.Visibility = Visibility.Collapsed;

            // Mostrar grupo 2
            foreach (var text in fondo.Children.OfType<TextBlock>()
                         .Where(tb => int.TryParse(tb.Text, out int n) && n >= 6 && n <= 10))
                text.Visibility = Visibility.Visible;

            foreach (var zona in fondo.Children.OfType<Border>().Where(b =>
                         b.Name.StartsWith("dropZona") && int.TryParse(b.Name.Substring(8), out int n) && n >= 6 &&
                         n <= 10))
                zona.Visibility = Visibility.Visible;

            // Opcional: mensaje motivacional
            var ventana = new ventanaNotificacionNivel();
            SonidoManager.Instance.ReproducirSonido("Sounds/vamosConLosSiguientes.wav");
            SonidoManager.Instance.ReproducirSonido("Sounds/sonidoPunto.wav");
            ventana.CambiarTexto("¡Muy bien! Vamos con los siguientes.");
            ventana.CambiarImagen("/Images/Imagenes/Personaje/alegre.png", 200, 200);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void MostrarNivelCompletado()
        {
            SonidoManager.Instance.ReproducirSonido("Sounds/aplausos.mp3");
            SonidoManager.Instance.ReproducirSonido("Sounds/nivelCompletado.wav");

            if (SesionActual.PadreAutenticado)
            {
                var db = new LiteDbService();
                db.RegistrarAcierto(SesionActual.PadreId, "Matematicas", 2, 1);
            }

            var ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto("🎉 ¡Has completado el nivel! 🎉");
            ventana.CambiarImagen("/Images/Imagenes/Personaje/alegre.png", 200, 200);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
            NavigationService.Navigate(new PageMatesLevel3());
        }


        private void MostrarVentanaJuego(Window ventana)
        {
            ventana.WindowState = WindowState.Normal;
            ventana.Topmost = true;
            ventana.Left = (SystemParameters.PrimaryScreenWidth - ventana.Width) / 2;
            ventana.Top = (SystemParameters.PrimaryScreenHeight - ventana.Height) / 2;
            ventana.ShowDialog();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageJugar());
            SonidoManager.Instance.DetenerTodosLosSonidos();
        }

        private void BtnHover(object sender, MouseEventArgs e)
        {
            SonidoManager.Instance.ReproducirSonidoHover("Sounds/pasarporEncima.mp3");
        }

        private void volverNivel(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageMates());
        }

        private void pasarNivel(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageMatesLevel3());
        }

        private void BtnComoseJuega_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoMatesNivel2.mp4");
            MostrarVentanaJuego(ventana);
        }
    }
}