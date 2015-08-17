using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo.Messages
{
    public interface ICommand<T>
    {
        Task<Fallible<T>> Send(ICommandStream stream);
    }
}
