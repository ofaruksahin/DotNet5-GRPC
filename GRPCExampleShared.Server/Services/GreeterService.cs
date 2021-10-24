using Grpc.Core;
using GRPCExampleShared.Core.ProtoFiles;
using System;
using System.Threading.Tasks;

namespace GRPCExampleShared.Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply()
            {
                Message = $"Hello {request.Name}"
            });
        }

        public override async Task StreamingFromServer(ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new ExampleResponse()
                {
                    Message = "Hello World"
                });
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        public override async Task<ExampleResponse> StreamingForClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
            }
            return new ExampleResponse() { Message = "Hello World"};
        }
    }
}
