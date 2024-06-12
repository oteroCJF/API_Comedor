using Comedor.Service.EventHandler.Commands.LogCedulas;
using Comedor.Service.Queries.DTOs.LogCedulas;
using Comedor.Service.Queries.Queries.LogCedulas;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.LogCedula.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/logCedulas")]
    public class LCedulaQueryController : ControllerBase
    {
        private readonly ILogCedulasQueryService _logs;

        public LCedulaQueryController(ILogCedulasQueryService logs)
        {
            _logs = logs;
        }

        [HttpGet]
        [Route("getHistorialByCedula/{cedula}")]
        public async Task<List<LogCedulaDto>> GetHistorialByCedula(int cedula)
        {
            return await _logs.GetHistorialCedula(cedula);
        }
    }
}
