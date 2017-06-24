using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;

namespace Prometheus.Core
{
    public class GetHealthQuery : IRequest<object>
    {
        
    }

    public class GetHealthQueryHandler : IAsyncRequestHandler<GetHealthQuery, object>
    {
        private readonly IOptions<GeneralSettings> settings;

        public GetHealthQueryHandler(IOptions<GeneralSettings> settings)
        {
            this.settings = settings;
        }

        public async Task<object> Handle(GetHealthQuery message)
        {
            return await Task.FromResult(new
            {
                Online = true,
                Version = this.settings.Value.ApplicationVersion,
                Timestamp = DateTime.UtcNow,
                Environment.ProcessorCount,
                Environment.MachineName
            });
        }
    }
}