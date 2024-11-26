using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model.Model
{
    [Table("Notes")]
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? Color { get; set; } 
        public bool IsArchived { get; set; } 

        public bool IsDeleted { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

        public ICollection<Label> Labels { get; set; } = new List<Label>();

    }
}
