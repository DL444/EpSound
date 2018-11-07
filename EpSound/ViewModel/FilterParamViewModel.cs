using ClientLibrary;
using System;
using System.ComponentModel;
using Windows.UI.Xaml.Media;

namespace EpSound.ViewModel
{
    public class FilterParamViewModel : INotifyPropertyChanged
    {
        FilterParameter _param;
        bool _isEnabled;

        public FilterParameter Parameter
        {
            get => _param;
            set
            {
                _param = value;
                TagType = GetTagType(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        public FilterTagType TagType { get; private set; }
        public string DisplayName => Parameter.DisplayName;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                FilterChanged?.Invoke(this, new FilterChangedEventArgs(this));
            }
        }

        public SolidColorBrush BackgroundBrush
        {
            get
            {
                switch(TagType)
                {
                    case FilterTagType.Genre:
                    case FilterTagType.Subgenre:
                        return (SolidColorBrush)App.Current.Resources["GenresBrush"];
                    case FilterTagType.Mood:
                        return (SolidColorBrush)App.Current.Resources["MoodsBrush"];
                    case FilterTagType.Movement:
                        return (SolidColorBrush)App.Current.Resources["MovementBrush"];
                    case FilterTagType.Setting:
                        return (SolidColorBrush)App.Current.Resources["PlacesBrush"];
                    case FilterTagType.Energy:
                    case FilterTagType.Length:
                    case FilterTagType.Tempo:
                        return (SolidColorBrush)App.Current.Resources["MiscBrush"];
                    default:
                        return null;
                }
            }
        }

        public FilterParamViewModel() : this(new PlaceholderParameter()) { }
        public FilterParamViewModel(FilterParameter param)
        {
            Parameter = param ?? new PlaceholderParameter();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<FilterChangedEventArgs> FilterChanged;

        static FilterTagType GetTagType(FilterParameter param)
        {
            switch(param)
            {
                case PlaceholderParameter _:
                    return FilterTagType.Placeholder;
                case GenreParameter _:
                    return FilterTagType.Genre;
                case SubgenreParameter _:
                    return FilterTagType.Subgenre;
                case MoodParameter _:
                    return FilterTagType.Mood;
                case MovementParameter _:
                    return FilterTagType.Movement;
                case SettingParameter _:
                    return FilterTagType.Setting;
                case EnergyParameter _:
                    return FilterTagType.Energy;
                case TempoParameter _:
                    return FilterTagType.Tempo;
                case LengthParameter _:
                    return FilterTagType.Length;
                default:
                    return FilterTagType.Unknown;
            }
        }
    }

    public enum FilterTagType
    {
        Placeholder, Mood, Movement, Setting, Genre, Subgenre, Energy, Tempo, Length, Unknown
    }

    public class FilterChangedEventArgs : EventArgs
    {
        public FilterParamViewModel Filter { get; private set; }

        public FilterChangedEventArgs(FilterParamViewModel filter) => Filter = filter;

        public FilterChangedEventArgs() { }
    }
}
