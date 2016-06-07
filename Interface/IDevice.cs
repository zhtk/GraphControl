using System;
using System.ServiceModel;
using System.Drawing;
using System.Windows.Forms;

namespace Interface
{
    public interface IDevice
    {
        [OperationContract(IsOneWay = true)]
        void SetImage(byte[] image);

        [OperationContract(IsOneWay = true)]
        void SetMenuItems(String[] items);
    }
}
