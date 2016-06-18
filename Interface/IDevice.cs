using System;
using System.ServiceModel;
using System.Drawing;
using System.Windows.Forms;

namespace Interface
{
    /// <summary>
    /// Interfejs dający dostęp sterownikowi do elementów ekranu
    /// operatorskiego
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Ustawia obrazek wyświetlany na ekranie
        /// </summary>
        /// <param name="image"></param>
        [OperationContract(IsOneWay = true)]
        void SetImage(byte[] image);

        /// <summary>
        /// Ustawia możliwe do wykonania akcje w menu kontekstowym
        /// </summary>
        /// <param name="items">
        /// Lista napisów które mają być wyświetlone. Po wybraniu danej akcji napis
        /// zostanie odesłany do sterownika
        /// </param>
        [OperationContract(IsOneWay = true)]
        void SetMenuItems(String[] items);
    }
}
