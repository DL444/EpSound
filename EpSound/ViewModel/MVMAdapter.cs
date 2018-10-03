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

        public static async Task<TrackListViewModel> SearchAll(string term)
        {
            string str = await client.GetFullSearchListContent(term);
            List<Track> tracks = ContentFormatter.GetFullSearchResultTracks(str, out _, out _);
            return CreateTrackListViewModel(tracks);
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
