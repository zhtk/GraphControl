using System;
using System.ServiceModel;
using System.Drawing;
using System.Windows.Forms;

namespace Interface
{
    public interface IDevice
    {
        [OperationContract(IsOneWay = true)]
        void SetImage(Image image);

        [OperationContract(IsOneWay = true)]
        void SetMenuItems(MenuItem[] items);
    }
}
