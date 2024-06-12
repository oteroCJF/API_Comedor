using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.Queries.DTOs.Incidencias
{
    public class DetalleIncidenciaDto
    {
        public int IncidenciaId { get; set; }
        public int CIncidenciaId { get; set; }
        public decimal MontoPenalizacion { get; set; }
    }
}
