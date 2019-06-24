using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebManagerTasks.Data.Models;
using WebManagerTasks.Data.Repositories;
using WebManagerTasks.ViewModels;
using WebManagerTasks.Services;

namespace WebManagerTasks
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
            string connection = Configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<ProjectsManagerContext>(options =>
                options.UseSqlServer(connection));
            services.AddTransient<IRepository<User>, EntityRepository<User>>();
            services.AddTransient<IService<User>, EntityService<User>>();
            services.AddTransient<IRepository<Project>, EntityRepository<Project>>();
            services.AddTransient<IService<Project>, EntityService<Project>>();
            services.AddTransient<IRepository<UserProject>, EntityRepository<UserProject>>();
            services.AddTransient<IService<UserProject>, EntityService<UserProject>>();
            services.AddTransient<IRepository<MyTask>, EntityRepository<MyTask>>();
            services.AddTransient<IService<MyTask>, EntityService<MyTask>>();
            services.AddTransient<IRepository<ProjectTask>, EntityRepository<ProjectTask>>();
            services.AddTransient<IService<ProjectTask>, EntityService<ProjectTask>>();
            services.AddTransient<IRepository<Comment>, EntityRepository<Comment>>();
            services.AddTransient<IService<Comment>, EntityService<Comment>>();



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "ValidIssuer",
                    ValidAudience = "ValidateAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("IssuerSigningSecretKey")),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
