using OneCog.Io.Onkyo.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OneCog.Io.Onkyo.Messages.Zone2
{
    public class MuteStateResponse : IResponse
    {
        public MuteStateResponse(MuteState muteState)
        {
            MuteState = muteState;
        }

        public MuteState MuteState { get; private set; }
    }

    public class MuteStateParser : IParser
    {
        private static readonly IReadOnlyDictionary<string, MuteState> MuteStates = new Dictionary<string, MuteState>
        {
            { "00", MuteState.Normal },
            { "01", MuteState.Muted }
        };

        private const string MuteCommandGroup = "ZMT";
        private const string MuteStateGroup = "ZMTSTATE";
        private const string MuteRegex = @"(?<ZMT>ZMT(?<ZMTSTATE>(00|01)))";

        public string Regex
        {
            get { return MuteRegex; }
        }

        public Option<IResponse> Create(Match match)
        {
            Group muteCommandGroup = match.Groups[MuteCommandGroup];
            Group muteStateGroup = match.Groups[MuteStateGroup];

            if (muteCommandGroup != null && muteCommandGroup.Success && muteStateGroup != null && muteStateGroup.Success)
            {
                MuteState muteState;

                if (MuteStates.TryGetValue(muteStateGroup.Value, out muteState))
                {
                    return Option.Some<IResponse>(new MuteStateResponse(muteState));
                }
                else
                {
                    return Option.Some<IResponse>(new MuteStateResponse(MuteState.Unknown));
                }
            }
            else
            {
                return Option.None<IResponse>();
            }
        }
    }
}
