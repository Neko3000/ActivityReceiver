

// QuestionManage - Create, Edit
var shuffle = function (array) {
    var currentIndex = array.length, temporaryValue, randomIndex;

    // While there remain elements to shuffle...
    while (0 !== currentIndex) {

        // Pick a remaining element...
        randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex -= 1;

        // And swap it with the current element.
        temporaryValue = array[currentIndex];
        array[currentIndex] = array[randomIndex];
        array[randomIndex] = temporaryValue;
    }

    return array;
}

var generateFromSentenceEN = function (isRandom) {
    var sentenceEN = $("#sentence-en-input").val();
    sentenceEN = sentenceEN.replace(/\./g, "");

    var splittedSentenceENArray = sentenceEN.split(/[\s,]+/);

    if (isRandom) {
        splittedSentenceENArray = shuffle(splittedSentenceENArray);
    }

    var division = "";
    $.each(splittedSentenceENArray, function (index, word) {

        if (index == 0) {
            division = division + word;
        }
        else {
            division = division + '|' + word;
        }
    });

    return division;
};
