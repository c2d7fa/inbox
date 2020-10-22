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

namespace Inbox
{
    public static class AllItems
    {
        [FunctionName("AllItems")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var connectionString = $"Host=52.236.131.252;Username=postgres;Password=verysecret;Database=postgres";

            log.LogInformation("Connecting to PostgreSQL database...");
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            log.LogInformation("Successfully connected to database.");

            var messages = new List<Message>();

            using (var command = new NpgsqlCommand("SELECT * FROM message", connection)) {
                 using var reader = command.ExecuteReader();
                 while (reader.Read()) {
                     var message = new Message(reader.GetGuid(0), reader.GetDateTime(1), reader.GetFieldValue<IPAddress>(2), reader.GetString(3));
                     log.LogInformation("Got message: " + JsonConvert.SerializeObject(message, new JsonSerializerSettings().WithIPAddress()));
                     messages.Add(message);
                 }
            }

            return new JsonResult(messages, new JsonSerializerSettings().WithIPAddress());
        }
    }
}
