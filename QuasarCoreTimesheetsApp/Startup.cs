using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuasarCoreTimesheetsApp.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MySql.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using QuasarCoreTimesheetsApp.Data;
using AutoMapper;
using QuasarCoreTimesheetsApp.Models;

namespace QuasarCoreTimesheetsApp
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
            // Set up authentication using JWT (json web tokens)
            services.AddAuthentication(authBuilder =>
            {
                authBuilder.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
            {
                // Used SHA256 random hash (https://www.ipvoid.com/random-sha256-hash/)
                var key = Configuration.GetValue<string>("Auth:Jwtkey");
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddDbContext<UserContext>(optionsBuilder =>
            {
                // Need to EXPLICITLY import these 2:
                // using Microsoft.EntityFrameworkCore;
                // using MySql.EntityFrameworkCore.Extensions; <-- install v5.0.10
                //
                optionsBuilder.UseMySQL(Configuration.GetConnectionString("Timesheets"));
            });

            services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<UserContext>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(configOptions =>
            {
                // Add support for special characters in username
                configOptions.User.AllowedUserNameCharacters += "Â‰ˆ≈ƒ÷";
                configOptions.Password.RequireDigit = false;        
                configOptions.Password.RequiredLength = 6;          // min 6 chars
                configOptions.Password.RequireLowercase = false;    // Case insensitive
                configOptions.Password.RequireUppercase = false;
            });

            // Automapper configuration
            services.AddTransient<ITimesheetRepository>(_ => {
                return new TimesheetRepository(Configuration.GetConnectionString("Timesheets"));
            });

            // We're mapping models to responses
            var mapper = new MapperConfiguration(configMapper =>
            {
                configMapper.CreateMap<Timesheet, TimesheetResponseModel>();
                configMapper.CreateMap<UserTimesheet, TimesheetResponseModel>();
            }).CreateMapper();

            services.AddSingleton(mapper);  // this is how dotnet core rolls

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddCors(options =>
            {
                options.AddPolicy("PolicyLocalhostWithOrigins",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost", "https://localhost", "http://localhost:8080", "https://localhost:8080")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .SetIsOriginAllowed(origin => true)
                                            .AllowCredentials();
                    });
            });

            services.AddControllersWithViews();
            //services.AddControllers();

          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            /**/
            
            /**/

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseCors();

            app.UseCors(c =>
            {
                c.AllowAnyHeader()
                 .AllowAnyMethod()
                 .SetIsOriginAllowed(origin => true)
                 .AllowCredentials();
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
