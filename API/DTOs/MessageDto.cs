﻿using API.Entities;

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public required string SenderUserName { get; set; }
        public int SenderId { get; set; }
        public string? SenderPhotoUrl { get; set; }
        public required string RecipientUserName { get; set; }
        public int RecipientId { get; set; }
        public  string? RecipientPhotoUrl { get; set; }
        public required string Content { get; set; } = null!;
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }

    }
}
