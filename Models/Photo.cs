using DatingApp.API.Models;
using System;

namespace DatingApp.Api.Models
{
    public class Photo
    {
        public Photo()
        {
            //User = new Users();
        }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }

        //public Users User { get; set; }
        public int UsersId { get; set; }
    }
}
