using Serilog;
using SistemaMovilidad.Datos.Repositorio;
using SistemaMovilidad.Interfaces;
using SistemaMovilidad.Objetos.Models;
using static SistemaMovilidad.Objetos.DTOs.EstudianteDtos;

namespace SistemaMovilidad.Services
{
	public class EstudianteServicio : IEstudiante
	{
		private readonly EstudianteRepositorio _repositorio;

		public EstudianteServicio()
		{
			_repositorio = new EstudianteRepositorio();
		}

		public List<EstudianteResponse> ObtenerTodos()
		{
			return _repositorio.ObtenerTodos().Select(MapearAResponse).ToList();
		}

		public EstudianteResponse RegistrarEstudiante(RegistroEstudianteRequest request)
		{
			if (_repositorio.ExisteDocumento(request.Documento))
				throw new InvalidOperationException($"Ya existe un estudiante con el documento {request.Documento}.");

			if (!Enum.TryParse<TipoEstudiante>(request.Tipo, true, out var tipo))
				throw new ArgumentException("El tipo debe ser 'Escolar' o 'Universitario'.");

			var estudiante = new Estudiante
			{
				Nombre = request.Nombre.Trim(),
				Documento = request.Documento.Trim(),
				ZonaResidencia = request.ZonaResidencia.Trim(),
				Institucion = request.Institucion.Trim(),
				Tipo = tipo
			};

			if (tipo == TipoEstudiante.Escolar && request.Horarios.Count == 0)
			{
				var dias = new[] { DiaSemana.Lunes, DiaSemana.Martes, DiaSemana.Miercoles, DiaSemana.Jueves, DiaSemana.Viernes };
				foreach (var dia in dias)
					estudiante.Horarios.Add(new HorarioEstudiante { Dia = dia, HoraEntrada = "07:00", HoraSalida = "13:00" });
			}
			else
			{
				foreach (var h in request.Horarios)
				{
					if (!Enum.TryParse<DiaSemana>(NormalizarDia(h.Dia), true, out var dia))
						throw new ArgumentException($"Día inválido: {h.Dia}");
					estudiante.Horarios.Add(new HorarioEstudiante { Dia = dia, HoraEntrada = h.HoraEntrada, HoraSalida = h.HoraSalida });
				}
			}

			var guardado = _repositorio.Agregar(estudiante);
			Log.Information("Estudiante registrado: {Nombre} - Documento: {Documento}", guardado.Nombre, guardado.Documento);
			return MapearAResponse(guardado);
		}

		public List<EstudianteResponse> ConsultarPorZona(string zona)
		{
			Log.Information("Consulta por zona: {Zona}", zona);
			return _repositorio.ObtenerTodos()
				.Where(e => e.ZonaResidencia.Contains(zona, StringComparison.OrdinalIgnoreCase))
				.Select(MapearAResponse).ToList();
		}

		public List<EstudianteResponse> ConsultarPorUniversidad(string institucion)
		{
			Log.Information("Consulta por universidad: {Institucion}", institucion);
			return _repositorio.ObtenerTodos()
				.Where(e => e.Institucion.Contains(institucion, StringComparison.OrdinalIgnoreCase))
				.Select(MapearAResponse).ToList();
		}

		public List<EstudianteResponse> ConsultarPorHorario(string dia, string? jornada)
		{
			if (!Enum.TryParse<DiaSemana>(NormalizarDia(dia), true, out var diaSemana))
				throw new ArgumentException($"Día inválido: {dia}");

			Log.Information("Consulta por horario: Dia={Dia}, Jornada={Jornada}", dia, jornada ?? "todas");

			var resultado = _repositorio.ObtenerTodos()
				.Where(e => e.Horarios.Any(h => h.Dia == diaSemana && h.AsistEseDia))
				.ToList();

			if (!string.IsNullOrEmpty(jornada))
				resultado = resultado.Where(e => e.Horarios.Any(h =>
					h.Dia == diaSemana && h.AsistEseDia &&
					ObtenerJornada(h.HoraEntrada) == jornada.ToLower())).ToList();

			return resultado.Select(MapearAResponse).ToList();
		}

		public ReporteHorarioDiaResponse GenerarReporteHorarioPorDia(string dia)
		{
			if (!Enum.TryParse<DiaSemana>(NormalizarDia(dia), true, out var diaSemana))
				throw new ArgumentException($"Día inválido: {dia}.");

			Log.Information("Reporte generado para: {Dia}", dia);

			var horariosDelDia = _repositorio.ObtenerTodos()
				.SelectMany(e => e.Horarios.Where(h => h.Dia == diaSemana && h.AsistEseDia))
				.ToList();

			return new ReporteHorarioDiaResponse
			{
				Dia = diaSemana.ToString(),
				TotalEstudiantesEseDia = horariosDelDia.Count,
				Entradas = horariosDelDia.GroupBy(h => h.HoraEntrada).OrderBy(g => g.Key)
					.Select(g => new FranjaHorariaDto { Hora = g.Key, Cantidad = g.Count() }).ToList(),
				Salidas = horariosDelDia.GroupBy(h => h.HoraSalida).OrderBy(g => g.Key)
					.Select(g => new FranjaHorariaDto { Hora = g.Key, Cantidad = g.Count() }).ToList()
			};
		}

		private static EstudianteResponse MapearAResponse(Estudiante e) => new()
		{
			Id = e.Id,
			Nombre = e.Nombre,
			Documento = e.Documento,
			ZonaResidencia = e.ZonaResidencia,
			Institucion = e.Institucion,
			Tipo = e.Tipo.ToString(),
			Horarios = e.Horarios.Select(h => new HorarioDto
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
	}
}