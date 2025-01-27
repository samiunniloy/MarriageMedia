import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { MemberUpdateDto } from '../_models/MemberUpdateDto';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = model<UserParams>(new UserParams(this.user));

  resetUserParams() {
    this.userParams.set(new UserParams(this.user));
  }

  getMembers() {

    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));

    if (response) return this.setPaginatedresult(response);

    let params = this.setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http.get<Member[]>(this.baseUrl + 'User', {observe:'response',params}).subscribe({
      next: response => {

        this.setPaginatedresult(response);
        this.memberCache.set(Object.values(this.userParams()).join('-'), response);
      }
    })
  }
  getMember(username: string) {
    const member:Member = [... this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
    .find((m:Member)=>m.username===username)
    console.log(member);

    if (member) return of(member);


    return this.http.get<Member>(`${this.baseUrl}User/${username}`);
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


  private setPaginationHeaders(pageNumber: number, pageSize: number) {

    let params = new HttpParams();
    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    return params;

  }
  private setPaginatedresult(response: HttpResponse<Member[]>) {
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!)

    })
  }

}
