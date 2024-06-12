using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.Queries.DTOs.Oficios
{
    public class DetalleOficioDto
    {
        public int ServicioId { get; set; }
        public int OficioId { get; set; }
        public int FacturaId { get; set; }
        public int CedulaId { get; set; }
    }
}
