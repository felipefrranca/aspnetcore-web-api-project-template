using System.Net;

namespace Domain.CustomExceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string title, HttpStatusCode statusCode, string detail)
        {
            Title = title;
            StatusCode = statusCode;
            Detail = detail;
        }

        public string Title { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Detail { get; set; }
    }
}
