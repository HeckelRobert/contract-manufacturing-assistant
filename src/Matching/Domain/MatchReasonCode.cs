namespace QuotationAccelerator.Matching.Domain;

public enum MatchReasonCode
{
    SameMaterial,
    SameSurfaceTreatment,
    SharedManufacturingProcess,
    SimilarQuantity,
    KeywordMatchInTitle,
    ComparableAssembly,
    ContainsWeldingOperations,
    ExistingBendingSetup,
    PartDescriptionSimilarity,
}
