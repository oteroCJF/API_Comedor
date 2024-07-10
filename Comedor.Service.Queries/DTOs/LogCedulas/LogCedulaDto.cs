using System;

namespace Comedor.Service.Queries.DTOs.LogCedulas
{
    public class LogCedulaDto
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int EstatusId { get; set; }
        public string UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
