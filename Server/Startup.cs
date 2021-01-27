using System;
using Inbox.Server.TableStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inbox.Server {
    public class Startup {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddRazorPages();
            services.AddControllers();

            var azureConnectionString = configuration["AzureConnectionString"];
            var postgresConnectionString = configuration["PostgresConnectionString"];

            if (string.IsNullOrEmpty(azureConnectionString) || string.IsNullOrEmpty(postgresConnectionString)) {
                Console.Error.WriteLine("Must set both 'AzureConnectionString' and 'PostgresConnectionString'; see README.md for more information.");
                Environment.Exit(1);
            }

            IStorage storage = new PostgresStorage(postgresConnectionString);
            services.AddSingleton(storage);

            var account = CloudStorageAccount.Parse(azureConnectionString);
            var client = account.CreateCloudTableClient();
            IAuthentication authentication = new TableAuthentication(new AzureTable(client.GetTableReference("Authentication")));
            services.AddSingleton(authentication);
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
