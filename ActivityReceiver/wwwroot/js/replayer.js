class Point{
    constructor(x, y) {
      this.x = x;
      this.y = y;
    }
}

class WordItem{
    constructor(index,html) {
        this.index = index
        this.html = html
      }
}

(function ($) {

    $.fn.replayer = function (id) {
        var $this = $(this);

        // Get the dom object instead of warpped jQuery object
        var presentor = $this.find('.presentor');
        var presentorCtx = presentor[0].getContext("2d");

        var mainView = $this.find('.main-view');
        var sentenceJPLabel = $this.find('.sentence-jp');
        var answerLabel = $this.find('.answer');

        var textAnswer = $this.find('.text-answer .info-value');
        var textJP = $this.find('.text-jp .info-value');
        var textEN = $this.find('.text-en .info-value');

        var movementDistance = $this.find('.movement-distance .info-value');

        var accelerationX = $this.find('.acceleration-x');
        var accelerationY = $this.find('.acceleration-y');
        var accelerationZ = $this.find('.acceleration-z');

        var sentenceJP;
        var sentenceEN;
        var division;
        var answerDivision;
        var content;

        var wordItems = new Array();
        var currentActiveWordItem;
    
        var movementDTOs;
        var movementDTOCurrentIndex = 0;

        var deviceAccelerationDTOs;
        var deviceAccelerationDTOCurrentIndex = 0;

        var pointerBeganPositionXInWordItem;
        var pointerBeganPositionYInWordItem;

        var currentDrawPoint;
        var lastDrawPoint;

        var currentMillisecondTime = 0;

        var currentDistance = 0;

        var sortByLeft = function(a,b){
            return parseInt(a.css('left'))> parseInt(b.css('left'));
        }

        var generateAnswer = function(){

            //var wordItemsClone = wordItems.clone();

            wordItemsClone = wordItems.slice().sort(sortByLeft);

            var answerString = "";
            $.each(wordItemsClone, function (index, wordItem) {
                if(index == 0){
                    answerString = answerString + wordItem.text();
                }
                else{
                    answerString = answerString + " " + wordItem.text();
                }
            });
            answerString = answerString + ".";

            answerLabel.text(answerString);
        }

        var fitToContainer = function(obj){
            var width = obj.parent().width();
            var height = obj.parent().height();

            obj.attr('width',width);
            obj.attr('height',height);
        }

        var calculateDistance = function(pointA,pointB){

            if(pointA == null || pointB == null)
            {
                return;
            }

            return Math.sqrt(Math.pow(pointA.x-pointB.x,2)+Math.pow(pointA.y-pointB.y,2));
        }

        var drawRect= function(context,point) {

            // Calculate the center position
            context.fillRect(point.x - 2,point.y - 2,4,4);
        }

        var drawLineBetweenTwoPoints = function(context,pointA,pointB){

            if(pointA == null || pointB == null)
            {
                return;
            }

            context.beginPath();
            context.moveTo(pointA.x,pointA.y);
            context.lineTo(pointB.x,pointB.y);
            context.stroke();
        }

        var setLayout = function () {

            // Set the size of Canvas, which should be equal to its parent
            fitToContainer(presentor);

            // get-data btn
            $this.find('.get-data').click(function () {
                getAnswer();
            });

            $this.find('.start-play').click(function () {
                play();
            });

        };

        var showQuestion = function () {

            sentenceJPLabel.text(sentenceJP);

            textAnswer.text(content);
            textJP.text(sentenceJP);
            textEN.text(sentenceEN);

        };

        var getAnswer = function () {

            $.ajax(
                {
                    url: "/AnswerReplay/GetAnswer?id=" + id.toString(),
                    type: "get",
                    dataType: "json",
                    async: false,
                    success: function (answer) {

                        sentenceJP = answer.sentenceJP;
                        sentenceEN = answer.sentenceEN;
                        division = answer.division;
                        answerDivision = answer.answerDivision;
                        content = answer.content;

                        movementDTOs = answer.movementDTOs;
                        deviceAccelerationDTOs = answer.deviceAccelerationDTOs;

                        showQuestion();

                        generateWordItems();
                        arrangeWordItems();
                    }
                }
            );
        };

        var generateWordItems = function () {
            var wordItemTemplate = $('<div class="word-item"><div class="word-item-background">here we are</div></div>');

            var splittedDivision = division.split('|');

            $.each(splittedDivision, function (index, word) {
                var wordItem = wordItemTemplate.clone();
                wordItem.find('.word-item-background').first().text(word);

                wordItems.push(wordItem);
                wordItem.appendTo(mainView);
            });

        };

        var arrangeWordItems = function () {
            var horizontalPadding = 10.0;
            var verticalPadding = 10.0;

            // 26
            var wordItemHeight = wordItems[0].innerHeight();

            var containerLength = mainView.width();
            var containerHeight = mainView.height();
            var lines = new Array();
            var currentLine = new Array();
            var currentLineLength = 0.0;

            $.each(wordItems, function (index, wordItem) {

                var wordItemWidth = wordItem.innerWidth();

                if (currentLineLength + wordItemWidth + horizontalPadding > containerLength) {

                    lines.push(currentLine);
                    currentLine = new Array();
                    currentLineLength = 0.0;
                }

                currentLine.push(wordItem);
                currentLineLength = currentLineLength + horizontalPadding + wordItemWidth;

                // when over
                if (index >= wordItems.length - 1) {
                    lines.push(currentLine);
                    currentLine = new Array();
                    currentLineLength = 0.0;
                }


            });

            for (i = 0; i <= lines.length - 1; i++) {
                var currentXPosition = 0.0;

                for (j = 0; j <= lines[i].length - 1; j++) {

                    lines[i][j].css({
                        left: currentXPosition + horizontalPadding,
                        top: containerHeight - (lines.length - i) * (wordItemHeight + verticalPadding)
                    });

                    currentXPosition = currentXPosition + horizontalPadding + lines[i][j].innerWidth();
                }
            }

        };

        var playAnimation = function () {

            currentMillisecondTime += 100;

            if (movementDTOCurrentIndex > movementDTOs.length) {

                movementDTOCurrentIndex = 0;
                deviceAccelerationDTOCurrentIndex = 0;
                clearInterval();
                return;
            }

            while (movementDTOs[movementDTOCurrentIndex].time <= currentMillisecondTime) {
                var currentMovementDTO = movementDTOs[movementDTOCurrentIndex];


                if (currentMovementDTO.state == 0) {
                    // tap

                    currentActiveWordItem = wordItems[currentMovementDTO.targetElement];

                    pointerBeganPositionXInWordItem = currentMovementDTO.xPosition - parseInt(currentActiveWordItem.css('left'));
                    pointerBeganPositionYInWordItem = currentMovementDTO.yPosition - parseInt(currentActiveWordItem.css('top'));

                    lastDrawPoint = null;

                }
                else if (currentMovementDTO.state == 1) {
                    // move

                    currentActiveWordItem.css({
                        left: currentMovementDTO.xPosition - pointerBeganPositionXInWordItem,
                        top: currentMovementDTO.yPosition - pointerBeganPositionYInWordItem
                    });

                }
                else if (currentMovementDTO.state == 2) {
                    // end

                    currentActiveWordItem = null;
                }

                currentDrawPoint = new Point(currentMovementDTO.xPosition,currentMovementDTO.yPosition);

                if(lastDrawPoint == null){
                    lastDrawPoint = currentDrawPoint;
                }

                currentDistance += calculateDistance(currentDrawPoint,lastDrawPoint);

                movementDistance.text(currentDistance.toFixed(2));
                drawRect(presentorCtx,currentDrawPoint);
                drawLineBetweenTwoPoints(presentorCtx,lastDrawPoint,currentDrawPoint);

                // When finish drawing
                lastDrawPoint = currentDrawPoint;


                //presentorCtx.fillRect(currentMovementDTO.xPosition,currentMovementDTO.yPosition,1,1);

/*                 var point = $('<div class="point"></div>');

                point.css({
                    left: currentMovementDTO.xPosition,
                    top: currentMovementDTO.yPosition
                });

                mainView.append(point); */

                movementDTOCurrentIndex++;
            }

            while (deviceAccelerationDTOs[deviceAccelerationDTOCurrentIndex].time <= currentMillisecondTime) {

                //alert(deviceAccelerationDTOs[deviceAccelerationDTOCurrentIndex].x);

                accelerationX.css({
                    width: (Math.abs(deviceAccelerationDTOs[deviceAccelerationDTOCurrentIndex].x / 3.0 * 100)).toString() + "%"
                });

                accelerationY.css({
                    width: (Math.abs(deviceAccelerationDTOs[deviceAccelerationDTOCurrentIndex].y / 3.0 * 100)).toString() + "%"
                });

                accelerationZ.css({
                    width: (Math.abs(deviceAccelerationDTOs[deviceAccelerationDTOCurrentIndex].z / 3.0 * 100)).toString() + "%"
                });

                deviceAccelerationDTOCurrentIndex++;
            }

            generateAnswer();

        };

        var play = function () {

            setInterval(playAnimation, 100);
        };

        $(function () {
            //alert("init");

            //generateWordItems();
            //arrangeWordItems();

            setLayout();
        });

    };
})(jQuery);