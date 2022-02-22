namespace VShop.SharedKernel.EventStoreDb;

public class EventStoreOptions
{
    public const string Section = "EventStore";
    public string ConnectionString { get; set; }
}