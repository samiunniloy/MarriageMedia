import { Directive, Input, OnInit, TemplateRef, ViewContainerRef, inject } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  standalone:true,
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {

  @Input() appHasRole: string[] = [];

  private accountService = inject(AccountService);
  private viewContainerref = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  ngOnInit(): void {

    if (this.accountService.roles()?.some((r: string) => this.appHasRole.includes(r))) {
      this.viewContainerref.createEmbeddedView(this.templateRef);
    }
    else {
      this.viewContainerref.clear();
    }

  }

}
