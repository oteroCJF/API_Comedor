using MediatR;
using Comedor.Service.EventHandler.Commands.Oficios;
using Comedor.Service.Queries.Queries.Oficios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comedor.Service.EventHandler.Commands.LogOficios;

namespace Comedor.Api.Controllers.Oficios.Commands
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/oficios")]
    public class OficioCommandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OficioCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Produces("application/json")]
        [Consumes("multipart/form-data")]
        [Route("createOficio")]
        [HttpPost]
        public async Task<IActionResult> CreateOficio([FromForm] OficioCreateCommand request)
        {
            var oficio = await _mediator.Send(request);
            return Ok(oficio);
        }

        [Route("createDetalleOficio")]
        [HttpPost]
        public async Task<IActionResult> CreateDetalleOficio([FromBody] List<DetalleOficioCreateCommand> request)
        {
            try
            {
                foreach (var dt in request)
                {
                    await _mediator.Send(dt);
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("deleteDetalleOficio")]
        [HttpPost]
        public async Task<IActionResult> DeleteDetalleOficio([FromBody] DetalleOficioDeleteCommand request)
        {
            var oficio = await _mediator.Send(request);
            return Ok(oficio);
        }

        [Route("cancelarOficio")]
        [HttpPost]
        public async Task<IActionResult> CancelarOficio([FromBody] CancelarOficioCommand request)
        {
            try
            {
                var oficio = await _mediator.Send(request);
                if (oficio != null)
                {
                    LogOficiosCreateCommand logOficio = new LogOficiosCreateCommand
                    {
                        OficioId = request.Id,
                        UsuarioId = request.UsuarioId,
                        EstatusId = request.ESucesivoId,
                        Observaciones = request.Observaciones
                    };

                    var log = await _mediator.Send(logOficio);
                }
                return Ok(oficio);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }
        }

        [Route("corregirOficio")]
        [HttpPost]
        public async Task<IActionResult> CorregirOficio([FromBody] CorregirOficioCommand request)
        {
            try
            {
                var oficio = await _mediator.Send(request);
                if (oficio != null)
                {
                    LogOficiosCreateCommand logOficio = new LogOficiosCreateCommand
                    {
                        OficioId = request.Id,
                        UsuarioId = request.UsuarioId,
                        EstatusId = request.ESucesivoId,
                        Observaciones = request.Observaciones
                    };

                    var log = await _mediator.Send(logOficio);
                }
                return Ok(oficio);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }
        }

        [Route("enviarDGPPTOficio")]
        [HttpPost]
        public async Task<IActionResult> EnviarDGPPTOficio([FromBody] EDGPPTOficioCommand request)
        {
            try
            {
                var oficio = await _mediator.Send(request);
                if (oficio != null)
                {
                    LogOficiosCreateCommand logOficio = new LogOficiosCreateCommand
                    {
                        OficioId = request.Id,
                        UsuarioId = request.UsuarioId,
                        EstatusId = request.ESucesivoId,
                        Observaciones = request.Observaciones
                    };

                    var log = await _mediator.Send(logOficio);
                }
                return Ok(oficio);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }
        }

        [Route("pagarOficio")]
        [HttpPost]
        public async Task<IActionResult> PagarOficio([FromBody] PagarOficioCommand request)
        {
            try
            {
                var oficio = await _mediator.Send(request);
                if (oficio != null)
                {
                    LogOficiosCreateCommand logOficio = new LogOficiosCreateCommand
                    {
                        OficioId = request.Id,
                        UsuarioId = request.UsuarioId,
                        EstatusId = request.ESucesivoId,
                        Observaciones = request.Observaciones
                    };

                    var log = await _mediator.Send(logOficio);
                }
                return Ok(oficio);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }
        }

    }
}
