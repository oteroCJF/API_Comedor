using System;

namespace Comedor.Domain.DFacturas
{
    public class Factura
    {
        public int Id { get; set; }
        public int RepositorioId { get; set; }
        public Nullable<int> InmuebleId { get; set; }
        public int EstatusId { get; set; }
        public string UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Facturacion { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string Serie { get; set; }
        public Nullable<long> Folio { get; set; }
        public string UsoCFDI { get; set; }
        public string UUID { get; set; }
        public string UUIDRelacionado { get; set; }
        public Nullable<DateTime> FechaTimbrado { get; set; }
        public Nullable<decimal> IVA { get; set; }
        public Nullable<decimal> RetencionIVA { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; } = decimal.Zero;
        public string ArchivoXML { get; set; }
        public string ArchivoPDF { get; set; }
        public Nullable<DateTime> FechaCreacion { get; set; }
        public Nullable<DateTime> FechaActualizacion { get; set; }
        public Nullable<DateTime> FechaEliminacion { get; set; }
    }
}
