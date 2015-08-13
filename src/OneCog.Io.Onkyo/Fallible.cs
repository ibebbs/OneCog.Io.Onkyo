using System;

namespace OneCog.Io.Onkyo
{
    public static class Fallible
    {
        public static Fallible<T> Success<T>(T value)
        {
            return new Fallible<T>(value);
        }

        public static Fallible<T> Fail<T>(Exception exception)
        {
            return new Fallible<T>(exception);
        }
    }

    public class Fallible<T>
    {
        public Fallible(T value)
        {
            Succeeded = true;
            Failed = false;
            Value = value;
            Exception = null;
        }

        public Fallible(Exception exception)
        {
            Succeeded = false;
            Failed = true;
            Value = default(T);
            Exception = exception;
        }

        public bool Succeeded { get; private set; }
        public bool Failed { get; private set; }
        public T Value { get; private set; }
        public Exception Exception { get; private set; }
    }
}
