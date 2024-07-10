using MediatR;
using Comedor.Domain.DOficios;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Oficios
{
    public class EDGPPTOficioCommand : IRequest<Oficio>
    {
        public int Id { get; set; }
        public int ESucesivoId { get; set; }
        public int EFacturaId { get; set; }
        public int ECedulaId { get; set; }
        public string UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
