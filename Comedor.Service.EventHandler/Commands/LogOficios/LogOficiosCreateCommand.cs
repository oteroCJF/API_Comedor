using MediatR;
using Comedor.Domain.DHistorialOficios;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.LogOficios
{
    public class LogOficiosCreateCommand : IRequest<LogOficio>
    {
        public int Id { get; set; }
        public int OficioId { get; set; }
        public int EstatusId { get; set; }
        public string UsuarioId { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

}
