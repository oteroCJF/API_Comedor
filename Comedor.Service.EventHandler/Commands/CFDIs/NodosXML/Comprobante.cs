namespace Comedor.Service.EventHandler.Commands.CFDIs.NodosXML
{
    public class Comprobante
    {
        public string Serie { get; set; }
        public long Folio { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}