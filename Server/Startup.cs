using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inbox.Server {
    public class Startup {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddRazorPages();
            services.AddControllers();

            var connectionString = configuration["ConnectionString"];
            Console.WriteLine("Using connection string: " + connectionString);
            var account = CloudStorageAccount.Parse(connectionString);
            services.AddSingleton(account.CreateCloudTableClient());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
