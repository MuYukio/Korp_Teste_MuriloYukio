import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotaFiscalList } from './nota-fiscal-list';

describe('NotaFiscalList', () => {
  let component: NotaFiscalList;
  let fixture: ComponentFixture<NotaFiscalList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFiscalList],
    }).compileComponents();

    fixture = TestBed.createComponent(NotaFiscalList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
