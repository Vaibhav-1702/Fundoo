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
    public class CollaboratorBL : ICollaboratorBL
    {
        private readonly ICollaboratorDL _collaboratorDL;
        public CollaboratorBL(ICollaboratorDL collaboratorDL)
        {
            _collaboratorDL = collaboratorDL;
        }
        public async Task<ResponseModel<User>> AddCollaboratorAsync(AddCollaboratorDto dto)
        {
            return await _collaboratorDL.AddCollaboratorAsync(dto);
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
