using System;
using System.Linq;
using System.Collections.Generic;
using Force.DeepCloner;

using VShop.SharedKernel.EventSourcing.Exceptions;

namespace VShop.SharedKernel.EventSourcing
{
    public abstract class AggregateState<T> : IAggregateState<T> 
        where T : class, new()
    {
        public long Version { get; protected set; }

        public Guid Id { get; protected set; }
        
        public string StreamName => GetStreamName(Id);
        
        public string GetStreamName(Guid id) => $"{typeof(T).Name}-{id:N}"; // TODO - potentially trim "State"
        
        public abstract T When(T state, object @event);

        protected T With(T state, Action<T> update)
        {
            update(state);
            
            return state;
        }

        protected abstract bool EnsureValidState(T newState);

        private T Apply(T state, object @event)
        {
            T newState = state.DeepClone();
            newState = When(newState, @event);

            if (!EnsureValidState(newState))
                throw new InvalidEntityState(this, "Post-checks failed.");

            return newState;
        }

        public Result NoEvents() => new(this as T, new List<object>());

        public Result Apply(params object[] events)
        {
            T newState = this as T;
            newState = events.Aggregate(newState, Apply);
            
            return new Result(newState, events);
        }
        
        public class Result
        {
            public T State { get; }
            public IEnumerable<object> Events { get; }

            public Result(T state, IEnumerable<object> events)
            {
                State = state;
                Events = events;
            }
        }
    }
}