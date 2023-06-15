class NotFoundException extends Error {
  constructor(message) {
    super(message);
    this.message = message ?? "Not found";
    this.statusCode = 404;
  }
}
module.exports = NotFoundException;
