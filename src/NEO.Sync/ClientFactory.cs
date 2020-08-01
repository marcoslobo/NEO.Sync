using NeoModules.JsonRpc.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEO.Api.Worker
{
    public class ClientFactory
    {
        public static IClient GetClient(Settings settings)
        {
            return new RpcClient(new Uri(settings.GetRpcUrl()));
        }
    }
}
