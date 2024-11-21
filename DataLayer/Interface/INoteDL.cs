
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
    public interface INoteDL
    {
        public Task<ResponseModel<Note>> CreateNote(int userId,CreateNote note);

        public Task<ResponseModel<Note>> UpdateNote(int noteId, UpdateNote updatedNote);

        public Task<ResponseModel<Note>> ArchiveNote(int noteId);

        public Task<ResponseModel<Note>> GetNoteById(int noteId);

        public Task<ResponseModel<List<Note>>> GetNotesByUserId(int userId);

        public Task<ResponseModel<Note>> DeleteNoteById(int noteId, bool isDeleted);



    }
}
