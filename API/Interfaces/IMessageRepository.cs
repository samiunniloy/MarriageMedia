using API.DTOs;
using API.Entities;
using API.Extensions.Helpers;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);
        Task DeleteMessage(Message message);
        Task<Message?> GetMessage(int id);
        Task<PagedListM<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();

    }
}
