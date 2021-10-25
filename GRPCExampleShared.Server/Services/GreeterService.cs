using Grpc.Core;
using GRPCExampleShared.Core;
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
            return new ExampleResponse() { Message = "Hello World" };
        }

        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {

                }
                return Task.CompletedTask;
            });

            while (!readTask.IsCompleted)
            {
                await responseStream.WriteAsync(new ExampleResponse() { Message = "Hello World" });
                await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);
            }
        }

        public override Task<RoleReply> GetRoles(HelloRequest request, ServerCallContext context)
        {
            var result = new RoleReply();
            result.Roles.Add(new RoleItem()
            {
                Id = 1,
                Name = "Admin"
            });
            result.Roles.Add(new RoleItem()
            {
                Id = 2,
                Name = "User"
            });
            return Task.FromResult(result);
        }
    }
}
