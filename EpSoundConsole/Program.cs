using System;
using System.Collections.Generic;
using System.IO;
using ClientLibrary;

namespace EpSoundConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            ESWebClient client = new ESWebClient();

            while (true)
            {
                Console.Clear();
                Console.Write("Search Term: ");
                string term = Console.ReadLine();

                Console.WriteLine("Searching...");
                string searchStr = client.GetSearchListContent(term).GetAwaiter().GetResult();
                List<Track> tracks = ContentFormatter.GetTracks(searchStr);
                if (tracks.Count == 0)
                {
                    Console.WriteLine("No result found.");
                    Console.ReadLine();
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("Search Result: ");
                Console.WriteLine();
                for (int i = 0; i < tracks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}\t{tracks[i]}");
                    Console.WriteLine($"\tArtists: {tracks[i].Authors}\tGenres: {tracks[i].Genres}\tMoods: {tracks[i].Category}");
                }
                Console.WriteLine();

                int selection = 0;
                if(tracks.Count == 1)
                {
                    selection = 1;
                }
                else
                {
                    while (true)
                    {
                        Console.Write($"Enter result ID (1 - {tracks.Count}): ");
                        string input = Console.ReadLine();
                        if (int.TryParse(input, out selection) && selection < tracks.Count + 1 && selection > 0)
                        {
                            break;
                        }
                        Console.WriteLine("Selection invalid.");
                    }
                }

                Console.WriteLine("Preparing download...");
                selection--;
                Track selected = tracks[selection];
                string trackInfoStr = client.GetTrackInfoContent(selected.StreamId).GetAwaiter().GetResult();
                TrackInfo info = ContentFormatter.GetTrackInfo(trackInfoStr);

                Console.WriteLine();
                Console.WriteLine("Downloading...");
                Console.WriteLine(info);
                Console.WriteLine($"Artist: {selected.Authors}");
                TimeSpan duration = new TimeSpan(0, 0, info.Length);
                Console.WriteLine($"Duration: {duration.Minutes:0}:{duration.Seconds:00}");
                Console.WriteLine();
                Console.WriteLine($"Genres: {selected.Genres}");
                Console.WriteLine($"Moods: {selected.Category}");
                Console.WriteLine($"Bpm: {selected.Bpm}");
                Console.WriteLine($"Energy Level: {selected.Energy}");

                string workDir = System.Environment.CurrentDirectory;
                string filePath = workDir + $"\\{info.UriStreamId}.mp3";

                using (Stream trackStream = client.GetTrackStream(info).GetAwaiter().GetResult())
                {
                    using (FileStream file = File.Create(filePath))
                    {
                        trackStream.CopyTo(file);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Download completed.");
                Console.WriteLine($"Path: {filePath}");
                Console.ReadLine();
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            System.Environment.Exit(-1);
        }
    }
}
