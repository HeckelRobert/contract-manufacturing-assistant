namespace QuotationAccelerator.Export.Application.Abstractions;

using QuotationAccelerator.Export.Domain;
using QuotationAccelerator.SharedKernel.Results;

public interface IProposalWordExporter
{
    Result Export(ProposalExportDocument document, string filePath);
}
