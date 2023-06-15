class RecentChatModel {
  constructor(
    userId,
    firstName,
    lastName,
    profileImage,
    gender,
    lastMessage,
    lastMessageDate,
    unreadMessageCount
  ) {
    this.userId = userId;
    this.firstName = firstName;
    this.lastName = lastName;
    this.profileImage = profileImage;
    this.gender = gender;
    this.lastMessage = lastMessage;
    this.lastMessageDate = lastMessageDate;
    this.unreadMessageCount = unreadMessageCount;
  }
}
