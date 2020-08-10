using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MySocNet.Exceptions;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MySocNet.Logger
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILog logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exceptionContext = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionContext?.Error;

                    if (exception == null)
                        return;

                    logger.Error($"Something went wrong: {exceptionContext.Error}");

                    context.Response.ContentType = "application/json";

                    if (exception is EntityNotFoundException)
                         context.Response.StatusCode = 404;

                    if (exception is Exceptions.UnauthorizedAccessException)
                        context.Response.StatusCode = 401;

                    if (exception is Exception)
                        context.Response.StatusCode = 400;

                    await context.Response.WriteAsync(new ErrorDetails(exception.Message).ToString());
                });
            });
        }
    }
}
