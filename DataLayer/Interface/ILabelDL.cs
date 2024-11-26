using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
    public interface ILabelDL
    {
        public Task<ResponseModel<Label>> CreateLabelAsync(string labelName);

        public Task<ResponseModel<string>> AddNoteToLabelAsync(int noteId, int labelId);

        public Task<ResponseModel<List<Label>>> GetAllLabelsAsync();

        public Task<ResponseModel<Label>> GetLabelByIdAsync(int labelId);

        public  Task<ResponseModel<bool>> DeleteLabelAsync(int labelId);
    }
}
