using AprendeJugando.JuegoAnimales;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AprendeJugando
{
    public partial class PageJuegoAnimales : Page
    {
        private Animales juegoAnimales;
        private List<(string Nombre, string ImagenPath, string SonidoPath)> animalesSeleccionados;
        private int estrellas = 0;
        private const int ACIERTOS_NIVEL2 = 5;
        private const int ESTRELLAS_TOTALES_FIN_JUEGO = 10;

        public PageJuegoAnimales()
        {
            InitializeComponent();
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoAnimales.mp4");
            MostrarVentanaJuego(ventana);
            _ = IniciarJuego();
        }

        private async Task IniciarJuego()
        {
            try
            {
                juegoAnimales = new Animales();
                animalesSeleccionados = juegoAnimales.ObtenerAnimalesAleatorios();

                var botones = new List<Button> { animal1, animal2, animal3, animal4 };
                for (int i = 0; i < botones.Count; i++)
                {
                    AsignarImagen(botones[i], animalesSeleccionados[i].ImagenPath);
                    AnimacionRebote(botones[i]);
                }

                SonidoManager.Instance.ReproducirSonido("Sounds/cartasRevertidas.mp3");


                if (estrellas >= ACIERTOS_NIVEL2)
                {
                    AjustarPoscionesAnimales();
                    animal5.Visibility = Visibility.Visible;
                    animal6.Visibility = Visibility.Visible;

                    var animalesNivel2 = juegoAnimales.ListaAnimales
                        .Where(a => !animalesSeleccionados.Contains(a))
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(2)
                        .ToList();

                    animalesSeleccionados.AddRange(animalesNivel2);

                    AsignarImagen(animal5, animalesSeleccionados[4].ImagenPath);
                    AsignarImagen(animal6, animalesSeleccionados[5].ImagenPath);
                    AnimacionRebote(animal5);
                    AnimacionRebote(animal6);
                }
                else
                {
                    animal5.Visibility = Visibility.Collapsed;
                    animal6.Visibility = Visibility.Collapsed;
                }

                juegoAnimales.SeleccionarAnimalCorrecto(animalesSeleccionados);

                await ReproducirSonidos();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ha ocurrido un error al iniciar el juego: {e.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task ReproducirSonidos()
        {
            SonidoManager.Instance.ReproducirSonidoAnimales("Sounds/SoundsAnimals/preguntaSonidoAnimal.wav");
            await Task.Delay(2500);
            SonidoManager.Instance.ReproducirSonidoAnimales(AppDomain.CurrentDomain.BaseDirectory +
                                                            juegoAnimales.AnimalCorrecto.SonidoPath);
        }

        private void AsignarImagen(Button boton, string rutaImagen)
        {
            boton.Content = new Image
            {
                Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative)),
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }


        private void Animal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botonPresionado || botonPresionado.Content is not Image img) return;

            string nombreAnimalSeleccionado = animalesSeleccionados
                .FirstOrDefault(a => img.Source.ToString().Contains(a.ImagenPath)).Nombre;

            if (nombreAnimalSeleccionado == juegoAnimales.AnimalCorrecto.Nombre)
            {
                ManejarAcierto();
                AnimacionAgrandar(botonPresionado);
            }
            else
            {
                AnimacionSacudida(botonPresionado);
                ManejarError();
            }
        }

        private void AnimacionAgrandar(Button boton)
        {
            var scaleTransform = new ScaleTransform(1, 1);
            boton.RenderTransform = scaleTransform;
            boton.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 1,
                To = 1.3,
                Duration = TimeSpan.FromMilliseconds(300),
                AutoReverse = true,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }


        private void AjustarPoscionesAnimales()
        {
            Canvas.SetLeft(animal1, 275);
            Canvas.SetTop(animal1, 197);
            Canvas.SetLeft(animal2, 817);
            Canvas.SetTop(animal2, 197);
            Canvas.SetLeft(animal3, 275);
            Canvas.SetTop(animal3, 618);
            Canvas.SetLeft(animal4, 817);
            Canvas.SetTop(animal4, 618);
        }

        private void ManejarAcierto()
        {
            SonidoManager.Instance.DetenerTodosLosSonidos();
            estrellas++;

            if (estrellas >= ESTRELLAS_TOTALES_FIN_JUEGO)
            {
                SonidoManager.Instance.ReproducirSonido("Sounds/juegoCompletado.wav");
                SonidoManager.Instance.ReproducirSonido("Sounds/aplausos.mp3");

                MostrarNotificacion(
                    "¡Felicidades! Has completado el juego\n de sonidos de animales 🐾🎉",
                    "/Images/Imagenes/Personaje/alegre.png"
                );


                this.NavigationService.Navigate(new PageJugar());
                return;
            }

            new LiteDbService().RegistrarAcierto(SesionActual.PadreId, "Animales", 1);

            SonidoManager.Instance.ReproducirSonido("Sounds/sonidoPunto.wav");
            SonidoManager.Instance.ReproducirSonido("Sounds/vozEstrella.wav");

            MostrarNotificacion(
                "¡Correcto! Has elegido el animal correcto.\nHas ganado una estrella!",
                "/Images/Imagenes/estrella.png"
            );

            if (estrellas == ACIERTOS_NIVEL2)
            {
                SonidoManager.Instance.ReproducirSonido("Sounds/Nivel2Mensaje.mp4");

                MostrarNotificacion(
                    "¡Nivel 2 desbloqueado!\n Ahora hay más animales.",
                    "/Images/Imagenes/Personaje/alegre.png"
                );
            }

            _ = IniciarJuego();
        }


        private void ManejarError()
        {
            SonidoManager.Instance.ReproducirSonido("Sounds/intentalo.wav");
            MostrarNotificacion("¡Incorrecto! Intenta de nuevo.", "/Images/Imagenes/Personaje/duda.png");
        }

        private void MostrarVentanaJuego(Window ventana)
        {
            ventana.WindowState = WindowState.Normal;
            ventana.Topmost = true;
            ventana.Left = (SystemParameters.PrimaryScreenWidth - ventana.Width) / 2;
            ventana.Top = (SystemParameters.PrimaryScreenHeight - ventana.Height) / 2;
            ventana.ShowDialog();
        }


        private void MostrarNotificacion(String mensaje, String ruta)
        {
            ventanaNotificacionNivel ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto(mensaje);
            ventana.CambiarImagen(ruta, 200, 200);
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();
        }

        private void AnimacionRebote(Button boton)
        {
            var scaleTransform = new ScaleTransform(1, 1);
            boton.RenderTransform = scaleTransform;
            boton.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new BounceEase { Bounces = 2, Bounciness = 2 }
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        private void AnimacionSacudida(Button boton)
        {
            var transform = new TranslateTransform();
            boton.RenderTransform = transform;

            var animation = new DoubleAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromMilliseconds(300)
            };

            animation.KeyFrames.Add(new LinearDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250))));

            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void Btn_hover(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SonidoManager.Instance.ReproducirSonidoHover("/Sounds/SoundsAnimals/pasarporEncima.mp3");
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PageJugar());
            SonidoManager.Instance.DetenerSonidoAnimal();
        }

        private void Sonido_Click(object sender, RoutedEventArgs e)
        {
            string sonidoPath = AppDomain.CurrentDomain.BaseDirectory + juegoAnimales.AnimalCorrecto.SonidoPath;
            SonidoManager.Instance.ReproducirSonidoAnimales(sonidoPath);
        }


        private void BtnComoseJuega_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoAnimales.mp4");

            MostrarVentanaJuego(ventana);
        }
    }
}