using System;

namespace Comedor.Domain.DHistorialOficios
{
    public class LogOficio
    {
        public int Id { get; set; }
        public int OficioId { get; set; }
        public int EstatusId { get; set; }
        public string UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
