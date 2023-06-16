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

// Events

const {
  sendMessageEvent,
  joinGroupEvent,
} = require("./src/socket/socket_events");
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
const NotFoundException = require("./src/core/exceptions/not_found_excepiton");

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

io.on("connection", (socket) => {
  socket.on("connected", async (data) => {
    const { id } = data;
    const connectedUser = await ConnectedUser.find({ userId: id });
    if (connectedUser) {
      await ConnectedUser.findOneAndUpdate(
        { userId: id },
        { $set: { connectionId: socket.id } }
      );
    } else {
      await ConnectedUser.create({ userId: id, connectionId: socket.id });
    }
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

  sendMessageEvent(socket);

  socket.on("messageUpdate", async (message) => {
    const { id, userId, content } = message;

    const result = await Message.findOneAndUpdate(
      { _id: id, senderId: userId },
      { $set: { content: content, isUpdated: true, updatedAt: Date.now() } }
    );
    var targetUser = await ConnectedUser.findOne({ userId: result.receiverId });

    if (targetUser) {
      io.to(socket.id).to(targetUser.connectionId).emit("messageUpdated", {
        id,
        content,
      });
    } else {
      io.to(socket.id).emit("messageUpdated", {
        id,
        content,
      });
    }
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

  joinGroupEvent(socket);

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
