import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProdutoFormDialog } from './produto-form-dialog';

describe('ProdutoFormDialog', () => {
  let component: ProdutoFormDialog;
  let fixture: ComponentFixture<ProdutoFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProdutoFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(ProdutoFormDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
