using Comedor.Domain.DOficios;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Persistence.Database.Configuration
{
    public class DetalleOficioConfiguration
    {
        public DetalleOficioConfiguration(EntityTypeBuilder<DetalleOficio> entityBuilder)
        {
            entityBuilder.HasKey(x => new { x.ServicioId, x.OficioId, x.FacturaId, x.CedulaId });
        }
    }
}
