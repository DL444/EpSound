using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ClientLibrary
{
    public class ESWebClient
    {
        HttpClient httpClient;

        public ESWebClient(string baseUri = "https://player.epidemicsound.com")
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUri);
        }

        public async Task<string> GetTrackListContent(string parameters = "/browse_data/?&moods=Moods.Hopeful&active=moods&activeFilter=energy")
        {

            HttpResponseMessage message = await httpClient.GetAsync(HttpUtility.UrlEncode(parameters));
            return await message.Content.ReadAsStringAsync();
        }
    }


    public static class ContentFormatter
    {
        public static List<Track> GetTracks(string tracklist)
        {
            List<Track> tracks = new List<Track>();
            Regex rowRegex = new Regex(@"<tr class=""resultRow"" streamingId=""(.*?)"">(.*?)</tr>", RegexOptions.Singleline);
            MatchCollection matches = rowRegex.Matches(tracklist);
            foreach(Match match in matches)
            {
                Track track = new Track();
                track.StreamId = int.Parse(match.Groups[1].Value);
                Regex cellRegex = new Regex(@"<td .*?>(.*?)</td>", RegexOptions.Singleline);
                MatchCollection cellMatches = cellRegex.Matches(match.Groups[2].Value);
                for(int i = 0; i < cellMatches.Count; i++)
                {
                    switch(i)
                    {
                        case 0:
                            Regex titleRegex = new Regex(@"<a .*? title=.*?>(.*?)</a>", RegexOptions.Singleline);
                            track.Title = titleRegex.Match(cellMatches[i].Value).Groups[1].Value;

                            Regex authorsRegex = new Regex(@"<div class=""track-tags"">(.*?)</div>", RegexOptions.Singleline);
                            string authorCellValue = authorsRegex.Match(cellMatches[i].Value).Groups[1].Value.Trim();
                            track.Authors = GetUnbracketedString(authorCellValue);
                            break;
                        case 1:
                            track.Genre = GetUnbracketedString(cellMatches[i].Value);
                            break;
                        case 2:
                            track.Category = GetUnbracketedString(cellMatches[i].Value);
                            break;
                        case 3:
                            track.Bpm = int.Parse(GetUnbracketedString(cellMatches[i].Value));
                            break;
                        case 4:
                            string energyStr = GetUnbracketedString(cellMatches[i].Value);
                            if(string.Equals(energyStr, "medium", StringComparison.OrdinalIgnoreCase))
                            {
                                track.Energy = Energy.Medium;
                            }
                            else if (string.Equals(energyStr, "high", StringComparison.OrdinalIgnoreCase))
                            {
                                track.Energy = Energy.High;
                            }
                            else
                            {
                                track.Energy = Energy.Low;
                            }
                            break;
                        case 5:
                            track.ReleaseDate = DateTime.Parse(GetUnbracketedString(cellMatches[i].Value));
                            break;
                    }
                }
                tracks.Add(track);
            }
            return tracks;
        }

        static string GetUnbracketedString(string original, bool trim = true)
        {
            int bracketCount = 0;
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < original.Length; j++)
            {
                char current = original[j];
                if (current == '<') { bracketCount++; continue; }
                if (current == '>') { bracketCount--; continue; }
                if (bracketCount == 0) { builder.Append(current); }
            }
            if(trim)
            {
                return builder.ToString().Trim();
            }
            return builder.ToString();
        }
    }
}
