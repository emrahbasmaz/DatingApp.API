using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Extension;
using DatingApp.Api.Models;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public async Task<Users> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<Users>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(s => s.Id != userParams.UserId);
            users = users.Where(p => p.Gender == userParams.Gender);


            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(p => userLikers.Any(liker => liker.LikerId == p.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(p => userLikees.Any(likee => likee.LikerId == p.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                //users = users.Where(u => u.DateOfBirth.CalculateAge() >= userParams.MinAge && u.DateOfBirth.CalculateAge() <= userParams.MaxAge);
                var min = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var max = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth >= min && u.DateOfBirth <= max);
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<Users>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<Like>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                                    .Include(x => x.Likee)
                                    .Include(p => p.Liker)
                                    .FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likee.Where(s => s.LikeeId == id);
            }
            else
            {
                return user.Likee.Where(s => s.LikerId == id);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return await photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int id)
        {
            return await _context.Photos.Where(s => s.UsersId == id).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recepientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recepientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(p => p.Sender).ThenInclude(p => p.Photos)
                                            .Include(u => u.Recipient).ThenInclude(u => u.Photos)
                                            .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(p => p.RecipientId == messageParams.UserId);
                    break;
                case "Outbox":
                    messages = messages.Where(p => p.SenderId == messageParams.UserId);
                    break;
                default:
                    messages = messages.Where(p => p.RecipientId == messageParams.UserId && p.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(p => p.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }


        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(p => p.Sender).ThenInclude(p => p.Photos)
                                           .Include(u => u.Recipient).ThenInclude(u => u.Photos)
                                           .Where(p => p.RecipientId == userId && p.SenderId == recipientId ||
                                           p.RecipientId == recipientId && p.SenderId == userId)
                                           .OrderByDescending(m => m.MessageSent)
                                           .ToListAsync();

            return messages;
        }
    }
}
