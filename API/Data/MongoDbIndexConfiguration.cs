using MongoDB.Driver;
using API.Data;

public class MongoDbIndexConfiguration
{
    private readonly IMongoCollection<Message> _messages;

    public MongoDbIndexConfiguration(IMongoCollection<Message> messages)
    {
        _messages = messages;
    }

    public async Task CreateIndexes()
    {
        var indexKeysDefinition = Builders<Message>.IndexKeys
            .Ascending(m => m.SenderUsername)
            .Ascending(m => m.RecipientUsername);

        var indexOptions = new CreateIndexOptions
        {
            Name = "SenderRecipient_Index",
            Unique = true,
            Background = true
        };

        // Drop existing index if exists
        var indexes = await _messages.Indexes.ListAsync();
        var indexNames = await indexes.ToListAsync();

        if (indexNames.Any(index => index["name"] == "SenderRecipient_Index"))
        {
            await _messages.Indexes.DropOneAsync("SenderRecipient_Index");
        }

        await _messages.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(indexKeysDefinition, indexOptions)
        );
    }
}