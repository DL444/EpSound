using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

namespace EpSound.ViewModel
{
    public class TrackInfoViewModel : INotifyPropertyChanged
    {
        TrackInfo _info;

        public TrackInfo Info
        {
            get => _info;
            set
            {
                _info = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        public string Title => Info.Title;
        public string Authors => Info.Authors;
        public int Length => Info.Length;
        public bool HasBass => Info.HasBass;
        public bool HasDrums => Info.HasDrums;
        public bool HasInstruments => Info.HasInstruments;
        public bool HasMelody => Info.HasMelody;

        public TrackInfoType InfoType => Info.TrackInfoType;
        public bool HasVocals => Info.HasVocals;
        public string FileUri => Info.FileUri;


        public TrackInfoViewModel() : this(new TrackInfo()) { }
        public TrackInfoViewModel(TrackInfo info)
        {
            Info = info ?? new TrackInfo();
        }

        public override string ToString()
        {
            return Info.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
