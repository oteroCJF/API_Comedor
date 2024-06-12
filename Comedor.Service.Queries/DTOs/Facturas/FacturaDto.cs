using System;
using System.Collections.Generic;

namespace Comedor.Service.Queries.DTOs.Facturas
{
    public class FacturaDto
    {
        public int Id { get; set; }
        public int RepositorioId { get; set; }
        public int EstatusId { get; set; }
        public int? InmuebleId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Facturacion { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public long? Folio { get; set; }
        public string UsoCFDI { get; set; } = string.Empty;
        public string UUID { get; set; } = string.Empty;
        public string UUIDRelacionado { get; set; } = string.Empty;
        public DateTime? FechaTimbrado { get; set; }
        public decimal? IVA { get; set; }
        public decimal? RetencionIVA { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Total { get; set; }
        public string ArchivoXML { get; set; } = string.Empty;
        public string ArchivoPDF { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public Nullable<DateTime> FechaEliminacion { get; set; }

        public string Observaciones { get; set; } = string.Empty;
    }
}
