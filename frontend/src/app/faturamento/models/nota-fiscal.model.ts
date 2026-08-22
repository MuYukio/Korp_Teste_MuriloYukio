export type StatusNotaFiscal = 'Aberta' | 'Fechada';

export interface ItemNotaFiscal {
  id: string;
  produtoId: string;
  produtoCodigo: string;
  produtoDescricao: string;
  quantidade: number;
}

export interface NotaFiscal {
  id: string;
  numero: number;
  status: StatusNotaFiscal;
  criadaEm: string;
  fechadaEm: string | null;
  itens: ItemNotaFiscal[];
}

export interface NotaFiscalResumo {
  id: string;
  numero: number;
  status: StatusNotaFiscal;
  criadaEm: string;
  fechadaEm: string | null;
  totalItens: number;
}

export interface AdicionarItemRequest {
  produtoId: string;
  produtoCodigo: string;
  produtoDescricao: string;
  quantidade: number;
}

export interface ImprimirNotaFiscalResponse {
  sucesso: boolean;
  notaFiscalId: string;
  status: StatusNotaFiscal;
  mensagem: string;
}
