using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class LabelBL : ILabelBL
    {
        private readonly ILabelDL _labelDL;
        public LabelBL(ILabelDL labelDL)
        {
            _labelDL = labelDL;
        }

        public async Task<ResponseModel<Label>> CreateLabelAsync( string labelName)
        {
            return await _labelDL.CreateLabelAsync(labelName);
        }

        public async Task<ResponseModel<string>> AddNoteToLabelAsync(int noteId, int labelId)
        {
            return await _labelDL.AddNoteToLabelAsync(noteId, labelId);
        }

        public async Task<ResponseModel<List<Label>>> GetAllLabelsAsync()
        {
            return await _labelDL.GetAllLabelsAsync();
        }

        public async Task<ResponseModel<Label>> GetLabelByIdAsync(int labelId)
        {
            return await _labelDL.GetLabelByIdAsync(labelId);
        }

        public async Task<ResponseModel<bool>> DeleteLabelAsync(int labelId)
        {
            return await _labelDL.DeleteLabelAsync(labelId);
        }
    }
}
