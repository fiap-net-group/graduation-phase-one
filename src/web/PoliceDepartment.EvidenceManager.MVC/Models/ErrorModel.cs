using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public sealed class ErrorModel
    {
        public string Message { get; internal set; }
        public string Title { get; internal set; }
        public int ErrorCode { get; internal set; }

        public ErrorModel(int errorCode)
        {
            if (errorCode == 404)
            {
                Message = "The page you are looking for doesn't exists! <br />If you need help, contact our suport";
                Title = "Page not found.";
                ErrorCode = errorCode;
                return;
            }

            if (errorCode == 403)
            {
                Message = "You don't have permission to do that.";
                Title = "Access denied";
                ErrorCode = errorCode;
                return;
            }

            Message = "An error ocurred! Try again later.";
            Title = "An error ocurred!";
            ErrorCode = errorCode;
        }
    }
}