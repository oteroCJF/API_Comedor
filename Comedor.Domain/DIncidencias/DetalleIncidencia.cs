using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Domain.DIncidencias
{
    public class DetalleIncidencia
    {
        public int IncidenciaId { get; set; }
        public int CIncidenciaId { get; set; }
        public decimal MontoPenalizacion { get; set; }
    }
}
