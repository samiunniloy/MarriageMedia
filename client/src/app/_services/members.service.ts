import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { MemberUpdateDto } from '../_models/MemberUpdateDto';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'User');
  }
  getMember(username: string) {
    return this.http.get<Member>(`${this.baseUrl}User/username?username=${username}`);
  }
  updateMember(member: MemberUpdateDto) {
  //  console.log(member);
    return this.http.put(this.baseUrl+'User', member);
  }

  //getHttpOptions() {
  //  return {
  //    headers: new HttpHeaders({
  //      Authorization:`Bearer ${this.accountService.currentUser()?.token}`
  //    })
  //  }
  //}

}
