using Comedor.Domain.DCuestionario;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comedor.Persistence.Database.Configuration
{
    public class CuestionarioConfiguration
    {
        public CuestionarioConfiguration(EntityTypeBuilder<Cuestionario> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}
