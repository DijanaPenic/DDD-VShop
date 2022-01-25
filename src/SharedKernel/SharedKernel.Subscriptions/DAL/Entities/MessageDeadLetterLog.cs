using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Subscriptions.DAL.Entities
{
    public class MessageDeadLetterLog : DbEntityBase
    {
        public Guid Id { get; set; }
        public string StreamId { get; set; }
        public string MessageType { get; set; }
        public Guid MessageId { get; set; }
        public string MessageData { get; set; }
        public MessageProcessingStatus Status { get; set; }
        public string Error { get; set; }
    }
}