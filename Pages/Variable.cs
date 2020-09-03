using System;
using Expressive;

public class Variable
{
    private readonly Action onValueChanged;
    private string name;
    private VariableType type;
    private string value;

    public string Name
    {
        get => this.name;
        set
        {
            if (this.name != value)
            {
                this.name = value;

                this.NotifyValueChanged();
            }
        }
    }

    public VariableType Type 
    {
        get => this.type;
        set
        {
            if (this.type != value)
            {
                this.type = value;

                this.NotifyValueChanged();
            }
        }
    }

    public string Value
    {
        get { return this.value; }
        set
        {
            if (this.value != value)
            {
                this.value = value;

                // Let's see if we can auto detect the type.
                this.GuessVariableType();

                this.NotifyValueChanged();
            }
        }
    }

    internal object TypedValue
    {
        get
        {
            switch (this.Type)
            {
                case VariableType.Int:
                    return string.IsNullOrWhiteSpace(this.value) ? (int?) null : Convert.ToInt32(this.value);
                case VariableType.Decimal:
                    return string.IsNullOrWhiteSpace(this.value) ? (decimal?) null : Convert.ToDecimal(this.value);
                case VariableType.Double:
                    return string.IsNullOrWhiteSpace(this.value) ? (double?) null : Convert.ToDouble(this.value);
                case VariableType.Long:
                    return string.IsNullOrWhiteSpace(this.value) ? (long?) null : Convert.ToInt64(this.value);
                case VariableType.String:
                    return this.value;
                case VariableType.Date:
                    return Convert.ToDateTime(this.value);
                case VariableType.Boolean:
                    return Convert.ToBoolean(this.value);
                case VariableType.Null:
                    return null;
                case VariableType.Expression:
                    return new Expression(this.value as string);
                case VariableType.None:
                default:
                    return null;
            }
        }
    }

    public Variable(string name, Action onValueChanged) : this(onValueChanged)
    {
        this.name = name;
    }

    public Variable(Action onValueChanged)
    {
        this.onValueChanged = onValueChanged;
    }

    private void GuessVariableType()
    {
        if (this.Type == VariableType.None)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (int.TryParse(value, out _))
                {
                    this.Type = VariableType.Int;
                }
                else if (long.TryParse(value, out _))
                {
                    this.Type = VariableType.Long;
                }
                else if (double.TryParse(value, out _))
                {
                    this.Type = VariableType.Double;
                }
                else if (decimal.TryParse(value, out _))
                {
                    this.Type = VariableType.Decimal;
                }
                else if (bool.TryParse(value, out _))
                {
                    this.Type = VariableType.Boolean;
                }
                else if (DateTime.TryParse(value, out _))
                {
                    this.Type = VariableType.Date;
                }
                else
                {
                    this.Type = VariableType.String;
                }
            }
        }
    }

    private void NotifyValueChanged()
    {
        this.onValueChanged?.Invoke();
    }
}