using MediatR;
using Comedor.Domain.DIncidencias;
using System;

namespace Comedor.Service.EventHandler.Commands.Incidencias
{
    public class IncidenciaDeleteCommand : IRequest<int>
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int Pregunta { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }
}
