namespace Estoque.Domain.Exceptions;

public class ConcorrenciaException : Exception
{
    public ConcorrenciaException(string codigoProduto)
        : base($"O produto '{codigoProduto}' foi alterado por outra operação simultânea. Tente novamente.")
    {
    }
}