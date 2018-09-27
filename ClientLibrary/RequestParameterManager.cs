using System;
using System.Collections.Generic;
using System.Text;

namespace ClientLibrary
{
    public class FilterParameterManager
    {
        List<FilterParameter> parameters = new List<FilterParameter>();

        public FilterParameterManager() { }

        public FilterParameterManager(IEnumerable<FilterParameter> parameters)
        {
            this.parameters = new List<FilterParameter>(parameters);
        }

        public FilterParameter this[int index] => parameters[index];
        public void Add(FilterParameter parameter)
        {
            if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); }
            parameters.Add(parameter);
        }
        public void Remove(FilterParameter parameter)
        {
            if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); }
            parameters.Remove(parameter);
        }
        public void Clear()
        {
            parameters.Clear();
        }

        public string GetRequestString()
        {
            if (parameters.Count > 0)
            {
                StringBuilder builder = new StringBuilder("?");
                foreach (FilterParameter p in parameters)
                {
                    builder.Append(p.GetRequestString());
                }
                return builder.ToString();
            }
            else
            {
                return "";
            }
        }
    }

    public abstract class FilterParameter
    {
        public string DisplayName { get; set; }
        public virtual string TagType { get; }
        public string Tag { get; set; }

        public virtual string GetRequestString()
        {
            return $"&{TagType}={Tag}";
        }

        public override string ToString()
        {
            return $"{TagType}: {DisplayName}";
        }
    }

    public class MoodParameterBase : FilterParameter
    {
        public override string TagType => "moods";
    }

    public sealed class MoodParameter : MoodParameterBase { }
    public sealed class MovementParameter : MoodParameterBase { }
    public sealed class SettingParameter : MoodParameterBase { }

    public sealed class GenreParameter : FilterParameter
    {
        public override string TagType => "fatherGenres";
    }

    public sealed class SubgenreParameter : FilterParameter
    {
        public GenreParameter ParentGenre { get; set; }
        public override string TagType => "genres";
    }

    public sealed class EnergyParameter : FilterParameter
    {
        public override string TagType => "energyLevel";
    }

    public sealed class TempoParameter : FilterParameter
    {
        public override string TagType => "tempo";
    }

    public sealed class LengthParameter : FilterParameter
    {
        public override string TagType => "trackLength";
    }
}
