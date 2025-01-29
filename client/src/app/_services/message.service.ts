import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { Member } from '../_models/member';
import { map } from 'rxjs';
import { setPaginatedResponse, setPaginationHeaders } from '../_helpers/paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

  //getMessages(pageNumber: number, pageSize: number, container: string) {
  //  let params = this.setPaginationHeaders(pageNumber, pageSize);
  //  params = params.append('Container', container);

  //  return this.http.get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
  //    .subscribe({
  //      next: response => this.setPaginatedResponse(response, this, PaginatedResult);

  //    })


  //}


  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params =setPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);

    //return this.http.get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
    //.subscribe({
    //  next: response => setPaginatedResponse(response, this.paginatedResult)
    //})

    return this.http.get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
      .subscribe({
        next: response => {
          this.paginatedResult.set({
            items: response.body as Message[],
            pagination: JSON.parse(response.headers.get('Pagination')!)
          });
        }
      });

  }

  constructor() { }
}


