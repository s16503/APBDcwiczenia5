using APBDcwiczenia5.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBDcwiczenia5.Middlewares
{
    public class IndexMiddleware
    {
        private readonly RequestDelegate _next;
        IStudentDbService _service;
        public IndexMiddleware(RequestDelegate next, IStudentDbService service)
        {
            _next = next;
            _service = service;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {

            if (!httpContext.Request.Headers.ContainsKey("Index"))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Nie podałeś indeksu !!!");
                return;
            }


            string index = httpContext.Request.Headers["Index"].ToString();
            var stud = _service.GetStudent(index);

            if (stud == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsync("Nie znaleziono indeksu!");
                return;
            }


            await _next(httpContext);

        }
    }
}
