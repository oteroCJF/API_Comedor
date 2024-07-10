using MediatR;
using Comedor.Domain.DEntregables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Entregables.Create
{
    public class EntregableCreateCommand : IRequest<Entregable>
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public string? UsuarioId { get; set; }
        public int EstatusId { get; set; }
        public int EntregableId { get; set; }
        public string Entregable { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
