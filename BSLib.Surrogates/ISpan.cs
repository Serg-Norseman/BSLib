namespace BSLib.Surrogates
{
    public interface ISpan<T>
    {
        T this[int index] { get; }
        int Length { get; }
        bool IsEmpty { get; }

        ISpan<T> Slice(int start);
        ISpan<T> Slice(int start, int length);

        T[] ToArray();
        string ToString();
    }
}
