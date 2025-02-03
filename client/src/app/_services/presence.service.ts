import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';  
@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl=environment.hubsUrl;

  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);
  onlineUsers = signal<string[]>([])

  createHubConnection(user: User) {

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on('UserIsOnline', username => {
      this.toastr.info(username + 'has connected');
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.toastr.warning(username + 'has disconnected');
    });

    this.hubConnection.on('GetOnlineUsers', username => {
      this.onlineUsers.set(username)
    })

  }

  stopHubConnection() {
   if(this.hubConnection?.state===HubConnectionState.Connected)
    this.hubConnection.stop().catch(error => console.log(error));
  }


}
