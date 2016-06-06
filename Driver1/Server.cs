using System;
using System.Collections.Concurrent;
using System.ServiceModel;

namespace Driver1
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server : Interface.IDriver
    {
        public ConcurrentDictionary<Interface.IDevice, int> devices = new ConcurrentDictionary<Interface.IDevice, int>();

        public void Authenticate()
        {
            OperationContext.Current.Channel.Faulted += (sender, args) =>
                Console.WriteLine("{0} - Client connection failed.", DateTime.Now);
            OperationContext.Current.Channel.Closed += (sender, args) =>
            {
                int unused;
                devices.TryRemove((Interface.IDevice) sender, out unused);
                Console.WriteLine("{0} - Client connection closed. {1} active.", 
                                  DateTime.Now, devices.Count);
            };

            Interface.IDevice user = OperationContext.Current.GetCallbackChannel<Interface.IDevice>();
            devices.TryAdd(user, 0);
            Console.WriteLine("{0} - Client called 'Authenticate'. {1} active.", 
                              DateTime.Now, devices.Count);

            BroadcastState();
        }

        public void BroadcastState()
        {
            // TODO
        }

        public void Execute(string action)
        { 
            // TODO

            BroadcastState();
        }

        public void Disconnect()
        {
            Console.WriteLine("{0} - Client called 'Disconnect', {1} users active", 
                              DateTime.Now, devices.Count - 1);
        }
    }
}
