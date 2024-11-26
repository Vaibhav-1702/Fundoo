using Model.DTO;
using Model.Model;
using Model.Utility;

namespace BusinessLayer.Interface
{
    public interface INoteBL
    {
        public Task<ResponseModel<Note>> CreateNote(int userId, CreateNote note);
        public Task<ResponseModel<Note>> UpdateNote(int noteId, int userId, UpdateNote updatedNote);
        public Task<ResponseModel<Note>> ArchiveNote(int noteId);
        public Task<ResponseModel<Note>> GetNoteById(int noteId);
        public Task<ResponseModel<List<Note>>> GetNotesByUserId(int userId);
        public Task<ResponseModel<Note>> DeleteNoteById(int noteId, int userId, bool isDeleted);
    }
}
