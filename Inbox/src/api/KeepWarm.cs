using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;

namespace Inbox
{
    public static class KeepWarm
    {
        [FunctionName("KeepWarm")]
        public static void Run(
            [TimerTrigger("30 */8 8-23,0-3 * * *")] TimerInfo timer,
            [Table("UnreadMessages")] CloudTable unreadMessagesTable,
            [Table("Authentication")] CloudTable authenticationTable,
            ILogger log)
        {
            log.LogInformation("Warming functin ran");
        }
    }
}
