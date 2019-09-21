using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Extension;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;

        public PhotosController(IDatingRepository datingRepository,
                                IHostingEnvironment hostingEnvironment,
                                IMapper mapper)
        {
            this._datingRepository = datingRepository;
            this._hostingEnvironment = hostingEnvironment;
            this._mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _datingRepository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost("{id}", Name = "AddPhotoForUser")]
        public async Task<IActionResult> AddPhotoForUser([FromBody] PhotoForCreationDto photoForCreationDto)
        {
            try
            {
                var currentUser = await _datingRepository.GetUser(photoForCreationDto.UserId);
                if (currentUser == null)
                {
                    return BadRequest("User not found");
                }
                string webRootPath = @"C:\Users\emrah\source\repos\Demo\DatingApp\DatingApp.API\WebImages\Upload";
                AmazonUploader.AmazonS3Uploader.keyName = photoForCreationDto.FileName;
                AmazonUploader.AmazonS3Uploader.filePath = webRootPath;
                var result = await AmazonUploader.AmazonS3Uploader.UploadFileAsync();
                photoForCreationDto.Url = "https://barstechcloudtestbucket2019.s3.amazonaws.com/" + photoForCreationDto.FileName;

                var photo = _mapper.Map<Photo>(photoForCreationDto);

                if (!photo.IsMain)
                {
                    photo.IsMain = true;
                }
                currentUser.Photos.Add(photo);

                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                if (await _datingRepository.SaveAll())
                {
                    return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
                }

                return BadRequest("Phot could not added to AWS s3 service...");
            }
            catch (Exception ex)
            {
                return BadRequest("Phot could not added to AWS s3 service...Message :" + ex.Message);
            }
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int id)
        {
            var photoFromRepo = await _datingRepository.GetPhoto(id);
            if (photoFromRepo == null)
                return NotFound();

            if (photoFromRepo.IsMain)
                return BadRequest("This is already set Main picture");

            var currentMainPhoto = await _datingRepository.GetMainPhotoForUser(id);
            if (currentMainPhoto != null)
                currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _datingRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to Main...");
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadImage()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = @"C:\Users\emrah\source\repos\Demo\DatingApp\DatingApp.API\WebImages\";
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Ok("Upload Successful.");
            }
            catch (Exception ex)
            {
                return Ok("Upload Failed: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            var photoFromRepo = await _datingRepository.GetPhoto(id);
            if (photoFromRepo == null)
                return NotFound();
            if (photoFromRepo.IsMain)
                return BadRequest("Can not delete Main picture");

            if (photoFromRepo.PublicId != null)
            {
                #region Delete From amazon S3
                //toDo
                #endregion
            }
            else
            {
                _datingRepository.Delete(photoFromRepo);
            }

            if (await _datingRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to Main...");
        }
    }
}