using AutoMapper;
using BlockCovid.ConfigurationSettings;
using BlockCovid.Dal;
using BlockCovid.Dal.Repositories;
using BlockCovid.Interfaces;
using BlockCovid.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlockCovid
{
    public class Startup
    {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            


        services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://*.azurewebsites.net", "http://localhost:3000", "http://*.azurewebsites.net", "https://blockcovid-f89e6.web.app/")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .SetIsOriginAllowedToAllowWildcardSubdomains(); 
                                  });
            });

            services.Configure<FireBaseSettings>(Configuration.GetSection("FireBase"));
                



            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlockCovid", Version = "v1" });
            });

            services.AddDbContext<BlockCovidContext>(opts =>
            {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:BlockCovidConnection"]);
            });
            services.AddScoped<ICitizensRepository, EFCitizensRepository>();
            services.AddScoped<IParticipantsRepository, EFParticipantsRepository>();
            services.AddScoped<IQrCodesRepository, EFQrCodesRepository>();
            services.AddAutoMapper(typeof(Startup).Assembly);


            // services.Configure<JWTSettings>(Configuration.GetSection("JwtSettings"));
            var jwtsettings = new JWTSettings();
            Configuration.Bind(nameof(jwtsettings), jwtsettings);
            services.AddSingleton(jwtsettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //ce qu'on va utiliser
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        //setup validate data
                        ValidIssuer = jwtsettings.Issuer,
                        ValidAudience = jwtsettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsettings.Secret_key))
                    };
                });
    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlockCovid v1"));

            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlockCovid v1"); 
                c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root (http://localhost:<port>/)
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            SeedData.EnsurePopulated(app); 
        }
    }
}
