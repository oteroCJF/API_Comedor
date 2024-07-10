using System;

namespace Comedor.Service.Queries.DTOs.EntregablesContratacion
{
    public class EntregableContratacionDto
    {
        public int Id { get; set; }
        public string? UsuarioId { get; set; } = string.Empty;
        public int ContratoId { get; set; }
        public Nullable<int> ConvenioId { get; set; }
        public int EntregableId { get; set; }
        public string Archivo { get; set; } = string.Empty;
        public Nullable<DateTime> FechaProgramada { get; set; }
        public Nullable<DateTime> FechaEntrega { get; set; }
        public Nullable<DateTime> InicioVigencia { get; set; }
        public Nullable<DateTime> FinVigencia { get; set; }
        public Nullable<decimal> MontoGarantia { get; set; }
        public Nullable<bool> Penalizable { get; set; }
        public Nullable<decimal> MontoPenalizacion { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public Nullable<DateTime> FechaCreacion { get; set; }
        public Nullable<DateTime> FechaActualizacion { get; set; }
        public Nullable<DateTime> FechaEliminacion { get; set; }
    }
}
