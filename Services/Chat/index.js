const express = require("express");
const connectDB = require("./src/core/connection/mongodb_connection");
const cors = require("cors");
const dotenv = require("dotenv");
dotenv.config();
const Message = require("./src/models/message_model");
const ConnectedUser = require("./src/models/connectedUser_model");
const Group = require("./src/models/group_model");

const ReceiveMessageModel = require("./src/dtos/receive_message_model");
const MessageStatus = require("./src/dtos/message_status");

const chatRoutes = require("./src/routes/chat_routes");

// Connect to DB
connectDB();

const app = express();

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use(cors());

// ROUTES
app.use("/chat", chatRoutes);

const server = require("http").createServer(app);
const { Server } = require("socket.io");

const io = new Server({
  cors: {
    origin: "http://localhost:5173",
    methods: ["GET", "POST"],
    transports: ["websocket", "polling"],
    credentials: true,
  },
  allowEIO3: true,
});
io.listen(4000);

//const connectedUsers = {};

io.on("connection", (socket) => {
  socket.on("joinGroup", async (data) => {
    const { groupId, userId } = data;

    let group = await Group.findOne({ groupId: groupId });

    if (!group) {
      group = await Group.create({ groupId: groupId });
    }

    await Group.findOneAndUpdate(
      { groupId: groupId },
      {
        $push: {
          users: {
            userId: userId,
            connectionId: socket.id,
          },
        },
      }
    );

    socket.join(groupId);
    console.log("joinGroup çalıştı");
  });

  socket.on("sendMessageToGroup", async (data) => {
    var group = Group.findOne({ groupId: data.groupId });
    if (group) {
      io.to(data.groupId).emit("receiveMessage", {
        from: data.from,
        content: data.content,
        createdAt: data.createdAt,
      });
    } else {
      console.log("Grup bulunamadı");
    }
  });

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

  socket.on("messageUpdate", async (message) => {
    const { id, userId, content } = message;

    const result = await Message.findOneAndUpdate(
      { _id: id, senderId: userId },
      { $set: { content: content, isUpdated: true, updatedAt: Date.now() } }
    );
    var targetUser = await ConnectedUser.findOne({ userId: result.receiverId });

    console.log("Update result: ", result);

    io.to(socket.id).to(targetUser.connectionId).emit("messageUpdated", {
      id,
      content,
    });
  });

  socket.on("messageDelete", async (message) => {
    const { id, userId } = message;

    const result = await Message.findOneAndUpdate(
      {
        _id: id,
        senderId: userId,
      },
      {
        $set: { isDeleted: true, deletedAt: Date.now() },
      }
    );

    const targetUser = await ConnectedUser.findOne({
      userId: result.receiverId,
    });

    if (targetUser) {
      io.to(socket.id).to(targetUser.connectionId).emit("messageDeleted", {
        id,
      });
    } else {
      io.to(socket.id).emit("messageDeleted", {
        id,
      });
    }
  });

  // Soket bağlantısı kesildiğinde gerçekleşecek olaylar
  socket.on("disconnect", async () => {
    await ConnectedUser.findOneAndDelete({ connectionId: socket.id });
    console.log("Kullanıcı bağlantısı kesildi:", socket.id);
  });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`App Started on ${PORT}`);
});
