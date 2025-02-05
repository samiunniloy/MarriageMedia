import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { Member } from '../_models/member';
import { map } from 'rxjs';
import { setPaginatedResponse, setPaginationHeaders } from '../_helpers/paginationHelper';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;
  messageThread=signal<Message[]>([]);
  private http = inject(HttpClient);
  private hubConnection?:HubConnection;
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => {
      console.log(error);
      console.log('could not connect to hub');
    });
    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThread.set(messages);
    });

    this.hubConnection.on('NewMessage', message => {
      this.messageThread.update(messages=>[...messages, message])
    });


  }

  stopHubConnection() {
    if (this.hubConnection?.state===HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }



  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params =setPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return this.http.get<Message[]>(this.baseUrl + 'Messages', { observe: 'response', params })
      .subscribe({
        next: response => {
          this.paginatedResult.set({
            items: response.body as Message[],
            pagination: JSON.parse(response.headers.get('Pagination')!)
          });
        }
      });
  }
  getMessageThread(username: string) {
    const url = `${this.baseUrl}messages/thread/${username}`;
    console.log('Generated URL:', url);
    return this.http.get<Message[]>(url)
  }

 async sendMessage(username: string, content: string) {
    return  this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content });
  
  }

  deleteMessage(id: number) {
    return this.http.delete(`http://localhost:5126/api/messages/${id}`);
  }


}



