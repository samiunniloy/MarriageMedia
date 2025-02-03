import { Component, ViewEncapsulation, computed, inject, input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { MembersService } from '../../_services/members.service';
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';
@Component({
  selector: 'app-member-card',
  standalone:true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
   
})
export class MemberCardComponent {

  private likeService = inject(LikesService);
  member = input.required<Member>();
  private presenceService = inject(PresenceService);
  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id));

  isOnline = computed(() => this.presenceService.onlineUsers().includes(this.member().username));
   

}
