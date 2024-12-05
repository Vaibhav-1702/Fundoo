using BusinessLayer.Interface;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System.Net;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorBL _collaboratorBL;
        private readonly ICacheDL _cacheDL;
        public CollaboratorController(ICollaboratorBL collaboratorBL,ICacheDL cacheDL)
        {
            _collaboratorBL = collaboratorBL;
            _cacheDL = cacheDL;
        }

        [HttpPost]
        public async Task<ResponseModel<User>> AddCollaboratorAsync(AddCollaboratorDto dto)
        {
            return await _collaboratorBL.AddCollaboratorAsync(dto);
        }


        [HttpGet("Get-Collaborators-For-Note")]
        public async Task<ActionResult<List<User>>> GetCollaboratorsForNoteAsync(int noteId)
        {
            
            string cacheKey = $"Collaborators_Note_{noteId}";

            // Attempt to retrieve collaborators from Redis cache
            var cachedCollaborators = _cacheDL.GetData<List<User>>(cacheKey);
            if (cachedCollaborators != null && cachedCollaborators.Count > 0)
            {
                return Ok(new ResponseModel<List<User>>
                {
                    Data = cachedCollaborators,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Collaborators retrieved from cache successfully",
                    Success = true
                });
            }

           
            var response = await _collaboratorBL.GetCollaboratorsForNoteAsync(noteId);

            if (response == null || response.Count == 0)
            {
                return NotFound(new ResponseModel<List<User>>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Collaborators not found",
                    Success = false
                });
            }

            // Cache the result if the response is successful
            var cacheExpiration = DateTimeOffset.Now.AddMinutes(15); 
            _cacheDL.SetData(cacheKey, response, cacheExpiration);

            return Ok(new ResponseModel<List<User>>
            {
                Data = response,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Collaborators retrieved successfully",
                Success = true
            });
        }


        [HttpDelete("Remove-Collaborator")]
        public async Task<ActionResult<bool>> RemoveCollaboratorAsync(int collaboratorId, int noteId)
        {
            
            var result = await _collaboratorBL.RemoveCollaboratorAsync(collaboratorId);

            if (!result)
            {
                return BadRequest(new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Failed to remove collaborator",
                    Success = false
                });
            }

            
            string cacheKey = $"Collaborators_Note_{noteId}";

            
            _cacheDL.RemoveData(cacheKey);

            return Ok(new ResponseModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Collaborator removed successfully",
                Success = true
            });
        }


    }
}
