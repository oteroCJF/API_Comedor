using MediatR;
using System;

namespace Comedor.Service.EventHandler.Commands.Convenios
{
    public class ConvenioDeleteCommand : IRequest<int>
    {
        public int Id { get; set; }
        public System.Nullable<DateTime> FechaEliminacion { get; set; }
    }
}
