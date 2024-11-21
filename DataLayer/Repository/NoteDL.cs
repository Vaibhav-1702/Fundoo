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
    public class NoteDL : INoteDL
    { 
        private readonly FundooContext _Context;

        public NoteDL(FundooContext Context)
        {
            _Context = Context; 
        }


        public async Task<ResponseModel<Note>> CreateNote(int userId, CreateNote note)
        {
            try
            {
                var user = _Context.users.FirstOrDefault(user=>user.UserId.Equals(userId));
                var notes = new Note()
                {
                    Title = note.Title,
                    Description = note.Description,
                    CreatedAt = DateTime.Now,
                    UserId = user.UserId
                };

                await _Context.notes.AddAsync(notes);
                await _Context.SaveChangesAsync();

                return new ResponseModel<Note>
                {
                    Data = notes,
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = "Note Created",
                    Success = true
                };
            }
            catch (DbUpdateException dbEx)
            {
                
                Console.WriteLine($"Database Update Exception: {dbEx.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Database error occurred: " + dbEx.Message,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"General Exception: {ex.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Note Creation Failed",
                    Success = false
                };
            }
        }


        public async Task<ResponseModel<Note>> UpdateNote(int noteId, UpdateNote updatedNote)
        {
            try
            {
                // Find the note by its ID
                var note = await _Context.notes.FirstOrDefaultAsync(n => n.NoteId == noteId);

                if (note == null)
                {
                    return new ResponseModel<Note>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Note not found",
                        Success = false
                    };
                }

                // Update note properties
                note.Title = updatedNote.Title;
                note.Description = updatedNote.Description;
                note.UpdatedAt = DateTime.Now;
                note.Color = updatedNote.Color;
                note.IsArchived = updatedNote.IsArchived;

                _Context.notes.Update(note);
                await _Context.SaveChangesAsync();

                return new ResponseModel<Note>
                {
                    Data = note,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Note updated successfully",
                    Success = true
                };
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Update Exception: {dbEx.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Database error occurred: " + dbEx.Message,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Note update failed",
                    Success = false
                };
            }
        }


        public async Task<ResponseModel<Note>> ArchiveNote(int noteId)
        {
            try
            {
                
                var note = await _Context.notes.FirstOrDefaultAsync(n => n.NoteId == noteId);

                if (note == null)
                {
                    return new ResponseModel<Note>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Note not found",
                        Success = false
                    };
                }

                
                note.IsArchived = true;
                note.UpdatedAt = DateTime.Now;

                _Context.notes.Update(note);
                await _Context.SaveChangesAsync();

                return new ResponseModel<Note>
                {
                    Data = note,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Note archived successfully",
                    Success = true
                };
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Update Exception: {dbEx.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Database error occurred: " + dbEx.Message,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to archive note",
                    Success = false
                };
            }
        }


        public async Task<ResponseModel<Note>> GetNoteById(int noteId)
        {
            try
            {
                // Find the note by its ID
                var note = await _Context.notes.FirstOrDefaultAsync(n => n.NoteId == noteId);

                if (note == null)
                {
                    return new ResponseModel<Note>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Note not found",
                        Success = false
                    };
                }

                return new ResponseModel<Note>
                {
                    Data = note,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Note retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to retrieve note",
                    Success = false
                };
            }
        }


        public async Task<ResponseModel<List<Note>>> GetNotesByUserId(int userId)
        {
            try
            {
                // Find all notes associated with the given userId
                var notes = await _Context.notes
                                          .Where(n => n.UserId == userId)
                                          .ToListAsync();

                if (notes == null || notes.Count == 0)
                {
                    return new ResponseModel<List<Note>>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "No notes found for the user",
                        Success = false
                    };
                }

                return new ResponseModel<List<Note>>
                {
                    Data = notes,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Notes retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new ResponseModel<List<Note>>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to retrieve notes",
                    Success = false
                };
            }
        }


        public async Task<ResponseModel<Note>> DeleteNoteById(int noteId, bool isDeleted)
        {
            try
            {
                // Find the note by its ID
                var note = await _Context.notes.FirstOrDefaultAsync(n => n.NoteId == noteId);

                if (note == null)
                {
                    return new ResponseModel<Note>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Note not found",
                        Success = false
                    };
                }

                if (!isDeleted)
                {
                    // Move the note to trash by setting IsDeleted flag to true
                    note.IsDeleted = true;
                    note.UpdatedAt = DateTime.Now;

                    _Context.notes.Update(note);
                    await _Context.SaveChangesAsync();

                    return new ResponseModel<Note>
                    {
                        Data = note,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = "Note moved to trash successfully",
                        Success = true
                    };
                }
                else
                {
                    // Permanently delete the note
                    _Context.notes.Remove(note);
                    await _Context.SaveChangesAsync();

                    return new ResponseModel<Note>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.OK,
                        Message = "Note deleted permanently",
                        Success = true
                    };
                }
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Update Exception: {dbEx.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Database error occurred: " + dbEx.Message,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return new ResponseModel<Note>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Failed to delete note",
                    Success = false
                };
            }
        }




    }
}
