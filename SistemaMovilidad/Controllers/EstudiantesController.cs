using Microsoft.AspNetCore.Mvc;
using Serilog;
using SistemaMovilidad.Interfaces;
using static SistemaMovilidad.Objetos.DTOs.EstudianteDtos;

namespace SistemaMovilidad.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EstudiantesController : ControllerBase
	{
		private IEstudiante iEstudiante;

		public EstudiantesController(IEstudiante estudiante)
		{
			this.iEstudiante = estudiante;
		}

		[HttpGet("ObtenerTodos")]
		public List<EstudianteResponse> ObtenerTodos()
		{
			Log.Information("Consulta: ObtenerTodos ejecutado.");
			return this.iEstudiante.ObtenerTodos();
		}

		[HttpPost("RegistrarEstudiante")]
		public ActionResult<EstudianteResponse> Registrar([FromBody] RegistroEstudianteRequest request)
		{
			try
			{
				var resultado = this.iEstudiante.RegistrarEstudiante(request);
				return Ok(resultado);
			}
			catch (InvalidOperationException ex)
			{
				Log.Warning("Registro duplicado: {Mensaje}", ex.Message);
				return Conflict(new { mensaje = ex.Message });
			}
			catch (ArgumentException ex)
			{
				Log.Warning("Datos inválidos: {Mensaje}", ex.Message);
				return BadRequest(new { mensaje = ex.Message });
			}
		}

		[HttpGet("ConsultarPorZona")]
		public List<EstudianteResponse> ConsultarPorZona(string zona)
		{
			return this.iEstudiante.ConsultarPorZona(zona);
		}

		[HttpGet("ConsultarPorUniversidad")]
		public List<EstudianteResponse> ConsultarPorUniversidad(string institucion)
		{
			return this.iEstudiante.ConsultarPorUniversidad(institucion);
		}

		[HttpGet("ConsultarPorHorario")]
		public List<EstudianteResponse> ConsultarPorHorario(string dia, string? jornada = null)
		{
			return this.iEstudiante.ConsultarPorHorario(dia, jornada);
		}

		[HttpGet("ReporteHorarioPorDia")]
		public ReporteHorarioDiaResponse ReporteHorarioPorDia(string dia)
		{
			return this.iEstudiante.GenerarReporteHorarioPorDia(dia);
		}
	}
}