
using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class NoteBL : INoteBL
    {
        private readonly INoteDL _noteDL;

        public NoteBL(INoteDL noteDL)
        {
            _noteDL = noteDL; 
        }
        public async Task<ResponseModel<Note>> CreateNote(int userId,CreateNote note)
        {
            return await _noteDL.CreateNote(userId,note);
        }

        public async Task<ResponseModel<Note>> UpdateNote(int noteId, int userId, UpdateNote updatedNote)
        {
            return await _noteDL.UpdateNote(noteId,userId,updatedNote);
        }

        public async Task<ResponseModel<Note>> ArchiveNote(int noteId)
        {
            return await _noteDL.ArchiveNote(noteId);
        }

        public async Task<ResponseModel<Note>> GetNoteById(int noteId)
        {
            return await _noteDL.GetNoteById(noteId);
        }

        public async Task<ResponseModel<List<Note>>> GetNotesByUserId(int userId)
        {
            return await _noteDL.GetNotesByUserId(userId);
        }

        public async Task<ResponseModel<Note>> DeleteNoteById(int noteId, int userId, bool isDeleted)
        {
            return await _noteDL.DeleteNoteById(noteId,userId, isDeleted);
        }
    }
}
