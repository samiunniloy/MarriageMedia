import { Component, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule,
    CommonModule,
    BsDropdownModule,
    RouterLink,
    RouterLinkActive,
    HasRoleDirective
  ],

  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  model: any = {};
  login(){
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members');
      },
     // error: error=>console.log(error.message)
      error: error => this.toastr.error(error.error)
    })
  // console.log(this.model);
  }
  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
