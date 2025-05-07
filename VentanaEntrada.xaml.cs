using System.Windows;
using System.Windows.Controls;

namespace AprendeJugando
{
    public partial class VentanaEntrada : Window
    {
        private int resultadoEsperado;
        public string UsuarioIngresado { get; private set; }
        public string ContrasenaIngresada { get; private set; }

        public VentanaEntrada()
        {
            InitializeComponent();
            GenerarSumaAleatoria();
        }

        private void GenerarSumaAleatoria()
        {
            Random random = new();
            int num1 = random.Next(1, 100);
            int num2 = random.Next(1, 100);
            resultadoEsperado = num1 + num2;
            Pregunta.Text = $"{num1} + {num2} = ?";
        }

        private void BtnValidar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RespuestaTextBox.Text, out int respuesta) && respuesta == resultadoEsperado)
            {
                MostrarPanelLogin();
            }
            else
            {
                NotificacionHandler.MostrarVentana("Respuesta incorrecta, intenta de nuevo.\nValidación de Padre");
            }
        }

        private void MostrarPanelLogin()
        {
            Pregunta.Visibility = Visibility.Collapsed;
            BtnValidar.Visibility = Visibility.Collapsed;
            TextoPregunta.Visibility = Visibility.Collapsed;
            RespuestaTextBox.Visibility = Visibility.Collapsed;
            BorderName.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            UsuarioIngresado = UsuarioTextBox.Text;
            ContrasenaIngresada = PasswordBox.Password;

            var dbService = new LiteDbService();
            var padre = dbService.BuscarPadre(UsuarioIngresado);

            if (padre != null && padre.Contrasena == ContrasenaIngresada)
            {
                SesionActual.PadreId = padre.Id;
                SesionActual.Usuario = padre.Usuario;
                SesionActual.Ninos = padre.NombresNinos ?? new List<Nino>();
                SesionActual.NombreNinoActivo = (ComboBoxNinos.SelectedItem as Nino)?.Nombre;

                NotificacionHandler.MostrarVentana($"¡Bienvenido, {SesionActual.NombreNinoActivo ?? "\nNiño/a"}!");
                DialogResult = true;
                Close();
            }
            else
            {
                NotificacionHandler.MostrarVentana("Usuario o contraseña incorrectos.\n", "Error de autenticación");
            }
        }


        private void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (CamposDeRegistroVacios())
            {
                NotificacionHandler.MostrarVentana(
                    "Por favor ingrese un usuario, una contraseña\ny el nombre del niño.");
                return;
            }

            string usuario = UsuarioTextBox.Text;
            string contrasena = PasswordBox.Password;
            string nombreNino = NameNino.Text;

            var dbService = new LiteDbService();
            var padreExistente = dbService.BuscarPadre(usuario);

            if (padreExistente != null)
            {
                if (!padreExistente.NombresNinos.Any(n => n.Nombre == nombreNino))
                {
                    padreExistente.NombresNinos.Add(new Nino { Nombre = nombreNino });
                    dbService.ActualizarPadre(padreExistente);
                    NotificacionHandler.MostrarVentana($"Se añadió el niño\n '{nombreNino}'\n al usuario '{usuario}'.");

                    ComboBoxNinos.ItemsSource = padreExistente.NombresNinos;
                    ComboBoxNinos.DisplayMemberPath = "Nombre";

                    ComboBoxNinos.SelectedItem =
                        padreExistente.NombresNinos.FirstOrDefault(n => n.Nombre == nombreNino);
                    SesionActual.NombreNinoActivo = nombreNino;
                }
                else
                {
                    NotificacionHandler.MostrarVentana($"Ese niño ya está registrado\n para el usuario '{usuario}'.");
                }
            }
            else
            {
                var nuevoPadre = new CredencialesPadres
                {
                    Usuario = usuario,
                    Contrasena = contrasena,
                    NombresNinos = new List<Nino> { new Nino { Nombre = nombreNino } }
                };

                dbService.InsertarPadre(nuevoPadre);
                NotificacionHandler.MostrarVentana(
                    $"Usuario {usuario}\n registrado correctamente.\nNiño: {nombreNino}");

                ComboBoxNinos.ItemsSource = nuevoPadre.NombresNinos;
                ComboBoxNinos.DisplayMemberPath = "Nombre";
                ComboBoxNinos.SelectedItem = nuevoPadre.NombresNinos.First();
                SesionActual.NombreNinoActivo = nombreNino;
            }
        }


        private bool CamposDeRegistroVacios()
        {
            return string.IsNullOrWhiteSpace(UsuarioTextBox.Text) ||
                   string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                   string.IsNullOrWhiteSpace(NameNino.Text);
        }

        private void ComboBoxNinos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxNinos.SelectedItem is Nino seleccionado)
            {
                SesionActual.NombreNinoActivo = seleccionado.Nombre;
            }
        }

        private void UsuarioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var db = new LiteDbService();
            var padre = db.BuscarPadre(UsuarioTextBox.Text);

            if (padre != null && padre.NombresNinos?.Any() == true)
            {
                ComboBoxNinos.ItemsSource = padre.NombresNinos;
                ComboBoxNinos.SelectedIndex = 0;
                seleccionarNino.Visibility = Visibility.Visible;
                ComboBoxNinos.Visibility = Visibility.Visible;


                SesionActual.NombreNinoActivo = padre.NombresNinos.First().Nombre;
            }
            else
            {
                ComboBoxNinos.ItemsSource = null;
                seleccionarNino.Visibility = Visibility.Collapsed;
                ComboBoxNinos.Visibility = Visibility.Collapsed;
                SesionActual.NombreNinoActivo = null;
            }
        }
    }
}