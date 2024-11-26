using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;

using Model.DTO;
using Model.Model;
using Model.Utility;
using Microsoft.AspNetCore.Authorization;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteBL _noteBL;

        public NotesController(INoteBL noteBL)
        {
            _noteBL = noteBL;
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

        [Authorize]
        [HttpGet("GetByNoteId")]
        public async Task<ResponseModel<Note>> GetNoteById(int noteId)
        {
            return await _noteBL.GetNoteById(noteId);
        }

        [HttpGet("GetByUserId")]
        public async Task<ResponseModel<List<Note>>> GetNotesByUserId(int userId)
        {
            return await _noteBL.GetNotesByUserId(userId);
        }

        [HttpDelete("DeleteById")]
        public async Task<ResponseModel<Note>> DeleteNoteById(int noteId, int userId, bool isDeleted)
        {
            return await _noteBL.DeleteNoteById(noteId, userId, isDeleted);
        }
    }
}