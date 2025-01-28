using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Data
{
    public class MessageRepository(DataContext context) : IMessageRepository
    {
       public  void AddMessage(Message message)
        {
           context.Messages.Add(message);
        }

       public  void DeleteMessage(Message message)
        {
           context.Messages.Remove(message);
        }

      public async  Task<Message?>GetMessage(int id)
        {
            throw new NotImplementedException();
        }

        Task<PagedList<MessageDto>> IMessageRepository.GetMessagesForUser()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<MessageDto>> IMessageRepository.GetMessageThread(string currentUsername, string recipientUsername)
        {
            throw new NotImplementedException();
        }

       public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync()>0;
        }
    }
}
