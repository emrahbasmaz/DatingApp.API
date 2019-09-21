﻿using DatingApp.Api.Models;
using DatingApp.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Api.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task<bool> SaveAll();

        Task<IEnumerable<Users>> GetUsers();
        Task<Users> GetUser(int id);
        Task<Photo> GetPhoto(int id);

        Task<Photo> GetMainPhotoForUser(int id);

    }
}
