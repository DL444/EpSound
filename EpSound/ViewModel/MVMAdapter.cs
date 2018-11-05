using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary;

namespace EpSound.ViewModel
{
    static class ModelVmAdapter
    {
        static ESWebClient client = new ESWebClient();

        public static TrackViewModel CreateTrackViewModel(Track track)
        {
            return new TrackViewModel(track);
        }

        public static TrackListViewModel CreateTrackListViewModel(IEnumerable<Track> tracks)
        {
            TrackListViewModel model = new TrackListViewModel();
            foreach(Track t in tracks)
            {
                TrackViewModel trackVm = CreateTrackViewModel(t);
                model.Tracks.Add(trackVm);
            }
            return model;
        }

        public static TrackInfoViewModel CreateTrackInfoViewModel(TrackInfo info)
        {
            return new TrackInfoViewModel(info);
        }

        public static FilterParamViewModel CreateFilterParamViewModel(FilterParameter param)
        {
            return new FilterParamViewModel(param);
        }

        public static FilterParamMgrViewModel CreateFilterParamMgrViewModel(IEnumerable<FilterParameter> parameters)
        {
            List<FilterParamViewModel> filters = new List<FilterParamViewModel>();
            foreach(FilterParameter p in parameters)
            {
                filters.Add(CreateFilterParamViewModel(p));
            }
            return new FilterParamMgrViewModel(filters);
        }

        public static async Task<TrackListViewModel> SearchAll(string term)
        {
            string str = await client.GetFullSearchListContent(term);
            IEnumerable<Track> tracks = ContentFormatter.GetFullSearchResultTracks(str, out _, out _);
            return CreateTrackListViewModel(tracks);
        }

        public static async Task<TrackListViewModel> SearchAll(FilterParamMgrViewModel filterMgr)
        {
            List<Track> tracks = ContentFormatter.GetTracks(await client.GetTrackListContent(filterMgr.RequestString));
            return CreateTrackListViewModel(tracks);
        }

        public static async Task<TrackInfo> GetTrackInfo(Track track)
        {
            string str = await client.GetTrackInfoContent(track.StreamId);
            return ContentFormatter.GetTrackInfo(str);
        }

        public static async Task<TrackListViewModel> GetSimilarTracks(Track track)
        {
            string str = await client.GetSimilarListContent(track.StreamId);
            return CreateTrackListViewModel(ContentFormatter.GetTracks(str));
        }

        public static async Task DownloadToStream(TrackInfo info, System.IO.Stream fileStream)
        {
            System.IO.Stream stream = await client.GetTrackStream(info);
            await stream.CopyToAsync(fileStream);
        }

        public static async Task<FilterParamMgrViewModel> GetFilterParameters()
        {
            string str = await client.GetFilterListContent();
            IEnumerable<FilterParameter> paramList = ContentFormatter.GetFilterParameters(str);
            return CreateFilterParamMgrViewModel(paramList);
        }

        public static async Task<TrackListViewModel> Test()
        {
            // TODO: Remove test code after implementation. 
            FilterParameter hopefulParam = new MoodParameter() { Tag = "Moods.Hopeful", DisplayName = "Hopeful" };
            FilterParameter highEParam = new EnergyParameter() { Tag = "Low", DisplayName = "Medium" };

            List<Track> tracks = ContentFormatter.GetTracks(await client.GetTrackListContent(new List<FilterParameter>() { hopefulParam, /*highEParam*/ }));
            return CreateTrackListViewModel(tracks);
        }
    }
}
