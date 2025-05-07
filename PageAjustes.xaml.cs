using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1;

namespace AprendeJugando
{
    public partial class PageAjustes : Page
    {
        public PageAjustes()
        {
            InitializeComponent();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double volumen = e.NewValue;
            SonidoManager.Instance.AjustarVolumen(volumen);
            ActualizarIconoVolumen(volumen);
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = null;
            }
        }

        private void BtnPantallaCompleta_Click(object sender, RoutedEventArgs e) =>
            CambiarModoPantalla(WindowState.Maximized, WindowStyle.None, true);

        private void BtnPantallaVentana_Click(object sender, RoutedEventArgs e) =>
            CambiarModoPantalla(WindowState.Normal, WindowStyle.SingleBorderWindow, false);

        private void Btn_AcercaDeMi(object sender, RoutedEventArgs e)
        {
            string acercaDe =
                "📘 Aprende Jugando - Versión 1.0\n" +
                "Desarrollado por Eduardo Roque Viera Santana\n" +
                "🗓️ Fecha: 09-05-2025\n\n" +
                "Este proyecto busca enseñar de forma divertida a los niños.\n" +
                "¡Gracias por jugar! 😊";

            MessageBox.Show(acercaDe, "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CambiarModoPantalla(WindowState estado, WindowStyle estilo, bool topmost)
        {
            Window ventana = Window.GetWindow(this);
            if (ventana != null)
            {
                ventana.WindowState = estado;
                ventana.WindowStyle = estilo;
                ventana.Topmost = topmost;
            }
        }

        private void ActualizarIconoVolumen(double volumen)
        {
            VolumeHigh.Kind = volumen switch
            {
                0 => MaterialDesignThemes.Wpf.PackIconKind.VolumeOff,
                <= 30 => MaterialDesignThemes.Wpf.PackIconKind.VolumeLow,
                <= 70 => MaterialDesignThemes.Wpf.PackIconKind.VolumeMedium,
                _ => MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh,
            };
        }

        private void BtnHover(object sender, MouseEventArgs e) =>
            SonidoManager.Instance.ReproducirSonidoHover("Sounds/pasarporEncima.mp3");
    }
}