using MediatR;
using Comedor.Domain.DOficios;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Oficios
{
    public class DetalleOficioDeleteCommand : IRequest<DetalleOficio>
    {
        public int ServicioId { get; set; }
        public int OficioId { get; set; }
        public int FacturaId { get; set; }
        public int CedulaId { get; set; }
    }
}
