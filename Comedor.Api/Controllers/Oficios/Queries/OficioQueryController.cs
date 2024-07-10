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

namespace Comedor.Api.Controllers.Oficios.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/oficios")]
    public class OficioQueryController : ControllerBase
    {
        private readonly IOficioQueryService _oficios;

        public OficioQueryController(IOficioQueryService oficios)
        {
            _oficios = oficios;
        }

        public async Task<IActionResult> GetAllOficiosAsync()
        {
            var oficios = await _oficios.GetAllOficiosAsync();
            return Ok(oficios);
        }

        [HttpGet]
        [Route("getOficiosByAnio/{anio}")]
        public async Task<IActionResult> GetOficiosByAnio(int anio)
        {
            var oficios = await _oficios.GetOficiosByAnio(anio);
            return Ok(oficios);
        }

        [HttpGet]
        [Route("getOficioById/{id}")]
        public async Task<IActionResult> GetOficioById(int id)
        {
            var oficio = await _oficios.GetOficioById(id);
            return Ok(oficio);
        }

        [HttpGet]
        [Route("getDetalleOficio/{id}")]
        public async Task<IActionResult> GetDetalleOficio(int id)
        {
            var oficio = await _oficios.GetDetalleOficio(id);
            return Ok(oficio);
        }

    }
}
