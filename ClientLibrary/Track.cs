using System;

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
        public int VocalsStreamId { get; set; }

        public string FileUri { get; set; }
        public int UriStreamId { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public TrackInfoType TrackInfoType
        {
            get
            {
                if(UriStreamId == -1) { return TrackInfoType.Unknown; }
                if(UriStreamId == FullStreamId) { return TrackInfoType.FullMix; }
                if(UriStreamId == BassStreamId) { return TrackInfoType.Bass; }
                if(UriStreamId == DrumsStreamId) { return TrackInfoType.Drums; }
                if(UriStreamId == InstrumentsStreamId) { return TrackInfoType.Instruments; }
                if(UriStreamId == MelodyStreamId) { return TrackInfoType.Melody; }
                if(UriStreamId == VocalsStreamId) { return TrackInfoType.Vocals; }
                return TrackInfoType.Unknown;
            }
        }
    }

    public enum Energy
    {
        Low, Medium, High
    }

    public enum TrackInfoType
    {
        FullMix, Bass, Drums, Instruments, Melody, Vocals, Unknown
    }
}
