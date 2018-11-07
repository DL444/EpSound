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
        private FilterParamMgrViewModel filterParamMgr;
        bool optionChanged;
        bool _noItem = true;
        bool _loading;
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
        public bool NoItem
        {
            get => _noItem;
            set
            {
                _noItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NoItem)));
            }
        }
        public bool IsLoading
        {
            get => _loading;
            set
            {
                _loading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is FilterParamMgrViewModel mgr)
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

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(!string.IsNullOrEmpty(args.QueryText))
            {
                filterParamMgr.Clear();
                optionChanged = false;
                IsLoading = true;
                TrackListViewModel list = await ModelVmAdapter.SearchAll(args.QueryText);
                IsLoading = false;
                SetTrackList(list);
            }
        }

        private void SetTrackList(TrackListViewModel tracks)
        {
            TrackListView.DataContext = tracks;
            NoItem = tracks.Tracks.Count == 0;
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
                IsLoading = true;
                var list = await ModelVmAdapter.SearchAll(filterParamMgr);
                IsLoading = false;
                SetTrackList(list);
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
            TrackInfoViewModel info = ModelVmAdapter.CreateTrackInfoViewModel(await ModelVmAdapter.GetTrackInfo(track));
            MediaSource source = MediaSource.CreateFromUri(new Uri(info.FileUri));

            Windows.Media.Playback.MediaPlaybackItem playbackItem = new Windows.Media.Playback.MediaPlaybackItem(source);
            Windows.Media.Playback.MediaItemDisplayProperties props = playbackItem.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = info.Title;
            props.MusicProperties.Artist = info.Authors;
            props.MusicProperties.Genres.Add(track.Genres);
            playbackItem.ApplyDisplayProperties(props);

            MediaPlayer.Source = playbackItem;
            MediaPlayer.MediaPlayer.AudioCategory = Windows.Media.Playback.MediaPlayerAudioCategory.Media;
            MediaPlayer.Visibility = Visibility.Visible;
            TrackListView.Padding = new Thickness(16, 46, 16, 120);
            (MediaPlayer.TransportControls as MediaControl.EsMediaPlayer).Title = info.Title;
            (MediaPlayer.TransportControls as MediaControl.EsMediaPlayer).Author = info.Authors;
            MediaPlayer.MediaPlayer.Play();
        }
        private async Task PlayMedia(TrackViewModel trackVm)
        {
            await PlayMedia(trackVm.Track);
        }

        private async void TrackListView_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            bool altDown = Windows.UI.Core.CoreWindow.GetForCurrentThread().GetKeyState(Windows.System.VirtualKey.Menu) 
                != Windows.UI.Core.CoreVirtualKeyStates.None;
            ClientLibrary.Track track = ((TrackViewModel)((ListViewItem)e.OriginalSource).Content).Track;

            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                await PlayMedia(track);
            }
            else if(e.Key == Windows.System.VirtualKey.T && altDown)
            {
                await StemWrapper(new TrackViewModel(track)); 
            }
            else if(e.Key == Windows.System.VirtualKey.S && altDown)
            {
                await SaveTrackWrapper(track);
            }
            else if(e.Key == Windows.System.VirtualKey.M && altDown)
            {
                filterParamMgr.Clear();
                optionChanged = false;
                IsLoading = true;
                var list = await ModelVmAdapter.GetSimilarTracks(track);
                IsLoading = false;
                SetTrackList(list);
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
        private async void DownloadSwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            await SaveTrackWrapper((args.SwipeControl.DataContext as TrackViewModel).Track);
        }
        #endregion

        #region Similar Tracks
        private async void SimilarButton_Click(object sender, RoutedEventArgs e)
        {
            filterParamMgr.Clear();
            optionChanged = false;
            IsLoading = true;
            var list = await ModelVmAdapter.GetSimilarTracks(((sender as Button).DataContext as TrackViewModel).Track);
            IsLoading = false;
            SetTrackList(list);
        }

        private async void SimilarMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (rightClickedTrack != null)
            {
                filterParamMgr.Clear();
                optionChanged = false;
                IsLoading = true;
                var list = await ModelVmAdapter.GetSimilarTracks(rightClickedTrack.Track);
                IsLoading = false;
                SetTrackList(list);
            }
        }
        private async void SimilarSwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            filterParamMgr.Clear();
            optionChanged = false;
            IsLoading = true;
            var list = await ModelVmAdapter.GetSimilarTracks((args.SwipeControl.DataContext as TrackViewModel).Track);
            IsLoading = false;
            SetTrackList(list);
        }
        #endregion

        #region Stems
        async Task StemWrapper(TrackViewModel trackVm)
        {
            StemSelector selector = new StemSelector(trackVm);
            await selector.ShowAsync();
            if(selector.Action == StemSelector.SelectedAction.Play)
            {
                TrackViewModel selectedTrack = selector.SelectedTrack;
                await PlayMedia(selectedTrack);
            }
            else if(selector.Action == StemSelector.SelectedAction.Download)
            {
                TrackViewModel selectedTrack = selector.SelectedTrack;
                await SaveTrackWrapper(selectedTrack.Track);
            }
        }

        private async void StemsButton_Click(object sender, RoutedEventArgs e)
        {
            await StemWrapper((sender as Button).DataContext as TrackViewModel);
        }

        private async void StemMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (rightClickedTrack != null)
            {
                await StemWrapper(new TrackViewModel(rightClickedTrack.Track));
            }
        }

        private async void StemSwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            await StemWrapper(args.SwipeControl.DataContext as TrackViewModel);
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

    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((bool)value == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility)value == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class BoolVisibilityInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value == true)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility)value == Visibility.Visible)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
