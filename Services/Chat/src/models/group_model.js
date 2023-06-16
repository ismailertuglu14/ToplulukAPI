const mongoose = require("mongoose");

const groupSchema = new mongoose.Schema({
  name: {
    type: String,
  },
  adminId: {
    type: String,
  },
  users: {
    type: Array,
  },
});

const Group = mongoose.model("GroupCollection", groupSchema);
module.exports = Group;
