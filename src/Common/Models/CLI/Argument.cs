using System.Diagnostics.CodeAnalysis;

namespace ascii_cli.Common.Models.CLI;

public class Argument
{
    public required string Name { get; init; }
    public required ArgumentValueType Type { get; init; }

    public string? Value
    {
        get => _value;
        set
        {
            if (value != null && !IsValidValue(value))
            {
                throw new InvalidArgumentValueTypeException($"Invalid value for type {Type}");
            }
            _value = value;
        }
    }

    public Argument() {}

    [SetsRequiredMembers]
    public Argument(string name, ArgumentValueType type)
    {
        Name = name;
        Type = type;
    }

    [SetsRequiredMembers]
    public Argument(string name, ArgumentValueType type, string? value = null)
    {
        Name = name;
        Type = type;
        _value = value;
    }

    private string? _value;

    private bool IsValidValue(string value)
    {
        return Type switch
        {
            ArgumentValueType.String => true,
            ArgumentValueType.Int => int.TryParse(value, out _),
            ArgumentValueType.Float => float.TryParse(value, out _),
            ArgumentValueType.Double => double.TryParse(value, out _),
            ArgumentValueType.Bool => bool.TryParse(value, out _),
            _ => false
        };
    }
}
