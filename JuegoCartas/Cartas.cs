public class Cartas
{
    private static List<string> cartas = new List<string>();
    private static Random random = new Random();

    static Cartas()
    {
        string basePath = "/Images/Imagenes/Cartas/";
        string[] nombres = { "orangutan", "chimpanze", "cobra", "cocodrilo", "gorila", "mono", "rana", "serpiente" };

        foreach (string nombre in nombres)
        {
            cartas.Add(basePath + nombre + ".png");
        }
    }

    public static List<string> ObtenerCartasAleatorias(int nivel)
    {
        List<string> cartasAleatorias = new List<string>();
        List<string> cartasDisponibles = new List<string>(cartas);

        if (nivel == 1)
        {
            string cartaIgual = cartasDisponibles[random.Next(cartasDisponibles.Count)];
            cartasAleatorias.Add(cartaIgual);
            cartasAleatorias.Add(cartaIgual);
            cartasDisponibles.Remove(cartaIgual);

            string cartaDiferente1 = cartasDisponibles[random.Next(cartasDisponibles.Count)];
            string cartaDiferente2 = cartasDisponibles[random.Next(cartasDisponibles.Count)];

            cartasAleatorias.Add(cartaDiferente1);
            cartasAleatorias.Add(cartaDiferente2);
        }
        else
        {
            string cartaIgual = cartasDisponibles[random.Next(cartasDisponibles.Count)];
            cartasAleatorias.Add(cartaIgual);
            cartasAleatorias.Add(cartaIgual);
            cartasAleatorias.Add(cartaIgual);
            cartasDisponibles.Remove(cartaIgual);

            string cartaDiferente1 = cartasDisponibles[random.Next(cartasDisponibles.Count)];
            string cartaDiferente2 = cartasDisponibles[random.Next(cartasDisponibles.Count)];

            cartasAleatorias.Add(cartaDiferente1);
            cartasAleatorias.Add(cartaDiferente2);
            cartasAleatorias.Add(cartaDiferente2);
        }

        return cartasAleatorias.OrderBy(x => random.Next()).ToList();
    }
}