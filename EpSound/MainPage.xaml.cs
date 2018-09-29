using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NavigationView_ItemInvoked(UI.NavigationView sender, UI.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args.InvokedItemContainer.Tag as string;
            if (tag == prevTag)
            {
                prevTag = "";
                FilterPaneVisibility = Visibility.Collapsed;
                SelectIndicatorColor = Colors.Transparent;
            }
            else
            {
                prevTag = tag;
                FilterPaneVisibility = Visibility.Visible;
                SelectIndicatorColor = Colors.Blue;
            }
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
            return !(bool)value;
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
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
