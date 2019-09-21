using Microsoft.AspNetCore.Http;
using System;

namespace DatingApp.Api.Dtos
{
    [Serializable]
    public class PhotoForCreationDto
    {
        public PhotoForCreationDto()
        {
            //DateAdded = DateTime.Now;
        }

        //public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        //public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public int UserId { get; set; }
        //public virtual IFormCollection[] FormData { get; set; }
    }
}
