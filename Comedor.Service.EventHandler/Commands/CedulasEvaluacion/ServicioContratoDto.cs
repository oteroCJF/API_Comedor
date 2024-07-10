using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.CedulasEvaluacion
{
    public class ServicioContratoDto
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public int ServicioId { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public decimal PorcentajeImpuesto { get; set; }

        public virtual CTServicioContratoDto Servicio { get; set; }
    }
}
