namespace QuotationAccelerator.Matching.UnitTests;

using FluentAssertions;
using QuotationAccelerator.Matching.Application.Scoring;

public sealed class ComparableTextMatcherTests
{
    [Theory]
    [InlineData("Stainless Steel 1.4301", "1.4301")]
    [InlineData("Aluminium EN AW-5754", "EN AW-5754")]
    [InlineData("S355", "S355")]
    public void AreMaterialsEquivalent_MatchesEquivalentForms(string left, string right)
    {
        ComparableTextMatcher.AreMaterialsEquivalent(left, right).Should().BeTrue();
    }

    [Theory]
    [InlineData("S355", "S235")]
    [InlineData("1.4301", "1.4571")]
    [InlineData("S235JR", "S235")]
    public void AreMaterialsEquivalent_DoesNotMatchDifferentGrades(string left, string right)
    {
        ComparableTextMatcher.AreMaterialsEquivalent(left, right).Should().BeFalse();
    }

    [Fact]
    public void SharesSignificantTokens_DetectsOverlapBetweenDescriptionAndTitle()
    {
        ComparableTextMatcher.SharesSignificantTokens(
                "Stainless enclosure for control cabinet",
                "Stainless Steel Enclosure")
            .Should()
            .BeTrue();
    }
}
