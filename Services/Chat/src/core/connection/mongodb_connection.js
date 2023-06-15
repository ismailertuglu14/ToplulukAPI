const mongoose = require("mongoose");

const connectDB = async () => {
  await mongoose.connect(process.env.MONGO_URI);

  console.log("mongoDb connected");
};

module.exports = connectDB;