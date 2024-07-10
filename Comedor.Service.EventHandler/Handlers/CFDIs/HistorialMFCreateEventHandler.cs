using MediatR;
using Comedor.Domain.DFacturas;
using Comedor.Persistence.Database;
using Comedor.Service.EventHandler.Commands.CFDIs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comedor.Service.EventHandler.Handlers.CFDIs
{
    public class HistorialMFCreateEventHandler : IRequestHandler<HistorialMFCreateCommand, HistorialMF>
    {
        private readonly ApplicationDbContext _context;

        public HistorialMFCreateEventHandler(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<HistorialMF> Handle(HistorialMFCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var historial = new HistorialMF
                {
                    Anio = request.Anio,
                    Mes = request.Mes,
                    RepositorioId = request.RepositorioId,
                    InmuebleId = request.InmuebleId,
                    UsuarioId = request.UsuarioId,
                    TipoArchivo = request.TipoArchivo,
                    Facturacion = request.Facturacion,
                    ArchivoXML = request.ArchivoXML,
                    ArchivoPDF = request.ArchivoPDF,
                    Observaciones = request.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                await _context.AddAsync(historial);
                await _context.SaveChangesAsync();

                return historial;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
