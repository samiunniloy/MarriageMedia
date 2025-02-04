import { Component, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit ,OnDestroy{

  @ViewChild('memberTabs', {static:true}) memberTabs?: TabsetComponent;

  private memberService = inject(MembersService);
  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
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



  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (messageTab) messageTab.active = true;
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.member) {
      const user = this.accountService.currentUser();
      if (!user) return;
      this.messageService.createHubConnection(user, this.member.username);

    }
    else {
      this.messageService.stopHubConnection();
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

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

}
