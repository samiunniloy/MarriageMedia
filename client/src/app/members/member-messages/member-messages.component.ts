import { Component, OnInit, inject, input } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone:true,
  imports: [],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent implements OnInit {

  private messageService = inject(MessageService);

  username = input.required<string>();
  messages=input.required<Message[]>();

  ngOnInit(): void {

  }


}
