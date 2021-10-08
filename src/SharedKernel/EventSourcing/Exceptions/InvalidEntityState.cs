using System;

namespace EventSourcing.Exceptions
{
    class InvalidEntityState : Exception
    {
        public InvalidEntityState(object entity, string message)
            : base(
                $"Entity {entity.GetType().Name} " +
                $"state change rejected, {message}"
            ) { }
    }
}