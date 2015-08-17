using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Responses.Zone2
{
    public class PowerStateResponse : IResponse
    {
        public PowerStateResponse(PowerState powerState)
        {
            PowerState = powerState;
        }

        public PowerState PowerState { get; private set; }
    }

    public class PowerStateParser : IParser
    {
        private static readonly IReadOnlyDictionary<string, PowerState> PowerStates = new Dictionary<string, PowerState>
        {
            { "00", PowerState.Off },
            { "01", PowerState.On }
        };

        private const string PowerCommandGroup = "PWR";
        private const string PowerStateGroup = "PWRSTATE";
        private const string PowerRegex = @"(?<PWR>ZPW(?<PWRSTATE>(00|01)))";

        public string Regex
        {
            get { return PowerRegex; }
        }

        public Option<IResponse> Create(Match match)
        {
            Group powerCommandGroup = match.Groups[PowerCommandGroup];
            Group powerStateGroup = match.Groups[PowerStateGroup];

            if (powerCommandGroup != null && powerCommandGroup.Success && powerStateGroup != null && powerStateGroup.Success)
            {
                PowerState powerState;

                if (PowerStates.TryGetValue(powerStateGroup.Value, out powerState))
                {
                    return Option.Some<IResponse>(new PowerStateResponse(powerState));
                }
                else
                {
                    return Option.Some<IResponse>(new PowerStateResponse(PowerState.Unknown));
                }
            }
            else
            {
                return Option.None<IResponse>();
            }
        }
    }
}
