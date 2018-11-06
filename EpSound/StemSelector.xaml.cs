using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EpSound
{
    public sealed partial class StemSelector : ContentDialog, INotifyPropertyChanged
    {
        ViewModel.TrackViewModel track;
        ViewModel.StemViewModel _selectedStem = new ViewModel.StemViewModel();


        ObservableCollection<ViewModel.StemViewModel> Stems { get; } = new ObservableCollection<ViewModel.StemViewModel>();

        public ViewModel.StemViewModel SelectedStem
        {
            get => _selectedStem;
            set
            {
                _selectedStem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStem)));
            }
        }
        public SelectedAction Action { get; set; }

        public StemSelector()
        {
            this.InitializeComponent();
        }
        public StemSelector(ViewModel.TrackViewModel trackVm) : this()
        {
            track = trackVm ?? new ViewModel.TrackViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int actionId = int.Parse(((Button)sender).Tag as string);
            Action = (SelectedAction)actionId;
            this.Hide();
        }

        private async void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            ProgRing.IsActive = true;
            if(track == null)
            {
                track = new ViewModel.TrackViewModel();
            }
            var stemsList = ViewModel.ModelVmAdapter.CreateStemViewModels(await ViewModel.ModelVmAdapter.GetTrackInfo(track.Track));
            foreach(var s in stemsList)
            {
                Stems.Add(s);
            }
            ProgRing.IsActive = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum SelectedAction
        {
            None, Play, Download
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStem = e.AddedItems[0] as ViewModel.StemViewModel;
        }
    }
}
