using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ThrottleHttpCalls.Middleware
{
    public class ThrottleRequestHandler : DelegatingHandler
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _period;
        private readonly LinkedList<DateTime> _previousRequests = new LinkedList<DateTime>();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public ThrottleRequestHandler(int maxRequests, TimeSpan period)
        {
            _maxRequests = maxRequests;
            _period = period;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken);

                if (_previousRequests.Count >= _maxRequests)
                {
                    var timeSinceLast = DateTime.Now - _previousRequests.Last.Value;

                    if (timeSinceLast < _period)
                    {
                        await Task.Delay(_period - timeSinceLast, cancellationToken);
                    }
                    _previousRequests.RemoveLast();
                }

                _previousRequests.AddFirst(DateTime.Now);

                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
