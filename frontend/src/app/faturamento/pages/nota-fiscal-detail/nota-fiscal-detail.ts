import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl, Validators } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { toSignal } from '@angular/core/rxjs-interop';
import { debounceTime, startWith, map, catchError, finalize, of } from 'rxjs';

import { NotaFiscal } from '../../models/nota-fiscal.model';
import { NotaFiscalService } from '../../services/nota-fiscal.service';
import { Produto } from '../../../estoque/models/produto.model';
import { ProdutoService } from '../../../estoque/services/produto.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-nota-fiscal-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './nota-fiscal-detail.html',
  styleUrl: './nota-fiscal-detail.scss',
})
export class NotaFiscalDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly notaFiscalService = inject(NotaFiscalService);
  private readonly produtoService = inject(ProdutoService);

  protected readonly nota = signal<NotaFiscal | null>(null);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly adicionandoItem = signal(false);

  private readonly produtosDisponiveis = signal<Produto[]>([]);
  protected readonly produtoSelecionado = signal<Produto | null>(null);

  protected readonly colunasItens = computed(() => {
    const notaAtual = this.nota();
    const colunasBase = ['codigo', 'descricao', 'quantidade'];
    return notaAtual?.status === 'Aberta' ? [...colunasBase, 'acoes'] : colunasBase;
  });
  
  // Controles do formulário de "adicionar item"
  protected readonly produtoCtrl = new FormControl('', { nonNullable: true });
  protected readonly quantidadeCtrl = new FormControl(1, {
    nonNullable: true,
    validators: [Validators.required, Validators.min(1)],
  });

  protected readonly produtosFiltrados = toSignal(
    this.produtoCtrl.valueChanges.pipe(
      startWith(''),
      debounceTime(200),
      map((termo) => this.filtrarProdutos(termo)),
      catchError(() => of([] as Produto[]))
    ),
    { initialValue: [] as Produto[] }
  );
  private readonly quantidadeValida = toSignal(
    this.quantidadeCtrl.statusChanges.pipe(
      startWith(this.quantidadeCtrl.status),
      map(() => this.quantidadeCtrl.valid)
    ),
    { initialValue: this.quantidadeCtrl.valid }
  );

  protected readonly podeAdicionarItem = computed(
    () => !!this.produtoSelecionado() && this.quantidadeValida()
  );

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.carregarNota(id);
    this.carregarProdutos();
  }

  private carregarNota(id: string): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.notaFiscalService
      .obterPorId(id)
      .pipe(
        catchError(() => {
          this.erro.set('Não foi possível carregar a nota fiscal.');
          return of(null);
        }),
        finalize(() => this.carregando.set(false))
      )
      .subscribe((nota) => this.nota.set(nota));
  }

  private carregarProdutos(): void {
    this.produtoService.listar().subscribe({
      next: (produtos) => this.produtosDisponiveis.set(produtos),
      error: () => {
  
      },
    });
  }

  private filtrarProdutos(valor: string | Produto | null): Produto[] {
    if (valor && typeof valor === 'object') {
      return this.produtosDisponiveis();
    }

    const busca = (valor ?? '').toLowerCase().trim();
    if (!busca) {
      return this.produtosDisponiveis();
    }
    return this.produtosDisponiveis().filter(
      (produto) =>
        produto.codigo.toLowerCase().includes(busca) ||
        produto.descricao.toLowerCase().includes(busca)
    );
  }

  protected exibirProduto(produto: Produto | null): string {
    return produto ? `${produto.codigo} — ${produto.descricao}` : '';
  }

  protected selecionarProduto(produto: Produto): void {
    this.produtoSelecionado.set(produto);
  }

  protected adicionarItem(): void {
    const nota = this.nota();
    const produto = this.produtoSelecionado();
    if (!nota || !produto || !this.podeAdicionarItem()) {
      return;
    }

    this.adicionandoItem.set(true);

    this.notaFiscalService
      .adicionarItem(nota.id, {
        produtoId: produto.id,
        produtoCodigo: produto.codigo,
        produtoDescricao: produto.descricao,
        quantidade: this.quantidadeCtrl.value,
      })
      .pipe(finalize(() => this.adicionandoItem.set(false)))
      .subscribe({
        next: () => {
          this.produtoCtrl.reset('');
          this.produtoSelecionado.set(null);
          this.quantidadeCtrl.setValue(1);
          this.atualizarItensSemRecarregarTela(nota.id); 
        },
        error: () => {
          this.erro.set('Não foi possível adicionar o item. Tente novamente.');
        },
      });
  }
  protected removerItem(itemId: string): void {
    const nota = this.nota();
    if (!nota) {
      return;
    }

    const confirmou = confirm('Remover este item da nota fiscal?');
    if (!confirmou) {
      return;
    }

    this.notaFiscalService.removerItem(nota.id, itemId).subscribe({
      next: () => this.atualizarItensSemRecarregarTela(nota.id),
      error: (err: HttpErrorResponse) => {
        const mensagemBackend = err.error?.erro;
        this.erro.set(mensagemBackend ?? 'Não foi possível remover o item. Tente novamente.');
      },
    });
  }
  protected readonly imprimindo = signal(false);

  protected imprimir(): void {
    const nota = this.nota();
    if (!nota || nota.status !== 'Aberta' || this.imprimindo()) {
      return;
    }

    this.imprimindo.set(true);
    this.erro.set(null);

    this.notaFiscalService
      .imprimir(nota.id)
      .pipe(finalize(() => this.imprimindo.set(false)))
      .subscribe({
        next: (resposta) => {
          this.nota.set({ ...nota, status: resposta.status });
        },
        error: (err: HttpErrorResponse) => {
          const mensagemBackend = err.error?.erro;
          this.erro.set(
            mensagemBackend ?? 'Não foi possível imprimir a nota fiscal. Tente novamente.'
          );
        },
      });
  }
  private atualizarItensSemRecarregarTela(id: string): void {
    this.notaFiscalService
      .obterPorId(id)
      .pipe(
        catchError(() => {
          this.erro.set('Item adicionado, mas não foi possível atualizar a lista. Recarregue a página.');
          return of(null);
        })
      )
      .subscribe((nota) => {
        if (nota) {
          this.nota.set(nota);
        }
      });
  }

  protected voltar(): void {
    this.router.navigate(['/notas-fiscais']);
  }
}
