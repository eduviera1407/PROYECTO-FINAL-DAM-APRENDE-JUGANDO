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
    /// <summary>
    /// Lógica de interacción para PageMatesLevel3.xaml
    /// </summary>
    public partial class PageMatesLevel3 : Page
    {
        private Popup dragPopup;
        private DispatcherTimer ghostTimer;
        private int aciertos = 0;
        private int nivelActual = 1;
        private Image dragGhostImage;

        public PageMatesLevel3()
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoMatesNivel3.mp4");
            MostrarVentanaJuego(ventana);
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private void Numero_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Image img && e.LeftButton == MouseButtonState.Pressed)
            {
                string numero = img.Tag?.ToString();
                if (string.IsNullOrEmpty(numero)) return;

                ghostTimer ??= new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
                ghostTimer.Tick += GhostTimer_Tick;

                ShowDragGhost(numero, e.GetPosition(this));
                ghostTimer.Start();

                DragDrop.DoDragDrop(img, numero, DragDropEffects.Move);

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

        private void ShowDragGhost(string numero, Point position)
        {
            dragGhostImage ??= new Image
            {
                Width = 150,
                Height = 150,
                Opacity = 0.8
            };

            dragPopup ??= new Popup
            {
                AllowsTransparency = true,
                Placement = PlacementMode.Absolute,
                IsHitTestVisible = false,
                Child = dragGhostImage
            };

            dragGhostImage.Source = new BitmapImage(new Uri($"Images/Imagenes/JuegoMates/imagenesmanos/{numero}.png",
                UriKind.Relative));
            dragPopup.HorizontalOffset = position.X;
            dragPopup.VerticalOffset = position.Y;
            dragPopup.IsOpen = true;
        }

        private void MostrarSegundoGrupo()
        {
            nivelActual = 2;
            aciertos = 0;

            OcultarElementosGrupo(1);
            MostrarElementosGrupo(2);

            // Mensaje motivacional
            SonidoManager.Instance.ReproducirSonido("Sounds/vamosConLosSiguientes.wav");
            SonidoManager.Instance.ReproducirSonido("Sounds/sonidoPunto.wav");

            var ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto("¡Muy bien! Ahora vamos con los siguientes.");
            ventana.CambiarImagen("/Images/Imagenes/Personaje/alegre.png", 200, 200);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void OcultarElementosGrupo(int grupo)
        {
            foreach (var img in fondo.Children.OfType<Image>()
                         .Where(i => i.Tag?.ToString() is string tag && int.TryParse(tag, out int n) &&
                                     n >= grupo * 5 - 4 && n <= grupo * 5))
                img.Visibility = Visibility.Collapsed;

            foreach (var zona in fondo.Children.OfType<Border>()
                         .Where(b => b.Name.StartsWith("dropZona") && int.TryParse(b.Name.Substring(8), out int n) &&
                                     n >= grupo * 5 - 4 && n <= grupo * 5))
                zona.Visibility = Visibility.Collapsed;

            foreach (var txt in fondo.Children.OfType<TextBlock>()
                         .Where(tb =>
                             int.TryParse(tb.Text, out int n) && n >= grupo * 5 - 4 && n <= grupo * 5 &&
                             Canvas.GetTop(tb) < 300))
                txt.Visibility = Visibility.Collapsed;
        }

        private void MostrarElementosGrupo(int grupo)
        {
            foreach (var img in fondo.Children.OfType<Image>()
                         .Where(i => i.Tag?.ToString() is string tag && int.TryParse(tag, out int n) &&
                                     n >= grupo * 5 - 4 && n <= grupo * 5))
                img.Visibility = Visibility.Visible;

            foreach (var zona in fondo.Children.OfType<Border>()
                         .Where(b => b.Name.StartsWith("dropZona") && int.TryParse(b.Name.Substring(8), out int n) &&
                                     n >= grupo * 5 - 4 && n <= grupo * 5))
                zona.Visibility = Visibility.Visible;

            foreach (var txt in fondo.Children.OfType<TextBlock>()
                         .Where(tb =>
                             int.TryParse(tb.Text, out int n) && n >= grupo * 5 - 4 && n <= grupo * 5 &&
                             Canvas.GetTop(tb) < 300))
                txt.Visibility = Visibility.Visible;
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
                if (sender is Border zona)
                {
                    string zonaEsperada = zona.Name.Replace("dropZona", "");

                    if (numero == zonaEsperada)
                    {
                        zona.Child = new Image
                        {
                            Source = new BitmapImage(new Uri($"/Images/Imagenes/JuegoMates/imagenesmanos/{numero}.png",
                                UriKind.Relative)),
                            Stretch = Stretch.Uniform
                        };

                        foreach (var img in fondo.Children.OfType<Image>())
                        {
                            if (img.Tag?.ToString() == numero)
                            {
                                img.Visibility = Visibility.Collapsed;
                                break;
                            }
                        }

                        SonidoManager.Instance.ReproducirSonido("Sounds/sonidoPunto.wav");
                        aciertos++;

                        if (aciertos == 5)
                        {
                            if (nivelActual == 1)
                                MostrarSegundoGrupo();
                            else
                                MostrarNivelCompletado();
                        }
                    }
                    else
                    {
                        SonidoManager.Instance.ReproducirSonido("Sounds/intentalo.wav");

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

        private void MostrarVentanaJuego(Window ventana)
        {
            ventana.WindowState = WindowState.Normal;
            ventana.Topmost = true;
            ventana.Left = (SystemParameters.PrimaryScreenWidth - ventana.Width) / 2;
            ventana.Top = (SystemParameters.PrimaryScreenHeight - ventana.Height) / 2;
            ventana.ShowDialog();
        }

        private void MostrarNivelCompletado()
        {
            SonidoManager.Instance.ReproducirSonido("Sounds/aplausos.mp3");
            SonidoManager.Instance.ReproducirSonido("Sounds/nivelCompletado.wav");

            if (SesionActual.PadreAutenticado)
            {
                var db = new LiteDbService();
                db.RegistrarAcierto(SesionActual.PadreId, "Matematicas", 3, 1);
            }

            var ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto("🎉 ¡Has completado el nivel! 🎉");
            ventana.CambiarImagen("/Images/Imagenes/Personaje/alegre.png", 200, 200);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
            NavigationService.Navigate(new PageMatesLevel4());
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
            NavigationService.Navigate(new PageMatesLevel2());
        }

        private void pasarNivel(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageMatesLevel4());
        }

        private void BtnComoseJuega_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoMatesNivel3.mp4");
            MostrarVentanaJuego(ventana);
        }
    }
}