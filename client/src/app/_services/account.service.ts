import { HttpClient } from '@angular/common/http';
import { Injectable,inject, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient); 
  baseUrl = 'https://localhost:7198/api/';
  currentUser = signal<User | null>(null);
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUser.set(user);
      })
    )
  }
  logout() {
    localStorage.getItem('user');
    this.currentUser.set(null);
  }
}
