using System;
using Lykke.Service.EthereumSamurai.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Common.Log;

namespace Lykke.Service.EthereumSamurai.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILog _logger;

        public GlobalExceptionFilter(ILog logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {

        }

        public void OnException(ExceptionContext context)
        {
            var action     = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];
            var httpCode   = 500;

            var type    = ExceptionType.None;
            var message = "Internal server error. Try again.";

            if (context.Exception is ClientSideException clientSideException)
            {
                type     = clientSideException.ExceptionType;
                httpCode = 400;
                message  = clientSideException.Message;
            }

            _logger.WriteErrorAsync
            (
                "GlobalExceptionFilter",
                "OnException",
                $"Controller: {controller}, action: {action}",
                context.Exception,
                DateTime.UtcNow
            );

            var ex = new ApiException
            {
                Error = new ApiError
                {
                    Code    = type,
                    Message = message
                }
            };

            context.Result = new ObjectResult(ex)
            {
                StatusCode   = httpCode,
                DeclaredType = typeof(ApiException)
            };
        }
    }

    public class ApiException
    {
        public ApiError Error { get; set; }
    }

    public class ApiError
    {
        public ExceptionType Code { get; set; }
        public string Message { get; set; }
    }

    public class ClientSideException : Exception
    {
        public ClientSideException(ExceptionType type) : this(type, "")
        {
        }

        public ClientSideException(ExceptionType type, string message) : base(message)
        {
            ExceptionType = type;
        }

        public ExceptionType ExceptionType { get; }
    }

    public enum ExceptionType
    {
        None
    }
}