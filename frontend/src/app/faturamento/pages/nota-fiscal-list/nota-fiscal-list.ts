import { Component, OnInit, ViewChild, inject, signal, effect } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { catchError, finalize, of } from 'rxjs';

import { NotaFiscalResumo } from '../../models/nota-fiscal.model';
import { NotaFiscalService } from '../../services/nota-fiscal.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-nota-fiscal-list',
  standalone: true,
  imports: [
    MatTableModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    DatePipe,
  ],
  templateUrl: './nota-fiscal-list.html',
  styleUrl: './nota-fiscal-list.scss',
})
 export class NotaFiscalList implements OnInit {
  private readonly notaFiscalService = inject(NotaFiscalService);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  @ViewChild(MatSort)
  private set sort(sort: MatSort) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

  protected readonly notas = signal<NotaFiscalResumo[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly colunasExibidas = ['numero', 'status', 'criadaEm', 'totalItens'];
  protected readonly dataSource = new MatTableDataSource<NotaFiscalResumo>([]);

  constructor() {
    effect(() => {
      this.dataSource.data = this.notas();
    });
  }

  ngOnInit(): void {
    this.carregarNotas();
  }

  protected carregarNotas(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.notaFiscalService
      .listar()
      .pipe(
        catchError(() => {
          this.erro.set(
            'Não foi possível carregar as notas fiscais. Verifique se o serviço de Faturamento está disponível.'
          );
          return of<NotaFiscalResumo[]>([]);
        }),
        finalize(() => this.carregando.set(false))
      )
      .subscribe((notas) => this.notas.set(notas));
  }

  protected abrirNota(nota: NotaFiscalResumo): void {
    this.router.navigate(['/notas-fiscais', nota.id]);
  }

  protected criarNovaNota(): void {
    this.notaFiscalService.criar().subscribe({
      next: (notaCriada) => {
        this.router.navigate(['/notas-fiscais', notaCriada.id]);
      },
      error: () => {
        this.erro.set('Não foi possível criar uma nova nota fiscal. Tente novamente.');
      },
    });
  }
}
