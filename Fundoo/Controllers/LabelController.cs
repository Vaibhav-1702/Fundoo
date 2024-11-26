using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Model;
using Model.Utility;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelBL _labelBL;
        public LabelController(ILabelBL labelBL)
        {
            _labelBL = labelBL;
        }

        [HttpPost("Create-Label")]
        public async Task<ResponseModel<Label>> CreateLabelAsync( string labelName)
        {
            return await _labelBL.CreateLabelAsync(labelName);
        }

        [HttpPost("Add-Notes-To-Label")]
        public async Task<ResponseModel<string>> AddNoteToLabelAsync(int noteId, int labelId)
        {
            return await _labelBL.AddNoteToLabelAsync(noteId, labelId);
        }

        [HttpGet("Get-All-label")]
        public async Task<ResponseModel<List<Label>>> GetAllLabelsAsync()
        {
            return await _labelBL.GetAllLabelsAsync();
        }

        [HttpGet("Get-Label-By-LabelId")]
        public async Task<ResponseModel<Label>> GetLabelByIdAsync(int labelId)
        {
            return await _labelBL.GetLabelByIdAsync(labelId);
        }

        [HttpDelete("Delete-label-By-LabelId")]
        public async Task<ResponseModel<bool>> DeleteLabelAsync(int labelId)
        {
            return await _labelBL.DeleteLabelAsync(labelId);
        }
    }
}
