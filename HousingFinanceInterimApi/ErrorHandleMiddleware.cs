using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HousingFinanceInterimApi
{

    public class ErrorHandlerMiddleware
    {

        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                Debugger.Break();
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int) HttpStatusCode.NotFound;

                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int) HttpStatusCode.InternalServerError;

                        break;
                }

                var result = JsonSerializer.Serialize(new
                {
                    message = error?.Message
                });
                await response.WriteAsync(result);
            }
        }

    }

}
