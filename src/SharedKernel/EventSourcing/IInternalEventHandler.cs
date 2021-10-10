namespace VShop.SharedKernel.EventSourcing
{
    public interface IInternalEventHandler
    {
        void Handle(object @event); 
    }
}