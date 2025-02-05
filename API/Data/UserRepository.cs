﻿using API.DTOs;
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

            var query = context.Users.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x =>
            x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob
            );

            query = userParams.Orderby switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            if (userParams.Gender != null)
            {
                query = query.Where(x => x.Gender ==userParams.Gender);
            }



           // var query = context.Users.ProjectTo<MemberDto>(mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider),
                userParams.PageNumber, userParams.PageSize);

        }

        public async Task<MemberDto?> GetMemberAsync(string username)
        {
            return await context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        

        public async Task<AppUser?> GetUserByIdAsync(int id)
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
