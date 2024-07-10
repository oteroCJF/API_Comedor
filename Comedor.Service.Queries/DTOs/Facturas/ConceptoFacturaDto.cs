using System;

namespace Comedor.Service.Queries.DTOs.Facturas
{
    public class ConceptoFacturaDto
    {
        public long FacturaId { get; set; }
        public decimal Cantidad { get; set; }
        public long ClaveProducto { get; set; }
        public string ClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaServicio { get; set; }
        public string ObservacionGeneral { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
    }
}
