using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBDcwiczenia5.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            if(httpContext.Request != null)
            {
                string path = httpContext.Request.Path;     // api/students
                string method = httpContext.Request.Method; // GET, POST
                string queryString = httpContext.Request.QueryString.ToString();
                string bodyStr = "";

                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024,true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }
                //zapis do pliku / bazy danych
                System.IO.File.AppendAllText(@"C:\Users\janek\Desktop\repos\APBDcwiczenia5\APBDcwiczenia5\requestsLog.txt", "REQUEST:\n");

                System.IO.File.AppendAllText(@"C:\Users\janek\Desktop\repos\APBDcwiczenia5\APBDcwiczenia5\requestsLog.txt", httpContext.Request.Method +"\n");
                System.IO.File.AppendAllText(@"C:\Users\janek\Desktop\repos\APBDcwiczenia5\APBDcwiczenia5\requestsLog.txt", "PATH: " + httpContext.Request.Path + "\n");
                System.IO.File.AppendAllText(@"C:\Users\janek\Desktop\repos\APBDcwiczenia5\APBDcwiczenia5\requestsLog.txt", "QS: " + httpContext.Request.QueryString.ToString() + "\n");
                System.IO.File.AppendAllText(@"C:\Users\janek\Desktop\repos\APBDcwiczenia5\APBDcwiczenia5\requestsLog.txt", "BODY: " +  bodyStr + "\n >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n");
               


            }

            if (httpContext != null)
            await _next(httpContext);
        }


    }
}
