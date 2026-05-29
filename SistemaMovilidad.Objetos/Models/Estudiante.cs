namespace SistemaMovilidad.Objetos.Models
{

	public enum TipoEstudiante
	{
		Escolar,
		Universitario
	}

	public class Estudiante
	{
		public int Id { get; set; }
		public string Nombre { get; set; } = string.Empty;
		public string Documento { get; set; } = string.Empty;
		public string ZonaResidencia { get; set; } = string.Empty;
		public string Institucion { get; set; } = string.Empty;
		public TipoEstudiante Tipo { get; set; }
		public List<HorarioEstudiante> Horarios { get; set; } = new();
	}
}
