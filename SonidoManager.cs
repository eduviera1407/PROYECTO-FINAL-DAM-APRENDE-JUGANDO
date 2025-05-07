using System.Windows;
using System.Windows.Media;

public class SonidoManager
{
    private static SonidoManager _instance;
    private MediaPlayer _mediaPlayer;
    private MediaPlayer _mediaPlayerExplicacion;
    private MediaPlayer _mediaPlayerHover;
    private List<MediaPlayer> _mediaPlayers = new List<MediaPlayer>();
    private MediaPlayer mediaPlayerAnimal;

    private SonidoManager()
    {
        _mediaPlayer = new MediaPlayer();
        _mediaPlayerExplicacion = new MediaPlayer();
        _mediaPlayerHover = new MediaPlayer();
    }

    public static SonidoManager Instance => _instance ??= new SonidoManager();

    public void ReproducirMusicaDeFondo(string rutaSonido)
    {
        if (_mediaPlayer.Source == null)
        {
            try
            {
                Uri soundUri = new Uri(rutaSonido, UriKind.Relative);
                _mediaPlayer.Open(soundUri);
                _mediaPlayer.Volume = 0.2;
                _mediaPlayer.Play();

                _mediaPlayer.MediaEnded += (sender, e) =>
                {
                    _mediaPlayer.Position = TimeSpan.Zero;
                    _mediaPlayer.Play();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al reproducir música de fondo: {ex.Message}");
                MessageBox.Show("Error al reproducir música de fondo: " + ex.Message);
            }
        }
    }

    public void ReproducirSonidoDeInicio()
    {
        SonidoManager.Instance.ReproducirMusicaDeFondo("Sounds/sonidoPrincipal.wav");
    }

    public void ReproducirSonido(string rutaSonido)
    {
        try
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(rutaSonido, UriKind.Relative));
            mediaPlayer.Volume = 0.7;
            mediaPlayer.Play();

            _mediaPlayers.Add(mediaPlayer);

            mediaPlayer.MediaEnded += (sender, args) =>
            {
                _mediaPlayers.Remove(mediaPlayer);
                mediaPlayer.Close();
            };
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error al reproducir el sonido: " + ex.Message);
        }
    }

    public void ReproducirSonidoAnimales(string rutaSonido)
    {
        try
        {
            mediaPlayerAnimal = new MediaPlayer();
            mediaPlayerAnimal.Open(new Uri(rutaSonido, UriKind.Relative));
            mediaPlayerAnimal.Volume = 1;
            mediaPlayerAnimal.Play();

            _mediaPlayers.Add(mediaPlayerAnimal);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al reproducir el sonido: " + ex.Message);
        }
    }


    public void DetenerSonidoAnimal()
    {
        try
        {
            mediaPlayerAnimal.Stop();
            mediaPlayerAnimal.Close();


            foreach (var player in _mediaPlayers)
            {
                player.Stop();
                player.Close();
            }

            _mediaPlayers.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al detener todos los sonidos: {ex.Message}");
            MessageBox.Show("Error al detener todos los sonidos: " + ex.Message);
        }
    }


    public void ReproducirSonidoHover(string rutaSonidoHover)
    {
        try
        {
            _mediaPlayerHover.Stop();
            _mediaPlayerHover.Open(new Uri(rutaSonidoHover, UriKind.Relative));
            _mediaPlayerHover.Volume = 0.5;
            _mediaPlayerHover.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al reproducir el sonido de hover: {ex.Message}");
        }
    }

    public void AjustarVolumen(double volumen)
    {
        if (_mediaPlayer != null)
        {
            try
            {
                _mediaPlayer.Volume = volumen / 100;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ajustar el volumen: {ex.Message}");
                MessageBox.Show("Error al ajustar el volumen: " + ex.Message);
            }
        }
    }

    public void DetenerTodosLosSonidos()
    {
        try
        {
            _mediaPlayerExplicacion.Stop();
            _mediaPlayerExplicacion.Close();


            // No detener la música de fondo
            foreach (var player in _mediaPlayers)
            {
                player.Stop();
                player.Close();
            }

            _mediaPlayers.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al detener todos los sonidos: {ex.Message}");
            MessageBox.Show("Error al detener todos los sonidos: " + ex.Message);
        }
    }
}