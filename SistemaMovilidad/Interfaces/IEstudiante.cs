using static SistemaMovilidad.DTOs.EstudianteDtos;

namespace SistemaMovilidad.Interfaces
{
	public interface IEstudiante
	{
		List<EstudianteResponse> ObtenerTodos();
		EstudianteResponse RegistrarEstudiante(RegistroEstudianteRequest request);
		List<EstudianteResponse> ConsultarPorZona(string zona);
		List<EstudianteResponse> ConsultarPorUniversidad(string institucion);
		List<EstudianteResponse> ConsultarPorHorario(string dia, string? jornada);
		ReporteHorarioDiaResponse GenerarReporteHorarioPorDia(string dia);
	}
}
