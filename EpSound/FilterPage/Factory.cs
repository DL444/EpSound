using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EpSound.FilterPage
{
    interface IFilterPage
    {
        ViewModel.FilterParamMgrViewModel FilterManager { get; }
    }

    static class Factory
    {
        public static IFilterPage GetFilterPage(FilterPageType type, ViewModel.FilterParamMgrViewModel filterMgr)
        {
            switch (type)
            {
                case FilterPageType.Genres:
                    return new GenresPage(filterMgr);
                case FilterPageType.Moods:
                    return new MoodsPage(filterMgr);
                case FilterPageType.Movement:
                    return new MovementPage(filterMgr);
                case FilterPageType.Places:
                    return new PlacesPage(filterMgr);
                case FilterPageType.Misc:
                    return new MiscFilterPage(filterMgr);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }

    enum FilterPageType
    {
        Genres, Moods, Movement, Places, Misc
    }
}
