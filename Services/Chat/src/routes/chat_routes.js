const {
  tokenMiddleware,
  getUserIdFromToken,
} = require("../middleware/token_middlewares");
const Message = require("../models/message_model");
const router = require("express").Router();
const isNullOrEmpty = require("../helpers/null_or_empty");
const BaseModel = require("../core/base/base_response_model");
const axios = require("axios");
const https = require("https");
const RecentChatModel = require("../dtos/recent_chat_model");
const MessageStatus = require("../dtos/message_status");

// Exceptions
const NotFoundException = require("../core/exceptions/not_found_excepiton");

router.get(
  "/history/:targetUserId",
  tokenMiddleware,
  async (req, res, next) => {
    try {
      const { skip = 0, take = 10 } = req.query;
      const targetUserId = req.params.targetUserId;
      const sourceUserId = getUserIdFromToken(req.token);

      if (isNullOrEmpty(sourceUserId) || isNullOrEmpty(targetUserId)) {
        return res.status(400).send("Invalid user id");
      }

      // The MessageStatus information of the messages sent by the target user is assigned as "SEEN".
      await Message.updateMany(
        { receiverId: sourceUserId, senderId: targetUserId },
        { $set: { status: MessageStatus.SEEN } }
      );

      let messages = await Message.find({
        $or: [
          { receiverId: sourceUserId, senderId: targetUserId },
          { receiverId: targetUserId, senderId: sourceUserId },
        ],
      })
        .sort({ createdAt: -1 })
        .skip(skip * take)
        .limit(take);

      res.status(200).send(new BaseModel(messages, 200, true, null));
    } catch (error) {
      res
        .status(500)
        .send(new BaseModel(null, error.statusCode, false, error.message));
    }
  }
);

router.get("/recent-chats", tokenMiddleware, async (req, res, next) => {
  const httpsAgent = new https.Agent({
    rejectUnauthorized: false,
  });
  axios.defaults.httpsAgent = httpsAgent;

  const userId = getUserIdFromToken(req.token);

  if (isNullOrEmpty(userId)) {
    return res.status(400).send("Invalid user id");
  }

  /**
   * @type {Message[]}
   */
  const userIds = await Message.aggregate([
    // userId ile eşleşen belgeleri seçme
    { $match: { $or: [{ senderId: userId }, { receiverId: userId }] } },
    // En son sohbet tarihine göre gruplama
    { $sort: { createdAt: -1 } },
    {
      $group: {
        _id: {
          $cond: [{ $eq: ["$senderId", userId] }, "$receiverId", "$senderId"],
        },
        lastChat: { $first: "$$ROOT" },
      },
    },
    // Son sohbetlerin listesini döndürme
    { $replaceRoot: { newRoot: "$lastChat" } },
  ]);

  const body = {
    ids: userIds.map((x) => (x.senderId == userId ? x.receiverId : x.senderId)),
  };

  const response = await axios.post(
    "https://localhost:7149/api/user/get-user-info-list",
    body
  );

  /**
   * @type {RecentChatModel[]}
   */
  let recentChats = [];

  response.data.data.forEach((user) => {
    recentChats.push({
      userId: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      profileImage: user.profileImage,
      gender: user.gender,
      lastMessage: null,
      lastMessageDate: null,
      unreadMessageCount: 0,
    });
  });

  let messages = await Message.aggregate([
    { $match: { $or: [{ senderId: userId }, { receiverId: userId }] } },
    { $sort: { createdAt: -1 } },
    {
      $group: {
        _id: { senderId: "$senderId", receiverId: "$receiverId" },
        message: { $first: "$$ROOT" },
      },
    },
    { $replaceRoot: { newRoot: "$message" } },
    { $sort: { createdAt: -1 } },
  ]);
  messages.forEach((message) => {
    let recentChat = recentChats.find(
      (x) => x.userId == message.senderId || x.userId == message.receiverId
    );
    if (recentChat) {
      recentChat.lastMessage = message.content;
      recentChat.lastMessageDate = message.createdAt;
      recentChat.unreadMessageCount = messages.filter(
        (x) =>
          x.senderId == message.senderId &&
          x.receiverId == userId &&
          x.status == MessageStatus.SENT
      ).length;
    }
  });

  res.status(200).send(new BaseModel(recentChats, 200, true, null));
});

module.exports = router;
