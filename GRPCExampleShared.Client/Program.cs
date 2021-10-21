using Grpc.Net.Client;
using GRPCExampleShared.Core.ProtoFiles;
using System;

namespace GRPCExampleShared.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            new GRPCHelper();
            Console.ReadKey();
        }
    }

    public class GRPCHelper
    {
        public GRPCHelper()
        {
            Initialize();
        }
        
        public async void Initialize()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:44332/"))
            {
                var client = new Greeter.GreeterClient(channel);
                var reply = await client.SayHelloAsync(new HelloRequest()
                {
                    Name = "Ömer Faruk Şahin"
                });
                Console.WriteLine(reply.Message);
            }
        }
    }
}
