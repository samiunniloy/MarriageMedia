using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository(DataContext context) : IUserRepository
    {
        async Task<AppUser?> IUserRepository.GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        async Task<AppUser?> IUserRepository.GetUserByUsernameAsync(string username)
        {
           return await context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        async Task<IEnumerable<AppUser>> IUserRepository.GetUsersAsync()
        {
            return await context.Users.ToListAsync();
        }

       async Task<bool> IUserRepository.SaveAllAsync()
        {
           return await  context.SaveChangesAsync() > 0;
        }

        void IUserRepository.update(AppUser user)
        {
           context.Entry(user).State = EntityState.Modified;
        }
    }
}
