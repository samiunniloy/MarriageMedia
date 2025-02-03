import { Injectable, inject } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  getUserWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    const roleParams = roles.join(','); // Convert array to a comma-separated string
    const url = `${this.baseUrl}admin/edit-roles/${username}?roles=${roleParams}`;

    return this.http.post<string[]>(url, {});
  }


}
