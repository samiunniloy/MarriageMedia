using Microsoft.AspNetCore.Authentication;

namespace API.Entities
{
    public class Connection
    {

        public required string ConectionId { get; set; }
        
        public required string Username { get; set; }
    }
}
