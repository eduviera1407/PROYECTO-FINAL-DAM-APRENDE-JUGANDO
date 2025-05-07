namespace AprendeJugando
{
    public static class SesionActual
    {
        public static int PadreId { get; set; }
        public static string Usuario { get; set; }
        public static List<Nino> Ninos { get; set; } = new();
        public static string NombreNinoActivo { get; set; }
        public static bool PadreAutenticado { get; set; }
    }
}