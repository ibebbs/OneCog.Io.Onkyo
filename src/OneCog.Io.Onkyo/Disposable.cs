using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Io.Onkyo
{
    public interface IDisposable<out T> : IDisposable
    {
        T Instance { get; }
    }

    public class Disposable<T> : IDisposable<T>
    {
        private readonly Action _action;

        public Disposable(T instance, Action action)
        {
            _action = action;
            Instance = instance;
        }

        public void Dispose()
        {
            _action();
            Instance = default(T);
        }

        public T Instance { get; private set; }
    }
}
