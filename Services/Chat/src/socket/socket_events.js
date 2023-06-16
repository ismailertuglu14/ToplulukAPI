const Message = require("../models/message_model");
const GroupModel = require("../models/group_model");
const ConnectedUser = require("../models/connectedUser_model");
const sendMessageEvent = async (socket) => {
  socket.on("sendMessage", async (message) => {
    try {
      /**
       * @type {{from: ReceiveMessageModel , to: string, content: string, createdAt: Date}}
       */
      const { to, from, content, createdAt } = message;

      var createdMessage = await Message.create({
        content: content,
        senderId: from.id,
        receiverId: to,
        createdAt: createdAt,
      });
      var targetUser = await ConnectedUser.findOne({ userId: to });

      if (targetUser) {
        io.to(socket.id).to(targetUser.connectionId).emit("receiveMessage", {
          from,
          content,
          createdAt,
        });
        await Message.findOneAndUpdate(
          { _id: createdMessage._id },
          { $set: { status: MessageStatus.DELIVERED } }
        );
        console.log("Mesaj gönderildi:", message);
      } else {
        io.to(socket.id).emit("receiveMessage", {
          from,
          content,
          createdAt,
        });

        console.log("Hedef kullanıcı bulunamadı: ", to);
      }
    } catch (err) {
      return err;
    }
  });
};

const joinGroupEvent = async (socket) => {
  socket.on("joinGroup", async (data) => {
    const { groupId, userId } = data;

    let group = await Group.findOne({ _id: groupId });

    if (!group) throw new NotFoundException("Group not found");

    await Group.findOneAndUpdate(
      { _id: groupId },
      {
        $push: {
          users: userId,
        },
      }
    );

    socket.join(groupId);
    console.log("joinGroup çalıştı");
  });
};

module.exports = { sendMessageEvent, joinGroupEvent };
