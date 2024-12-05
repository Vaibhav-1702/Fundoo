using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
     public interface ICollaboratorBL
    {
        public Task<ResponseModel<User>> AddCollaboratorAsync(AddCollaboratorDto dto);

        public Task<List<User>> GetCollaboratorsForNoteAsync(int noteId);

        public Task<bool> RemoveCollaboratorAsync(int collaboratorId);
    }
}
