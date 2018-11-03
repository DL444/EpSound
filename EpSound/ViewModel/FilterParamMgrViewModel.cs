using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpSound.ViewModel
{
    public class FilterParamMgrViewModel
    {
        public FilterParamMgrViewModel() { }

        public FilterParamMgrViewModel(IEnumerable<FilterParamViewModel> filters)
        {
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
            }
        }

        public ObservableCollection<FilterParamViewModel> GenresFilters = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> MoodsFilters = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> MovementFilters = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> PlacesFilters = new ObservableCollection<FilterParamViewModel>();

        public ObservableCollection<FilterParamViewModel> EnergyFilters = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> TempoFilters = new ObservableCollection<FilterParamViewModel>();
        public ObservableCollection<FilterParamViewModel> LengthFilters = new ObservableCollection<FilterParamViewModel>();
    }
}
