using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Extension;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace DatingApp.Apı
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //public void ConfigureProductionServices(IServiceCollection services)
        //{
        //    //var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:token").Value);
        //    services.AddDbContext<DataContext>(x =>
        //                                       x.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
        //                                      .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning))
        //                                      );

        //    services.AddTransient<Seed>();
        //    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        //    services.AddCors(options =>
        //    {
        //        options.AddPolicy("AllowSpecificOrigin",
        //            builder => builder.WithOrigins("http://localhost:4200")
        //            .AllowAnyMethod()
        //            .AllowAnyHeader()
        //            .AllowCredentials()
        //            );
        //    });

        //    #region AutoMapper
        //    // Auto Mapper Configurations
        //    var mappingConfig = new MapperConfiguration(mc =>
        //    {
        //        mc.AddProfile(new MappingProfile());
        //    });

        //    IMapper mapper = mappingConfig.CreateMapper();
        //    services.AddSingleton(mapper);
        //    #endregion

        //    services.AddScoped<IAuthRepository, AuthRepository>();
        //    services.AddScoped<IDatingRepository, DatingRepository>();
        //    services.AddScoped<LogUserActivity>();
        //    //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    //    .AddJwtBearer(options =>
        //    //    {
        //    //        options.TokenValidationParameters = new TokenValidationParameters
        //    //        {
        //    //            ValidateIssuerSigningKey = true,
        //    //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //    //            ValidateIssuer = false,
        //    //            ValidateAudience = false
        //    //        };
        //    //    });
        //    // Register the Swagger generator, defining 1 or more Swagger documents

        //    services.AddMvc().AddJsonOptions(opt =>
        //    {
        //        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        //    });
        //    //services.AddMvc().AddJsonOptions(o =>
        //    //{
        //    //    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //    //    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //    //    o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        //    //    // ^^ IMPORTANT PART ^^
        //    //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        //    #region Help Nested Dto

        //    services.Configure<JwtBearerOptions>(options =>
        //    {
        //        var validator = options.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().SingleOrDefault();

        //        // Turn off Microsoft's JWT handler that maps claim types to .NET's long claim type names
        //        validator.InboundClaimTypeMap = new Dictionary<string, string>();
        //        validator.OutboundClaimTypeMap = new Dictionary<string, string>();
        //    });

        //    #endregion

        //    services.AddSwaggerGen(c =>
        //    {
        //        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
        //    });

        //}

        // This method gets called by the runtime. Use this method to add services to the container.
        [System.Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            //var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:token").Value);
            services.AddDbContext<DataContext>(x =>
                                               x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
                                              .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning))
                                              );

            services.AddTransient<Seed>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddCors(options =>
                                {
                                    options.AddPolicy("AllowSpecificOrigin",
                                        builder => builder.WithOrigins("http://localhost:4200")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials()
                                        );
                                });

            #region AutoMapper
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<LogUserActivity>();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(key),
            //            ValidateIssuer = false,
            //            ValidateAudience = false
            //        };
            //    });
            // Register the Swagger generator, defining 1 or more Swagger documents

            services.AddMvc(option => option.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(
                                  options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; }
                   );
            //services.AddMvc().AddJsonOptions(o =>
            //{
            //    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //    o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            //    // ^^ IMPORTANT PART ^^
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region Help Nested Dto

            services.Configure<JwtBearerOptions>(options =>
            {
                var validator = options.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().SingleOrDefault();

                // Turn off Microsoft's JWT handler that maps claim types to .NET's long claim type names
                validator.InboundClaimTypeMap = new Dictionary<string, string>();
                validator.OutboundClaimTypeMap = new Dictionary<string, string>();
            });

            #endregion


            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //    options.HttpsPort = 5001;
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);

                        }
                    });
                });
                app.UseExceptionHandler("/Error");
                //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //seeder.SeedUsers();
            app.UseCors("AllowSpecificOrigin");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
