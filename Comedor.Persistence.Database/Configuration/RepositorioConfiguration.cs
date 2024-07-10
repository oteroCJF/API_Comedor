using Comedor.Domain.DRepositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class RepositorioConfiguration
    {
        public RepositorioConfiguration(EntityTypeBuilder<Repositorio> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property(x => x.EstatusId).HasDefaultValue(1);
        }
    }
}
