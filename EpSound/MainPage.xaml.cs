using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
using Windows.Media.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UI = Microsoft.UI.Xaml.Controls;
using EpSound.ViewModel;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EpSound
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private Visibility _filterPaneVisibility = Visibility.Collapsed;
        private string prevTag = "";
        private ViewModel.FilterParamMgrViewModel filterParamMgr;
        bool optionChanged;
        TrackViewModel rightClickedTrack;

        Color _selectIndicatorColor = Colors.Transparent;

        public Color SelectIndicatorColor
        {
            get => _selectIndicatorColor;
            set
            {
                _selectIndicatorColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectIndicatorColor)));
            }
        }

        public Visibility FilterPaneVisibility
        {
            get => _filterPaneVisibility;
            set
            {
                _filterPaneVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilterPaneVisibility)));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is ViewModel.FilterParamMgrViewModel mgr)
            {
                filterParamMgr = mgr;
                filterParamMgr.FilterOptionChanged += FilterParamMgr_FilterOptionChanged;
            }
        }

        private void FilterParamMgr_FilterOptionChanged(object sender, EventArgs e)
        {
            optionChanged = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void NavigationView_ItemInvoked(UI.NavigationView sender, UI.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args.InvokedItemContainer.Tag as string;
            if (tag == prevTag)
            {
                await CompleteSelection();
            }
            else
            {
                prevTag = tag;

                if(tag == "Genres")
                {
                    Navigate(typeof(FilterPage.GenresPage));
                }
                else if(tag == "Moods")
                {
                    Navigate(typeof(FilterPage.MoodsPage));
                }
                else if(tag == "Movement")
                {
                    Navigate(typeof(FilterPage.MovementPage));
                }
                else if (tag == "Places")
                {
                    Navigate(typeof(FilterPage.PlacesPage));
                }
                else if (tag == "Misc")
                {
                    Navigate(typeof(FilterPage.MiscFilterPage));
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                FilterPaneVisibility = Visibility.Visible;
                SetSelectPipeColor(tag);
            }

            void Navigate(Type t)
            {
                FilterFrame.Navigate(t, filterParamMgr, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            }
        }

        void SetSelectPipeColor(string tag)
        {
            switch (tag)
            {
                case "":
                    SelectIndicatorColor = Colors.Transparent;
                    break;
                case "Genres":
                    SelectIndicatorColor = (Color)App.Current.Resources["GenresColor"];
                    break;
                case "Moods":
                    SelectIndicatorColor = (Color)App.Current.Resources["MoodsColor"];
                    break;
                case "Movement":
                    SelectIndicatorColor = (Color)App.Current.Resources["MovementColor"];
                    break;
                case "Places":
                    SelectIndicatorColor = (Color)App.Current.Resources["PlacesColor"];
                    break;
                case "Misc":
                    SelectIndicatorColor = (Color)App.Current.Resources["MiscColor"];
                    break;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Remove test code.
            TrackListView.DataContext = await ViewModel.ModelVmAdapter.Test();
        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(!string.IsNullOrEmpty(args.QueryText))
            {
                TrackListView.DataContext = await ViewModel.ModelVmAdapter.SearchAll(args.QueryText);
            }
        }

        private async void LightDismissHelper_Click(object sender, RoutedEventArgs e)
        {
            await CompleteSelection();
        }

        private async Task CompleteSelection()
        {
            prevTag = "";
            FilterPaneVisibility = Visibility.Collapsed;
            SetSelectPipeColor(prevTag);
            if (optionChanged)
            {
                TrackListView.DataContext = await ViewModel.ModelVmAdapter.SearchAll(filterParamMgr);
                optionChanged = false;
            }
        }

        private void TrackListItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if(e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                ViewModel.TrackViewModel filter = (sender as FrameworkElement).DataContext as ViewModel.TrackViewModel;
                filter.IsHovered = true;
            }
        }

        private void TrackListItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                ViewModel.TrackViewModel filter = (sender as FrameworkElement).DataContext as ViewModel.TrackViewModel;
                filter.IsHovered = false;
            }
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            rightClickedTrack = ((sender as MenuFlyout).Target as ListViewItem).Content as TrackViewModel;
        }

        #region Play Media
        private async Task PlayMedia(ClientLibrary.Track track)
        {
            // TODO: Implement custom controls and play.
            TrackInfoViewModel info = ModelVmAdapter.CreateTrackInfoViewModel(await ModelVmAdapter.GetTrackInfo(track));
        }

        private async void TrackListView_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                ClientLibrary.Track track = ((TrackViewModel)((ListViewItem)e.OriginalSource).Content).Track;
                await PlayMedia(track);
            }
        }

        private async void TrackListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await PlayMedia((e.ClickedItem as TrackViewModel).Track);
        }

        private async void PlayMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if(rightClickedTrack != null)
            {
                await PlayMedia(rightClickedTrack.Track);
            }
        }
        #endregion

        #region Save Track
        async Task<Windows.Storage.StorageFile> InvokeSavePicker(string suggestedName)
        {
            string name = suggestedName;
            foreach(char c in Path.GetInvalidFileNameChars())
            {
                suggestedName.Replace(c, ' ');
            }

            Windows.Storage.Pickers.FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            picker.FileTypeChoices.Add("MP3 Files", new List<string>() { ".mp3" });
            picker.SuggestedFileName = name;

            return await picker.PickSaveFileAsync();
        }
        async Task<bool> SaveTrack(ClientLibrary.Track track)
        {
            Windows.Storage.StorageFile file = await InvokeSavePicker(track.Title);
            if(file != null)
            {
                ClientLibrary.TrackInfo info = await ModelVmAdapter.GetTrackInfo(track);
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                using (Stream str = await file.OpenStreamForWriteAsync())
                {
                    await ModelVmAdapter.DownloadToStream(info, str);
                }
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if(status == Windows.Storage.Provider.FileUpdateStatus.Failed || status == Windows.Storage.Provider.FileUpdateStatus.Incomplete)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task SaveTrackWrapper(ClientLibrary.Track track)
        {
            bool success = await SaveTrack(track);
            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Download failed",
                    Content = "Please check your Internet connection, \nor try to save to another location.",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private async void DownloadMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (rightClickedTrack != null)
            {
                await SaveTrackWrapper(rightClickedTrack.Track);
            }
        }
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveTrackWrapper(((sender as Button).DataContext as TrackViewModel).Track);
        }
        #endregion

        #region Similar Tracks
        private async void SimilarButton_Click(object sender, RoutedEventArgs e)
        {
            TrackListView.DataContext = await ModelVmAdapter.GetSimilarTracks(((sender as Button).DataContext as TrackViewModel).Track);
        }

        private async void SimilarMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (rightClickedTrack != null)
            {
                TrackListView.DataContext = await ModelVmAdapter.GetSimilarTracks(rightClickedTrack.Track);
            }
        }
        #endregion
    }

    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }

    public class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }

    public class DateDisplayConverter : IValueConverter
    {
        static DateTimeFormatter formatter = new DateTimeFormatter("shortdate");
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return formatter.Format((DateTime)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Parse((string)value);
        }
    }

    public class EnergyBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ClientLibrary.Energy energy = (ClientLibrary.Energy)value;
            if (energy == ClientLibrary.Energy.Medium)
            {
                return App.Current.Resources["MidEnergyBrush"];
            }
            if(energy == ClientLibrary.Energy.High)
            {
                return App.Current.Resources["HighEnergyBrush"];
            }
            return App.Current.Resources["LowEnergyBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
