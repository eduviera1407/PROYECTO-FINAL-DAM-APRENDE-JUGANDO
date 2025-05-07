using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1;

namespace AprendeJugando
{
    public partial class PageJugar : Page
    {
        private bool sonidoActivo = true;

        public PageJugar()
        {
            InitializeComponent();

            ActualizarInterfazSesion();
        }


        private void ActualizarInterfazSesion()
        {
            Btn_CerrarSesion.Visibility = SesionActual.PadreAutenticado ? Visibility.Visible : Visibility.Hidden;
            Btn_Progreso.Visibility = SesionActual.PadreAutenticado ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnSilenciar_Click(object sender, RoutedEventArgs e)
        {
            sonidoActivo = !sonidoActivo;
            SonidoManager.Instance.AjustarVolumen(sonidoActivo ? 50 : 0);
            VolumenIcon.Kind = sonidoActivo
                ? MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh
                : MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
        }

        private void BtnAjustes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PageAjustes());
        }

        private void BtnLoginPadre_Click(object sender, RoutedEventArgs e)
        {
            if (SesionActual.PadreAutenticado)
            {
                NotificacionHandler.MostrarVentana(
                    $"Padre: {SesionActual.Usuario}\nNiño/a: {SesionActual.NombreNinoActivo}");
                return;
            }

            var ventana = new VentanaEntrada();

            if (Window.GetWindow(this) is Window ventanaPrincipal)
            {
                ventana.Owner = ventanaPrincipal;
                bool? resultado = ventana.ShowDialog();

                if (resultado == true)
                {
                    var usuario = ventana.UsuarioIngresado;
                    var contrasena = ventana.ContrasenaIngresada;

                    var db = new LiteDbService();
                    var padre = db.BuscarPadre(usuario);

                    if (padre is not null && padre.Contrasena == contrasena)
                    {
                        SesionActual.PadreId = padre.Id;
                        SesionActual.Usuario = padre.Usuario;
                        SesionActual.Ninos = padre.NombresNinos ?? new List<Nino>();
                        SesionActual.PadreAutenticado = true;
                        ActualizarInterfazSesion();
                    }
                    else
                    {
                        NotificacionHandler.MostrarVentana("Credenciales inválidas");
                    }
                }
                else
                {
                    NotificacionHandler.MostrarVentana("Acceso cancelado");
                }
            }
        }


        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            NotificacionHandler.MostrarVentana("Cerrando sesión de \n padre.");
            SesionActual.PadreAutenticado = false;
            SesionActual.PadreId = 0;
            SesionActual.Usuario = null;
            SesionActual.Ninos = null;
            ActualizarInterfazSesion();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
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

        private void Btn_JuegoCartas_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PageJuegoCartas());
        }

        private void Btn_JuegoMates_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PageMates());
        }

        private void Btn_JuegoAnimales_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PageJuegoAnimales()); 
        
        }


        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            miScrollViewer.ScrollToHorizontalOffset(miScrollViewer.HorizontalOffset - 400);
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            miScrollViewer.ScrollToHorizontalOffset(miScrollViewer.HorizontalOffset + 400);
        }

        private void BtnHover(object sender, MouseEventArgs e)
        {
            SonidoManager.Instance.ReproducirSonidoHover("Sounds/pasarporEncima.mp3");
        }

        private void MostrarProgreso_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MostrarVentanaJuego(new VentanaProgreso());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar progreso: {ex.Message}");
            }
        }
    }
}