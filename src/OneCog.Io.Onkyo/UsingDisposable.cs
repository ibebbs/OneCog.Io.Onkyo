using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;

namespace OneCog.Io.Onkyo
{
    /// <summary>
    /// Represents a disposable resource that only disposes its underlying disposable resource when all <see cref="GetDisposable">dependent disposable objects</see> have been disposed.
    /// </summary>
    public sealed class SharedDisposable : ICancelable
    {
        private readonly object _gate = new object();
        private Func<IDisposable> _disposableFactory;
        private IDisposable _disposable;
        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Reactive.Disposables.RefCountDisposable"/> class with the specified disposable.
        /// </summary>
        /// <param name="disposable">Underlying disposable.</param>
        /// <exception cref="ArgumentNullException"><paramref name="disposable"/> is null.</exception>
        public SharedDisposable(Func<IDisposable> disposableFactory)
        {
            if (disposableFactory == null)
                throw new ArgumentNullException("disposableFactory");

            _disposableFactory = disposableFactory;
            _count = 0;
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _disposable == null; }
        }

        /// <summary>
        /// Returns a dependent disposable that when disposed decreases the refcount on the underlying disposable.
        /// </summary>
        /// <returns>A dependent disposable contributing to the reference count that manages the underlying disposable's lifetime.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Backward compat + non-trivial work for a property getter.")]
        public IDisposable GetDisposable()
        {
            lock (_gate)
            {
                if (_disposableFactory != null)
                {
                    if (_disposable == null)
                    {
                        _disposable = _disposableFactory();
                    }

                    _count++;
                    return new InnerDisposable(this);
                }
                else
                {
                    return Disposable.Empty;
                }
            }
        }

        /// <summary>
        /// Disposes the underlying disposable only when all dependent disposables have been disposed.
        /// </summary>
        public void Dispose()
        {
            if (_disposable != null)
            {
                _disposable.Dispose();
                _disposable = null;
            }

            _count = 0;
            _disposableFactory = null;
        }

        private void Release()
        {
            var disposable = default(IDisposable);

            lock (_gate)
            {
                if (_disposable != null)
                {
                    _count--;

                    if (_count == 0)
                    {
                        disposable = _disposable;
                        _disposable = null;
                    }
                }
            }

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        sealed class InnerDisposable : IDisposable
        {
            private SharedDisposable _parent;

            public InnerDisposable(SharedDisposable parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                var parent = Interlocked.Exchange(ref _parent, null);
                if (parent != null)
                    parent.Release();
            }
        }
    }
}
