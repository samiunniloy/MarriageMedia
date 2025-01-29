using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        //public static string GetUsername(this ClaimsPrincipal user)
        //{
        //    return user.FindFirstValue(ClaimTypes.Name)
        //        ?? throw new Exception("No username found");
        //}

        public static string GetUsername(this ClaimsPrincipal user)
        {
            var claims = user.Claims.ToList();
            // Log or inspect claims here
            return user.FindFirstValue(ClaimTypes.Name)
                ?? throw new Exception("No username found");
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("No user ID found");

            if (!int.TryParse(idClaim, out int userId))
            {
                throw new Exception("Invalid user ID format");
            }

            return userId;
        }
    }
}
   
