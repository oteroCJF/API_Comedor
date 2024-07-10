using MediatR;
using Comedor.Domain.DEntregables;

namespace Comedor.Service.EventHandler.Commands.Entregables.Delete
{
    public class EntregableDeleteCommand : IRequest<Entregable>
    {
        public int CedulaEvaluacionId { get; set; }
        public int EntregableId { get; set; }
        public string UsuarioId { get; set; }
        public string TipoEntregable { get; set; }
        public string Archivo { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Folio { get; set; }
    }
}
