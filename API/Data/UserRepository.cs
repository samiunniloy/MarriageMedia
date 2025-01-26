using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace API.Data
{
    public class UserRepository(DataContext context,IMapper mapper) : IUserRepository
    {
        public async Task<PagedList<MemberDto>> GetMemberAsync(UserParams userParams)
        {
            //return await context.Users
            //    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            //    .ToListAsync();

            var query = context.Users.ProjectTo<MemberDto>(mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<MemberDto?> GetMemberAsync(string username)
        {
            return await context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        

        async Task<AppUser?> IUserRepository.GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        async Task<AppUser?> IUserRepository.GetUserByUsernameAsync(string username)
        {
            return await context.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        async Task<IEnumerable<AppUser>> IUserRepository.GetUsersAsync()
        {
            return await context.Users.Include(x=>x.Photos).ToListAsync();
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
