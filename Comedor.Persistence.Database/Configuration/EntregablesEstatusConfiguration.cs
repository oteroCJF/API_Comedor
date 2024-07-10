using Comedor.Domain.DEntregables;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class EntregablesEstatusConfiguration
    {
        public EntregablesEstatusConfiguration(EntityTypeBuilder<EntregableEstatus> entityBuilder)
        {
            entityBuilder.HasKey(x => new { x.EntregableId, x.EstatusId, x.Multiple });
        }
    }
}
