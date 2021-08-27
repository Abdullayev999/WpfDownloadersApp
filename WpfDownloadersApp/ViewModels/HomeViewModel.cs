using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using WpfDownloadersApp.Models;
using WpfDownloadersApp.Views;

namespace WpfDownloadersApp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public WebClient webClient { get; set; }
        public string SearchPath { get; set; }
        public string DownloadPath { get; set; } 
        public bool IsAll { get; set; }
        public bool IsError { get; set; }
        public bool IsDownloaded { get; set; }
        public bool InProcDown { get; set; } = true; 
        public HomeViewModel()
        {
            webClient = new WebClient(); 
            ListDownload = new ObservableCollection<DownFile>();
            Dovwnloads = new ObservableCollection<DownFile>(); 
        }
         
        private RelayCommand searchFolderPathCommand = null;
        public RelayCommand SearchFolderPathCommand => searchFolderPathCommand = new RelayCommand(
        () =>
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SearchPath = folderBrowserDialog.SelectedPath;
            } 
        });
        private ObservableCollection<DownFile> dovwnloads;

        public ObservableCollection<DownFile> Dovwnloads
        {
            get { return dovwnloads; }
            set {
                dovwnloads = value; 
                Refresh();
            }
        }

        public ObservableCollection<DownFile> ListDownload { get; set; }

        private RelayCommand startCommand = null;
        public RelayCommand StartCommand => startCommand = new RelayCommand(
        () =>
        {
            DownloadFile();
            Refresh();
        });

        private RelayCommand<DownFile> deleteCommand = null;

        public RelayCommand<DownFile> DeleteCommand => deleteCommand = new RelayCommand<DownFile>(
        (file) =>
        {
            if (file.Messages.Equals("This file is downloaded"))
            {
                File.Delete(file.Path);
                Dovwnloads.Remove(file); 
                Refresh();
            }
            else  MessageBox.Show("This file not download", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
        });

        private RelayCommand<DownFile> moveCommand = null;

        public RelayCommand<DownFile> MoveCommand => moveCommand = new RelayCommand<DownFile>(
        (file) =>
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var newPath = folderBrowserDialog.SelectedPath + "\\" + file.Name;
                File.Move(file.Path, newPath);
                file.Path = newPath;
                Refresh();
            }
        });


        private RelayCommand<DownFile> renameCommand = null;

        public RelayCommand<DownFile> RenameCommand => renameCommand = new RelayCommand<DownFile>(
        (file) =>
        {
            RenameView renameView = new RenameView(file);
            renameView.ShowDialog();
            Refresh();
        });

        private RelayCommand<DownFile> clearCommand = null;

        public RelayCommand<DownFile> ClearCommand => clearCommand = new RelayCommand<DownFile>(
        (file) =>
        {
            Dovwnloads.Remove(file);
            Refresh();
        });
        private void DownloadFile()
        {
            webClient = new WebClient();
            Task.Run(() =>
            {
                var newFile = new DownFile();
                try
                { 
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Dovwnloads.Add(newFile);
                        Refresh();
                    });
                    Task.Run(() =>
                    { 
                        Random random = new Random();
                        do
                        {
                            Thread.Sleep(random.Next(500, 900));
                            if (newFile.Value <= 100 - 5) newFile.Value = random.Next(1, 5); 
                            else newFile.Value = 0; 
                        } while (!newFile.IsFinsh);
                    });

                    var arr = DownloadPath.Split('/');
                    var nameWithoutExtension = arr[arr.Length - 1].Split('.')[0];
                    var nameWithExtension = arr[arr.Length - 1];
                    newFile.Name = nameWithoutExtension;
                    webClient.DownloadFile(new Uri(DownloadPath), nameWithExtension);
                    var tmp = SearchPath + "\\" + nameWithExtension;
                    newFile.Path = tmp;
                    File.Move(nameWithExtension, tmp);
                    newFile.Messages = "This file is downloaded";
                    newFile.IsFinsh = true;
                    newFile.IsMove = true;
                    newFile.IsRename = true;
                    newFile.IsDelete = true;
                    newFile.Brush = System.Windows.Media.Brushes.Green;
                    Thread.Sleep(900);
                    newFile.Value = 100;
                    Refresh();
                }
                catch (Exception ex)
                { 
                    newFile.Messages = ex.Message;
                    newFile.IsFinsh = true;
                    Refresh();
                } 
            });

        }

        private RelayCommand inProcDownloadCommand = null;

        public RelayCommand InProcDownloadCommand => inProcDownloadCommand = new RelayCommand(
        () =>
        {
            ListDownload.Clear();
            App.Current.Dispatcher.Invoke(() =>
            {
                ListDownload = new ObservableCollection<DownFile>(Dovwnloads.Where(i => i.Messages.Equals("In the process of loading")));
            });
        });


        private RelayCommand allDownloadCommand = null;

        public RelayCommand AllDownloadCommand => allDownloadCommand = new RelayCommand(
        () =>
        {
            ListDownload.Clear();
            App.Current.Dispatcher.Invoke(() =>
            {
                ListDownload = new ObservableCollection<DownFile>(Dovwnloads);
            });

        });


        private RelayCommand downloadedCommand = null;

        public RelayCommand DownloadedCommand => downloadedCommand = new RelayCommand(
        () =>
        {
            ListDownload.Clear();
            App.Current.Dispatcher.Invoke(() =>
            {
                ListDownload = new ObservableCollection<DownFile>(Dovwnloads.Where(i => i.Messages.Equals("This file is downloaded")));
            });

        });


        private RelayCommand downloadErorCommand = null;

        public RelayCommand DownloadErorCommand => downloadErorCommand = new RelayCommand(
        () =>
        {
            ListDownload.Clear();
            App.Current.Dispatcher.Invoke(() =>
            {
                ListDownload = new ObservableCollection<DownFile>(Dovwnloads.Where(i => !i.Messages.Equals("This file is downloaded") && !i.Messages.Equals("In the process of loading")));
            });

        });


        public void Refresh()
        {
            if (IsError)
            {
               
                App.Current.Dispatcher.Invoke(() =>
                {
                    ListDownload.Clear();
                    ListDownload = new ObservableCollection<DownFile>(Dovwnloads.Where(i => !i.Messages.Equals("This file is downloaded") && !i.Messages.Equals("In the process of loading")));
                });
            }
            else if (IsDownloaded)
            {
               
                App.Current.Dispatcher.Invoke(() =>
                {
                    ListDownload.Clear();
                    ListDownload = new ObservableCollection<DownFile>(Dovwnloads.Where(i => i.Messages.Equals("This file is downloaded")));
                });
            }
            else if (IsAll)
            { 
                App.Current.Dispatcher.Invoke(() =>
                {
                    ListDownload.Clear();
                    ListDownload = new ObservableCollection<DownFile>(Dovwnloads);
                });
            }
            else
            { 
                App.Current.Dispatcher.Invoke(() =>
                {
                    ListDownload.Clear();
                    ListDownload = new ObservableCollection<DownFile>(Dovwnloads.Where(i => i.Messages.Equals("In the process of loading")));
                });
            }
        }
    }
}
