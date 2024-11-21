using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<string> AddCollaboratorAsync(int userId, int noteId)
        {
            // Check if User exists
            var userExists = await _context.users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return "User does not exist.";
            }

            // Check if Note exists
            var noteExists = await _context.notes.AnyAsync(n => n.NoteId == noteId);
            if (!noteExists)
            {
                return "Note does not exist.";
            }

            // Check if the collaborator already exists
            var collaboratorExists = await _context.Collaborators
                .AnyAsync(c => c.UserId == userId && c.NoteId == noteId);
            if (collaboratorExists)
            {
                return "Collaborator already exists for this note.";
            }

            // Add the collaborator
            var collaborator = new Collaborator
            {
                UserId = userId,
                NoteId = noteId
            };

            _context.Collaborators.Add(collaborator);
            await _context.SaveChangesAsync();

            return "Collaborator added successfully.";
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
