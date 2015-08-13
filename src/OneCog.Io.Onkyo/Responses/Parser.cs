using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Responses
{
    public interface IParser
    {
        IEnumerable<IResponse> Parse(string response);
    }

    public class Parser
    {
        private const string MasterVolumeRegex = @"(?<MVL>MVL(?<MVLVALUE>([0..9,A..F]){2}))";

        private readonly IEnumerable<IFactory> _factories;
        private readonly Regex _regex;

        public Parser(IEnumerable<IFactory> factories)
        {
            _factories = (factories ?? Enumerable.Empty<IFactory>()).ToArray();

            string regexPattern = string.Format(@"!({0})", string.Join("|", _factories.Select(factory => factory.Regex)).ToArray());

            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        }

        public IEnumerable<IResponse> Parse(string response)
        {
            Match match = _regex.Match(response);

            return _factories
                .Select(factory => factory.Create(match))
                .Where(option => option.IsSome)
                .Select(option => option.Value)
                .ToArray();
        }
    }
}
