using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class LabelDL : ILabelDL
    {
        private readonly FundooContext _context;
        public LabelDL(FundooContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<Label>> CreateLabelAsync(string labelName)
        {
            
            if (string.IsNullOrWhiteSpace(labelName))
            {
                return new ResponseModel<Label>
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Label name cannot be empty.",
                    Data = null,
                    Success = false
                };
            }

            
            var existingLabel = await _context.Labels
                .FirstOrDefaultAsync(l => l.LabelName == labelName);

            if (existingLabel != null)
            {
                return new ResponseModel<Label>
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = "A label with the same name already exists.",
                    Data = null,
                    Success = false
                };
            }

            
            var label = new Label
            {
                LabelName = labelName
            };

           
            _context.Labels.Add(label);
            await _context.SaveChangesAsync();

            return new ResponseModel<Label>
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "Label created successfully.",
                Data = label,
                Success = true
            };
        }

        public async Task<ResponseModel<string>> AddNoteToLabelAsync(int noteId, int labelId)
        {
            
            var label = await _context.Labels
                .Include(l => l.Notes) // Include related notes for checking duplicates
                .FirstOrDefaultAsync(l => l.LabelId == labelId);

            if (label == null)
            {
                return new ResponseModel<string>
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Label not found.",
                    Data = null,
                    Success = false
                };
            }

            
            var note = await _context.notes.FindAsync(noteId);
            if (note == null)
            {
                return new ResponseModel<string>
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Note not found.",
                    Data = null,
                    Success = false
                };
            }

            // Check if the note is already associated with the label
            if (label.Notes.Any(n => n.NoteId == noteId))
            {
                return new ResponseModel<string>
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = "This note is already associated with the label.",
                    Data = null,
                    Success = false
                };
            }

            
            label.Notes.Add(note);
            await _context.SaveChangesAsync();

            return new ResponseModel<string>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Note successfully added to the label.",
                Data = "Note added to label.",
                Success = true
            };
        }

        public async Task<ResponseModel<List<Label>>> GetAllLabelsAsync()
        {
            var labels = await _context.Labels.ToListAsync();

            if (!labels.Any())
            {
                return new ResponseModel<List<Label>>
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "No labels found.",
                    Data = null,
                    Success = false
                };
            }

            return new ResponseModel<List<Label>>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Labels retrieved successfully.",
                Data = labels,
                Success = true
            };
        }

        public async Task<ResponseModel<Label>> GetLabelByIdAsync(int labelId)
        {
            var label = await _context.Labels.FindAsync(labelId);

            if (label == null)
            {
                return new ResponseModel<Label>
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Label not found.",
                    Data = null,
                    Success = false
                };
            }

            return new ResponseModel<Label>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Label retrieved successfully.",
                Data = label,
                Success = true
            };
        }

        public async Task<ResponseModel<bool>> DeleteLabelAsync(int labelId)
        {
            
            var label = await _context.Labels.FindAsync(labelId);

            if (label == null)
            {
                return new ResponseModel<bool>
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Label not found.",
                    Data = false,
                    Success = false
                };
            }

            
            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Label deleted successfully.",
                Data = true,
                Success = true
            };
        }



    }
}
