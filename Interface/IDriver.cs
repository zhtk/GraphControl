using System;
using System.ServiceModel;

namespace Interface
{
    /// <summary>
    /// Interfejs sterownika
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IDevice), SessionMode = SessionMode.Required)]
    public interface IDriver
    {
        /// <summary>
        /// Podłączenie się do sterownika i wpisanie na jego listę subskrynentów
        /// </summary>
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void Authenticate();

        /// <summary>
        /// Wydaje sterownikowi rozkaz wykonania pewnej akcji
        /// </summary>
        /// <param name="action"> Napis, który pojawił się na menu kontekstowym </param>
        [OperationContract(IsInitiating = false, IsOneWay = true)]
        void Execute(string action);

        /// <summary>
        /// Rozłącza z sterownikiem
        /// </summary>
        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Disconnect();
    }
}
