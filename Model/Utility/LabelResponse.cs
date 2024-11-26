using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utility
{
    public class LabelResponse
    {
        public int LabelId { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        public LabelResponse()
        {
            
        }

        public LabelResponse(int labelid, string name, string message)
        {
            LabelId = labelid;
            Name = name;    
            Message = message;
        }
    }
}
