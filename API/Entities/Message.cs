//namespace API.Entities
//{
//    public class Message
//    {
//        public int Id { get; set; }
//        public required string SenderUsername { get; set; }
//        public required string  RecipientUsername { get; set; }
//        public required string Content { get; set; } = null!;
//        public DateTime? DateRead { get; set; }
//        public DateTime MessageSent { get; set; }=DateTime.UtcNow;
//        public bool SenderDeleted { get; set; }
//        public bool RecipientDeleted { get; set; }

//        public int SenderId { get; set; }
//        public AppUser Sender { get;set; } = null!;
//        public int RecipientId { get; set; }
//        public AppUser Recipient { get; set; } = null!;
//    }
//}

using API.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Message
{
    [BsonId]
   // public ObjectId Id { get; set; }

    public int Id { get; set; }
    public string SenderUsername { get; set; } = null!;
    public string RecipientUsername { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    public int SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;
    public int RecipientId { get; set; }
    public AppUser Recipient { get; set; } = null!;
}

