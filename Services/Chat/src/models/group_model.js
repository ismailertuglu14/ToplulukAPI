const mongoose = require("mongoose");

const groupSchema = new mongoose.Schema({
  name: {
    type: String,
  },
  groupId: {
    type: String,
    required: true,
    unique: true,
  },
  users: [
    {
      userId: {
        type: String,
      },
      connectionId: {
        type: String,
      },
    },
  ],
});

const Group = mongoose.model("GroupCollection", groupSchema, "GroupCollection");
module.exports = Group;
