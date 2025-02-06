import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { MemberUpdateDto } from '../../_models/MemberUpdateDto';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';
import { CommonModule } from '@angular/common';
import { PhotoComponent } from '../photo/photo.component';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule,PhotoEditorComponent,CommonModule,PhotoComponent],
  template: `
    @if(member){
    <div class="row">
      <!-- Left column with photo and basic info -->
      <div class="col-4">
        <h1>Your Profile</h1>
        <div class="card">
          <img [src]="member.photoUrl || './assets/user.png'"
               [alt]="member.knownAs"
               class="card-img img-thumbnail">
          <div class="card-body">
            <div>
              <strong>Location:</strong>
              <p>{{member.city}}, {{member.country}}</p>
            </div>
            <div>
              <strong>Age:</strong>
              <p>{{member.age}}</p>
            </div>
          </div>
          <div class="card-footer">
            <button [disabled]="!editForm?.dirty"
                    form="editForm"
                    type="submit"
                    class="btn btn-success col-12">
              Save Changes
            </button>
          </div>
        </div>
      </div>

      <!-- Right column with form -->
      <div class="col-8">
        @if (editForm?.dirty) {
          <div class="alert alert-info">
            <p><strong>Information: </strong>You have made changes. Any unsaved changes will be lost.</p>
          </div>
        }
        
        <tabset class="member-tabset">
          <tab heading="About {{member.knownAs}}">
            <form #editForm="ngForm" id="editForm" (ngSubmit)="updateMember()">
              <h4>Description</h4>
              <textarea class="form-control"
                        name="introduction"
                        [(ngModel)]="member.introduction"
                        rows="6"></textarea>
              
              <h4>Looking for</h4>
              <textarea class="form-control"
                        name="lookingFor"
                        [(ngModel)]="member.lookingFor"
                        rows="6"></textarea>
              
              <h4 class="mt-2">Interests</h4>
              <textarea class="form-control"
                        name="interests"
                        [(ngModel)]="member.interests"
                        rows="6"></textarea>
              
              <h4 class="mt-2">Location Details</h4>
              <div class="d-flex align-items-center">
                <label for="city">City:</label>
                <input type="text"
                       id="city"
                       [(ngModel)]="member.city"
                       class="form-control mx-2"
                       name="city">
                       
                <label for="country">Country:</label>
                <input type="text"
                       id="country"
                       [(ngModel)]="member.country"
                       class="form-control mx-2"
                       name="country">
              </div>
            </form>
          </tab>
          <tab heading="Photos">
                   <app-photo></app-photo>
          </tab>
        </tabset>
      </div>
    </div>
    }
  `
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;
  member?: Member;

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;

    this.memberService.getMember(user.username).subscribe({
      next: member => this.member = member,
      error: error => {
        console.error('Error loading member:', error);
        this.toastr.error('Could not load member data');
      }
    });
  }

  updateMember() {
    if (!this.member) return;

    const memberUpdateDto: MemberUpdateDto = {
      introduction: this.member.introduction,
      lookingFor: this.member.lookingFor,
      interests: this.member.interests,
      city: this.member.city,
      country: this.member.country
    };

    this.memberService.updateMember(memberUpdateDto).subscribe({
      next: () => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member);
      },
      error: error => {
        console.error('Update error:', error);
        this.toastr.error('Failed to update profile');
      }
    });
  }
}
