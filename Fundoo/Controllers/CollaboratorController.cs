using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Model;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorBL _collaboratorBL;
        public CollaboratorController(ICollaboratorBL collaboratorBL)
        {
            _collaboratorBL = collaboratorBL;
        }

        [HttpPost]
        public async Task<IActionResult> AddCollaboratorAsync(int userId, int noteId)
        {
            var result  =  await _collaboratorBL.AddCollaboratorAsync(userId, noteId);
            if (result == "User does not exist." || result == "Note does not exist.")
            {
                return NotFound(result);
            }

            if (result == "Collaborator already exists for this note.")
            {
                return BadRequest(result);
            }

            return Ok(result);

        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetCollaboratorsForNoteAsync(int noteId)
        {
            var response = await _collaboratorBL.GetCollaboratorsForNoteAsync(noteId);

            if (response == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete]
        public async Task<bool> RemoveCollaboratorAsync(int collaboratorId)
        {
            return await _collaboratorBL.RemoveCollaboratorAsync(collaboratorId);
        }

    }
}
