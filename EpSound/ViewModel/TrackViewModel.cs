using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;
using Windows.UI.Xaml.Data;

namespace EpSound.ViewModel
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        Track _track;

        public Track Track
        {
            get => _track;
            set
            {
                _track = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        public string Title => Track.Title;
        public string Authors => Track.Authors;
        public string Genres => Track.Genres;
        public string Category => Track.Category;
        public int Bpm => Track.Bpm;
        public Energy Energy => Track.Energy;
        public DateTime ReleaseTime => Track.ReleaseDate.Date;

        public TrackViewModel() : this(new Track()) { }
        public TrackViewModel(Track track)
        {
            this.Track = track ?? new Track();
        }

        public override string ToString()
        {
            return Track.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
