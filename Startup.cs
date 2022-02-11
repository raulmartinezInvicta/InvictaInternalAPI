using InvictaInternalAPI.Context;
using InvictaInternalAPI.Exceptions;
using InvictaInternalAPI.Interfaces;
using InvictaInternalAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace InvictaInternalAPI
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

            services.AddControllers(options => 
            {
                options.Filters.Add<GlobalExceptionFIlter>();
            });

            services.AddTransient<ICancelService, CancelService>();
            services.AddTransient<ICancelInvictaService, CancelInvictaService>();
            services.AddTransient<IMagentoSupport, MagentoSupport>();
            services.AddTransient<IShipStationService, ShipStationService>();
            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            }));
            services.AddSwaggerGen(doc =>
            {
                doc.SwaggerDoc("v1", new OpenApiInfo { Title = "Invicta InternalAPI", Version = "v1" });
            });

            services.AddDbContext<MerlinContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnectionMerlin")));

            services.AddDbContext<InvictaAUXContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnectionInvicta")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Invicta InternalAPI");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
