/**
 * 
 * @param {string} str 
 * @returns {boolean}
 */
function isNullOrEmpty(str){
    str = str.trim();
    console.log("str: "+str)
    switch(str){
        case undefined:
        case null:
        case '':
            return true;
        default:
            return false;
    }
}
module.exports = isNullOrEmpty;