using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class CollaboratorBL : ICollaboratorBL
    {
        private readonly ICollaboratorDL _collaboratorDL;
        public CollaboratorBL(ICollaboratorDL collaboratorDL)
        {
            _collaboratorDL = collaboratorDL;
        }
        public async Task<string> AddCollaboratorAsync(int userId, int noteId)
        {
            return await _collaboratorDL.AddCollaboratorAsync(userId, noteId);
        }

        public async Task<List<User>> GetCollaboratorsForNoteAsync(int noteId)
        {
            return await _collaboratorDL.GetCollaboratorsForNoteAsync(noteId);
        }

        public async Task<bool> RemoveCollaboratorAsync(int collaboratorId)
        {
            return await _collaboratorDL.RemoveCollaboratorAsync(collaboratorId);
        }
    }
}
