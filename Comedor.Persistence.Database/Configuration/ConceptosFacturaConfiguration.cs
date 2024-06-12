using Comedor.Domain.DFacturas;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Persistence.Database.Configuration
{
    public class ConceptosFacturaConfiguration
    {
        public ConceptosFacturaConfiguration(EntityTypeBuilder<ConceptosFactura> entityBuilder)
        {
            entityBuilder.HasKey(x => new { x.FacturaId, x.Cantidad, x.ClaveProducto, x.ClaveUnidad, x.Unidad, x.Descripcion, x.PrecioUnitario, x.Subtotal, x.Descuento, x.IVA, x.FechaCreacion });
        }
    }
}
