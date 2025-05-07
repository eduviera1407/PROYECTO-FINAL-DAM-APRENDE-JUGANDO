using AprendeJugando;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AjustarEscala();
            SonidoManager.Instance.ReproducirSonidoDeInicio();

            var liteDbService = new LiteDbService();
            liteDbService.CrearBaseDatos();
        }

        private void AjustarEscala()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            Width = screenWidth;
            Height = screenHeight;
            WindowState = WindowState.Maximized;

            MainScaleTransform.ScaleX = screenWidth / 1920.0;
            MainScaleTransform.ScaleY = screenHeight / 1080.0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IniciarAnimacion("Flotar", botonIniciar);
                IniciarAnimacion("Flotar", botonAjustes);
                IniciarAnimacion("Flotar2", botonSalir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las animaciones: " + ex.Message);
            }
        }

        private void IniciarAnimacion(string resourceKey, UIElement target)
        {
            if (FindResource(resourceKey) is Storyboard storyboard)
            {
                Storyboard.SetTarget(storyboard, target);
                storyboard.Begin();
            }
        }

        private void ReproducirSonidoHover()
        {
            SonidoManager.Instance.ReproducirSonidoHover("Sounds/pasarporEncima.mp3");
        }

        private void BtnHover(object sender, MouseEventArgs e) => ReproducirSonidoHover();

        private void botonIniciar_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new PageJugar());

        private void botonAjustes_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new PageAjustes());

        private void botonSalir_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
        }
    }
}