namespace Comedor.Service.EventHandler.Commands.CFDIs.NodosXML
{
    public class Impuestos
    {
        public decimal Base { get; set; }
        public decimal Impuesto { get; set; }
        public string TipoFactor { get; set; }
        public decimal TasaOCuota { get; set; }
        public decimal Importe { get; set; }
    }
}