namespace QuotationAccelerator.Matching.Application.Scoring;

public static class ComparableTextMatcher
{
    private const int MinSignificantTokenLength = 4;

    private static readonly char[] TokenSeparators = [' ', '\t', ',', ';', '/', '-', '_'];

    public static bool AreMaterialsEquivalent(string? left, string? right)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        if (string.Equals(left, right, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var leftForm = ToAlphanumericForm(left);
        var rightForm = ToAlphanumericForm(right);

        if (leftForm.Length == 0 || rightForm.Length == 0)
        {
            return false;
        }

        if (string.Equals(leftForm, rightForm, StringComparison.Ordinal))
        {
            return true;
        }

        return SharesSignificantTokens(left, right, minimumSharedTokens: 1);
    }

    public static bool SharesSignificantTokens(
        string? left,
        string? right,
        int minimumSharedTokens = 1,
        int minTokenLength = MinSignificantTokenLength)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        var leftTokens = ExtractSignificantTokens(left, minTokenLength);
        var rightTokens = ExtractSignificantTokens(right, minTokenLength);

        if (leftTokens.Count == 0 || rightTokens.Count == 0)
        {
            return false;
        }

        var sharedCount = leftTokens.Count(token => rightTokens.Contains(token));
        return sharedCount >= minimumSharedTokens;
    }

    private static HashSet<string> ExtractSignificantTokens(string text, int minTokenLength)
    {
        return text
            .Split(TokenSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ToAlphanumericForm)
            .Where(token => token.Length >= minTokenLength)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static string ToAlphanumericForm(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant));
}
