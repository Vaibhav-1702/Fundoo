using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class UpdateNote
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
       
        public string? Color { get; set; }
        public bool IsArchived { get; set; }

        public bool IsDeleted { get; set; }
    }
}
