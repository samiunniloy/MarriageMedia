﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub(IMessageRepository messageRepository,IUserRepository userRepository,IMapper mapper):Hub
    {  
       
        public override async Task OnConnectedAsync()
        {
          var httpContext = Context.GetHttpContext();
          var otherUser = httpContext?.Request.Query["user"];

            if(Context.User==null|| string.IsNullOrEmpty(otherUser))
            {
                throw new Exception("Invalid user");
            }

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            await base.OnDisconnectedAsync(exception);
            return;
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User?.GetUsername()??throw new Exception("could not get user");
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("Cant message yourself");
            }
            var sender = await userRepository.GetUserByUsernameAsync(username);
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null || sender == null ||
                sender.UserName == null || recipient.UserName == null
                )  throw new HubException("cant send message");
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            messageRepository.AddMessage(message);
            if (await messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage", message,mapper.Map<MessageDto>(message));
            }
            throw new HubException("cant send message");
        }

        private string GetGroupName(string caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }


    }
}
