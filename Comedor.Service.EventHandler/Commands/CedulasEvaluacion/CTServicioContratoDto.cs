using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.CedulasEvaluacion
{
    public class CTServicioContratoDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string Abreviacion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
