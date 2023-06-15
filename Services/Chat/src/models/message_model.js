const mongoose = require("mongoose");
const MessageStatus = require("../dtos/message_status");
const messageSchema = new mongoose.Schema({
  content: {
    type: String,
    default: "",
  },
  status: {
    type: Number,
    default: MessageStatus.SENT,
  },
  senderId: {
    type: String,
    required: true,
  },
  receiverId: {
    type: String,
    required: true,
  },
  groupId: {
    type: String,
  },
  createdAt: {
    type: Date,
    default: Date.now,
  },
  updatedAt: {
    type: Date,
    default: null,
  },
  isUpdated: {
    type: Boolean,
    default: false,
  },
  isDeleted: {
    type: Boolean,
    default: false,
  },
});

const Message = mongoose.model("Chat", messageSchema, "MessageCollection");
module.exports = Message;
