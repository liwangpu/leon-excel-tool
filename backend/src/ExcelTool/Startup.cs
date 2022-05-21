using ExcelTool.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using ExcelTool.Infrastructures.JTW;
using Microsoft.AspNetCore.Http;

namespace ExcelTool
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
            services.AddCors();

            //var jwtSetting = Configuration.GetSection("JWTSetting").Get<JwtSetting>();

            ////JWTÈÏÖ¤
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            //}).AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.SecretKey)),
            //        ValidIssuer = jwtSetting.Issuer,
            //        ValidAudience = jwtSetting.Audience,
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateLifetime = true
            //    };
            //    //x.Events = new JwtBearerEvents
            //    //{
            //    //    OnMessageReceived = context =>
            //    //    {
            //    //        var accessToken = context.Request.Headers["Authorization"];
            //    //        Console.WriteLine(accessToken);
            //    //        var path = context.HttpContext.Request.Path;
            //    //        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            //    //        {
            //    //            context.Token = accessToken;
            //    //        }

            //    //        return Task.CompletedTask;
            //    //    }
            //    //};
            //});
            services.Configure<AppConfig>(Configuration);
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddSingleton<IStaticFileSetting, StaticFileSetting>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSignalR();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStaticFileSetting staticFileSetting)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            if (!Directory.Exists(env.WebRootPath))
            {
                Directory.CreateDirectory(env.WebRootPath);
            }

            if (!Directory.Exists(staticFileSetting.TmpFolder))
            {
                Directory.CreateDirectory(staticFileSetting.TmpFolder);
            }

            if (!Directory.Exists(staticFileSetting.AttactmentFolder))
            {
                Directory.CreateDirectory(staticFileSetting.AttactmentFolder);
            }
            //app.Use(async (context, next) =>
            //{
            //    var token = context.Session.GetString("Token");
            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        context.Request.Headers.Add("Authorization", "Bearer " + token);
            //    }
            //    await next();
            //});


            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseWebSockets();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllers();
            });
        }
    }
}
