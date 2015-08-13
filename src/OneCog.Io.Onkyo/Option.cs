
namespace OneCog.Io.Onkyo
{
    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value);
        }

        public static Option<T> None<T>()
        {
            return new Option<T>();
        }
    }

    public class Option<T>
    {
        public Option(T value)
        {
            IsSome = true;
            IsNone = false;
            Value = value;
        }

        public Option()
        {
            IsSome = false;
            IsNone = true;
            Value = default(T);
        }

        public bool IsSome { get; private set; }
        public bool IsNone { get; private set; }
        public T Value { get; private set; }
    }
}
