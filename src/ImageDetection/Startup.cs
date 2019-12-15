using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ImageDetection
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            BaiduAI.ImageAI.APP_ID = Configuration["BaiduAI:image:APP_ID"];
            BaiduAI.ImageAI.API_KEY = Configuration["BaiduAI:image:API_KEY"];
            BaiduAI.ImageAI.SECRET_KEY = Configuration["BaiduAI:image:SECRET_KEY"];

            AliyunAI.ImageAI.Region_Id = Configuration["AliyunAI:Region_Id"];
            AliyunAI.ImageAI.API_KEY = Configuration["AliyunAI:API_KEY"];
            AliyunAI.ImageAI.SECRET_KEY = Configuration["AliyunAI:SECRET_KEY"];

            QcloudAI.ImageAI.Region_Id = Configuration["QcloudAI:Region_Id"];
            QcloudAI.ImageAI.API_KEY = Configuration["QcloudAI:API_KEY"];
            QcloudAI.ImageAI.SECRET_KEY = Configuration["QcloudAI:SECRET_KEY"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
