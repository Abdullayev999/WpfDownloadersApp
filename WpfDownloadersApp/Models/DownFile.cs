using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDownloadersApp.Models
{
    public class DownFile : ViewModelBase
    {
        public int Value { get; set; } = 0;
        public string Messages { get; set; } = "In the process of loading";
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsRename { get; set; }
        public bool IsMove { get; set; }
        public bool IsDelete { get; set; }
        public Brush Brush { get; set; } =Brushes.Red;
        public bool IsFinsh { get; set; }

    }
}
