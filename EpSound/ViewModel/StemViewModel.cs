using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EpSound.ViewModel
{
    public class StemViewModel : INotifyPropertyChanged
    {
        StemType _type;
        int _streamId;
        bool _isAvailable;

        public StemType StemType
        {
            get => _type;
            set
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StemType)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
            }
        }
        public string DisplayName => StemType.ToString();
        public int StreamId
        {
            get => _streamId;
            set
            {
                _streamId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StreamId)));
            }
        }
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAvailable)));
            }
        }

        public StemViewModel() { }
        public StemViewModel(StemType type, int streamId, bool isAvailable)
        {
            StemType = type;
            StreamId = streamId;
            IsAvailable = isAvailable;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum StemType
    {
        Base, Drums, Instruments, Melody, Unknown
    }
}
