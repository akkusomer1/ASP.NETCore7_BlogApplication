
function convertFirstLetterToUpperCase(text) {

    return text.charAt(0).toUpperCase() + text.slice(1)
}

function convertToDateString(datestring) {
    const shortDate = new Date(datestring).toLocaleDateString('tr-TR')
    return shortDate
}