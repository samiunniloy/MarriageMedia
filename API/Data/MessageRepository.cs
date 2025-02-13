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
            _messages = database.GetCollection<Message>("messages"); 
            _mapper = mapper;
        }

      
        public async Task AddMessage(Message message)
        {
            var maxId = await _messages.Find(_ => true)
                .SortByDescending(m => m.Id)
                .Limit(1)
                .Project(m => m.Id)
                .FirstOrDefaultAsync();

            message.Id = maxId + 1; 
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
                "OutBox" => builder.And(
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
            return true; 
        }

       
    }
}
