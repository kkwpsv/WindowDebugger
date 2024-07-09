using System.Globalization;

namespace WindowDebugger.Services.NativeWindows;

public readonly record struct WindowSearchingFilter
{
    public WindowSearchingFilter()
    {
        SearchText = null;
        SearchingValueAsHex = null;
        SearchingValueAsDecimal = null;
    }

    public WindowSearchingFilter(string? searchText)
    {
        searchText = searchText?.Trim();
        SearchText = searchText;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            // There is no search text.
            SearchingValueAsHex = null;
            SearchingValueAsDecimal = null;
        }
        else
        {
            if (searchText.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                // This must be a hex value.
                SearchingValueAsHex = uint.TryParse(searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal)
                    ? uintHexVal
                    : null;
                SearchingValueAsDecimal = null;
            }
            else
            {
                // This may be a hex value or a decimal value.
                SearchingValueAsHex = uint.TryParse(searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal)
                    ? uintHexVal
                    : null;
                SearchingValueAsDecimal = uint.TryParse(searchText, out var uintVal) ? uintVal : null;
            }
        }
    }

    public string? SearchText { get; }

    public uint? SearchingValueAsHex { get; }

    public uint? SearchingValueAsDecimal { get; }

    public bool IncludingInvisibleWindow { get; init; } = true;

    public bool IncludingEmptyTitleWindow { get; init; } = true;

    public bool IncludingChildWindow { get; init; } = true;

    public bool IncludingMessageOnlyWindow { get; init; } = true;
}
