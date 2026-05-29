namespace SistemaMovilidad.Objetos.Models
{
	public enum DiaSemana
	{
		Lunes,
		Martes,
		Miercoles,
		Jueves,
		Viernes,
		Sabado
	}

	public class HorarioEstudiante
	{
		public int Id { get; set; }
		public int EstudianteId { get; set; }
		public DiaSemana Dia { get; set; }
		public string HoraEntrada { get; set; } = "00:00";
		public string HoraSalida { get; set; } = "00:00";

		public bool AsistEseDia => HoraEntrada != "00:00" && HoraSalida != "00:00";
	}
}
