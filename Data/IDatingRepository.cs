using DatingApp.Api.Extension;
using DatingApp.Api.Models;
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

        Task<PagedList<Users>> GetUsers(UserParams userParams);
        Task<Users> GetUser(int id);
        Task<Photo> GetPhoto(int id);

        Task<Photo> GetMainPhotoForUser(int id);

        Task<Like> GetLike(int userId, int recepientId);

        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams message);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);

    }
}
