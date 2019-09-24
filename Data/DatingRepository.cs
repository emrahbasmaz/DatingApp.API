﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Extension;
using DatingApp.Api.Models;
using DatingApp.API.Data;
using DatingApp.API.Models;
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

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                users = users.Where(u => u.DateOfBirth.CalculateAge() >= userParams.MinAge && u.DateOfBirth.CalculateAge() <= userParams.MaxAge);
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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Photo> GetPhoto(int id)
        {
            var photo = _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public Task<Photo> GetMainPhotoForUser(int id)
        {
            return _context.Photos.Where(s => s.UsersId == id).FirstOrDefaultAsync(p => p.IsMain);
        }
    }
}
