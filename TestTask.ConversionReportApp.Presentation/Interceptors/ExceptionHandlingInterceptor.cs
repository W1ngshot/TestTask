using Grpc.Core;
using Grpc.Core.Interceptors;

namespace TestTask.ConversionReportApp.Presentation.Interceptors;

public class ExceptionHandlingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}