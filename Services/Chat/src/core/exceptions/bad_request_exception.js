class BadRequestException extends Error {
  constructor(message) {
    super(message);
    this.message = message ?? "Bad Request";
    this.statusCode = 400;
  }
}

module.exports = BadRequestException;
