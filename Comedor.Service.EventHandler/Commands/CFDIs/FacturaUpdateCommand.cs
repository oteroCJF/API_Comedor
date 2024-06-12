using MediatR;
using Comedor.Domain.DFacturas;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.Facturas
{
    public class FacturaUpdateCommand : IRequest<Factura>
    {
        public int Id {  get; set; }
        public int Anio { get; set; }
        public string Mes {  get; set; }
        public string Inmueble { get; set; }
        public IFormFile PDF { get; set; }
        public System.Nullable<DateTime> FechaActualizacion { get; set; }

    }
}
