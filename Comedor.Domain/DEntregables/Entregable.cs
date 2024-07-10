using System;

namespace Comedor.Domain.DEntregables
{
    public class Entregable
    {
        public int Id { get; set; }
        public Nullable<int> FacturaId { get; set; }
        public int EntregableId { get; set; }
        public int EstatusId { get; set; } = 0;
        public string? UsuarioId { get; set; } = string.Empty;
        public int CedulaEvaluacionId { get; set; }
        public string Archivo { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public Nullable<bool> Validado { get; set; }
        public Nullable<DateTime> FechaCreacion { get; set; }
        public Nullable<DateTime> FechaActualizacion { get; set; }
        public Nullable<DateTime> FechaEliminacion { get; set; }
    }
}
