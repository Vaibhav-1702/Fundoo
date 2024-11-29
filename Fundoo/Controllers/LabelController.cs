using BusinessLayer.Interface;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Model;
using Model.Utility;
using System.Net;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelBL _labelBL;
        private readonly ICacheDL _cacheDL;
        public LabelController(ILabelBL labelBL, ICacheDL cacheDL)
        {
            _labelBL = labelBL;
            _cacheDL = cacheDL;
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

        [HttpGet("Get-All-Label")]
        public async Task<ResponseModel<List<Label>>> GetAllLabelsAsync()
        {
            
            string cacheKey = "AllLabels";

            // Attempt to retrieve labels from Redis cache
            var cachedLabels = _cacheDL.GetData<List<Label>>(cacheKey);
            if (cachedLabels != null && cachedLabels.Count > 0)
            {
                return new ResponseModel<List<Label>>
                {
                    Data = cachedLabels,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Labels retrieved from cache successfully",
                    Success = true
                };
            }

            // Fetch labels from the database if not in cache
            var response = await _labelBL.GetAllLabelsAsync();

            // Cache the result if the response is successful
            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                var cacheExpiration = DateTimeOffset.Now.AddMinutes(10); 
                _cacheDL.SetData(cacheKey, response.Data, cacheExpiration);
            }

            return response;
        }


        [HttpGet("Get-Label-By-LabelId")]
        public async Task<ResponseModel<Label>> GetLabelByIdAsync(int labelId)
        {
            
            string cacheKey = $"Label_{labelId}";

            // Attempt to retrieve the label from Redis cache
            var cachedLabel = _cacheDL.GetData<Label>(cacheKey);
            if (cachedLabel != null)
            {
                return new ResponseModel<Label>
                {
                    Data = cachedLabel,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Label retrieved from cache successfully",
                    Success = true
                };
            }

            // Fetch the label from the database if not in cache
            var response = await _labelBL.GetLabelByIdAsync(labelId);

            // Cache the result if the response is successful
            if (response.Success && response.Data != null)
            {
                var cacheExpiration = DateTimeOffset.Now.AddMinutes(10);
                _cacheDL.SetData(cacheKey, response.Data, cacheExpiration);
            }

            return response;
        }


        [HttpDelete("Delete-Label-By-LabelId")]
        public async Task<ResponseModel<bool>> DeleteLabelAsync(int labelId)
        {
            
            var response = await _labelBL.DeleteLabelAsync(labelId);

            if (response.Success)
            {
                
                string labelCacheKey = $"Label_{labelId}";
                string allLabelsCacheKey = "AllLabels";

               
                _cacheDL.RemoveData(labelCacheKey);

                // Also remove the cached list of all labels to ensure consistency
                _cacheDL.RemoveData(allLabelsCacheKey);
            }

            return response;
        }

    }
}
