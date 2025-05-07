using System.Windows;

namespace AprendeJugando
{
    /// <summary>
    /// Lógica de interacción para VentanaProgreso.xaml
    /// </summary>
    public partial class VentanaProgreso : Window
    {
        public VentanaProgreso()
        {
            InitializeComponent();
            CargarProgreso();
        }

        private void CargarProgreso()
        {
            if (SesionActual.PadreAutenticado)
            {
                try
                {
                    LiteDbService dbService = new LiteDbService();
                    List<Progreso> progresos = dbService.ObtenerProgresoPorPadre(SesionActual.PadreId);

                    if (progresos != null && progresos.Count > 0)
                    {
                        dgProgreso.ItemsSource = progresos;
                    }
                    else
                    {
                        MostrarNotificacion("No se encontró progreso para el usuario actual.",
                            "/Images/Imagenes/Personaje/duda.png");
                    }
                }
                catch (Exception ex)
                {
                    MostrarNotificacion("Error al cargar el progreso:\n" + ex.Message,
                        "/Images/Imagenes/Personaje/duda.png");
                }
            }
            else
            {
                MostrarNotificacion("No hay sesión iniciada.\nInicia sesión para ver el progreso.",
                    "/Images/Imagenes/Personaje/duda.png");
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MostrarNotificacion(string mensaje, string rutaImagen)
        {
            var ventana = new ventanaNotificacionNivel();
            ventana.CambiarTexto(mensaje);
            ventana.CambiarImagen(rutaImagen, 200, 200);
            ventana.Owner = this;
            ventana.ShowDialog();
        }
    }
}