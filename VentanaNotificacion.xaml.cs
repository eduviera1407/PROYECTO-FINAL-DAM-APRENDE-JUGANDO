using System.Windows;
using System.Windows.Media.Imaging;

namespace AprendeJugando
{
    public partial class VentanaNotificacion : Window
    {
        public VentanaNotificacion(string mensaje)
        {
            InitializeComponent();
            txtNotificacion.Text = mensaje;
        }


    }
    public static class NotificacionHandler
    {
        public static void MostrarVentana(string texto, string rutaImagen = null, int ancho = 100, int alto = 100)
        {
            var ventana = new ventanaNotificacionNivel
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize
            };

            ventana.CambiarTexto(texto);

            if (!string.IsNullOrEmpty(rutaImagen))
            {
                ventana.CambiarImagen(rutaImagen, ancho, alto);
            }

            ventana.ShowDialog();
        }
    }
}