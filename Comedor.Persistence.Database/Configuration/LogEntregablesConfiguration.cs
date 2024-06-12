using Comedor.Domain.DHistorialEntregables;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class LogEntregablesConfiguration
    {
        public LogEntregablesConfiguration(EntityTypeBuilder<LogEntregable> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}
