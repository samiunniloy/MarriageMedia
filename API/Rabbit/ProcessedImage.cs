using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Rabbit
{
    public class ProcessedImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); 

        [BsonElement("UserId")]
        public int? UserId { get; set; } 

        [BsonElement("Base64Image")]
        public string Base64Image { get; set; } 

        [BsonElement("Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Date { get; set; } = DateTime.UtcNow; 
    }
}
