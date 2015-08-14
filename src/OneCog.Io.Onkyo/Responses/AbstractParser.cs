using OneCog.Io.Onkyo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Responses
{
    public interface IAbstractParser
    {
        IEnumerable<IResponse> Parse(string response);
    }

    public class AbstractParser : IAbstractParser
    {
        private readonly IEnumerable<IParser> _factories;
        private readonly Regex _regex;

        public AbstractParser(IEnumerable<IParser> factories)
        {
            _factories = (factories ?? Enumerable.Empty<IParser>()).ToArray();

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
