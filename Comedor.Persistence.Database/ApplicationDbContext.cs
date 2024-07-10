using Comedor.Domain.DCedulaEvaluacion;
using Comedor.Domain.DContratos;
using Comedor.Domain.DCuestionario;
using Comedor.Domain.DEntregables;
using Comedor.Domain.DEntregablesContratacion;
using Comedor.Domain.DRepositorios;
using Comedor.Domain.DFacturas;
using Comedor.Domain.DFirmantes;
using Comedor.Domain.DHistorial;
using Comedor.Domain.DHistorialEntregables;
using Comedor.Domain.DIncidencias;
using Comedor.Persistence.Database.Configuration;
using Microsoft.EntityFrameworkCore;
using Comedor.Domain.DFlujos;
using Comedor.Domain.DOficios;

namespace Comedor.Persistence.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Comedor");

            ModelConfig(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<Repositorio> Repositorios{ get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<ConceptosFactura> ConceptosFactura { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Convenio> Convenio { get; set; }
        public DbSet<RubroConvenio> RubroConvenio { get; set; }
        public DbSet<ServicioContrato> ServicioContrato { get; set; }
        public DbSet<CedulaEvaluacion> CedulaEvaluacion { get; set; }
        public DbSet<Cuestionario> Cuestionarios { get; set; }
        public DbSet<CuestionarioMensual> CuestionarioMensual { get; set; }
        public DbSet<RespuestaEvaluacion> Respuestas { get; set; }
        public DbSet<Incidencia> Incidencias { get; set; }
        public DbSet<DetalleIncidencia> DetalleIncidencias { get; set; }
        public DbSet<ConfiguracionIncidencias> ConfiguracionIncidencias { get; set; }
        public DbSet<Entregable> Entregables { get; set; }
        public DbSet<EntregableEstatus> EntregablesEstatus { get; set; }
        public DbSet<EntregableContratacion> EntregableContratacion { get; set; }
        public DbSet<FlujoCedula> Flujo { get; set; }
        public DbSet<LogCedula> LogCedulas { get; set; }
        public DbSet<LogEntregable> LogEntregables { get; set; }
        public DbSet<Firmante> Firmantes { get; set; }
        public DbSet<HistorialMF> HistorialMF { get; set; }
        public DbSet<Oficio> Oficios { get; set; }
        public DbSet<DetalleOficio> DetalleOficios { get; set; }

        private void ModelConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factura>().ToTable("Facturas");
            modelBuilder.Entity<Cuestionario>().ToTable("Cuestionarios");
            modelBuilder.Entity<CedulaEvaluacion>().ToTable("CedulasEvaluacion");
            modelBuilder.Entity<RespuestaEvaluacion>().ToTable("RespuestasEvaluacion");
            modelBuilder.Entity<Contrato>().ToTable("Contratos");
            modelBuilder.Entity<Convenio>().ToTable("Convenios");
            modelBuilder.Entity<RubroConvenio>().ToTable("RubrosConvenio"); 
            modelBuilder.Entity<ServicioContrato>().ToTable("ServiciosContrato");
            modelBuilder.Entity<EntregableContratacion>().ToTable("EntregablesContratacion");
            modelBuilder.Entity<Oficio>().ToTable("Oficios");
            modelBuilder.Entity<DetalleOficio>().ToTable("DetalleOficios");
            modelBuilder.Entity<DetalleIncidencia>().ToTable("DetallesIncidencias");

            new CedulaEvaluacionConfiguration(modelBuilder.Entity<CedulaEvaluacion>());
            new ContratosConfiguration(modelBuilder.Entity<Contrato>());
            new ConvenioConfiguration(modelBuilder.Entity<Convenio>());
            new ServicioContratoConfiguration(modelBuilder.Entity<ServicioContrato>());
            new RubroConvenioConfiguration(modelBuilder.Entity<RubroConvenio>());
            new FacturasConfiguration(modelBuilder.Entity<Factura>());
            new ConceptosFacturaConfiguration(modelBuilder.Entity<ConceptosFactura>());
            new CuestionarioConfiguration(modelBuilder.Entity<Cuestionario>());
            new CuestionarioMensualConfiguration(modelBuilder.Entity<CuestionarioMensual>());
            new RespuestasEvaluacionConfiguration(modelBuilder.Entity<RespuestaEvaluacion>());
            new ConfiguracionIncidenciasConfiguration(modelBuilder.Entity<ConfiguracionIncidencias>());
            new EntregablesConfiguration(modelBuilder.Entity<Entregable>());
            new RepositorioConfiguration(modelBuilder.Entity<Repositorio>());
            new EntregablesEstatusConfiguration(modelBuilder.Entity<EntregableEstatus>());
            new FlujoCedulaConfiguration(modelBuilder.Entity<FlujoCedula>());
            new LogCedulasConfiguration(modelBuilder.Entity<LogCedula>());
            new LogEntregablesConfiguration(modelBuilder.Entity<LogEntregable>());
            new FirmantesConfiguration(modelBuilder.Entity<Firmante>());
            new LogMasivoFacturasConfiguration(modelBuilder.Entity<HistorialMF>());
            new OficiosConfiguration(modelBuilder.Entity<Oficio>());
            new DetalleOficioConfiguration(modelBuilder.Entity<DetalleOficio>());
            new DetalleIncidenciaConfiguration(modelBuilder.Entity<DetalleIncidencia>());
        }
    }
}
