using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AprendeJugando.JuegoMates;

namespace AprendeJugando
{
    public partial class PageMates : Page
    {
        private HashSet<string> numerosPulsados = new();
        private const int TOTAL_NUMEROS = 10;

        public PageMates()
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoMatesNivel1.mp4");
            MostrarVentanaJuego(ventana);
            InitializeComponent();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageJugar());
        }

        private async void Btn_Click_Number(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is TextBlock textBlock)
            {
                string number = textBlock.Text;

                btn.Opacity = 0.5;
                btn.IsEnabled = false;

                string nombre = number switch
                {
                    "1" => "Uno",
                    "2" => "Dos",
                    "3" => "Tres",
                    "4" => "Cuatro",
                    "5" => "Cinco",
                    "6" => "Seis",
                    "7" => "Siete",
                    "8" => "Ocho",
                    "9" => "Nueve",
                    "10" => "Diez",
                    _ => ""
                };

                await MostrarNumeroAsync(nombre);

                if (numerosPulsados.Add(number) && numerosPulsados.Count == TOTAL_NUMEROS)
                {
                    await MostrarVentanaCompletadoAsync();
                }
            }
        }


        private async Task MostrarNumeroAsync(string nombre)
        {
            string sonidoPath = $"Sounds/SoundsNumeros/Sound{nombre}.wav";
            string imagenPath = $"/Images/Imagenes/JuegoMates/numero{nombre}.png";

            SonidoManager.Instance.ReproducirSonido(sonidoPath);

            try
            {
                cambioNumero.Source = new BitmapImage(new Uri(imagenPath, UriKind.Relative));
                cambioNumero.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cargar la imagen: {imagenPath}\n{ex.Message}");
            }

            await Task.Delay(500);
        }


        private async Task MostrarVentanaCompletadoAsync()
        {
            SonidoManager.Instance.ReproducirSonido("Sounds/nivelCompletado.wav");
            SonidoManager.Instance.ReproducirSonido("Sounds/aplausos.mp3");

            if (SesionActual.PadreAutenticado)
            {
                var db = new LiteDbService();
                db.RegistrarAcierto(SesionActual.PadreId, "Matematicas", 1, 1);
            }

            var ventana = new ventanaNotificacionNivel
            {
                Owner = Window.GetWindow(this)
            };

            ventana.CambiarTexto("¡Has completado todos los números!\n¡Excelente trabajo!");
            ventana.CambiarImagen("/Images/Imagenes/Personaje/alegre.png", 200, 200);
            ventana.ShowDialog();

            NavigationService.Navigate(new PageMatesLevel2());
        }


        private void BtnHover(object sender, MouseEventArgs e)
        {
            SonidoManager.Instance.ReproducirSonidoHover("Sounds/pasarporEncima.mp3");
        }

        private void pasarNivel(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageMatesLevel2());
        }

        private void MostrarVentanaJuego(Window ventana)
        {
            ventana.WindowState = WindowState.Normal;
            ventana.Topmost = true;
            ventana.Left = (SystemParameters.PrimaryScreenWidth - ventana.Width) / 2;
            ventana.Top = (SystemParameters.PrimaryScreenHeight - ventana.Height) / 2;
            ventana.ShowDialog();
        }

        private void BtnComoseJuega_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaExplicacionJC("Videos/tutorialJuegoMatesNivel1.mp4");
            MostrarVentanaJuego(ventana);
        }
    }
}