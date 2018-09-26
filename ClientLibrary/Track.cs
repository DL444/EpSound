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

    public enum Energy
    {
        Low, Medium, High
    }
}
