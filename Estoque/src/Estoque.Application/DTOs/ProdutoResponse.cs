using Estoque.Domain.Entities;

namespace Estoque.Application.DTOs;

public class ProdutoResponse
{
	public Guid Id { get; set; }
	public string Codigo { get; set; } = string.Empty;
	public string Descricao { get; set; } = string.Empty;
	public int Saldo { get; set; }
	public DateTime CriadoEm { get; set; }
	public DateTime AtualizadoEm { get; set; }

	public static ProdutoResponse DeEntidade(Produto produto)
	{
		return new ProdutoResponse
		{
			Id = produto.Id,
			Codigo = produto.Codigo,
			Descricao = produto.Descricao,
			Saldo = produto.Saldo,
			CriadoEm = produto.CriadoEm,
			AtualizadoEm = produto.AtualizadoEm
		};
	}
}