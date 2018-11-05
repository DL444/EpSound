using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UI = Microsoft.UI.Xaml.Controls;

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

        private async System.Threading.Tasks.Task CompleteSelection()
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
            ViewModel.TrackViewModel filter = (sender as FrameworkElement).DataContext as ViewModel.TrackViewModel;
            filter.IsHovered = true;
        }

        private void TrackListItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.TrackViewModel filter = (sender as FrameworkElement).DataContext as ViewModel.TrackViewModel;
            filter.IsHovered = false;
        }

        private void TrackListView_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            // TODO: Implement play.
            // Get TrackViewModel from e.OriginalSource.Content
        }
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
