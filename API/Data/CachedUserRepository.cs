using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace API.Data
{
    public class CachedUserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public CachedUserRepository(DataContext context, IMapper mapper, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
        }

        public async Task<MemberDto?> GetMemberAsync(string username)
        {
            var cacheKey = $"member:{username}";

            // Try to get from cache first
            var cachedMember = await _cache.GetStringAsync(cacheKey);
            if (cachedMember != null)
            {
                return JsonSerializer.Deserialize<MemberDto>(cachedMember);
            }

            // If not in cache, get from database
            var member = await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (member != null)
            {
                // Store in cache
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(member),
                    _cacheOptions
                );
            }

            return member;
        }

        public async Task<PagedList<MemberDto>> GetMemberAsync(UserParams userParams)
        {
            var cacheKey = $"members:page{userParams.PageNumber}:size{userParams.PageSize}:gender{userParams.Gender}:minage{userParams.MinAge}:maxage{userParams.MaxAge}:orderby{userParams.Orderby}";

            // Try to get from cache first
            var cachedMembers = await _cache.GetStringAsync(cacheKey);
            if (cachedMembers != null)
            {
                var ret= JsonSerializer.Deserialize<PagedListDto<MemberDto>>(cachedMembers);
                return ret.ToPagedList();
            }

            // If not in cache, get from database
            var query = _context.Users.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.Orderby switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            if (userParams.Gender != null)
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }

            var pagedList = await PagedList<MemberDto>.CreateAsync(
                query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize
            );

            // Store in cache
           // var pagedListDto = new PagedListDto<MemberDto>(pagedList);
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(pagedList.ToDto()),
                _cacheOptions
            );

            return pagedList;
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            var cacheKey = $"user:name:{username}";

            // Try to get from cache first
            var cachedUser = await _cache.GetStringAsync(cacheKey);
            if (cachedUser != null)
            {
                return JsonSerializer.Deserialize<AppUser>(cachedUser);
            }

            // If not in cache, get from database
            var user = await _context.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);

            if (user != null)
            {
                // Store in cache
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(user),
                    _cacheOptions
                );
            }

            return user;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var cacheKey = "users:all";

            // Try to get from cache first
            var cachedUsers = await _cache.GetStringAsync(cacheKey);
            if (cachedUsers != null)
            {
                return JsonSerializer.Deserialize<IEnumerable<AppUser>>(cachedUsers);
            }

            // If not in cache, get from database
            var users = await _context.Users
                .Include(x => x.Photos)
                .ToListAsync();

            // Store in cache
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(users),
                _cacheOptions
            );

            return users;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task update(AppUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Invalidate cached versions
            await _cache.RemoveAsync($"user:{user.Id}");
            await _cache.RemoveAsync($"user:name:{user.UserName}");
            await _cache.RemoveAsync($"member:{user.UserName}");
            await _cache.RemoveAsync("users:all");
            // Note: We should also invalidate any paged results that might contain this user
            // In a production system, you might want to implement a more sophisticated cache invalidation strategy
        }
    }
}