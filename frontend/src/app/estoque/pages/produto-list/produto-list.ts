import { Component, OnInit, AfterViewInit, ViewChild, inject, signal, effect } from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { catchError, finalize, of } from 'rxjs';

import { Produto } from '../../models/produto.model';
import { ProdutoService } from '../../services/produto.service';
import { ProdutoFormDialog } from '../produto-form-dialog/produto-form-dialog';

@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [
    MatTableModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './produto-list.html',
  styleUrl: './produto-list.scss',
})
export class ProdutoList implements OnInit, AfterViewInit {
  private readonly produtoService = inject(ProdutoService);
  private readonly dialog = inject(MatDialog);

  @ViewChild(MatSort)
  private set sort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

  protected readonly produtos = signal<Produto[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly colunasExibidas = ['codigo', 'descricao', 'saldo'];
  protected readonly dataSource = new MatTableDataSource<Produto>([]);

  constructor() {
    effect(() => {
      this.dataSource.data = this.produtos();
    });
  }

  ngOnInit(): void {
    this.carregarProdutos();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
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

  protected abrirFormularioNovoProduto(): void {
    const dialogRef = this.dialog.open(ProdutoFormDialog);

    dialogRef.afterClosed().subscribe((produtoCriado) => {
      if (produtoCriado) {
        this.carregarProdutos();
      }
    });
  }
}
