using Comedor.Service.EventHandler.Commands.LogEntregables;
using Comedor.Service.Queries.DTOs.LogEntregables;
using Comedor.Service.Queries.Queries.LogEntregables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comedor.Api.Controllers.LogEntregables.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/logEntregables")]
    public class LEntregableQueryController : ControllerBase
    {
        private readonly ILogEntregablesQueryService _logs;

        public LEntregableQueryController(ILogEntregablesQueryService logs)
        {
            _logs = logs;
        }

        [HttpGet]
        [Route("getHistorialEntregablesByCedula/{cedula}")]
        public async Task<List<LogEntregableDto>> GetHistorialEntregablesByCedula(int cedula)
        {
            return await _logs.GetHistorialEntregablesByCedula(cedula);
        }
    }
}
