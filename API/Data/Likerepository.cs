﻿using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace API.Data
{
    public class Likerepository(DataContext context,IMapper mapper) : ILikesRepository
    {
      public  void AddLike(UserLike like)
        {
            context.Likes.Add(like);
        }

      public  void DeleteLike(UserLike like)
        {

            context.Likes.Remove(like);
        }

       public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            return await context.Likes
                 .Where(x => x.SourceUserId == currentUserId)
                 .Select(x => x.TargetUserId)
                 .ToListAsync();

        }

        public async Task<UserLike> GetUserLike(int sourceId, int targetId)
        {
            return await context.Likes.FindAsync(sourceId, targetId); 
        }

       public async Task<IEnumerable<MemberDto>>GetUserLikes(string predicate, int userId)
        {
            var likes = context.Likes.AsQueryable();

            switch (predicate)
            {
                case "liked":
                    return await likes
                        .Where(x => x.SourceUserId == userId)
                        .Select(x => x.TargetUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

                case "likedBy":
                    return await likes
                        .Where(x => x.TargetUserId == userId)
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

                default:
                    var likeIds = await GetCurrentUserLikeIds(userId);
                    return await likes
                        .Where(x => x.TargetUserId == userId && likeIds.Contains(x.SourceUserId))
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

            }

        }

       public async  Task<bool> SaveChanges()
        {
            return await context.SaveChangesAsync()>0;
        }
    }
}
