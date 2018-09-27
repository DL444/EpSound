using ClientLibrary;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClientTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ESWebClient();

            ////string cont = client.GetFilterListContent().GetAwaiter().GetResult();
            ////List<FilterParameter> filters = ClientLibrary.ContentFormatter.GetFilterParameters(cont);
            ////FilterParameter hopefulParam = filters.Find(x => x.Tag == "Moods.Hopeful");
            ////FilterParameter highEParam = filters.Find(x => x.Tag == "Medium" && x.TagType == "energyLevel");

            //FilterParameter hopefulParam = new MoodParameter() { Tag = "Moods.Hopeful", DisplayName = "Hopeful" };
            //FilterParameter highEParam = new EnergyParameter() { Tag = "Medium", DisplayName = "Medium" };

            //List<Track> tracks = ClientLibrary.ContentFormatter.GetTracks(client.GetTrackListContent(new List<FilterParameter>() { hopefulParam, highEParam }).GetAwaiter().GetResult());
            //foreach(var t in tracks)
            //{
            //    Console.WriteLine(t);
            //}

            //string infoJson = client.GetTrackInfoContent(161846).GetAwaiter().GetResult();
            //var info = ContentFormatter.GetTrackInfo(infoJson);
            //using (Stream mp3stream = client.GetTrackStream(info).GetAwaiter().GetResult())
            //{
            //    using (FileStream fileStr = File.Create("C:\\Test\\test.mp3"))
            //    {
            //        mp3stream.CopyTo(fileStr);
            //    }
            //}

            string cont = client.GetSimilarListContent(161846).GetAwaiter().GetResult();
            List<Track> tracks = ClientLibrary.ContentFormatter.GetTracks(cont);
            foreach (var t in tracks)
            {
                Console.WriteLine(t);
            }
        }
    }
}
