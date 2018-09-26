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

            HttpResponseMessage message = await httpClient.GetAsync(parameters);
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

        public static List<FilterParameter> GetFilterParameters(string filterlist)
        {
            List<FilterParameter> parameters = new List<FilterParameter>();
            string det = @"<div class=""visible-desktop"">";
            int startIndex = filterlist.IndexOf(det);
            int detLength = det.Length;
            string subStr = filterlist.Substring(startIndex + detLength);
            startIndex = subStr.IndexOf(det);
            subStr = subStr.Substring(startIndex);

            int layer = 0;
            for(int i = 0; i < subStr.Length; i++)
            {
                if(subStr[i] == '<')
                {
                    if(subStr.Substring(i, 4) == "<div")
                    {
                        layer++;
                        i += 3;
                    }
                    else if(subStr.Substring(i, 5) == "</div")
                    {
                        layer--;
                        i += 4;
                    }
                }
                if(layer == 0)
                {
                    subStr = subStr.Remove(i + 1);
                    break;
                }
            }

            Regex tagRegex = new Regex(@"<a class="".*?"" tag=""(.*?)"" tagType=""moods"".*?>(.*?)</a>", RegexOptions.Singleline);
            MatchCollection matches = tagRegex.Matches(subStr);
            foreach(Match match in matches)
            {
                string tag = match.Groups[1].Value;
                switch(tag.Split('.')[0])
                {
                    case "Moods":
                        MoodParameter pMood = new MoodParameter();
                        pMood.Tag = tag;
                        pMood.DisplayName = match.Groups[2].Value.Trim();
                        parameters.Add(pMood);
                        break;
                    case "Movement":
                        MovementParameter pMove = new MovementParameter();
                        pMove.Tag = tag;
                        pMove.DisplayName = match.Groups[2].Value;
                        parameters.Add(pMove);
                        break;
                    case "Settings":
                        SettingParameter pSet = new SettingParameter();
                        pSet.Tag = tag;
                        pSet.DisplayName = match.Groups[2].Value;
                        parameters.Add(pSet);
                        break;
                }
            }

            tagRegex = new Regex(@"<a class="".*?"" tag=""(.*?)"" tagType=""(energyLevel|tempo|trackLength)"".*?>(.*?)</a>");
            matches = tagRegex.Matches(subStr);
            foreach(Match match in matches)
            {
                switch(match.Groups[2].Value)
                {
                    case "energyLevel":
                        EnergyParameter pEnergy = new EnergyParameter();
                        pEnergy.Tag = match.Groups[1].Value;
                        pEnergy.DisplayName = match.Groups[3].Value;
                        parameters.Add(pEnergy);
                        break;
                    case "tempo":
                        TempoParameter pTempo = new TempoParameter();
                        pTempo.Tag = match.Groups[1].Value;
                        pTempo.DisplayName = match.Groups[3].Value;
                        parameters.Add(pTempo);
                        break;
                    case "trackLength":
                        LengthParameter pLength = new LengthParameter();
                        pLength.Tag = match.Groups[1].Value;
                        pLength.DisplayName = match.Groups[3].Value;
                        parameters.Add(pLength);
                        break;
                }
            }

            det = @"<ul class=""col col-genre"">";
            subStr = subStr.Substring(subStr.IndexOf(det));

            tagRegex = new Regex(@"<a class=[""'].*?[""'] tag=[""'](.*?)[""'] tagType=[""'](fatherGenres|genres)[""'].*?>(.*?)</a>", RegexOptions.Singleline);
            matches = tagRegex.Matches(subStr);
            GenreParameter prevGenre = null;
            foreach (Match match in matches)
            {
                switch (match.Groups[2].Value)
                {
                    case "fatherGenres":
                        if(match.Groups[3].Value == "All") { continue; }
                        prevGenre = new GenreParameter();
                        prevGenre.Tag = match.Groups[1].Value;
                        prevGenre.DisplayName = GetUnbracketedString(match.Groups[3].Value);
                        parameters.Add(prevGenre);
                        break;
                    case "genres":
                        SubgenreParameter pSubgen = new SubgenreParameter();
                        pSubgen.ParentGenre = prevGenre;
                        pSubgen.Tag = match.Groups[1].Value;
                        pSubgen.DisplayName = match.Groups[3].Value;
                        parameters.Add(pSubgen);
                        break;
                }
            }

            return parameters;
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
