using System.Windows;

namespace AprendeJugando
{
    public partial class VentanaExplicacionJC : Window
    {
        private readonly string rutaVideo;

        public VentanaExplicacionJC(string rutaVideo)
        {
            InitializeComponent();

            this.rutaVideo = rutaVideo;

            this.Loaded += VentanaExplicacionJC_Loaded;
        }

        private void VentanaExplicacionJC_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaTutorial.Source = new Uri(rutaVideo, UriKind.Relative);
                MediaTutorial.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reproducir el video: " + ex.Message);
            }
        }


        private void MediaTutorial_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}