namespace Faturamento.Domain.Exceptions;

public class EstoqueIndisponivelException : Exception
{
	public EstoqueIndisponivelException(string mensagem, Exception? innerException = null)
		: base(mensagem, innerException) { }
}