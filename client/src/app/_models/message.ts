export interface Message {
  id: number
  senderUserName: string
  senderId: number
  senderPhotoUrl: string
  recipientUserName: string
  recipientId: number
  recipientPhotoUrl: string
  content: string
  dateRead?: Date
  messageSent: Date
}
