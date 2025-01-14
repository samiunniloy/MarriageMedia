import { HttpClient } from '@angular/common/http';
import { Injectable,inject, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
 //  http = inject(HttpClient); 
  http = inject(HttpClient); 
  baseUrl = 'https://localhost:7198/api/';
  currentUser = signal<User | null>(null);
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        localStorage.setItem('user', JSON.stringify(user));
       
        this.currentUser.set(user);
        //console.log(this.currentUser());
      })
    )
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
