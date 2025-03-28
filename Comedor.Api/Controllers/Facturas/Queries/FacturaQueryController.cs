using Comedor.Service.EventHandler.Commands.Facturas;
using Comedor.Service.Queries.DTOs.Facturas;
using Comedor.Service.Queries.Queries.Facturas;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comedor.Service.EventHandler.Commands.CFDIs;
using System.IO;
using Comedor.Service.Queries.DTOs.LogMasivoFacturas;

namespace Comedor.Api.Controllers.Facturas.Queries
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/comedor/cfdi")]
    public class FacturaQueryController : ControllerBase
    {
        private readonly ICFDIQueryService _facturas;
        private readonly IHostingEnvironment _environment;
        private readonly IMediator _mediator;

        public FacturaQueryController(ICFDIQueryService facturas, IMediator mediator, IHostingEnvironment environment)
        {
            _facturas = facturas;
            _mediator = mediator;
            _environment = environment;
        }

        [Route("getAllFacturas")]
        [HttpGet]
        public async Task<List<FacturaDto>> GetAllFacturas()
        {
            var facturas = await _facturas.GetAllFacturas();

            return facturas;
        }

        [Route("getFacturasByFacturacion/{facturacion}")]
        [HttpGet]
        public async Task<List<FacturaDto>> GetAllFacturas(int facturacion)
        {
            var facturas = await _facturas.GetAllFacturasAsync(facturacion);

            return facturas;
        }

        [Route("getFacturasByInmueble/{inmueble}/{facturacion}")]
        [HttpGet]                                                                                                                                                                                                                                          
        public async Task<List<FacturaDto>> GetFacturasByInmueble(int inmueble, int facturacion)
        {
            var facturas = await _facturas.GetFacturasByInmuebleAsync(inmueble, facturacion);

            return facturas;
        }

        [Route("getConceptosFacturaByIdAsync/{factura}")]
        [HttpGet]
        public async Task<List<ConceptoFacturaDto>> GetConceptosFacturaByIdAsync(int factura)
        {
            return await _facturas.GetConceptosFacturaByIdAsync(factura);
        }

        [Route("getFacturasNCPendientes/{estatus}")]
        [HttpGet]
        public async Task<List<FacturaDto>> getFacturasNCPendientes(int estatus)
        {
            return await _facturas.GetFacturasNCPendientes(estatus);
        }

        [Route("getFacturasCargadas/{facturacion}")]
        [HttpGet]
        public async Task<int> GetFacturasCargadas(int facturacion)
        {
            return await _facturas.GetFacturasCargadasAsync(facturacion);
        }
        
        [Route("getFacturacionByCedula/{cedula}")]
        [HttpGet]
        public decimal GetFacturacionByCedulas(int cedula)
        {
            return _facturas.GetFacturacionByCedulas(cedula);
        }

        [Route("getNotasCreditoCargadas/{facturacion}")]
        [HttpGet]
        public async Task<int> GetNotasCreditoCargadas(int facturacion)
        {
            return await _facturas.GetNotasCreditoCargadasAsync(facturacion);
        }

        [Route("getTotalFacturasByInmueble/{inmueble}/{facturacion}")]
        [HttpGet]
        public async Task<int> GetTotalFacturasByInmueble(int inmueble, int facturacion)
        {
            return await _facturas.GetTotalFacturasByInmuebleAsync(facturacion, inmueble);
        }

        [Route("getNCByInmueble/{inmueble}/{facturacion}")]
        [HttpGet]
        public async Task<int> GetNCByInmueble(int inmueble, int facturacion)
        {
            return await _facturas.GetNCByInmuebleAsync(facturacion, inmueble);
        }

        [Route("getHistorialMFByFacturacion/{facturacion}")]
        [HttpGet]
        public async Task<List<LogMasivoFacturaDto>> GetHistorialMFByFacturacion(int facturacion)
        {
            var historial = await _facturas.GetHistorialMFByFacturacion(facturacion);

            HistorialMFDeleteCommand delete = new HistorialMFDeleteCommand();

            delete.RepositorioId = facturacion;

            await _mediator.Send(delete);

            return historial;
        }

        [Route("visualizarFactura/{anio}/{mes}/{folio}/{tipo}/{inmueble}/{archivo}")]
        [HttpGet]
        public string VisualizarFactura(int anio, string mes, string folio, string tipo, string inmueble, string archivo)
        {
            string folderName = "";
            if (tipo.Equals("NC"))
            {
                folderName = Directory.GetCurrentDirectory() + "\\Repositorio\\" + anio + "\\" + mes + "\\" + inmueble + "\\Notas de Crédito\\" + folio;
            }
            else
            {
                folderName = Directory.GetCurrentDirectory() + "\\Repositorio\\" + anio + "\\" + mes + "\\" + inmueble + "\\Facturas\\" + folio;
            }
            string webRootPath = _environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                return pathArchivo;
            }

            return "";
        }
    }
}
