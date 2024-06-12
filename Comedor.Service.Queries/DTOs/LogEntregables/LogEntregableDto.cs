using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.Queries.DTOs.LogEntregables
{
    public class LogEntregableDto
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int EstatusId { get; set; }
        public int EntregableId { get; set; }
        public string UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
