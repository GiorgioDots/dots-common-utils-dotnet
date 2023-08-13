using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dots.Commons
{
    public class HttpException : Exception
    {
        public HttpException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; set; }
    }
    public class UnprocessableEntityException: HttpException
    {
        public UnprocessableEntityException(string message) : base(422, message)
        {
        }
    }
}
