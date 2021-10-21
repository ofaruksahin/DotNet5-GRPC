using Grpc.Core;
using GRPCServer.Example.ProtoFiles;
using System.Threading.Tasks;

namespace GRPCServer.Example.Services
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
    }
}
