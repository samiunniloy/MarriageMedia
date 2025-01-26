import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { MemberUpdateDto } from '../_models/MemberUpdateDto';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);


  getMembers(userParams:UserParams) {
    

    let params = this.setPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.http.get<Member[]>(this.baseUrl + 'User', {observe:'response',params}).subscribe({
      next: response => {

        this.paginatedResult.set({
          items: response.body as Member[],
          pagination:JSON.parse(response.headers.get('Pagination')!)
        })

      }
    })
  }
  getMember(username: string) {
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

}
