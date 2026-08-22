namespace Faturamento.Domain.Exceptions;

public class OperacaoRecusadaPeloEstoqueException : Exception
{
    public OperacaoRecusadaPeloEstoqueException(string mensagem) : base(mensagem) { }
}