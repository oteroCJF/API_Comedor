using Comedor.Domain.DIncidencias;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Persistence.Database.Configuration
{
    public class DetalleIncidenciaConfiguration
    {
        public DetalleIncidenciaConfiguration(EntityTypeBuilder<DetalleIncidencia> entityBuilder)
        {
            entityBuilder.HasKey(x => new { x.IncidenciaId, x.CIncidenciaId, x.MontoPenalizacion });
        }
    }
}
