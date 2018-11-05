using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace ClientLibrary
{
    public class ESWebClient
    {
        readonly HttpClient httpClient;

        public ESWebClient(string baseUri = "https://player.epidemicsound.com")
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUri);
        }

        public async Task<string> GetTrackListContent(IEnumerable<FilterParameter> filterParameters)
        {
            return await GetTrackListContent(new FilterParameterManager(filterParameters).GetRequestString());
        }

        public async Task<string> GetTrackListContent(string requestString)
        {
            string paramStr = "/browse_data/?" + requestString;
            HttpResponseMessage message = await httpClient.GetAsync(paramStr);
            return await message.Content.ReadAsStringAsync();
        }

        public async Task<string> GetFilterListContent()
        {
            HttpResponseMessage message = await httpClient.GetAsync("/browse_data/?");
            return await message.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTrackInfoContent(int streamId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/track_url/{streamId}");
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            HttpResponseMessage message = await httpClient.SendAsync(request);
            return await message.Content.ReadAsStringAsync();
        }

        public async Task<Stream> GetTrackStream(TrackInfo info)
        {
            if(info.FileUri == null)
            {
                throw new TrackNotFoundException($"{info.Title} not found.");
            }
            HttpResponseMessage message = await httpClient.GetAsync(new Uri(info.FileUri, UriKind.Absolute));
            return await message.Content.ReadAsStreamAsync();
        }

        public async Task<string> GetSimilarListContent(int streamId)
        {
            HttpResponseMessage message = await httpClient.GetAsync($"/template/similar/{streamId}");
            return await message.Content.ReadAsStringAsync();
        }

        public async Task<string> GetSearchListContent(string term)
        {
            HttpResponseMessage message = await httpClient.GetAsync($"/template/search/?search_query={HttpUtility.UrlEncode(term)}");
            return await message.Content.ReadAsStringAsync();
        }

        public async Task<string> GetFullSearchListContent(string term, int page = 1)
        {
            HttpResponseMessage message = await httpClient.GetAsync($"/json/search/tracks/?order=asc&page={page}&sort=relevance&term={HttpUtility.UrlEncode(term)}");
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
                            track.Genres = GetUnbracketedString(cellMatches[i].Value);
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
            tracks.Sort();
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

        public static TrackInfo GetTrackInfo(string trackInfo)
        {
            TrackInfo info = new TrackInfo();
            using (System.IO.StringReader strReader = new System.IO.StringReader(trackInfo))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(strReader))
                {
                    while(jsonReader.Read())
                    {
                        if(jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value is string str)
                        {
                            switch(str)
                            {
                                case "dbPrimaryKey":
                                    info.DbId = jsonReader.ReadAsInt32() ?? -1;
                                    break;
                                case "title":
                                    info.Title = jsonReader.ReadAsString();
                                    break;
                                case "length":
                                    info.Length = jsonReader.ReadAsInt32() ?? -1;
                                    break;
                                case "fullmixTrackId":
                                    info.FullStreamId = jsonReader.ReadAsInt32() ?? -1;
                                    break;
                                case "bassStreamingTrackId":
                                    info.BassStreamId = jsonReader.ReadAsInt32() ?? -1;
                                    info.HasBass = true;
                                    break;
                                case "drumsStreamingTrackId":
                                    info.DrumsStreamId = jsonReader.ReadAsInt32() ?? -1;
                                    info.HasDrums = true;
                                    break;
                                case "instrumentsStreamingTrackId":
                                    info.InstrumentsStreamId = jsonReader.ReadAsInt32() ?? -1;
                                    info.HasInstruments = true;
                                    break;
                                case "melodyStreamingTrackId":
                                    info.MelodyStreamId = jsonReader.ReadAsInt32() ?? -1;
                                    info.HasMelody = true;
                                    break;
                                case "hasVocals":
                                    info.HasVocals = jsonReader.ReadAsBoolean() ?? false;
                                    break;
                                case "s3TrackId":
                                    info.UriStreamId = jsonReader.ReadAsInt32() ?? -1;
                                    break;
                                case "track_url":
                                    info.FileUri = jsonReader.ReadAsString();
                                    break;
                                case "albums":
                                    jsonReader.Skip();
                                    break;
                            }
                        }
                    }
                }
            }

            return info;
        }

        public static List<Track> GetFullSearchResultTracks(string searchList, out int resultCount, out int pageCount)
        {
            List<Track> tracks = new List<Track>();
            resultCount = -1;
            pageCount = -1;

            using (StringReader strReader = new StringReader(searchList))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(strReader))
                {
                    jsonReader.Read();
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            Track track = new Track();
                            string mainArtist = "";
                            string featArtist = "";
                            while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
                            {
                                if (jsonReader.TokenType == JsonToken.PropertyName)
                                {
                                    switch ((string)jsonReader.Value)
                                    {
                                        case "id":
                                            track.StreamId = jsonReader.ReadAsInt32() ?? -1;
                                            break;
                                        case "title":
                                            track.Title = jsonReader.ReadAsString();
                                            break;
                                        case "genres":
                                            jsonReader.Read();
                                            if (jsonReader.TokenType == JsonToken.String)
                                            {
                                                track.Genres = (string)jsonReader.Value;
                                                break;
                                            }
                                            else if (jsonReader.TokenType == JsonToken.StartArray)
                                            {
                                                track.Genres = jsonReader.ReadAsString();
                                                while (jsonReader.TokenType != JsonToken.EndArray)
                                                {
                                                    jsonReader.Read();
                                                }
                                                break;
                                            }
                                            break;
                                        case "moods":
                                            jsonReader.Read();
                                            if (jsonReader.TokenType == JsonToken.String)
                                            {
                                                track.Category = (string)jsonReader.Value;
                                                break;
                                            }
                                            else if (jsonReader.TokenType == JsonToken.StartArray)
                                            {
                                                track.Category = jsonReader.ReadAsString();
                                                while (jsonReader.TokenType != JsonToken.EndArray)
                                                {
                                                    jsonReader.Read();
                                                }
                                                break;
                                            }
                                            break;
                                        case "bpm":
                                            track.Bpm = jsonReader.ReadAsInt32() ?? -1;
                                            break;
                                        case "energyLevel":
                                            string energyStr = jsonReader.ReadAsString();
                                            if (energyStr == null) { track.Energy = Energy.Medium; }
                                            else if (string.Compare(energyStr, "medium", true) == 0)
                                            {
                                                track.Energy = Energy.Medium;
                                            }
                                            else if (string.Compare(energyStr, "high", true) == 0)
                                            {
                                                track.Energy = Energy.High;
                                            }
                                            else
                                            {
                                                track.Energy = Energy.Low;
                                            }
                                            break;
                                        case "added":
                                            track.ReleaseDate = DateTime.Parse(jsonReader.ReadAsString());
                                            break;
                                        case "artistName":
                                            mainArtist = jsonReader.ReadAsString();
                                            break;
                                        case "featuredArtistName":
                                            featArtist = jsonReader.ReadAsString();
                                            break;
                                        case "composerName":
                                            if(string.IsNullOrEmpty(mainArtist))
                                            {
                                                mainArtist = jsonReader.ReadAsString();
                                            }
                                            break;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(featArtist))
                            {
                                mainArtist += $" feat. {featArtist}";
                            }
                            track.Authors = mainArtist;
                            tracks.Add(track);
                        }
                        else if (jsonReader.TokenType == JsonToken.PropertyName)
                        {
                            string propName = (string)jsonReader.Value;
                            if (propName == "totalHits")
                            {
                                resultCount = jsonReader.ReadAsInt32() ?? -1;
                            }
                            else if(propName == "totalPages")
                            {
                                pageCount = jsonReader.ReadAsInt32() ?? -1;
                            }
                        }
                    }
                }
            }
            tracks.Sort();
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
                if (bracketCount == 0)
                {
                    if(trim)
                    {
                        if (char.IsWhiteSpace(current) && current != ' ') { continue; }
                        if (builder.Length > 0 && current == ' ' && builder[builder.Length - 1] == ' ')
                        {
                            continue;
                        }
                    }
                    builder.Append(current);
                }
            }
            if(trim)
            {
                return builder.ToString().Trim();
            }
            return builder.ToString();
        }
    }

    public class TrackNotFoundException : Exception
    {
        public TrackNotFoundException() : base() { }

        protected TrackNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public TrackNotFoundException(string message) : base(message) { }

        public TrackNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
