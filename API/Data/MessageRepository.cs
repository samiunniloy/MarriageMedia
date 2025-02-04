using API.DTOs;
using API.Entities;
using API.Extensions.Helpers;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository(DataContext context,IMapper mapper) : IMessageRepository
    {
       public  void AddMessage(Message message)
        {
           context.Messages.Add(message);
        }

       public  void DeleteMessage(Message message)
        {
           context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(int id)
        {
            return await context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);
        }


        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            
            var quey=context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            quey = messageParams.Container switch
            {
                "Inbox" => quey
                .Where(u => u.Recipient.UserName == messageParams.Username&&u.RecipientDeleted==false),
                "Outbox" => quey
                .Where(u => u.Sender.UserName == messageParams.Username&&u.SenderDeleted==false),
                _ => quey
                .Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null&& u.RecipientDeleted == false)
            };

            var messages = quey.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await context.Messages
                 .Include(x => x.Sender).ThenInclude(x => x.Photos)
                 .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                 .Where(x =>
                  x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername &&x.SenderDeleted==false||
                  x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername&& x.RecipientDeleted == false
                 )
                 .OrderBy(x=>x.MessageSent)
                 .ToListAsync();

            var unreadmessages=messages
                .Where(x => x.DateRead == null && x.RecipientUsername == currentUsername)
                .ToList();

            if (unreadmessages.Any())
            {
                unreadmessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            }

            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }




       public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync()>0;
        }

      // public  void Addgroup(Group group)
      //  {
      //      context.Groups.Add(group);
      //  }

      //  public async Task<Connection?> GetConnection(string connectionId)
      //  {
      //      return await context.Connections.FindAsync(connectionId);
      //  }

      // public async Task<Group?>getMessagegroup(string groupName)
      //  {
      //      return await context.Groups
      //          .Include(x => x.Connections)
      //          .FirstOrDefaultAsync(x => x.Name == groupName);
      //  }

      //public   void RemoveConnection(Connection connection)
      //  {
      //      context.Connections.Remove(connection);
      //  }
    }
}
