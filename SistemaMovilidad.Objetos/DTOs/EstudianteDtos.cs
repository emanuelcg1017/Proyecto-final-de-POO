namespace SistemaMovilidad.Objetos.DTOs
{
	public class EstudianteDtos
	{
		public class HorarioDto
		{
			public string Dia { get; set; } = string.Empty;
			public string HoraEntrada { get; set; } = "00:00";
			public string HoraSalida { get; set; } = "00:00";
		}

		public class RegistroEstudianteRequest
		{
			public string Nombre { get; set; } = string.Empty;
			public string Documento { get; set; } = string.Empty;
			public string ZonaResidencia { get; set; } = string.Empty;
			public string Institucion { get; set; } = string.Empty;
			public string Tipo { get; set; } = "Universitario";
			public List<HorarioDto> Horarios { get; set; } = new();
		}

		public class EstudianteResponse
		{
			public int Id { get; set; }
			public string Nombre { get; set; } = string.Empty;
			public string Documento { get; set; } = string.Empty;
			public string ZonaResidencia { get; set; } = string.Empty;
			public string Institucion { get; set; } = string.Empty;
			public string Tipo { get; set; } = string.Empty;
			public List<HorarioDto> Horarios { get; set; } = new();
		}

		public class FranjaHorariaDto
		{
			public string Hora { get; set; } = string.Empty;
			public int Cantidad { get; set; }
		}

		public class ReporteHorarioDiaResponse
		{
			public string Dia { get; set; } = string.Empty;
			public int TotalEstudiantesEseDia { get; set; }
			public List<FranjaHorariaDto> Entradas { get; set; } = new();
			public List<FranjaHorariaDto> Salidas { get; set; } = new();
		}
	}
}
