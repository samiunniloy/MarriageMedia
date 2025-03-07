﻿using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace API.Entities
{
    public class AppUser:IdentityUser<int>
    {
     
        public DateOnly DateOfBirth { get; set; }
        public required string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public required string Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public  string? City { get; set; }
        public  string? Country { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();

        //public List<UserLike> LikedByUsers { get; set; } = [];

        //public List<UserLike> LikedUsers { get; set; } = [];

        //public List<Message> MessagesSent { get; set; } = [];

        //public List<Message> MessagesReceived { get; set; } = [];

        public ICollection<AppUserRole> UserRoles { get; set; } = [];

        public int GetAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;

            // Adjust if the birthdate has not yet occurred this year
            if (DateOfBirth > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
