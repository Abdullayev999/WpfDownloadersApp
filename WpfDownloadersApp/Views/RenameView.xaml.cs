using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfDownloadersApp.Models;

namespace WpfDownloadersApp.Views
{ 
    public partial class RenameView : Window
    {
        public DownFile DownFile { get; set; }
        public RenameView(DownFile downFile)
        {
            InitializeComponent();
            DataContext = this;
            this.DownFile = downFile;
        }
         
        private RelayCommand cancelCommand = null;

        public RelayCommand CancelCommand => cancelCommand = new RelayCommand(
        () =>
        {
            this.Close();
        });

        private RelayCommand<string> saveCommand = null;

        public RelayCommand<string> SaveCommand => saveCommand = new RelayCommand<string>(
        (Name) =>
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                FileInfo fileInfo = new FileInfo(DownFile.Path);
                var newPath = fileInfo.FullName.Replace(DownFile.Name, Name);                
                DownFile.Name = Name; 
                File.Move(DownFile.Path, newPath);
                DownFile.Path = newPath; 
                this.Close(); 
            } 
        });
    }
     
}
