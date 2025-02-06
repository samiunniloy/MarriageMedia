using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Data
{
    public class RedisUserRepository(DataContext context) : IUserRepository
    {
        Task<PagedList<MemberDto>> IUserRepository.GetMemberAsync(UserParams userParams)
        {
            throw new NotImplementedException();
        }

        Task<MemberDto?> IUserRepository.GetMemberAsync(string username)
        {
            throw new NotImplementedException();
        }

        Task<AppUser?> IUserRepository.GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<AppUser?> IUserRepository.GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AppUser>> IUserRepository.GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        Task<bool> IUserRepository.SaveAllAsync()
        {
            throw new NotImplementedException();
        }

        Task IUserRepository.update(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
