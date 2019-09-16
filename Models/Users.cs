using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DatingApp.API.Models
{
    public class Users
    {
        public Users()
        {
            Photos = new Collection<Photo>();
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KonownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        //One to many
        public ICollection<Photo> Photos { get; set; }

    }

}