using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required byte[] PasswordHash { get; set; }
        public required Byte[] PasswordSalt { get; set; }

    }
}
