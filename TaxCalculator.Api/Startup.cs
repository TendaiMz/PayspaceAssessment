
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using TaxCalculator.Domain.Repository;
using TaxCalculator.Domain.Validation;
using TaxCalculator.Exceptions;
using TaxCalculator.Logging;
using TaxCalculator.Problem_Detail;
using TaxCalculator.Utils;

namespace PayspaceAssessment
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly SwaggerOptions swaggerOptions;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            swaggerOptions = Configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true;
                cfg.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("X-Version"), new QueryStringApiVersionReader("v"));
            });

            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc(swaggerOptions.SwaggerDocName, new OpenApiInfo
            {
                Title = swaggerOptions.Title,
                Version = swaggerOptions.Version,
                Description = swaggerOptions.Description,
                Contact = new OpenApiContact
                {
                    Name = swaggerOptions.Contact.Name,
                    Email = swaggerOptions.Contact.Email,
                }
            }));

            services.AddProblemDetails(x =>
            {
                x.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));               
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            services.Configure<ConnectionSettings>(Configuration.GetSection("ConnectionStrings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerOptions.SwaggerEndPoint, swaggerOptions.EndPointName);
            });
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
