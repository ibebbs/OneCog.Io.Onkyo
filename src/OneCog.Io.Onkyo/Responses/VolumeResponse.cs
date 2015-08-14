using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OneCog.Io.Onkyo.Responses
{
    public class VolumeResponse : IResponse
    {
        public VolumeResponse(byte volume)
        {
            Volume = volume;
        }

        public byte Volume { get; private set; }
    }

    public class VolumeResponseParser : IParser
    {
        private const string MasterVolumeRegex = @"(?<MVL>MVL(?<MVLVALUE>([0..9,A..F]){2}))";
        private const string MasterVolumeGroup = "MVL";
        private const string MasterVolumeValueGroup = "MVLVALUE";

        public string Regex
        {
            get { return MasterVolumeRegex; }
        }

        public Option<IResponse> Create(Match match)
        {
            Group masterVolumeGroup = match.Groups[MasterVolumeGroup];
            Group masterVolumeValueGroup = match.Groups[MasterVolumeValueGroup];

            if (masterVolumeGroup != null && masterVolumeGroup.Success && masterVolumeValueGroup != null && masterVolumeValueGroup.Success)
            {
                byte volume;

                if (byte.TryParse(masterVolumeValueGroup.Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out volume))
                {
                    return Option.Some<IResponse>(new VolumeResponse(volume));
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unable to parse master volume level from response: {0}", masterVolumeValueGroup.Value));
                }
            }
            else
            {
                return Option.None<IResponse>();
            }
        }
    }

}
