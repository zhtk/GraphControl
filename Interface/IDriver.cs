using System;
using System.ServiceModel;

namespace Interface
{
    [ServiceContract(CallbackContract = typeof(IDevice), SessionMode = SessionMode.Required)]
    public interface IDriver
    {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void Authenticate();

        [OperationContract(IsInitiating = false, IsOneWay = true)]
        void Execute(string action);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Disconnect();
    }
}
