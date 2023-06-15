const jwt = require("jsonwebtoken");
const { TokenConstants } = require("../core/constants/token_constants");
const { isValidObjectId } = require("mongoose");
function tokenMiddleware(req, res, next) {
  const bearerToken = req.headers.authorization;
  if (!bearerToken) {
    res.status(401).send("No token provided");
    return;
  }

  const token = bearerToken.split(" ")[1];

  if (token === undefined || token === null || token === "") {
    res.status(401).send("Token error");
    return;
  }
  
  if (!isValidObjectId(getUserIdFromToken(token))) {
    res.status(401).send("Invalid token");
    return;
  }

  req.token = token;

  next();
}

/**
 *
 * @param {string} token
 * @returns {string | null}
 */
function getUserIdFromToken(token) {
  const decodedToken = jwt.decode(token);
  if (decodedToken && TokenConstants.ID in decodedToken) {
    return decodedToken[TokenConstants.ID];
  } else {
    return null;
  }
}

module.exports = { tokenMiddleware, getUserIdFromToken };
