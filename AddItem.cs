using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Npgsql;
using System.Net;

namespace func
{
    public static class AddItem
    {
        [FunctionName("AddItem")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Reading body of request...");
            var content = new StreamReader(req.Body).ReadToEnd();
            log.LogInformation($"Got string with {content.Length} characters.");

            IPAddress author;
            if (req.Headers["X-Forwarded-For"].Count < 1 || !IPAddress.TryParse(req.Headers["X-Forwarded-For"], out author)) {
                log.LogWarning("Could not parse IP address from 'X-Forwarded-For' header. Ignoring request.");
                var result = new ContentResult();
                result.StatusCode = 500;
                result.ContentType = "text/plain";
                result.Content = "500 Internal Server Error\r\n\r\nUnable to read IP address.";
                return result;
            };
            log.LogInformation($"It looks like this message came from '{author}'.");

            log.LogInformation("Connecting to PostgreSQL database...");
            var connectionString = $"Host=52.236.131.252;Username=postgres;Password=verysecret;Database=postgres";
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            log.LogInformation("Successfully connected to database.");

            log.LogInformation("Inserting message into database...");
            using (var command = new NpgsqlCommand("INSERT INTO message (id, author, created, content) VALUES (uuid_generate_v4(), @author, NOW(), @content)", connection)) {
                command.Parameters.AddWithValue("author", author);
                command.Parameters.AddWithValue("content", content);
                command.ExecuteNonQuery();
            }
            log.LogInformation("Successfully inserted message.");

            return new OkResult();
        }
    }
}
