using Microsoft.Extensions.Logging;
using UsersMS.Core.Application.Logger;

namespace UsersMS.Core.Infrastructure.Logger
{
    public class LoggerAspect : ILoggerContract
    {
        private readonly ILogger<LoggerAspect> _logger;

        public LoggerAspect(ILogger<LoggerAspect> logger)
        {
            _logger = logger;
        }

        public void Log(params string[] data)
        {
            _logger.LogInformation("{Message}", string.Join(" ", data));
        }

        public void Error(params string[] data)
        {
            _logger.LogError("ERROR: {Message}", string.Join(" ", data));
        }

        public void Exception(params string[] data)
        {
            _logger.LogError("EXCEPTION: {Message}", string.Join(" ", data));
        }
    }
}