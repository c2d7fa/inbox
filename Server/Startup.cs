using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            var authenticationToken = configuration["AuthenticationToken"];
            var postgresConnectionString = configuration["PostgresConnectionString"];

            if (string.IsNullOrEmpty(authenticationToken) || string.IsNullOrEmpty(postgresConnectionString)) {
                Console.Error.WriteLine("Must set 'AuthenticationToken' and 'PostgresConnectionString'; see README.md for more information.");
                Environment.Exit(1);
            }

            IStorage storage = new PostgresStorage(postgresConnectionString);
            services.AddSingleton(storage);

            IAuthentication authentication = new TokenAuthentication(authenticationToken);
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

        private class TokenAuthentication : IAuthentication {
            private readonly string token;

            public TokenAuthentication(string token) {
                this.token = token;
            }

            public bool IsValidToken(string token) {
                return token == this.token;
            }
        }
    }
}
