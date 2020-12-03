using Grpc.Core;
using Grpc.Core.Interceptors;

namespace metrics.Identity.Client.Abstractions
{
    public class IdentityClientAuthInterceptor : Interceptor
    {
        private readonly ISystemTokenGenerationService _systemTokenGenerationService;
        public IdentityClientAuthInterceptor(ISystemTokenGenerationService systemTokenGenerationService)
        {
            _systemTokenGenerationService = systemTokenGenerationService;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = new Metadata {{"Authorization", $"Bearer {_systemTokenGenerationService.GetSystemToken()}"}};
            var callOptions = context.Options.WithHeaders(metadata);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOptions);
            
            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}