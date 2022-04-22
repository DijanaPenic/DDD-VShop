using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Events
{
    internal class EventMediator
    {
        public readonly Func<IEnumerable<Func<Task>>, Task> Publish;

        public EventMediator(Func<IEnumerable<Func<Task>>, Task> publish) => Publish = publish;
    }
}