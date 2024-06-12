using System;

namespace Comedor.Service.Queries.DTOs.CedulaEvaluacion
{
    public class RespuestaDto
    {
        public int CedulaEvaluacionId { get; set; }
        public int Pregunta { get; set; }
        public System.Nullable<bool> Respuesta { get; set; }
        public string? Detalles { get; set; } = string.Empty;
        public System.Nullable<bool> Penalizable { get; set; } = false;
        public System.Nullable<decimal> MontoPenalizacion { get; set; } = 0;
        public DateTime? FechaCreacion { get; set; } = Convert.ToDateTime("01/01/1990");
        public DateTime? FechaActualizacion { get; set; } = Convert.ToDateTime("01/01/1990");
        public DateTime? FechaEliminacion { get; set; } = Convert.ToDateTime("01/01/1990");
    }
}
