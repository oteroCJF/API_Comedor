using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.Queries.DTOs.Entregables
{
    public class EntregableDto
    {
        public int Id { get; set; }
        public string? UsuarioId { get; set; } = string.Empty;
        public Nullable<int> FacturaId { get; set; }
        public int EntregableId { get; set; }
        public int EstatusId { get; set; } = 0;
        public int CedulaEvaluacionId { get; set; }
        public string Archivo { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public System.Nullable<bool> Validado { get; set; }
        public System.Nullable<DateTime> FechaCreacion { get; set; }
        public System.Nullable<DateTime> FechaActualizacion { get; set; }
        public System.Nullable<DateTime> FechaEliminacion { get; set; }
    }
}
