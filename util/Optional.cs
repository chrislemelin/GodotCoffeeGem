using System.Windows.Markup;

public struct Optional<T>
{
    private readonly bool _hasValue;
    private readonly T _value;

    public bool HasValue => _hasValue;

    internal Optional(T value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }


    public bool TryGetValue(out T value)
    {
        value = _hasValue ? _value : default;
        return _hasValue;
    }
    public T GetValue()
    {
        return _hasValue ? _value : default;
    }
}

public static class Optional
{
    public static Optional<T> Some<T>(T value) => new Optional<T>(value, true);

    public static Optional<T> None<T>() => new Optional<T>(default(T), false);

    public static Optional<T> FromNullable<T>(T value) where T : class
        => value is null
            ? None<T>()
            : Some(value);

    public static Optional<T> FromNullable<T>(T? value) where T : struct
        => value.HasValue
            ? Some(value.Value)
            : None<T>();
}
