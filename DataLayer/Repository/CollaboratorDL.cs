using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class CollaboratorDL : ICollaboratorDL
    {
        private readonly FundooContext _context;
        public CollaboratorDL(FundooContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<User>> AddCollaboratorAsync(AddCollaboratorDto dto)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(dto.EmailAddress))
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "Email address is required.",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                if (dto.NoteId <= 0)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "Valid note ID is required.",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                
                var user = await _context.users.FirstOrDefaultAsync(u => u.EmailAddress == dto.EmailAddress);
                if (user == null)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "User does not exist.",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                
                var noteExists = await _context.notes.AnyAsync(n => n.NoteId == dto.NoteId);
                if (!noteExists)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "Note does not exist.",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                // Check if the collaborator already exists
                var collaboratorExists = await _context.Collaborators
                    .AnyAsync(c => c.UserId == user.UserId && c.NoteId == dto.NoteId);
                if (collaboratorExists)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "Collaborator already exists for this note.",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                // Add the collaborator
                var collaborator = new Collaborator
                {
                    UserId = user.UserId,
                    NoteId = dto.NoteId
                };

                _context.Collaborators.Add(collaborator);
                await _context.SaveChangesAsync();

                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "Collaborator added successfully.",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "An unexpected error occurred.",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }


        public async Task<List<User>> GetCollaboratorsForNoteAsync(int noteId)
        {
            // Check if the note exists
            var noteExists = await _context.notes.AnyAsync(n => n.NoteId == noteId);
            if (!noteExists)
            {
                throw new KeyNotFoundException($"Note with ID {noteId} does not exist.");
            }

            // Fetch collaborators
            var collaborators = await _context.Collaborators
                .Where(c => c.NoteId == noteId)
                .Include(c => c.User) 
                .Select(c => c.User)  
                .ToListAsync();

            return collaborators;
        }

        public async Task<bool> RemoveCollaboratorAsync(int collaboratorId)
        {
            var collaborator = await _context.Collaborators.FindAsync(collaboratorId);

            if (collaborator == null)
            {
                return false; // Collaborator not found
            }

            _context.Collaborators.Remove(collaborator);
            await _context.SaveChangesAsync();

            return true; // Successfully removed
        }



    }
}
