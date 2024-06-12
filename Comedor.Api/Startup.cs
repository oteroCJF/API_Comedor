using Comedor.Persistence.Database;
using Comedor.Service.Queries.Queries.CedulasEvaluacion;
using Comedor.Service.Queries.Queries.Contratos;
using Comedor.Service.Queries.Queries.Convenios;
using Comedor.Service.Queries.Queries.Cuestionarios;
using Comedor.Service.Queries.Queries.Entregables;
using Comedor.Service.Queries.Queries.EntregablesContratacion;
using Comedor.Service.Queries.Queries.Facturas;
using Comedor.Service.Queries.Queries.Firmantes;
using Comedor.Service.Queries.Queries.Flujo;
using Comedor.Service.Queries.Queries.Incidencias;
using Comedor.Service.Queries.Queries.LogCedulas;
using Comedor.Service.Queries.Queries.LogEntregables;
using Comedor.Service.Queries.Queries.Oficios;
using Comedor.Service.Queries.Queries.Repositorios;
using Comedor.Service.Queries.Queries.Respuestas;
using Comedor.Service.Queries.Queries.ServiciosContrato;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Comedor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(opts => {
                opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
               x => x.MigrationsHistoryTable("__EFMigrationHistory", "Comedor")
               );
            });

            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null; 
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false; });

            services.AddMediatR(Assembly.Load("Comedor.Service.EventHandler"));

            services.AddTransient<ICuestionarioQueryService, CuestionarioQueryService>();
            services.AddTransient<IFlujoCedulaQueryService, FlujoCedulaQueryService>();
            services.AddTransient<IFirmantesQueryService, FirmanteQueryService>();
            services.AddTransient<IRepositorioQueryService, RepositorioQueryService>();
            services.AddTransient<ICFDIQueryService, CFDIQueryService>();
            services.AddTransient<ICedulaQueryService, CedulaQueryService>();
            services.AddTransient<IRespuestaQueryService, RespuestaQueryService>();
            services.AddTransient<IContratosQueryService, ContratoQueryService>();
            services.AddTransient<IConvenioQueryService, ConvenioQueryService>();
            services.AddTransient<IServicioContratoQueryService, ServicioContratoQueryService>();
            services.AddTransient<IIncidenciasQueryService, IncidenciaQueryService>();
            services.AddTransient<IEntregableQueryService, EntregableQueryService>();
            services.AddTransient<IEntregableContratacionQueryService, EntregableContratacionQueryService>();
            services.AddTransient<IOficioQueryService, OficioQueryService>();
            services.AddTransient<ILogCedulasQueryService, LogCedulaQueryService>();
            services.AddTransient<ILogEntregablesQueryService, LogEntregableQueryService>();

            services.AddControllers();

            var secretKey = Encoding.ASCII.GetBytes(
               Configuration.GetValue<string>("SecretKey")
           );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
