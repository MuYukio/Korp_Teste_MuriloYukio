import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotaFiscalDetail } from './nota-fiscal-detail';

describe('NotaFiscalDetail', () => {
  let component: NotaFiscalDetail;
  let fixture: ComponentFixture<NotaFiscalDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFiscalDetail],
    }).compileComponents();

    fixture = TestBed.createComponent(NotaFiscalDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
