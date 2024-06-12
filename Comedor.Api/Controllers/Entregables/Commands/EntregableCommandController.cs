using Comedor.Service.EventHandler.Commands.Entregables;
using Comedor.Service.Queries.Queries.Entregables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Comedor.Service.EventHandler.Commands.LogEntregables;
using System;
using Comedor.Service.EventHandler.Commands.Entregables.Update;
using Comedor.Service.EventHandler.Commands.Entregables.Delete;
using Comedor.Service.EventHandler.Commands.Entregables.Create;

namespace Comedor.Api.Controllers.Entregables.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/entregables")]
    public class EntregableController : ControllerBase
    {
        private readonly IEntregableQueryService _entregables;
        private readonly IMediator _mediator;

        public EntregableController(IEntregableQueryService entregables, IMediator mediator)
        {
            _entregables = entregables;
            _mediator = mediator;
        }

        [Route("createEntregable")]
        [HttpPost]
        public async Task<IActionResult> CreateEntregable([FromBody] EntregableCreateCommand request)
        {
            var entregable = await _mediator.Send(request);

            if (entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = request.UsuarioId,
                    EstatusId = entregable.EstatusId,
                    Observaciones = "Se liberó el entregable " + request.Entregable,
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok(entregable);
        }

        [Consumes("multipart/form-data")]
        [Route("actualizarEntregable")]
        [HttpPut]
        public async Task<IActionResult> UpdateEntregable([FromForm] EntregableUpdateCommand request)
        {
            var entregable = await _mediator.Send(request);
            if(entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = entregable.UsuarioId,
                    EstatusId = entregable.EstatusId,
                    Observaciones = "Se actualizó el entregable.",
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok();
        }

        [Route("deleteEntregable")]
        [HttpPut]
        public async Task<IActionResult> DeleteEntregable([FromBody] EntregableDeleteCommand request)
        {
            var entregable = await _mediator.Send(request);

            if (entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = request.UsuarioId,
                    EstatusId = entregable.EstatusId,
                    Observaciones = "Se eliminó el entregable " + request.TipoEntregable,
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok(entregable);
        }

        [Route("updateEntregableSR")]
        [HttpPut]
        public async Task<IActionResult> UpdateEntregableSR([FromBody] ESREntregableUpdateCommand request)
        {
            var entregable = await _mediator.Send(request);
            if (entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = entregable.UsuarioId,
                    EstatusId = !request.Aprobada ? entregable.EstatusId : request.EstatusId,
                    Observaciones = request.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok();
        }

        [Route("autorizarEntregable")]
        [HttpPut]
        public async Task<IActionResult> AUpdateEntregable([FromBody] EEntregableUpdateCommand request)
        {
            var entregable = await _mediator.Send(request);
            if (entregable != null)
            {
                var log = new LogEntregablesCreateCommand
                {
                    CedulaEvaluacionId = entregable.CedulaEvaluacionId,
                    EntregableId = entregable.EntregableId,
                    UsuarioId = request.UsuarioId,
                    EstatusId = entregable.EstatusId,
                    Observaciones = request.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                await _mediator.Send(log);
            }
            return Ok();
        }
    }
}
