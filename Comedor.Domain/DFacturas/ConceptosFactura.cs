using System;
using System.ComponentModel.DataAnnotations;

namespace Comedor.Domain.DFacturas
{
    public class ConceptosFactura
    {
        [Key]
        public int FacturaId { get; set; }
        public decimal Cantidad { get; set; }
        public long ClaveProducto { get; set; }
        public string ClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaServicio { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}