using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDownloadersApp.Messages
{
    public class NavigationMessage : Messenger
    {
        public Type ViewModelType { get; set; }
    }
}
