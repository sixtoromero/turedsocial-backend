using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;

namespace gluky.webapi.Configurations
{
    public static class ExceptionHandlerConfiguration
    {
        public static IApplicationBuilder UseApiGlobalExceptionHandler(this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseExceptionHandler(HandleApiException());
        }

        private static Action<IApplicationBuilder> HandleApiException()
        {
            return appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var guidException = Guid.NewGuid().ToString();
                    if (exceptionHandlerFeature != null) Log.Error(exceptionHandlerFeature.Error, "");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var errorModel = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = exceptionHandlerFeature.Error.Message,
                        Detail = $"Trace id: {guidException}-{exceptionHandlerFeature.Error}",
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorModel));
                });
            };
        }
    }
}
