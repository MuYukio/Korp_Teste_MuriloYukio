namespace Estoque.Domain.Exceptions;

/// Falha ao comunicar com o serviço de IA externo (timeout, erro de rede, resposta inválida).
public class IaIndisponivelException : Exception
{
    public IaIndisponivelException(string mensagem, Exception? innerException = null)
        : base(mensagem, innerException) { }
}