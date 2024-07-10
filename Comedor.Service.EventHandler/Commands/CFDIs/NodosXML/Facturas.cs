using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Service.EventHandler.Commands.CFDIs.NodosXML
{
    public class Facturas
    {
        public int Id { get; set; }
        public int RepositorioId { get; set; }
        public int InmuebleId { get; set; }
        public string UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Facturacion { get; set; }
        public string Archivo { get; set; }
        public string ArchivoPDF { get; set; }
        public Comprobante Comprobante { get; set; }
        public Emisor Emisor { get; set; }
        public List<Concepto> Conceptos { get; set; }
        public Concepto Mconcepto { get; set; }
        public TimbreFiscal TimbreFiscal { get; set; }
        public Traslado Traslado { get; set; }
        public Receptor Receptor { get; set; }
        public Retencion Retencion { get; set; }
        public CFDIRelacionado CfdiRelacionado { get; set; }
        public Impuestos Impuestos { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
