namespace AprendeJugando.JuegoAnimales
{
    public class Animales
    {
        private static Random random = new Random();

        public List<(string Nombre, string ImagenPath, string SonidoPath)> ListaAnimales { get; private set; }
        public (string Nombre, string SonidoPath) AnimalCorrecto { get; private set; }

        public Animales()
        {
            ListaAnimales = new List<(string, string, string)>
            {
                ("Abeja", "/Images/Imagenes/Animals/abeja.jpeg", "/Sounds/SoundsAnimals/abeja.mp3"),
                ("Búho", "/Images/Imagenes/Animals/buho.jpeg", "/Sounds/SoundsAnimals/buho.mp3"),
                ("Caballo", "/Images/Imagenes/Animals/caballo.jpeg", "/Sounds/SoundsAnimals/caballo.mp3"),
                ("Cabra", "/Images/Imagenes/Animals/cabra.jpeg", "/Sounds/SoundsAnimals/cabra.mp3"),
                ("Cerdo", "/Images/Imagenes/Animals/cerdo.jpeg", "/Sounds/SoundsAnimals/cerdo.mp3"),
                ("Elefante", "/Images/Imagenes/Animals/elefante.jpeg", "/Sounds/SoundsAnimals/elefante.mp3"),
                ("Gallo", "/Images/Imagenes/Animals/gallo.jpg", "/Sounds/SoundsAnimals/gallo.mp3"),
                ("Gato", "/Images/Imagenes/Animals/gato.jpeg", "/Sounds/SoundsAnimals/gato.mp3"),
                ("León", "/Images/Imagenes/Animals/leon.jpeg", "/Sounds/SoundsAnimals/leon.mp3"),
                ("Mono", "/Images/Imagenes/Animals/mono.jpg", "/Sounds/SoundsAnimals/mono.mp3"),
                ("Pavo", "/Images/Imagenes/Animals/pavo.jpeg", "/Sounds/SoundsAnimals/pavo.mp3"),
                ("Perro", "/Images/Imagenes/Animals/perro.jpeg", "/Sounds/SoundsAnimals/perro.mp3"),
                ("Vaca", "/Images/Imagenes/Animals/vaca.jpeg", "/Sounds/SoundsAnimals/vaca.mp3"),
            };
        }

        public List<(string, string, string)> ObtenerAnimalesAleatorios()
        {
            return ListaAnimales.OrderBy(a => random.Next()).Take(4).ToList();
        }

        public void SeleccionarAnimalCorrecto(List<(string, string, string)> animales)
        {
            var animal = animales[random.Next(animales.Count)];
            AnimalCorrecto = (animal.Item1, animal.Item3);
        }
    }
}