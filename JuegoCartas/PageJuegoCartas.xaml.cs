
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AprendeJugando
{
    public partial class PageJuegoCartas : Page
    {
        private const int MaxEstrellas = 10;
        private const int Nivel2Puntos = 5;
        private const int CartasNivel1 = 4;
        private const int CartasNivel2 = 6;

        private int estrellas = 0;
        private int nivel = 1;
        private int puntos = 0;
        private int clics = 0;
        private bool bloqueo = false;

        private List<string> cartasAleatorias;
        private readonly List<Button> botonesCartas = new();
        private readonly List<Button> cartasSeleccionadas = new();
        private readonly List<string> imagenesSeleccionadas = new();

        public PageJuegoCartas()
        {
            InitializeComponent();
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoCartas.mp4");
            MostrarVentanaJuego(ventana);
            CargarNuevasCartas();
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
        }

        private void Btn_hover(object sender, MouseEventArgs e)
        {
            SonidoManager.Instance.ReproducirSonidoHover("/Sounds/pasarporEncima.mp3");
        }

        private void AnimacionRebote(Button boton)
        {
            var scale = new ScaleTransform(1, 1);
            boton.RenderTransform = scale;
            boton.RenderTransformOrigin = new Point(0.5, 0.5);

            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500))
            {
                EasingFunction = new BounceEase { Bounces = 2, Bounciness = 2 }
            };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private void Carta_Click(object sender, RoutedEventArgs e)
        {
            if (bloqueo || sender is not Button btn || !int.TryParse(btn.Tag.ToString(), out int idx))
                return;

            if (cartasSeleccionadas.Contains(btn))
                return;

            clics++;
            cartasSeleccionadas.Add(btn);
            imagenesSeleccionadas.Add(cartasAleatorias[idx]);

            AnimacionVoltearCarta(btn, cartasAleatorias[idx]);
            SonidoManager.Instance.ReproducirSonidoHover("Sounds/SonidoCartas.mp3");

            if ((nivel == 1 && clics == 2) || (nivel >= 2 && clics == 3))
            {
                bloqueo = true;
                _ = CompararCartas();
            }
        }


        private async Task CompararCartas()
        {
            try
            {
                bool iguales = imagenesSeleccionadas.All(img => img == imagenesSeleccionadas[0]);

                if (iguales)
                {
                    estrellas++;
                    puntos++;
                    new LiteDbService().RegistrarAcierto(SesionActual.PadreId, "Cartas", nivel);


                    if (estrellas < MaxEstrellas && (puntos < Nivel2Puntos || nivel > 1))
                    {
                        await Task.Delay(500);
                        NotificarEstrella();
                        bloqueo = false;
                    }

                    if (puntos >= Nivel2Puntos && nivel == 1)
                    {
                        nivel++;
                        await Task.Delay(500);
                        textoNivel.Text = "NIVEL 2";
                        MostrarNivelSuperado();
                    }

                    if (estrellas >= MaxEstrellas && nivel == 2)
                    {
                        await Task.Delay(500);
                        MostrarJuegoFinalizado();
                        NavigationService.Navigate(new PageJugar());
                        return;
                    }

                    await Task.Delay(2000);
                    CargarNuevasCartas();
                }
                else
                {
                    await Task.Delay(500);
                    SonidoManager.Instance.ReproducirSonido("Sounds/sonidoError.mp3");
                    SonidoManager.Instance.ReproducirSonido("Sounds/intentalo.wav");
                    MostrarNotificacion("¡Inténtalo de nuevo!", "/Images/Imagenes/Personaje/duda.png");
                    AnimacionSacudirCartas();
                    RevertirCartas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en CompararCartas: " + ex.Message);
            }
        }

        private void MostrarNivelSuperado()
        {
            var ventana = new ventanaNotificacionNivel();
            SonidoManager.Instance.ReproducirSonido("Sounds/subirNivel.wav");
            ventana.Topmost = true;
            ventana.ShowDialog();
        }

        private void MostrarJuegoFinalizado()
        {
            var ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto("¡Felicidades, has completado el juego!");
            ventana.CambiarImagen("/Images/Imagenes/Personaje/alegre.png", 200, 200);
            SonidoManager.Instance.ReproducirSonido("Sounds/juegoCompletado.wav");
            SonidoManager.Instance.ReproducirSonido("Sounds/aplausos.mp3");

            ventana.Topmost = true;
            ventana.ShowDialog();
        }

        private void NotificarEstrella()
        {
            SonidoManager.Instance.ReproducirSonido("Sounds/sonidoPunto.wav");
            SonidoManager.Instance.ReproducirSonido("Sounds/vozEstrella.wav");
            MostrarNotificacion($"¡Felicidades, has ganado una estrella!\nTotal de estrellas: {estrellas}",
                "/Images/Imagenes/estrella.png");
        }

        private void ResetCartasTemporales()
        {
            clics = 0;
            bloqueo = false;
            cartasSeleccionadas.Clear();
            imagenesSeleccionadas.Clear();
        }

        private void configurarPosicionCartas()
        {
            Canvas.SetLeft(CartaVacia1, 402);
            Canvas.SetTop(CartaVacia1, 136);
            Canvas.SetLeft(CartaVacia2, 402);
            Canvas.SetTop(CartaVacia2, 545);
            Canvas.SetLeft(CartaVacia3, 825);
            Canvas.SetTop(CartaVacia3, 136);
            Canvas.SetLeft(CartaVacia4, 825);
            Canvas.SetTop(CartaVacia4, 545);
        }

        private void InicializarCarta(Button btn, int index)
        {
            btn.Tag = index.ToString();
            btn.Content = new Image
            {
                Source = new BitmapImage(new Uri("/Images/Imagenes/Cartas/cartaVacia.png", UriKind.Relative)),
                Stretch = Stretch.UniformToFill
            };
            btn.IsEnabled = true;
            AnimacionRebote(btn);
        }

        private void CargarNuevasCartas()
        {
            try
            {
                cartasAleatorias = Cartas.ObtenerCartasAleatorias(nivel);
                int cantidad = (nivel == 1) ? CartasNivel1 : CartasNivel2;

                botonesCartas.Clear();
                botonesCartas.AddRange(new[] { CartaVacia1, CartaVacia2, CartaVacia3, CartaVacia4 });
                if (nivel >= 2)
                {
                    botonesCartas.Add(CartaVacia5);
                    botonesCartas.Add(CartaVacia6);
                    configurarPosicionCartas();
                    CartaVacia5.Visibility = Visibility.Visible;
                    CartaVacia6.Visibility = Visibility.Visible;
                }

                for (int i = 0; i < cantidad; i++)
                {
                    InicializarCarta(botonesCartas[i], i);
                }

                SonidoManager.Instance.ReproducirSonido("Sounds/cartasRevertidas.mp3");

                ResetCartasTemporales();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al cargar nuevas cartas: " + e.Message);
            }
        }

        private void MostrarNotificacion(string mensaje, string ruta)
        {
            var ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto(mensaje);
            ventana.CambiarImagen(ruta, 200, 200);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void RevertirCartas()
        {
            botonesCartas.ForEach(b => b.IsEnabled = false);

            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            timer.Tick += (s, e) =>
            {
                foreach (var boton in cartasSeleccionadas)
                {
                    boton.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("/Images/Imagenes/Cartas/cartaVacia.png", UriKind.Relative)),
                        Stretch = Stretch.UniformToFill
                    };
                }

                botonesCartas.ForEach(b => b.IsEnabled = true);
                ResetCartasTemporales();
                timer.Stop();
            };

            timer.Start();
        }

        private void AnimacionVoltearCarta(Button carta, string img)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, e) =>
            {
                carta.Content = new Image
                {
                    Source = new BitmapImage(new Uri(img, UriKind.Relative)),
                    Stretch = Stretch.UniformToFill
                };
                carta.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            };
            carta.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void AnimacionSacudirCartas()
        {
            foreach (var boton in cartasSeleccionadas)
            {
                var anim = new DoubleAnimation(-5, 5, TimeSpan.FromMilliseconds(50))
                {
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(5)
                };

                var transform = new TranslateTransform();
                boton.RenderTransform = new TransformGroup { Children = { transform } };
                transform.BeginAnimation(TranslateTransform.XProperty, anim);
            }
        }

        private void BtnComoseJuega_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoCartas.mp4");

            MostrarVentanaJuego(ventana);
        }
    }
}