using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
    public interface ICollaboratorDL
    {
        public Task<string> AddCollaboratorAsync(int userId, int noteId);

        public Task<List<User>> GetCollaboratorsForNoteAsync(int noteId);

        public Task<bool> RemoveCollaboratorAsync(int collaboratorId);
    }
}
