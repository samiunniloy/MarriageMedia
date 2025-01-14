import { Component ,OnInit,inject} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HttpClient, HttpClientModule} from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {NavComponent} from './nav/nav.component'; 
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule ,CommonModule,NavComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements  OnInit{
  http = inject(HttpClient);
  private accountService = inject(AccountService);
  title = 'MarriageMedia';
  users: any;

  ngOnInit(): void {
    this.getUsers();
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

  getUsers() {
    this.http.get('https://localhost:7198/api/User').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    })
  }
}
