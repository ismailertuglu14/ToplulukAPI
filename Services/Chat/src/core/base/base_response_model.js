class BaseModel {
    constructor(data, statusCode, isSuccess, errors) {
        this.data = data;
        this.statusCode = statusCode;
        this.isSuccess = isSuccess;
        this.errors = errors;
    }
}

module.exports = BaseModel;