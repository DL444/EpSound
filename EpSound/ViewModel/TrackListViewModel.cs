using System.Collections.ObjectModel;

namespace EpSound.ViewModel
{
    public class TrackListViewModel
    {
        public ObservableCollection<TrackViewModel> Tracks { get; } = new ObservableCollection<TrackViewModel>();
    }
}
