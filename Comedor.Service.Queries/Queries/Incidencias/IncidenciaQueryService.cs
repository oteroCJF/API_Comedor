using Comedor.Persistence.Database;
using Comedor.Service.Queries.DTOs.Incidencias;
using Comedor.Service.Queries.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comedor.Service.Queries.Queries.Incidencias
{
    public interface IIncidenciasQueryService
    {
        Task<List<IncidenciaDto>> GetAllIncidenciasAsync();
        Task<List<IncidenciaDto>> GetIncidenciasByCedula(int cedula);
        Task<List<DetalleIncidenciaDto>> GetDetalleIncidenciasById(int incidenciaId);
        Task<List<IncidenciaDto>> GetIncidenciasByPreguntaAndCedula(int cedula, int pregunta);
        Task<IncidenciaDto> GetIncidenciaByPreguntaAndCedula(int cedula, int pregunta);
        Task<List<ConfiguracionIncidenciaDto>> GetConfiguracionIncidencias();
        Task<ConfiguracionIncidenciaDto> GetConfiguracionIncidenciasByPregunta(int pregunta, bool respuesta);
    }

    public class IncidenciaQueryService : IIncidenciasQueryService
    {
        private readonly ApplicationDbContext _context;

        public IncidenciaQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<IncidenciaDto>> GetAllIncidenciasAsync()
        {
            try
            {
                var collection = await _context.Incidencias.Where(x => !x.FechaEliminacion.HasValue).OrderByDescending(x => x.Id).ToListAsync();

                return collection.MapTo<List<IncidenciaDto>>();
            }
            catch(Exception ex) 
            { 
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<IncidenciaDto>> GetIncidenciasByPreguntaAndCedula(int cedula, int pregunta)
        {
            try
            {
                var incidencias = await _context.Incidencias.Where(x => x.CedulaEvaluacionId == cedula && x.Pregunta == pregunta && !x.FechaEliminacion.HasValue)
                    .Select(i => new IncidenciaDto { 
                        Id = i.Id,
                        IncidenciaId = i.IncidenciaId,
                        CedulaEvaluacionId = i.CedulaEvaluacionId,
                        UsuarioId = i.UsuarioId,
                        TipoId = i.TipoId,
                        Pregunta = i.Pregunta,
                        FechaIncidencia = i.FechaIncidencia,
                        FechaProgramada = i.FechaProgramada,
                        FechaLimite = i.FechaLimite,
                        UltimoDia = i.UltimoDia,
                        FechaRealizada = i.FechaRealizada,
                        FechaInventario = i.FechaInventario,
                        FechaNotificacion = i.FechaNotificacion,
                        FechaAcordadaAdmin = i.FechaAcordadaAdmin,
                        FechaEntrega = i.FechaEntrega,
                        EntregaEnseres = i.EntregaEnseres,
                        HoraInicio = i.HoraInicio.ToString(),
                        HoraReal = i.HoraReal.ToString(),
                        Ponderacion = i.Ponderacion,
                        Cantidad = i.Cantidad,
                        Observaciones = i.Observaciones,
                        Penalizable = i.Penalizable,
                        MontoPenalizacion = i.MontoPenalizacion
                    })
                    .ToListAsync();

                return incidencias.MapTo<List<IncidenciaDto>>();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<List<IncidenciaDto>> GetIncidenciasByCedula(int cedula)
        {
            try
            {
                var incidencia = await _context.Incidencias.Where(x => x.CedulaEvaluacionId == cedula && !x.FechaEliminacion.HasValue).Select(i => new IncidenciaDto
                {
                    Id = i.Id,
                    IncidenciaId = i.IncidenciaId,
                    CedulaEvaluacionId = i.CedulaEvaluacionId,
                    UsuarioId = i.UsuarioId,
                    TipoId = i.TipoId,
                    Pregunta = i.Pregunta,
                    FechaIncidencia = i.FechaIncidencia,
                    FechaProgramada = i.FechaProgramada,
                    UltimoDia = i.UltimoDia,
                    FechaRealizada = i.FechaRealizada,
                    FechaInventario = i.FechaInventario,
                    FechaNotificacion = i.FechaNotificacion,
                    FechaAcordadaAdmin = i.FechaAcordadaAdmin,
                    FechaEntrega = i.FechaEntrega,
                    EntregaEnseres = i.EntregaEnseres,
                    HoraInicio = i.HoraInicio.ToString(),
                    HoraReal = i.HoraReal.ToString(),
                    Cantidad = i.Cantidad,
                    Ponderacion = i.Ponderacion,
                    Observaciones = i.Observaciones,
                    Penalizable = i.Penalizable,
                    MontoPenalizacion = i.MontoPenalizacion
                }).ToListAsync();

                return incidencia.MapTo<List<IncidenciaDto>>();
            }
            catch (Exception ex)
            { 
                string msg = ex.Message; 
                return null;
            }
        }
        
        public async Task<IncidenciaDto> GetIncidenciaByPreguntaAndCedula(int cedula, int pregunta)
        {
            try
            {
                var incidencia = await _context.Incidencias.SingleOrDefaultAsync(x => x.CedulaEvaluacionId == cedula && x.Pregunta == pregunta && !x.FechaEliminacion.HasValue);

                return incidencia.MapTo<IncidenciaDto>();
            }
            catch (Exception ex)
            { 
                string msg = ex.Message; 
                return null;
            }
        }

        public async Task<List<DetalleIncidenciaDto>> GetDetalleIncidenciasById(int id)
        {
            var incidencias = await _context.DetalleIncidencias.Where(dt => dt.IncidenciaId == id).ToListAsync();

            return incidencias.MapTo<List<DetalleIncidenciaDto>>();
        }

        public async Task<List<ConfiguracionIncidenciaDto>> GetConfiguracionIncidencias()
        {
            try
            {
                var incidencia = await _context.ConfiguracionIncidencias.ToListAsync();

                return incidencia.MapTo<List<ConfiguracionIncidenciaDto>>();
            }
            catch(Exception ex) 
            { 
                string message = ex.Message;
                return null;
            }
        }

        public async Task<ConfiguracionIncidenciaDto> GetConfiguracionIncidenciasByPregunta(int pregunta, bool respuesta)
        {
            try
            {
                var incidencia = await _context.ConfiguracionIncidencias.SingleAsync(cn => cn.Pregunta == pregunta && cn.Respuesta == respuesta);

                return incidencia.MapTo<ConfiguracionIncidenciaDto>();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

    }
}
