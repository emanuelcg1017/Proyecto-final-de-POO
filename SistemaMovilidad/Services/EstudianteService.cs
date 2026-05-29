using SistemaMovilidad.DTOs;
using SistemaMovilidad.Interfaces;
using SistemaMovilidad.Models;
using static SistemaMovilidad.DTOs.EstudianteDtos;

namespace SistemaMovilidad.Services
{
	public class EstudianteServicio : IEstudiante
	{
		private readonly List<Estudiante> _estudiantes = new();
		private int _nextId = 1;

		public EstudianteServicio()
		{
			CargarDatosEjemplo();
		}

		public List<EstudianteResponse> ObtenerTodos()
		{
			return _estudiantes.Select(MapearAResponse).ToList();
		}

		public EstudianteResponse RegistrarEstudiante(RegistroEstudianteRequest request)
		{
			if (_estudiantes.Any(estudiante => estudiante.Documento == request.Documento))
				throw new InvalidOperationException(
					$"Ya existe un estudiante con el documento {request.Documento}.");

			if (!Enum.TryParse<TipoEstudiante>(request.Tipo, true, out var tipo))
				throw new ArgumentException("El tipo debe ser 'Escolar' o 'Universitario'.");

			var estudiante = new Estudiante
			{
				Id = _nextId++,
				Nombre = request.Nombre.Trim(),
				Documento = request.Documento.Trim(),
				ZonaResidencia = request.ZonaResidencia.Trim(),
				Institucion = request.Institucion.Trim(),
				Tipo = tipo
			};

			if (tipo == TipoEstudiante.Escolar && request.Horarios.Count == 0)
			{
				var diasEscolares = new[] {
					DiaSemana.Lunes, DiaSemana.Martes, DiaSemana.Miercoles,
					DiaSemana.Jueves, DiaSemana.Viernes
				};
				int hId = 1;
				foreach (var dia in diasEscolares)
				{
					estudiante.Horarios.Add(new HorarioEstudiante
					{
						Id = hId++,
						EstudianteId = estudiante.Id,
						Dia = dia,
						HoraEntrada = "07:00",
						HoraSalida = "13:00"
					});
				}
			}
			else
			{
				int hId = 1;
				foreach (var h in request.Horarios)
				{
					if (!Enum.TryParse<DiaSemana>(NormalizarDia(h.Dia), true, out var dia))
						throw new ArgumentException($"Día inválido: {h.Dia}");

					estudiante.Horarios.Add(new HorarioEstudiante
					{
						Id = hId++,
						EstudianteId = estudiante.Id,
						Dia = dia,
						HoraEntrada = h.HoraEntrada,
						HoraSalida = h.HoraSalida
					});
				}
			}

			_estudiantes.Add(estudiante);
			return MapearAResponse(estudiante);
		}

		public List<EstudianteResponse> ConsultarPorZona(string zona)
		{
			return _estudiantes
				.Where(estudiante => estudiante.ZonaResidencia.Contains(zona, StringComparison.OrdinalIgnoreCase))
				.Select(MapearAResponse)
				.ToList();
		}

		public List<EstudianteResponse> ConsultarPorUniversidad(string institucion)
		{
			return _estudiantes
				.Where(estudiante => estudiante.Institucion.Contains(institucion, StringComparison.OrdinalIgnoreCase))
				.Select(MapearAResponse)
				.ToList();
		}

		public List<EstudianteResponse> ConsultarPorHorario(string dia, string? jornada)
		{
			if (!Enum.TryParse<DiaSemana>(NormalizarDia(dia), true, out var diaSemana))
				throw new ArgumentException($"Día inválido: {dia}");

			var resultado = _estudiantes
				.Where(estudiante => estudiante.Horarios.Any(h => h.Dia == diaSemana && h.AsistEseDia))
				.ToList();

			if (!string.IsNullOrEmpty(jornada))
			{
				resultado = resultado
					.Where(estudiante => estudiante.Horarios.Any(h =>
						h.Dia == diaSemana &&
						h.AsistEseDia &&
						ObtenerJornada(h.HoraEntrada) == jornada.ToLower()))
					.ToList();
			}

			return resultado.Select(MapearAResponse).ToList();
		}

		public ReporteHorarioDiaResponse GenerarReporteHorarioPorDia(string dia)
		{
			if (!Enum.TryParse<DiaSemana>(NormalizarDia(dia), true, out var diaSemana))
				throw new ArgumentException($"Día inválido: {dia}.");

			var horariosDelDia = _estudiantes
				.SelectMany(e => e.Horarios
					.Where(h => h.Dia == diaSemana && h.AsistEseDia))
				.ToList();

			var entradas = horariosDelDia
				.GroupBy(h => h.HoraEntrada)
				.OrderBy(g => g.Key)
				.Select(g => new FranjaHorariaDto { Hora = g.Key, Cantidad = g.Count() })
				.ToList();

			var salidas = horariosDelDia
				.GroupBy(h => h.HoraSalida)
				.OrderBy(g => g.Key)
				.Select(g => new FranjaHorariaDto { Hora = g.Key, Cantidad = g.Count() })
				.ToList();

			return new ReporteHorarioDiaResponse
			{
				Dia = diaSemana.ToString(),
				TotalEstudiantesEseDia = horariosDelDia.Count,
				Entradas = entradas,
				Salidas = salidas
			};
		}

		private static EstudianteResponse MapearAResponse(Estudiante estudiante) => new()
		{
			Id = estudiante.Id,
			Nombre = estudiante.Nombre,
			Documento = estudiante.Documento,
			ZonaResidencia = estudiante.ZonaResidencia,
			Institucion = estudiante.Institucion,
			Tipo = estudiante.Tipo.ToString(),
			Horarios = estudiante.Horarios.Select(h => new HorarioDto
			{
				Dia = h.Dia.ToString(),
				HoraEntrada = h.HoraEntrada,
				HoraSalida = h.HoraSalida
			}).ToList()
		};

		private static string ObtenerJornada(string horaEntrada)
		{
			if (!TimeOnly.TryParse(horaEntrada, out var hora)) return "desconocida";
			if (hora.Hour >= 6 && hora.Hour < 12) return "mañana";
			if (hora.Hour >= 12 && hora.Hour < 18) return "tarde";
			return "noche";
		}

		private static string NormalizarDia(string dia) =>
			dia.Trim()
			   .Replace("é", "e").Replace("É", "e")
			   .Replace("á", "a").Replace("ó", "o")
			   .Replace("Miércoles", "Miercoles")
			   .Replace("miércoles", "Miercoles");

		private void CargarDatosEjemplo()
		{
			var ejemplos = new List<RegistroEstudianteRequest>
			{
				new() {
					Nombre = "Carlos Andrés Ruiz", Documento = "1001234567",
					ZonaResidencia = "San Vicente",
					Institucion = "Universidad de Antioquia", Tipo = "Universitario",
					Horarios = new List<HorarioDto> {
						new() { Dia = "Lunes",   HoraEntrada = "16:00", HoraSalida = "22:00" },
						new() { Dia = "Martes",  HoraEntrada = "09:00", HoraSalida = "12:00" },
						new() { Dia = "Jueves",  HoraEntrada = "14:00", HoraSalida = "18:00" },
						new() { Dia = "Viernes", HoraEntrada = "07:00", HoraSalida = "12:00" },
					}
				},
				new() {
					Nombre = "Laura Camila Pérez", Documento = "1009876543",
					ZonaResidencia = "Santa Rita",
					Institucion = "EAFIT", Tipo = "Universitario",
					Horarios = new List<HorarioDto> {
						new() { Dia = "Lunes",     HoraEntrada = "07:00", HoraSalida = "13:00" },
						new() { Dia = "Miercoles", HoraEntrada = "07:00", HoraSalida = "13:00" },
						new() { Dia = "Viernes",   HoraEntrada = "07:00", HoraSalida = "13:00" },
					}
				},
				new() {
					Nombre = "Sebastián Torres", Documento = "1112223334",
					ZonaResidencia = "Chaparral",
					Institucion = "ITM", Tipo = "Universitario",
					Horarios = new List<HorarioDto> {
						new() { Dia = "Lunes",     HoraEntrada = "18:00", HoraSalida = "22:00" },
						new() { Dia = "Martes",    HoraEntrada = "18:00", HoraSalida = "22:00" },
						new() { Dia = "Miercoles", HoraEntrada = "18:00", HoraSalida = "22:00" },
						new() { Dia = "Jueves",    HoraEntrada = "18:00", HoraSalida = "22:00" },
					}
				},
				new() {
					Nombre = "Valeria Montoya", Documento = "1005556677",
					ZonaResidencia = "San Vicente",
					Institucion = "Colegio San José", Tipo = "Escolar",
					Horarios = new List<HorarioDto>()
				},
				new() {
					Nombre = "Juan David Gómez", Documento = "1098765432",
					ZonaResidencia = "San Vicente", 
					Institucion = "Universidad de Antioquia", Tipo = "Universitario",
					Horarios = new List<HorarioDto> {
						new() { Dia = "Lunes",  HoraEntrada = "07:00", HoraSalida = "12:00" },
						new() { Dia = "Martes", HoraEntrada = "14:00", HoraSalida = "18:00" },
						new() { Dia = "Jueves", HoraEntrada = "07:00", HoraSalida = "12:00" },
						new() { Dia = "Sabado", HoraEntrada = "08:00", HoraSalida = "12:00" },
					}
				},
				new() {
					Nombre = "Daniela Ríos", Documento = "1003331122",
					ZonaResidencia = "Las Partidas", 
					Institucion = "EAFIT", Tipo = "Universitario",
					Horarios = new List<HorarioDto> {
						new() { Dia = "Martes", HoraEntrada = "09:00", HoraSalida = "13:00" },
						new() { Dia = "Jueves", HoraEntrada = "09:00", HoraSalida = "13:00" },
						new() { Dia = "Sabado", HoraEntrada = "09:00", HoraSalida = "14:00" },
					}
				},
				new() {
					Nombre = "Miguel Herrera", Documento = "1007778899",
					ZonaResidencia = "La Enea",
					Institucion = "Universidad Nacional", Tipo = "Universitario",
					Horarios = new List<HorarioDto> {
						new() { Dia = "Lunes",     HoraEntrada = "07:00", HoraSalida = "11:00" },
						new() { Dia = "Martes",    HoraEntrada = "07:00", HoraSalida = "11:00" },
						new() { Dia = "Miercoles", HoraEntrada = "14:00", HoraSalida = "18:00" },
						new() { Dia = "Viernes",   HoraEntrada = "07:00", HoraSalida = "11:00" },
					}
				},
				new() {
					Nombre = "Sofia Cardona", Documento = "1001112233",
					ZonaResidencia = "Chaparral",
					Institucion = "Colegio La Presentación", Tipo = "Escolar",
					Horarios = new List<HorarioDto>()
				},
			};

			foreach (var e in ejemplos)
				RegistrarEstudiante(e);
		}
	}
}