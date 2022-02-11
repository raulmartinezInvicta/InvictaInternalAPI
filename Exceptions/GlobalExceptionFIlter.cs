using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Exceptions
{
    public class GlobalExceptionFIlter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception.GetType() == typeof(BusinessException))
            {
                var exception = (BusinessException)context.Exception;
                var validation = new
                {
                    Status = 409,
                    Title = "Invicta Business Exception",
                    Detail = exception.Message
                };
                var json = new
                {
                    errors = new[] { validation }
                };
                context.Result = new ConflictObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.ExceptionHandled = true;
            }
        }
    }
}
