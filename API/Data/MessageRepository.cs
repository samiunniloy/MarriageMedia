//using API.Entities;
//using API.Interfaces;
//using API.DTOs;
//using API.Helpers;
//using MongoDB.Driver;
//using API.Extensions.Helpers;
//using Microsoft.EntityFrameworkCore;

//namespace API.Data.Repositories
//{
//    public class MessageRepository : IMessageRepository
//    {
//        private readonly IMongoCollection<Message> _messages;
//        private readonly DataContext _context;
//        private readonly IMongoDatabase _database;

//        public MessageRepository(IMongoDatabase database, DataContext context)
//        {
//            _database = database;

//            //var collections = _database.ListCollectionNames().ToList();
//            //if (!collections.Contains("Messages"))
//            //{
//            //    _database.CreateCollection("Messages");
//            //}

//            _messages = database.GetCollection<Message>("Messages");
//            _context = context;

//            // Create indexes
//            var indexKeysDefinition = Builders<Message>.IndexKeys
//                .Ascending(m => m.SenderUsername)
//                .Ascending(m => m.RecipientUsername);
//            var indexOptions = new CreateIndexOptions { Name = "SenderRecipient_Index" };
//            _messages.Indexes.CreateOne(new CreateIndexModel<Message>(indexKeysDefinition, indexOptions));
//        }

//        public async Task AddMessage(Message message)
//        {
//            await _messages.InsertOneAsync(message);
//        }

//        public async Task DeleteMessage(Message message)
//        {
//            var filter = Builders<Message>.Filter.Eq(m => m.Id, message.Id);
//            await _messages.DeleteOneAsync(filter);
//        }

//        public async Task<Message?> GetMessage(int id)
//        {
//            var filter = Builders<Message>.Filter.Eq(m => m.Id, id);
//            return await _messages.Find(filter).FirstOrDefaultAsync();
//        }

//        public async Task<PagedListM<MessageDto>> GetMessagesForUser(MessageParams messageParams)
//        {
//            var builder = Builders<Message>.Filter;
//            var filter = builder.Empty;

//            switch (messageParams.Container?.ToLower())
//            {
//                case "inbox":
//                    filter = builder.And(
//                        builder.Eq(m => m.RecipientUsername, messageParams.Username),
//                        builder.Eq(m => m.RecipientDeleted, false)
//                    );
//                    break;
//                case "outbox":
//                    filter = builder.And(
//                        builder.Eq(m => m.SenderUsername, messageParams.Username),
//                        builder.Eq(m => m.SenderDeleted, false)
//                    );
//                    break;
//                default: // Unread messages
//                    filter = builder.And(
//                        builder.Eq(m => m.RecipientUsername, messageParams.Username),
//                        builder.Eq(m => m.RecipientDeleted, false),
//                        builder.Eq(m => m.DateRead, null)
//                    );
//                    break;
//            }

//            var messages = await _messages.Find(filter)
//                .SortByDescending(m => m.MessageSent)
//                .ToListAsync();
//            if (messages.Count == 0) return await PagedListM<MessageDto>.CreateAsync(new List<MessageDto>(), messageParams.PageNumber, messageParams.PageSize);
//            // Get user photos from context
//            var usernames = messages.SelectMany(m => new[] { m.SenderUsername, m.RecipientUsername }).Distinct();

//            var users = await _context.Users
//                .Where(u => usernames.Contains(u.UserName))
//                .Include(u => u.Photos)
//                .ToDictionaryAsync(u => u.UserName);

//            var messageDtos = messages.Select(m => new MessageDto
//            {
//                Id = m.Id,
//                SenderId = m.SenderId,
//                SenderUserName = m.SenderUsername,
//                SenderPhotoUrl = users.GetValueOrDefault(m.SenderUsername)?.Photos?.FirstOrDefault(p => p.IsMain)?.Url ?? string.Empty,
//                RecipientId = m.RecipientId,
//                RecipientUserName = m.RecipientUsername,
//                RecipientPhotoUrl = users.GetValueOrDefault(m.RecipientUsername)?.Photos?.FirstOrDefault(p => p.IsMain)?.Url ?? string.Empty,
//                Content = m.Content,
//                DateRead = m.DateRead,
//                MessageSent = m.MessageSent
//            });

//            return await PagedListM<MessageDto>.CreateAsync(messageDtos, messageParams.PageNumber, messageParams.PageSize);
//        }

//        //public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
//        //{
//        //    var builder = Builders<Message>.Filter;
//        //    var filter = builder.Or(
//        //        builder.And(
//        //            builder.Eq(m => m.SenderUsername, currentUsername),
//        //            builder.Eq(m => m.RecipientUsername, recipientUsername),
//        //            builder.Eq(m => m.SenderDeleted, false)
//        //        ),
//        //        builder.And(
//        //            builder.Eq(m => m.SenderUsername, recipientUsername),
//        //            builder.Eq(m => m.RecipientUsername, currentUsername),
//        //            builder.Eq(m => m.RecipientDeleted, false)
//        //        )
//        //    );

//        //    var messages = await _messages.Find(filter)
//        //        .SortBy(m => m.MessageSent)
//        //        .ToListAsync();

//        //    // Mark messages as read
//        //    var unreadMessages = messages
//        //        .Where(m => m.DateRead == null && m.RecipientUsername == currentUsername)
//        //        .ToList();

//        //    if (unreadMessages.Any())
//        //    {
//        //        foreach (var message in unreadMessages)
//        //        {
//        //            message.DateRead = DateTime.UtcNow;
//        //            var updateFilter = Builders<Message>.Filter.Eq(m => m.Id, message.Id);
//        //            var update = Builders<Message>.Update.Set(m => m.DateRead, message.DateRead);
//        //            await _messages.UpdateOneAsync(updateFilter, update);
//        //        }
//        //    }

//        //    // Get user photos
//        //    //var users = await _context.Users
//        //    //    .Where(u => u.UserName == currentUsername || u.UserName == recipientUsername)
//        //    //    .Include(u => u.Photos)
//        //    //    .ToDictionaryAsync(u => u.UserName);

//        //    return messages.Select(m => new MessageDto
//        //    {
//        //        Id = m.Id,
//        //        SenderId = m.SenderId,
//        //        SenderUserName = m.SenderUsername,
//        //        //SenderPhotoUrl = users.GetValueOrDefault(m.SenderUsername)?.Photos?.FirstOrDefault(p => p.IsMain)?.Url ?? string.Empty,
//        //        RecipientId = m.RecipientId,
//        //        RecipientUserName = m.RecipientUsername,
//        //        //RecipientPhotoUrl = users.GetValueOrDefault(m.RecipientUsername)?.Photos?.FirstOrDefault(p => p.IsMain)?.Url ?? string.Empty,
//        //        Content = m.Content,
//        //        DateRead = m.DateRead,
//        //        MessageSent = m.MessageSent
//        //    }).ToList();
//        //}


//        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
//        {
//            var builder = Builders<Message>.Filter;

//            // Define the filter to fetch messages where the current user is either the sender or recipient
//            var filter = builder.Or(
//                builder.And(
//                    builder.Eq(m => m.SenderUsername, currentUsername),
//                    builder.Eq(m => m.RecipientUsername, recipientUsername)
//                ),
//                builder.And(
//                    builder.Eq(m => m.SenderUsername, recipientUsername),
//                    builder.Eq(m => m.RecipientUsername, currentUsername)
//                )
//            );

//            // Query the messages from the collection
//            var messages = await _messages.Find(filter)
//                .SortBy(m => m.MessageSent)
//                .ToListAsync();

//            // If no messages exist, return a default message
//            if (!messages.Any())
//            {
//                return new List<MessageDto>
//                    {
//            new MessageDto
//            {
//                Id = 0,
//                SenderId = 0,
//                SenderUserName = "System",
//                RecipientId = 0,
//                RecipientUserName = "System",
//                Content = "No messages found.",
//                DateRead = null,
//                MessageSent = DateTime.UtcNow
//            }
//                };
//            }

//            // Map the messages to MessageDto and return
//            //return messages.Select(m => new MessageDto
//            //{
//            //    Id = m.Id,
//            //    SenderId = m.SenderId,
//            //    SenderUserName = m.SenderUsername,
//            //    RecipientId = m.RecipientId,
//            //    RecipientUserName = m.RecipientUsername,
//            //    Content = m.Content,
//            //    DateRead = m.DateRead,
//            //    MessageSent = m.MessageSent
//            //}).ToList();


//            return new List<MessageDto>
//                    {
//            new MessageDto
//            {
//                Id = 0,
//                SenderId = 0,
//                SenderUserName = "System",
//                RecipientId = 0,
//                RecipientUserName = "System",
//                Content = "No messages found.",
//                DateRead = null,
//                MessageSent = DateTime.UtcNow
//            }
//                };

//        }



//        public Task<bool> SaveAllAsync()
//        {
//            return Task.FromResult(true); // MongoDB saves immediately, no need for SaveChanges
//        }


//    }
//}


using API.DTOs;
using API.Entities;
using API.Extensions.Helpers;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using MongoDB.Driver;

namespace API.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messages;
        private readonly IMapper _mapper;

        public MessageRepository(IMongoDatabase database, IMapper mapper)
        {
            _messages = database.GetCollection<Message>("messages"); // Collection name: messages
            _mapper = mapper;
        }

        //public async Task AddMessage(Message message)
        //{
        //    message.Id = _messages.Count();
        //    await _messages.InsertOneAsync(message);
        //}
        public async Task AddMessage(Message message)
        {
            var maxId = await _messages.Find(_ => true)
                .SortByDescending(m => m.Id)
                .Limit(1)
                .Project(m => m.Id)
                .FirstOrDefaultAsync();

            message.Id = maxId + 1; // Increment max ID for new message
            await _messages.InsertOneAsync(message);
        }

        public async Task DeleteMessage(Message message)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, message.Id);
            await _messages.DeleteOneAsync(filter);
        }

        public async Task<Message?> GetMessage(int id)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, id);
            return await _messages.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<PagedListM<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var builder = Builders<Message>.Filter;
            var filter = messageParams.Container switch
            {
                "Inbox" => builder.And(
                    builder.Eq(m => m.RecipientUsername, messageParams.Username),
                    builder.Eq(m => m.RecipientDeleted, false)
                ),
                "Outbox" => builder.And(
                    builder.Eq(m => m.SenderUsername, messageParams.Username),
                    builder.Eq(m => m.SenderDeleted, false)
                ),
                _ => builder.And(
                    builder.Eq(m => m.RecipientUsername, messageParams.Username),
                    builder.Eq(m => m.RecipientDeleted, false),
                    builder.Eq(m => m.DateRead, null)
                )
            };

            var totalCount = await _messages.CountDocumentsAsync(filter);
            var messages = await _messages.Find(filter)
                .SortByDescending(m => m.MessageSent)
                .Skip((messageParams.PageNumber - 1) * messageParams.PageSize)
                .Limit(messageParams.PageSize)
                .ToListAsync();

            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return new PagedListM<MessageDto>(messageDtos.ToList(), (int)totalCount, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var builder = Builders<Message>.Filter;
            var filter = builder.Or(
                builder.And(
                    builder.Eq(m => m.SenderUsername, currentUsername),
                    builder.Eq(m => m.RecipientUsername, recipientUsername),
                    builder.Eq(m => m.SenderDeleted, false)
                ),
                builder.And(
                    builder.Eq(m => m.SenderUsername, recipientUsername),
                    builder.Eq(m => m.RecipientUsername, currentUsername),
                    builder.Eq(m => m.RecipientDeleted, false)
                )
            );

            var messages = await _messages.Find(filter)
                .SortBy(m => m.MessageSent)
                .ToListAsync();

            // Mark unread messages as read
            var unreadMessages = messages
                .Where(m => m.DateRead == null && m.RecipientUsername == currentUsername)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                    var update = Builders<Message>.Update.Set(m => m.DateRead, message.DateRead);
                    await _messages.UpdateOneAsync(builder.Eq(m => m.Id, message.Id), update);
                }
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return true; // MongoDB saves instantly, so this always returns true.
        }

       
    }
}
