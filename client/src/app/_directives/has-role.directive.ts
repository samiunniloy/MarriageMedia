import { Directive, Input, OnInit, TemplateRef, ViewContainerRef, inject } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {

  @Input() appHasRole: string[] = [];

  private accountService = inject(AccountService);
  private viewContainerref = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  ngOnInit(): void {

  }

 

}
