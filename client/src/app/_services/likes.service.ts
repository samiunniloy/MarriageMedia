import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class LikesService {

  baseUrl = environment.apiUrl;

  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);

  toggleLike(targetId: number) {
    return this.http.post(`${this.baseUrl}likes/${targetId}`, {});
  }
  getlikes(predicate: string) {
    return this.http.get<number[]>(`${this.baseUrl}likes?predicate=${predicate}`);
  }

  //getLikeIds() {
  //  return this.http.get<number[]>(`${this.baseUrl}likes?/list`).subscribe({
  //    next:ids=>this.likeIds.set(ids)
  //  })
  //}
  getLikeIds() {
    const user = localStorage.getItem('user');
    if (!user) {
      console.error('No user found in local storage');
      return;
    }

    return this.http
      .get<number[]>(`${this.baseUrl}api/Likes/list?user=${encodeURIComponent(user)}`)
      .subscribe({
        next: (ids) => this.likeIds.set(ids),
        error: (err) => console.error('Error fetching like IDs:', err),
      });
  }

  constructor() { }
}
