import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  NotaFiscal,
  NotaFiscalResumo,
  AdicionarItemRequest,
  ImprimirNotaFiscalResponse,
} from '../models/nota-fiscal.model';

@Injectable({ providedIn: 'root' })
export class NotaFiscalService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.faturamentoApiUrl}/notas-fiscais`;

  listar(): Observable<NotaFiscalResumo[]> {
    return this.http.get<NotaFiscalResumo[]>(this.baseUrl);
  }

  obterPorId(id: string): Observable<NotaFiscal> {
    return this.http.get<NotaFiscal>(`${this.baseUrl}/${id}`);
  }

  criar(): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(this.baseUrl, {});
  }

  adicionarItem(notaFiscalId: string, request: AdicionarItemRequest): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${notaFiscalId}/itens`, request);
  }
  removerItem(notaFiscalId: string, itemId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${notaFiscalId}/itens/${itemId}`);
  }

  imprimir(notaFiscalId: string): Observable<ImprimirNotaFiscalResponse> {
    return this.http.post<ImprimirNotaFiscalResponse>(
      `${this.baseUrl}/${notaFiscalId}/imprimir`,
      {}
    );
  }
}
