using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AprendeJugando
{
    public partial class ventanaNotificacionNivel : Window
    {
        public static ventanaNotificacionNivel instancia;

        public ventanaNotificacionNivel()
        {
            InitializeComponent();
            AnimarAparicion();
        }

        private void AnimarAparicion()
        {
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
        }

        public void CambiarTexto(string texto)
        {
            textNotificacion.Text = texto;
        }

        public void CambiarImagen(string ruta, int w, int h)
        {
            imagenCambiar.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
            imagenCambiar.Width = w;
            imagenCambiar.Height = h;
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            SonidoManager.Instance.DetenerTodosLosSonidos();
            this.Close();
        }
    }
}