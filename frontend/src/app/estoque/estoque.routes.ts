import { Routes } from '@angular/router';

export const ESTOQUE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/produto-list/produto-list').then(
        (m) => m.ProdutoList
      ),
  },
];
