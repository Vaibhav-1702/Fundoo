using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    [Table("label")]
    public class Label
    {
        [Key]
        public int LabelId { get; set; }

        [Required]
        public string LabelName { get; set; }
       
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
