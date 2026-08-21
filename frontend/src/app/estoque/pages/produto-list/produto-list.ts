import { Component, OnInit, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { catchError, finalize, of } from 'rxjs';

import { Produto } from '../../models/produto.model';
import { ProdutoService } from '../../services/produto.service';

@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [
    MatTableModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './produto-list.html',
  styleUrl: './produto-list.scss',
})
export class ProdutoList implements OnInit {
  private readonly produtoService = inject(ProdutoService);

  protected readonly produtos = signal<Produto[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly colunasExibidas = ['codigo', 'descricao', 'saldo'];

  ngOnInit(): void {
    this.carregarProdutos();
  }

  protected carregarProdutos(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.produtoService
      .listar()
      .pipe(
        catchError(() => {
          this.erro.set(
            'Não foi possível carregar os produtos. Verifique se o serviço de Estoque está disponível.'
          );
          return of<Produto[]>([]);
        }),
        finalize(() => this.carregando.set(false))
      )
      .subscribe((produtos) => this.produtos.set(produtos));
  }
}
