using System;

namespace Comedor.Domain.DCedulaEvaluacion
{
    public class RespuestaEvaluacion
    {
        public int CedulaEvaluacionId { get; set; }
        public int Pregunta { get; set; }
        public Nullable<bool> Respuesta { get; set; }
        public string? Detalles { get; set; } = string.Empty;
        public Nullable<bool> Penalizable { get; set; } = false;
        public Nullable<decimal> MontoPenalizacion { get; set; } = 0;
        public DateTime? FechaCreacion { get; set; } = Convert.ToDateTime("01/01/1990");
        public DateTime? FechaActualizacion { get; set; } = Convert.ToDateTime("01/01/1990");
        public DateTime? FechaEliminacion { get; set; } = Convert.ToDateTime("01/01/1990");
    }
}
