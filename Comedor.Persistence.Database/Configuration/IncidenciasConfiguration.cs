using Comedor.Domain.DIncidencias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comedor.Persistence.Database.Configuration
{
    public class IncidenciasConfiguration
    {
        public IncidenciasConfiguration(EntityTypeBuilder<Incidencia> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property(x => x.Cantidad).HasDefaultValue(1);
        }
    }
}
