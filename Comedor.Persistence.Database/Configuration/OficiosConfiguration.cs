using Comedor.Domain.DOficios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class OficiosConfiguration
    {
        public OficiosConfiguration(EntityTypeBuilder<Oficio> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property(x => x.EstatusId).HasDefaultValue(2);
        }
    }
}
