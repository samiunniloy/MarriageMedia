import { Component,HostListener,OnInit,ViewChild,inject, viewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { MemberUpdateDto } from '../../_models/MemberUpdateDto';


@Component({
  selector: 'app-member-edit',
  standalone:true,
  imports: [TabsModule,FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editFrom') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
  if (this.editForm?.dirty) {
    $event.returnValue=true
  }
  }
  member?: Member;
  member1?: MemberUpdateDto;

  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);
  ngOnInit(): void {
    this.loadMember();
    //this.updateMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;
    this.memberService.getMember(user.username).subscribe({

      next:member=>this.member=member
    })
  }

  updateMember() {
    // Collecting DTO values from the form
    const memberUpdateDto: MemberUpdateDto = {
      introduction: this.editForm?.value.introduction,
      lookingFor: this.editForm?.value.lookingFor,
      interests: this.editForm?.value.interests,
      city: this.editForm?.value.city,
      country: this.editForm?.value.country
    };

    // Log the DTO for debugging purposes (optional)
    console.log('MemberUpdateDto:', memberUpdateDto);

    // Call the service method with the DTO
    this.memberService.updateMember(memberUpdateDto).subscribe({
      next: () => {
        this.toastr.success('Profile Updated successfully');
        // Reset the form to reflect the updated member state
       // this.editForm?.reset(this.member);
      },
      error: err => {
        this.toastr.error('Update failed');
        console.error('Update error:', err);
      }
    });
  }


}
