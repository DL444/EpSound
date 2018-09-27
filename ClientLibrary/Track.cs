using System;
using System.Collections.Generic;
using System.Text;

namespace ClientLibrary
{
    public class Track
    {
        public int StreamId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Genre { get; set; }
        public string Category { get; set; }
        public int Bpm { get; set; }
        public Energy Energy { get; set; }
        public DateTime ReleaseDate { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public class TrackInfo
    {
        public int DbId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public int Length { get; set; }

        public int FullStreamId { get; set; }
        public bool HasBass { get; set; }
        public int BassStreamId { get; set; }
        public bool HasDrums { get; set; }
        public int DrumsStreamId { get; set; }
        public bool HasInstruments { get; set; }
        public int InstrumentsStreamId { get; set; }
        public bool HasMelody { get; set; }
        public int MelodyStreamId { get; set; }
        public bool HasVocals { get; set; }
        public int VocalStreamId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public enum Energy
    {
        Low, Medium, High
    }
}
