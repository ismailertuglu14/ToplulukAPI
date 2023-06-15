const mongoose = require("mongoose");

const connectedUserSchema = new mongoose.Schema({
  userId: {
    type: String,
    required: true,
  },
  connectionId: {
    type: String,
    required: true,
  },
  connectedDate: {
    type: Date,
    default: Date.now,
  },
  disconnectedDate: {
    type: Date,
    default: null,
  },
});

const ConnectedUser = mongoose.model(
  "ConnectedUser",
  connectedUserSchema,
  "ConnectedUserCollection"
);
module.exports = ConnectedUser;
