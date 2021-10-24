using Grpc.Net.Client;
using GRPCExampleShared.Core.ProtoFiles;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        private string baseUrl => "https://localhost:44332/";

        public GRPCHelper()
        {
            Console.WriteLine("Select grpc method:");
            var cursor = Console.ReadLine();
            switch (cursor)
            {
                case "1":
                    SayHello();
                    break;
                case "2":
                    StreamingFromServer();
                    break;
                case "3":
                    StreamingFromClient();
                    break;
                case "4":
                    StreamingBothWays();
                    break;
            }
        }

        public async void SayHello()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(baseUrl))
            {
                var client = new Greeter.GreeterClient(channel);
                var reply = await client.SayHelloAsync(new HelloRequest()
                {
                    Name = "Ömer Faruk Şahin"
                });
                Console.WriteLine(reply.Message);
            }
        }

        public async void StreamingFromServer()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(baseUrl))
            {
                var client = new Greeter.GreeterClient(channel);
                var token = new CancellationToken();
                var reply = client.StreamingFromServer(new ExampleRequest()
                {
                    PageSize = 1,
                    PageIndex = 1,
                    IsDescending = true
                }, null, null, token);

                var index = 0;
                while (await reply.ResponseStream.MoveNext(token))
                {
                    if (index == 5)
                    {
                        token.ThrowIfCancellationRequested();
                        break;
                    }
                    else
                        index++;
                    var current = reply.ResponseStream.Current;
                    Console.WriteLine(current.Message);
                }
                Console.WriteLine("Finish");
            }
        }

        public async void StreamingFromClient()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(baseUrl))
            {
                var client = new Greeter.GreeterClient(channel);

                var index = 0;
                var request = client.StreamingForClient();
                while (index != 5)
                {
                    await request.RequestStream.WriteAsync(new ExampleRequest());
                    index++;
                }
                await request.RequestStream.CompleteAsync();
                var res = await request.ResponseAsync;
                Console.WriteLine(res.Message);
            }
        }

        public async void StreamingBothWays()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(baseUrl))
            {
                var client = new Greeter.GreeterClient(channel);
                var request = client.StreamingBothWays();
                var token = new CancellationToken();

                var task = Task.Run(async () =>
                {
                    var index = 0;
                    while (index != 5)
                    {
                        await request.RequestStream.WriteAsync(new ExampleRequest());
                        index++;
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                    await request.RequestStream.CompleteAsync();
                });


                while (await request.ResponseStream.MoveNext(token))
                {
                    var current = request.ResponseStream.Current;
                    Console.WriteLine(current.Message);
                }

            }
        }
    }
}
