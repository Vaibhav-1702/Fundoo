using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utility
{
    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool Success { get; set; }

        public ResponseModel()
        {
            
        }

        public ResponseModel(int statusCode, string message, T data, bool success)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Success = success;
        }
    }
}
