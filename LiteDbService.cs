using LiteDB;
using System.IO;

namespace AprendeJugando
{
    public class LiteDbService
    {
        private static string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AprendeJugando.db");

        public LiteDbService()
        {
            CrearBaseDatos();
        }

        private LiteDatabase GetDatabase()
        {
            return new LiteDatabase($"Filename={DatabasePath}; Connection=Shared;");
        }

        public void CrearBaseDatos()
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var padres = db.GetCollection<CredencialesPadres>("Padres");
                    var progreso = db.GetCollection<Progreso>("Progreso");

                    padres.EnsureIndex(p => p.Usuario, unique: true);
                    progreso.EnsureIndex(p => new { p.PadreId, p.TipoJuego, p.Nivel }, unique: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la base de datos: {ex.Message}");
            }
        }

        public bool InsertarPadre(CredencialesPadres padre)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var padres = db.GetCollection<CredencialesPadres>("Padres");
                    var padreExistente = padres.FindOne(p => p.Usuario == padre.Usuario);

                    if (padreExistente != null)
                    {
                        string nuevoNombre = padre.NombresNinos.First().Nombre;

                        bool yaExiste = padreExistente.NombresNinos.Any(n => n.Nombre == nuevoNombre);
                        if (!yaExiste)
                        {
                            padreExistente.NombresNinos.Add(new Nino
                            {
                                Nombre = nuevoNombre,
                                Id = ObjectId.NewObjectId() 
                            });
                            padres.Update(padreExistente);
                            return true;
                        }

                        return false;
                    }

                    padre.NombresNinos.ForEach(n =>
                    {
                        if (n.Id == ObjectId.Empty)
                            n.Id = ObjectId.NewObjectId();
                    });

                    padres.Insert(padre);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar el padre: {ex.Message}");
                return false;
            }
        }


        public CredencialesPadres BuscarPadre(string usuario)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var padres = db.GetCollection<CredencialesPadres>("Padres");
                    return padres.FindOne(p => p.Usuario == usuario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar el padre: {ex.Message}");
                return null;
            }
        }


        public void RegistrarAcierto(int padreId, string tipoJuego, int nivel, int estrellasGanadas = 1)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var nombreNino = SesionActual.NombreNinoActivo ?? "Niño";

                    var progresoCol = db.GetCollection<Progreso>("Progreso");
                    var registro = progresoCol.FindOne(p =>
                        p.PadreId == padreId && p.NombreNino == nombreNino && p.TipoJuego == tipoJuego &&
                        p.Nivel == nivel);

                    if (registro == null)
                    {
                        registro = new Progreso
                        {
                            PadreId = padreId,
                            NombreNino = nombreNino,
                            TipoJuego = tipoJuego,
                            Nivel = nivel,
                            Estrellas = estrellasGanadas
                        };
                        progresoCol.Insert(registro);
                    }
                    else
                    {
                        registro.Estrellas += estrellasGanadas;
                        progresoCol.Update(registro);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar acierto: {ex.Message}");
            }
        }

        public void ActualizarPadre(CredencialesPadres padre)
        {
            using var db = GetDatabase();
            var padres = db.GetCollection<CredencialesPadres>("Padres");
            padres.Update(padre);
        }


        // Obtiene todo el progreso de un niño asociado a un padre.
        public List<Progreso> ObtenerProgresoPorPadre(int padreId)
        {
            try
            {
                using (var db = GetDatabase())
                {
                    var progresoCol = db.GetCollection<Progreso>("Progreso");
                    return progresoCol.Find(p => p.PadreId == padreId).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el progreso del usuario: {ex.Message}");
                return new List<Progreso>(); 
            }
        }
    }


    // Modelo de credenciales de los padres.
    public class CredencialesPadres
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }

        public List<Nino> NombresNinos { get; set; } = new();
    }


    public class Nino
    {
        [BsonId] public ObjectId Id { get; set; } = ObjectId.NewObjectId();

        public string Nombre { get; set; }
    }


    public class Progreso
    {
        public int Id { get; set; }

        public int PadreId { get; set; }

        public string NombreNino { get; set; } 
        public string TipoJuego { get; set; }
        public int Nivel { get; set; }
        public int Estrellas { get; set; }
    }
}