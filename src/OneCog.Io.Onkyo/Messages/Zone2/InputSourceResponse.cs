using OneCog.Io.Onkyo.Common;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OneCog.Io.Onkyo.Messages.Zone2
{
    public class InputSourceResponse : IResponse
    {
        public InputSourceResponse(InputSource inputSource)
        {
            InputSource = inputSource;
        }

        public InputSource InputSource { get; private set; }
    }

    public class InputSourceResponseParser : IParser
    {
        private const string InputSourceRegex = @"(?<SLI>SLZ(?<SLIVALUE>[0-9,A-F,a-f]{2}))";
        private const string InputSourceGroup = "SLI";
        private const string InputSourceValueGroup = "SLIVALUE";

        public string Regex
        {
            get { return InputSourceRegex; }
        }

        public Option<IResponse> Create(Match match)
        {
            Group inputSourceGroup = match.Groups[InputSourceGroup];
            Group inputSourceValueGroup = match.Groups[InputSourceValueGroup];

            if (inputSourceGroup != null && inputSourceGroup.Success && inputSourceValueGroup != null && inputSourceValueGroup.Success)
            {
                uint id;
                InputSource inputSource;

                if (uint.TryParse(inputSourceValueGroup.Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out id)
                 && InputSources.TryGetFromId(id, out inputSource))
                {
                    return Option.Some<IResponse>(new InputSourceResponse(inputSource));
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unable to parse input source from response: {0}", inputSourceValueGroup.Value));
                }
            }
            else
            {
                return Option.None<IResponse>();
            }
        }
    }
}
