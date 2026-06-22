namespace QuotationAccelerator.Inbox.UnitTests;

using FluentAssertions;
using QuotationAccelerator.Inbox.Application.Extraction;

public class HeuristicInquiryExtractionServiceTests
{
    private readonly HeuristicInquiryExtractionService _service = new();

    [Fact]
    public void Extract_ShouldParseGermanContractManufacturingEmail()
    {
        const string body = """
            100 Stück Haltewinkel aus Stahl 3 mm
            Material: S235
            Oberfläche: verzinkt
            Liefertermin: 14 Tage
            Dicke: 3 mm
            Länge: 120 mm
            """;

        var result = _service.Extract("Lohnfertigung Anfrage", body);

        result.Quantity.Should().Be(100);
        result.Material.Should().Be("S235");
        result.SurfaceTreatment.Should().Be("verzinkt");
        result.DeliveryDeadline.Should().Be("14 Tage");
        result.WorkPreparationSummary.Should().Contain("Thickness: 3 mm");
    }

    [Fact]
    public void Extract_ShouldReturnEmpty_WhenBodyIsMissing()
    {
        var result = _service.Extract(null, null);

        result.Quantity.Should().BeNull();
        result.Material.Should().BeNull();
    }
}
