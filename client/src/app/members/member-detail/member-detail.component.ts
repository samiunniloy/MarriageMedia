import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {

  @ViewChild('memberTabs', {static:true}) memberTabs?: TabsetComponent;

  private memberService = inject(MembersService);
  private messageService = inject(MessageService);
  presenceService = inject(PresenceService);
  private route = inject(ActivatedRoute);

  member: Member = {} as Member;
  images: GalleryItem[] = [];

  activeTab?: TabDirective;
  messages: Message[] = [];

  ngOnInit(): void {
   // this.loadMember();
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p => {
          this.images.push(new ImageItem({src:p.url,thumb:p.url}))
        })
      }
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab']&&this.selectTab(params['tab'])
      }
    })

  }

  onUpdatedMessages(event: Message) {
    this.messages.push(event);
  }


  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (messageTab) messageTab.active = true;
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.member) {
      this.messageService.getMessageThread(this.member.username).subscribe({
        next: messages => this.messages = messages
      })
    }
  }

  loadMember() {
    const username = this.route.snapshot.paramMap.get("username");

    if (!username) {
      return;
    }

    this.memberService.getMember(username).subscribe({
      next: member => {
        this.member = member;
        this.images = member.photos.map(p => new ImageItem({ src: p.url, thumb: p.url }));

        // Activate the "Messages" tab if it's the default tab
        if (this.memberTabs && this.memberTabs.tabs.length > 0) {
          this.memberTabs.tabs[3].active = true; // Assuming "Messages" is the 4th tab (index 3)
        }
      }
    });
  }
}
