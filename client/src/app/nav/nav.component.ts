import { Component, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';


@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule,
    CommonModule,
    BsDropdownModule,],

  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  accountService = inject(AccountService);
  model: any = {};
  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
       
      },
      error: error => console.log(error)
    })
  // console.log(this.model);
  }
  logout(){
    this.accountService.logout();
  }
}
