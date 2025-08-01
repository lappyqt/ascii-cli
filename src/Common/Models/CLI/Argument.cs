namespace ascii_cli.Common.Models.CLI;

public class Argument
{
    private string? _value;

    public required string Name { get; init; }
    public required ArgumentValueType Type { get; init; }

    public string? Value
    {
        get => _value;
        set
        {
            if (value != null && !IsValidValue(value)) throw new InvalidArgumentValueTypeException($"Invalid value for type {Type}");
            _value = value;
        }
    }

    private bool IsValidValue(string value)
    {
        return Type switch
        {
            ArgumentValueType.String => true,
            ArgumentValueType.Int => int.TryParse(value, out _),
            ArgumentValueType.Bool => bool.TryParse(value, out _),
            _ => false
        };
    }
}
