using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EpSound.ViewModel
{
    public class FilterParamMgrViewModel
    {
        ClientLibrary.FilterParameterManager manager = new ClientLibrary.FilterParameterManager();

        public FilterParamMgrViewModel() { }

        public FilterParamMgrViewModel(IEnumerable<FilterParamViewModel> filters)
        {
            this.filters = filters.ToList();
            foreach(FilterParamViewModel f in filters)
            {
                switch(f.TagType)
                {
                    case FilterTagType.Genre:
                        GenresFilters.Add(f);
                        break;
                    case FilterTagType.Mood:
                        MoodsFilters.Add(f);
                        break;
                    case FilterTagType.Movement:
                        MovementFilters.Add(f);
                        break;
                    case FilterTagType.Setting:
                        PlacesFilters.Add(f);
                        break;
                    case FilterTagType.Energy:
                        EnergyFilters.Add(f);
                        break;
                    case FilterTagType.Tempo:
                        TempoFilters.Add(f);
                        break;
                    case FilterTagType.Length:
                        LengthFilters.Add(f);
                        break;
                }
                f.FilterChanged += Filter_FilterChanged;
            }
        }

        public string RequestString => manager.GetRequestString();

        List<FilterParamViewModel> filters = new List<FilterParamViewModel>();

        public ObservableCollection<FilterParamViewModel> GenresFilters { get; } = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> MoodsFilters { get; } = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> MovementFilters { get; } = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> PlacesFilters { get; } = new ObservableCollection<FilterParamViewModel>();

        public ObservableCollection<FilterParamViewModel> EnergyFilters { get; } = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> TempoFilters { get; } = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> LengthFilters { get; } = new ObservableCollection<FilterParamViewModel>();

        public void Clear()
        {
            for(int i = 0; i < filters.Count; i++)
            {
                filters[i].IsEnabled = false;
            }
            manager.Clear();
        }

        public event EventHandler FilterOptionChanged;

        private void Filter_FilterChanged(object sender, FilterChangedEventArgs e)
        {
            if (e.Filter.IsEnabled)
            {
                manager.Add(e.Filter.Parameter);
            }
            else
            {
                manager.Remove(e.Filter.Parameter);
            }
            FilterOptionChanged?.Invoke(this, null);
        }
    }
}
