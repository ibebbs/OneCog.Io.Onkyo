using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Responses
{
    public interface IFactory
    {
        string Regex { get; }

        Option<IResponse> Create(Match match);
    }
}
