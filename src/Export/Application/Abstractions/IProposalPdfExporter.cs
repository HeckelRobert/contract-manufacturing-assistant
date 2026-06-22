namespace QuotationAccelerator.Export.Application.Abstractions;

using QuotationAccelerator.Export.Domain;
using QuotationAccelerator.SharedKernel.Results;

public interface IProposalPdfExporter
{
    Result Export(ProposalExportDocument document, string filePath);
}
