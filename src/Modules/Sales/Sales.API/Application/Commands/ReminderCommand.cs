using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
     public record ReminderCommand : Command
     {
         public string Type { get; init; }
         public Guid ProcessId { get; init; }
         public int Status { get; init; }
         public string Command { get; init; }
     }
}