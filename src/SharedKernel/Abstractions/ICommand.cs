namespace QuotationAccelerator.SharedKernel.Abstractions;

public interface ICommand<TResponse>
    where TResponse : notnull;
