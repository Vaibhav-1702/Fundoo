using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    [Table("Collaborators")]
    public class Collaborator
    {
        [Key]
        public int CollaboratorId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Note")]
        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
