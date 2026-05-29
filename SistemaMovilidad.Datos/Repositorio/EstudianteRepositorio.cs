using SistemaMovilidad.Objetos.Models;
using static SistemaMovilidad.Objetos.DTOs.EstudianteDtos;

namespace SistemaMovilidad.Datos.Repositorio
{
	public class EstudianteRepositorio
	{
		private readonly List<Estudiante> _estudiantes = new();
		private int _nextId = 1;

		public EstudianteRepositorio()
		{
			CargarDatosEjemplo();
		}

		public List<Estudiante> ObtenerTodos() => _estudiantes.ToList();

		public bool ExisteDocumento(string documento) =>
			_estudiantes.Any(e => e.Documento == documento);

		public Estudiante Agregar(Estudiante estudiante)
		{
			estudiante.Id = _nextId++;
			_estudiantes.Add(estudiante);
			return estudiante;
		}

		#region DatosDePrueba
		private void CargarDatosEjemplo()
		{
			var datos = new List<(string Nombre, string Doc, string Zona, string Inst, string Tipo, List<HorarioEstudiante> Horarios)>
			{
				("Carlos Andrés Ruiz", "1001234567", "Santa Rita", "Universidad de Antioquia", "Universitario",
					new List<HorarioEstudiante> {
						new() { Dia = DiaSemana.Lunes,   HoraEntrada = "16:00", HoraSalida = "22:00" },
						new() { Dia = DiaSemana.Martes,  HoraEntrada = "09:00", HoraSalida = "12:00" },
						new() { Dia = DiaSemana.Jueves,  HoraEntrada = "14:00", HoraSalida = "18:00" },
						new() { Dia = DiaSemana.Viernes, HoraEntrada = "07:00", HoraSalida = "12:00" },
					}),
				("Laura Camila Pérez", "1009876543", "San Vicente", "EAFIT", "Universitario",
					new List<HorarioEstudiante> {
						new() { Dia = DiaSemana.Lunes,     HoraEntrada = "07:00", HoraSalida = "13:00" },
						new() { Dia = DiaSemana.Miercoles, HoraEntrada = "07:00", HoraSalida = "13:00" },
						new() { Dia = DiaSemana.Viernes,   HoraEntrada = "07:00", HoraSalida = "13:00" },
					}),
				("Sebastián Torres", "1112223334", "Chaparral", "ITM", "Universitario",
					new List<HorarioEstudiante> {
						new() { Dia = DiaSemana.Lunes,     HoraEntrada = "18:00", HoraSalida = "22:00" },
						new() { Dia = DiaSemana.Martes,    HoraEntrada = "18:00", HoraSalida = "22:00" },
						new() { Dia = DiaSemana.Miercoles, HoraEntrada = "18:00", HoraSalida = "22:00" },
						new() { Dia = DiaSemana.Jueves,    HoraEntrada = "18:00", HoraSalida = "22:00" },
					}),
				("Valeria Montoya", "1005556677", "Chaparral", "Colegio San José", "Escolar", new()),
				("Juan David Gómez", "1098765432", "La Enea", "Universidad de Antioquia", "Universitario",
					new List<HorarioEstudiante> {
						new() { Dia = DiaSemana.Lunes,  HoraEntrada = "07:00", HoraSalida = "12:00" },
						new() { Dia = DiaSemana.Martes, HoraEntrada = "14:00", HoraSalida = "18:00" },
						new() { Dia = DiaSemana.Jueves, HoraEntrada = "07:00", HoraSalida = "12:00" },
						new() { Dia = DiaSemana.Sabado, HoraEntrada = "08:00", HoraSalida = "12:00" },
					}),
				("Daniela Ríos", "1003331122", "La Enea", "EAFIT", "Universitario",
					new List<HorarioEstudiante> {
						new() { Dia = DiaSemana.Martes, HoraEntrada = "09:00", HoraSalida = "13:00" },
						new() { Dia = DiaSemana.Jueves, HoraEntrada = "09:00", HoraSalida = "13:00" },
						new() { Dia = DiaSemana.Sabado, HoraEntrada = "09:00", HoraSalida = "14:00" },
					}),
				("Miguel Herrera", "1007778899", "San Vicente", "Universidad Nacional", "Universitario",
					new List<HorarioEstudiante> {
						new() { Dia = DiaSemana.Lunes,     HoraEntrada = "07:00", HoraSalida = "11:00" },
						new() { Dia = DiaSemana.Martes,    HoraEntrada = "07:00", HoraSalida = "11:00" },
						new() { Dia = DiaSemana.Miercoles, HoraEntrada = "14:00", HoraSalida = "18:00" },
						new() { Dia = DiaSemana.Viernes,   HoraEntrada = "07:00", HoraSalida = "11:00" },
					}),
				("Sofia Cardona", "1001112233", "Santa Rita", "Colegio La Presentación", "Escolar", new()),
			};

			foreach (var d in datos)
			{
				Enum.TryParse<TipoEstudiante>(d.Tipo, out var tipo);
				var est = new Estudiante
				{
					Id = _nextId++,
					Nombre = d.Nombre,
					Documento = d.Doc,
					ZonaResidencia = d.Zona,
					Institucion = d.Inst,
					Tipo = tipo,
					Horarios = d.Horarios
				};

				if (tipo == TipoEstudiante.Escolar && est.Horarios.Count == 0)
				{
					var dias = new[] { DiaSemana.Lunes, DiaSemana.Martes, DiaSemana.Miercoles, DiaSemana.Jueves, DiaSemana.Viernes };
					foreach (var dia in dias)
						est.Horarios.Add(new HorarioEstudiante { Dia = dia, HoraEntrada = "07:00", HoraSalida = "13:00" });
				}

				_estudiantes.Add(est);
			}
		}
		#endregion
	}
}