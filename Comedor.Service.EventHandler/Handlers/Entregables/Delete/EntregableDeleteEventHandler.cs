using Comedor.Persistence.Database;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.IO;
using Comedor.Domain.DEntregables;
using Comedor.Service.EventHandler.Commands.Entregables.Delete;

namespace Comedor.Service.EventHandler.Handlers.Entregables.Delete
{
    public class EntregableDeleteEventHandler : IRequestHandler<EntregableDeleteCommand, Entregable>
    {
        private readonly ApplicationDbContext _context;

        public EntregableDeleteEventHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entregable> Handle(EntregableDeleteCommand request, CancellationToken cancellationToken)
        {
            var cedula = _context.CedulaEvaluacion.SingleOrDefault(c => c.Id == request.CedulaEvaluacionId);
            Entregable entregable = _context.Entregables
                .Where(e => e.CedulaEvaluacionId == request.CedulaEvaluacionId && 
                            e.EntregableId == request.EntregableId &&
                           !e.FechaEliminacion.HasValue).FirstOrDefault();
            DateTime fechaEliminacion = DateTime.Now;

            eliminaArchivoActual(cedula.Anio, request.Mes, cedula.Folio, entregable.Archivo, request.TipoEntregable);

            entregable.FechaEliminacion = fechaEliminacion;

            await _context.SaveChangesAsync();

            return entregable;
        }

        public bool eliminaArchivoActual(int anio, string mes, string folio, string archivo, string tipoEntregable)
        {
            try
            {
                string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + anio + "" + "\\" + mes + "\\" + folio + "\\" + tipoEntregable;

                if (archivo != null && !archivo.Equals("")) {
                    newPath += "\\" + archivo;

                    File.Delete(newPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return false;
            }
        }
    }
}
