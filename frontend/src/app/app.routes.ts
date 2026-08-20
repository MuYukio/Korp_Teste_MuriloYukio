import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'produtos', pathMatch: 'full' },
  {
    path: 'produtos',
    loadChildren: () =>
      import('./estoque/estoque.routes').then((m) => m.ESTOQUE_ROUTES),
  },
  {
    path: 'notas-fiscais',
    loadChildren: () =>
      import('./faturamento/faturamento.routes').then((m) => m.FATURAMENTO_ROUTES),
  },
];
