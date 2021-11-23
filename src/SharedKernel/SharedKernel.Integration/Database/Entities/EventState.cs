namespace VShop.SharedKernel.Integration.Database.Entities
{
    // TODO - rename Database to Infrastructure
    public enum EventState
    {
        NotPublished = 0,
        InProgress = 1,
        Published = 2,
        PublishedFailed = 3
    }
}