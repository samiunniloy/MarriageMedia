import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NavComponent } from './nav/nav.component';
import { AccountService } from './_services/account.service';
import { HomeComponent } from './home/home.component';


@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterOutlet, HttpClientModule, CommonModule, NavComponent,HomeComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  
  private accountService = inject(AccountService);
  title = 'MarriageMedia';

  ngOnInit(): void {
   // this.getUsers();
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) {
      console.log('No user found in localStorage');
      return;
    }
    const user = JSON.parse(userString);
    //console.log('User from localStorage:', user);  // Log the user
    this.accountService.currentUser.set(user);
    console.log('Current user after ngOnInit:', this.accountService.currentUser());
  }

 
}
