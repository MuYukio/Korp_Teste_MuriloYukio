namespace Faturamento.Domain.Exceptions;

public class NotaFiscalInvalidaException : Exception
{
    public NotaFiscalInvalidaException(string mensagem) : base(mensagem)
    {
    }
}