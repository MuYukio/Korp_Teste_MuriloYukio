import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { catchError, finalize, of } from 'rxjs';

import { Produto } from '../../models/produto.model';
import { ProdutoService } from '../../services/produto.service';

@Component({
  selector: 'app-produto-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './produto-form-dialog.html',
  styleUrl: './produto-form-dialog.scss',
})
export class ProdutoFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly produtoService = inject(ProdutoService);
  private readonly dialogRef = inject(MatDialogRef<ProdutoFormDialog>);

  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = this.fb.group({
    codigo: ['', [Validators.required, Validators.maxLength(50)]],
    descricao: ['', [Validators.required, Validators.maxLength(200)]],
    saldoInicial: [0, [Validators.required, Validators.min(0)]],
  });

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando.set(true);
    this.erro.set(null);

    const { codigo, descricao, saldoInicial } = this.form.getRawValue();

    this.produtoService
      .cadastrar({
        codigo: codigo!,
        descricao: descricao!,
        saldoInicial: saldoInicial!,
      })
      .pipe(
        catchError((err) => {
          const mensagem =
            err.status === 409
              ? 'Já existe um produto cadastrado com esse código.'
              : 'Não foi possível cadastrar o produto. Tente novamente.';
          this.erro.set(mensagem);
          return of(null);
        }),
        finalize(() => this.salvando.set(false))
      )
      .subscribe((produto: Produto | null) => {
        if (produto) {
          this.dialogRef.close(produto);
        }
      });
  }

  protected cancelar(): void {
    this.dialogRef.close();
  }
}
