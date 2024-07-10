using System.Text;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SeWzc.X11Sharp;
using SeWzc.X11Sharp.Structs;

namespace WindowDebugger.Services.NativeWindows.Linux.Models;

public class WindowProperty(X11DisplayWindow window, X11DisplayAtom property)
{
    public string Name { get; } = property.GetAtomName() ?? "(null)";

    public IReadOnlyWindowPropertyValue Value => GetValue();

    public IReadOnlyWindowPropertyValue GetValue()
    {
        var propertyData = window.GetProperty(property);
        if (propertyData is null)
        {
            throw new Exception("Failed to get property.");
        }

        if (property.GetAtomName() == "_NET_WM_ICON")
        {
            return WindowPropertyIconValue.FromPropertyData(propertyData);
        }

        switch (propertyData.PropertyType.GetAtomName())
        {
            case WindowPropertyValues.String:
            case WindowPropertyValues.Utf8String:
                return WindowPropertyStringValue.FromPropertyData(propertyData);
            case WindowPropertyValues.Cardinal:
            case WindowPropertyValues.Window:
                return WindowPropertyCardinalValue.FromPropertyData(propertyData);
            case WindowPropertyValues.Atom:
                return WindowPropertyAtomValue.FromPropertyData(propertyData);
            default:
                return new WindowPropertyUnKnowValue(propertyData.PropertyType.GetAtomName() ?? "(null)");
        }
    }

    public void SetValue(IWindowPropertyValue value)
    {
        window.ChangeProperty(property, PropertyMode.Replace, value.ToPropertyData());
    }
}

public static class WindowPropertyValues
{
    public const string String = "STRING";
    public const string Utf8String = "UTF8_STRING";
    public const string Cardinal = "CARDINAL";
    public const string Window = "WINDOW";
    public const string Atom = "ATOM";
    public const string MotifWmHints = "_MOTIF_WM_HINTS";
}

public interface IReadOnlyWindowPropertyValue
{
    string ValueType { get; }
}

public interface IWindowPropertyValue : IReadOnlyWindowPropertyValue
{
    PropertyData ToPropertyData();
}

public interface IReadOnlyWindowPropertyValue<out TSelf> : IReadOnlyWindowPropertyValue
    where TSelf : IReadOnlyWindowPropertyValue<TSelf>
{
    static abstract TSelf FromPropertyData(PropertyData propertyData);
}

public interface IWindowPropertyValue<out TSelf> : IReadOnlyWindowPropertyValue<TSelf>, IWindowPropertyValue
    where TSelf : IWindowPropertyValue<TSelf>;

public record WindowPropertyUnKnowValue(string ValueType) : IReadOnlyWindowPropertyValue
{
    public override string ToString()
    {
        return $"暂不支持类型：{ValueType}";
    }
}

public record WindowPropertyIconValue(Bitmap[] Icons) : IReadOnlyWindowPropertyValue<WindowPropertyIconValue>
{
    public string ValueType => WindowPropertyValues.Cardinal;

    public static WindowPropertyIconValue FromPropertyData(PropertyData propertyData)
    {
        if (propertyData is not PropertyData.Format32Array format32Array ||
            propertyData.PropertyType.GetAtomName() != WindowPropertyValues.Cardinal)
        {
            return new WindowPropertyIconValue([]);
        }

        var iconList = new List<Bitmap>();
        var dataArray = format32Array.Value.AsSpan();
        for (var i = 0; i < dataArray.Length;)
        {
            var width = (int)dataArray[i++];
            var height = (int)dataArray[i++];
            var writeableBitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96),
                PixelFormat.Bgra8888, AlphaFormat.Unpremul);
            using (var lockedFramebuffer = writeableBitmap.Lock())
            {
                unsafe
                {
                    var address = (int*)lockedFramebuffer.Address;
                    var size = width * height;
                    for (var j = 0; j < size; j++)
                        address[j] = (int)dataArray[i++];
                }
            }

            iconList.Add(writeableBitmap);
        }

        return new WindowPropertyIconValue(iconList.ToArray());
    }
}

public record WindowPropertyStringValue : IWindowPropertyValue<WindowPropertyStringValue>
{
    private readonly X11DisplayAtom _propertyType;

    private WindowPropertyStringValue(X11DisplayAtom propertyType, string[] values)
    {
        _propertyType = propertyType;
        Values = values;
    }

    public string[] Values { get; init; }

    public static WindowPropertyStringValue FromPropertyData(PropertyData propertyData)
    {
        if (propertyData is PropertyData.Format8Array format8Array)
        {
            switch (propertyData.PropertyType.GetAtomName())
            {
                case WindowPropertyValues.String:
                    return new WindowPropertyStringValue(propertyData.PropertyType,
                        Encoding.Latin1.GetString(format8Array.Value).Split('\0'));
                case WindowPropertyValues.Utf8String:
                    return new WindowPropertyStringValue(propertyData.PropertyType,
                        Encoding.UTF8.GetString(format8Array.Value).Split('\0'));
            }
        }

        return new WindowPropertyStringValue(propertyData.PropertyType, ["(error)"]);
    }

    public PropertyData ToPropertyData()
    {
        var target = Values switch
        {
            [] => "",
            [var str] => str,
            _ => string.Concat(Values.Select(x => x + '\0'))
        };
        var bytes = _propertyType.GetAtomName() switch
        {
            WindowPropertyValues.String => Encoding.Latin1.GetBytes(target),
            WindowPropertyValues.Utf8String => Encoding.UTF8.GetBytes(target),
            _ => throw new Exception("Unknown property type.")
        };
        return new PropertyData.Format8Array(_propertyType, bytes);
    }

    public string ValueType => _propertyType.GetAtomName() ?? "(null)";
}

public record WindowPropertyCardinalValue : IWindowPropertyValue<WindowPropertyCardinalValue>
{
    private readonly X11DisplayAtom _propertyType;

    private WindowPropertyCardinalValue(X11DisplayAtom propertyType, int[] value)
    {
        _propertyType = propertyType;
        Value = value;
    }

    public string ValueType => _propertyType.GetAtomName() ?? "(null)";
    public int[] Value { get; init; }

    public static WindowPropertyCardinalValue FromPropertyData(PropertyData propertyData)
    {
        if (propertyData is PropertyData.Format32Array format32Array)
        {
            switch (propertyData.PropertyType.GetAtomName())
            {
                case WindowPropertyValues.Cardinal:
                case WindowPropertyValues.Window:
                    return new WindowPropertyCardinalValue(propertyData.PropertyType, format32Array.Value.Select(x => (int)x).ToArray());
            }
        }

        return new WindowPropertyCardinalValue(propertyData.PropertyType, []);
    }

    public PropertyData ToPropertyData()
    {
        return new PropertyData.Format32Array(_propertyType, Value.Select(x => (Long)x).ToArray());
    }
}

public record WindowPropertyAtomValue : IWindowPropertyValue<WindowPropertyAtomValue>
{
    private readonly X11DisplayAtom _propertyType;

    private WindowPropertyAtomValue(X11DisplayAtom propertyType, string[] atoms)
    {
        _propertyType = propertyType;
        Atoms = atoms;
    }

    public static WindowPropertyAtomValue FromPropertyData(PropertyData propertyData)
    {
        var display = propertyData.PropertyType.Display;

        if (propertyData is PropertyData.Format32Array format32Array)
        {
            switch (propertyData.PropertyType.GetAtomName())
            {
                case WindowPropertyValues.Atom:
                    return new WindowPropertyAtomValue(propertyData.PropertyType,
                        format32Array.Value.Select(x => new X11Atom(x).WithDisplay(display).GetAtomName() ?? "(null)").ToArray());
            }
        }

        return new WindowPropertyAtomValue(propertyData.PropertyType, []);
    }

    public string ValueType => _propertyType.GetAtomName() ?? "(null)";
    
    public string[] Atoms { get; }
    
    public PropertyData ToPropertyData()
    {
        return new PropertyData.Format32Array(_propertyType, _propertyType.Display.InternAtoms(Atoms).Select(x => (Long)(int)x.Atom).ToArray());
    }
}
