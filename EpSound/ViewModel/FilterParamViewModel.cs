using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

namespace EpSound.ViewModel
{
    public class FilterParamViewModel : INotifyPropertyChanged
    {
        FilterParameter _param;

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

        public bool IsEnabled { get; set; }

        public FilterParamViewModel() : this(new PlaceholderParameter()) { }
        public FilterParamViewModel(FilterParameter param)
        {
            Parameter = param ?? new PlaceholderParameter();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
                    return FilterTagType.Tempo;
                default:
                    return FilterTagType.Unknown;
            }
        }
    }

    public enum FilterTagType
    {
        Placeholder, Mood, Movement, Setting, Genre, Subgenre, Energy, Tempo, Length, Unknown
    }
}
