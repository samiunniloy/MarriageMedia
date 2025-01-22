import { Component, Input, OnInit, input } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/member';
import { CommonModule, NgFor } from '@angular/common';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports:[CommonModule,NgFor],
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent {

  member = input.required<Member>();

}
