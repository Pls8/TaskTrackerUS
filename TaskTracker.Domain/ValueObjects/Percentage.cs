using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Domain.ValueObjects;

public class Percentage
{
    public int Value { get; private set; }

    public Percentage(int value)
    {
        if (value < 0 || value > 100)
        {
            throw new DomainException("Percentage must be between 0 and 100.");
        }
        Value = value;
    }

    public static implicit operator int(Percentage percentage) => percentage.Value;
    public static implicit operator Percentage(int value) => new(value);
}
