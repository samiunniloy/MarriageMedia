//import { Injectable, inject, signal } from '@angular/core';
//import { environment } from '../../environments/environment';
//import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
//import { PaginatedResult } from '../_models/pagination';
//import { Message } from '../_models/message';
//import { Member } from '../_models/member';
//import { map } from 'rxjs';
//import { setPaginatedResponse, setPaginationHeaders } from '../_helpers/paginationHelper';
//import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
//import { User } from '../_models/user';

//@Injectable({
//  providedIn: 'root'
//})
//export class MessageService {
//  baseUrl = environment.apiUrl;
//  hubUrl = environment.hubsUrl;
//  messageThread=signal<Message[]>([]);
//  private http = inject(HttpClient);
//  private hubConnection?:HubConnection;
//  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

//  createHubConnection(user: User, otherUsername: string) {
//    this.hubConnection = new HubConnectionBuilder()
//      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
//        accessTokenFactory: () => user.token
//      })
//      .withAutomaticReconnect()
//      .build();

//    this.hubConnection.start().catch(error => console.log(error));
//    this.hubConnection.on('ReceiveMessageThread', messages => {
//      this.messageThread.set(messages);
//    });

//    this.hubConnection.on('NewMessage', message => {
//      this.messageThread.update(messages=>[...messages, message])
//    });


//  }

//  stopHubConnection() {
//    if (this.hubConnection?.state===HubConnectionState.Connected) {
//      this.hubConnection.stop().catch(error => console.log(error));
//    }
//  }



//  getMessages(pageNumber: number, pageSize: number, container: string) {
//    let params =setPaginationHeaders(pageNumber, pageSize);
//    params = params.append('Container', container);
//    return this.http.get<Message[]>(this.baseUrl + 'Messages', { observe: 'response', params })
//      .subscribe({
//        next: response => {
//          this.paginatedResult.set({
//            items: response.body as Message[],
//            pagination: JSON.parse(response.headers.get('Pagination')!)
//          });
//        }
//      });
//  }
//  getMessageThread(username: string) {
//    const url = `${this.baseUrl}messages/thread/${username}`;
//    console.log('Generated URL:', url);
//    return this.http.get<Message[]>(url)
//  }

// async sendMessage(username: string, content: string) {
//    return  this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content });
//  }

//  deleteMessage(id: number) {
//    return this.http.delete(`http://localhost:5126/api/messages/${id}`);
//  }


//}




import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { setPaginatedResponse, setPaginationHeaders } from '../_helpers/paginationHelper';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private http = inject(HttpClient);

  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;

  // State signals
  messageThread = signal<Message[]>([]);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);
  currentPage = signal<number>(1);
  messagesPerPage = 10; // Number of messages per page

  /** 
   * Creates a SignalR connection and listens for new messages 
   */
  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}message?user=${otherUsername}`, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.error('SignalR Connection Error:', error));

    // Load initial message thread
    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThread.set(messages);
      this.fetchMessages(this.currentPage());
    });

    // Listen for new messages
    this.hubConnection.on('NewMessage', message => {
      this.messageThread.update(messages => [...messages, message]);
      this.fetchMessages(this.currentPage());
    });
  }

  /** 
   * Stops the SignalR connection 
   */
  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log('Error stopping connection:', error));
    }
  }

  /**
   * Fetch messages with pagination logic
   */
  fetchMessages(page: number) {
    const startIndex = (page - 1) * this.messagesPerPage;
    const paginatedMessages = this.messageThread().slice(startIndex, startIndex + this.messagesPerPage);
    this.paginatedResult.set({ items: paginatedMessages, pagination: { currentPage: page, totalItems: this.messageThread().length, totalPages: Math.ceil(this.messageThread().length / this.messagesPerPage) } });
  }

  /**
   * Loads paginated messages from the API (for inbox/outbox)
   */
  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);

    return this.http.get<Message[]>(`${this.baseUrl}Messages`, { observe: 'response', params })
      .subscribe({
        next: response => {
          if (response.body) {
            this.paginatedResult.set({
              items: response.body,
              pagination: JSON.parse(response.headers.get('Pagination')!)
            });
          }
        }
      });
  }

  /**
   * Gets message thread from API
   */
  getMessageThread(username: string) {
    return this.http.get<Message[]>(`${this.baseUrl}messages/thread/${username}`)
      .subscribe({
        next: messages => {
          this.messageThread.set(messages);
          this.fetchMessages(this.currentPage());
        }
      });
  }

  /**
   * Sends a new message via SignalR
   */
  async sendMessage(username: string, content: string) {
    if (this.hubConnection) {
      return this.hubConnection.invoke('SendMessage', { recipientUsername: username, content }).catch(error => console.error('Error sending message:', error));
    }
  }

  /**
   * Deletes a message
   */
  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }

  /**
   * Pagination navigation
   */
  goToPreviousPage() {
    if (this.currentPage() > 1) {
      this.currentPage.update(value => value - 1);
      this.fetchMessages(this.currentPage());
    }
  }

  goToNextPage() {
    const totalPages = Math.ceil(this.messageThread().length / this.messagesPerPage);
    if (this.currentPage() < totalPages) {
      this.currentPage.update(value => value + 1);
      this.fetchMessages(this.currentPage());
    }
  }
}
