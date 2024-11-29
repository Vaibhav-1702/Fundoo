using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;

using Model.DTO;
using Model.Model;
using Model.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteBL _noteBL;
        private readonly ICacheDL _cacheDL;

        public NotesController(INoteBL noteBL , ICacheDL cacheDL)
        {
            _noteBL = noteBL;
            _cacheDL = cacheDL;
        }

        [HttpPost("Add-note")]
        public async Task<ResponseModel<Note>> CreateNote([FromQuery]int userId,[FromBody]CreateNote note)
        {
             
            return await _noteBL.CreateNote(userId,note);
        }

        [HttpPut("Update-note")]
        public async Task<ResponseModel<Note>> UpdateNote(int noteId, int userId, UpdateNote updatedNote)
        {
            return await _noteBL.UpdateNote(noteId, userId, updatedNote);
        }

        [HttpPost("Archive-note")]
        public async Task<ResponseModel<Note>> ArchiveNote(int noteId)
        {
            return await _noteBL.ArchiveNote(noteId);
        }


        [HttpGet("GetByNoteId")]
        public async Task<ResponseModel<Note>> GetNoteById(int noteId)
        {
            
            string cacheKey = $"Note_{noteId}";

            // Attempt to retrieve the note from Redis
            var cachedNote = _cacheDL.GetData<Note>(cacheKey);
            if (cachedNote != null)
            {
                return new ResponseModel<Note>
                {
                    Data = cachedNote,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Note retrieved from cache successfully",
                    Success = true
                };
            }

            // If not in cache, retrieve it from the database
            var response = await _noteBL.GetNoteById(noteId);

            // If the note is found in the database, cache it
            if (response.Success && response.Data != null)
            {
                
                var cacheExpiration = DateTimeOffset.Now.AddMinutes(10);
                _cacheDL.SetData(cacheKey, response.Data, cacheExpiration);
            }

            return response;
        }


        [HttpGet("GetByUserId")]
        public async Task<ResponseModel<List<Note>>> GetNotesByUserId(int userId)
        {
            
            string cacheKey = $"UserNotes_{userId}";

            // Attempt to retrieve the notes from Redis
            var cachedNotes = _cacheDL.GetData<List<Note>>(cacheKey);
            if (cachedNotes != null && cachedNotes.Count > 0)
            {
                return new ResponseModel<List<Note>>
                {
                    Data = cachedNotes,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Notes retrieved from cache successfully",
                    Success = true
                };
            }

            // If not in the cache, retrieve them from the database
            var response = await _noteBL.GetNotesByUserId(userId);

            // If notes are found in the database, cache them
            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                
                var cacheExpiration = DateTimeOffset.Now.AddMinutes(10);
                _cacheDL.SetData(cacheKey, response.Data, cacheExpiration);
            }

            return response;
        }


        [HttpDelete("DeleteById")]
        public async Task<ResponseModel<Note>> DeleteNoteById(int noteId, int userId, bool isDeleted)
        {
            
            var response = await _noteBL.DeleteNoteById(noteId, userId, isDeleted);

            if (response.Success && response.Data != null)
            {
                
                string noteCacheKey = $"Note_{noteId}";
                string userNotesCacheKey = $"UserNotes_{userId}";

                // Remove the specific note from the cache if it exists
                _cacheDL.RemoveData(noteCacheKey);
                
                _cacheDL.RemoveData(userNotesCacheKey);
            }

            return response;
        }

    }
}