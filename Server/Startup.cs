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
            var client = account.CreateCloudTableClient();
            services.AddSingleton(client);

            IStorage storage = new Messages(
                new AzureTable(client.GetTableReference("UnreadMessages")),
                new AzureTable(client.GetTableReference("ReadMessages"))
            );
            services.AddSingleton(storage);

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
