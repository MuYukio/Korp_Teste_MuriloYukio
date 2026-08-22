import { Routes } from '@angular/router';

export const FATURAMENTO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/nota-fiscal-list/nota-fiscal-list').then(
        (m) => m.NotaFiscalList
      ),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./pages/nota-fiscal-detail/nota-fiscal-detail').then(
        (m) => m.NotaFiscalDetail
      ),
  },
];
