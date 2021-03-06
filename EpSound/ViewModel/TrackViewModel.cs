﻿using ClientLibrary;
using System;
using System.ComponentModel;

namespace EpSound.ViewModel
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        Track _track;
        bool _isHovered;

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

        public bool IsHovered
        {
            get => _isHovered;
            set
            {
                _isHovered = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHovered)));
            }
        }

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
