class Point{
    constructor(x, y) {
      this.x = x;
      this.y = y;
    }
}

class ElementPosition{
    constructor(time,left,top) {
        this.time = time;
        this.left = left;
        this.top = top;
      }
}
class WordItem{
    constructor(index,obj,elementPositionCollection) {
        this.index = index
        this.obj = obj
        this.elementPositionCollection = elementPositionCollection
      }
}
class PresentorProxy{
    constructor(context){
        this.context = context
    }

    drawRect(point) {

        // Calculate the center position
        this.context.fillRect(point.x - 2,point.y - 2,4,4);
    }

    drawLineBetweenTwoPoints(pointA,pointB){

        if(pointA == null || pointB == null)
        {
            return;
        }

        this.context.beginPath();
        this.context.moveTo(pointA.x,pointA.y);
        this.context.lineTo(pointB.x,pointB.y);
        this.context.stroke();
    }
}

(function ($) {

    $.fn.replayer = function (id) {
        var $this = $(this);

        // Get the dom object instead of warpped jQuery object

        var presentor = $this.find('.presentor');
        var presentorProxy = new PresentorProxy(presentor[0].getContext("2d"));

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
        var totalMillisecondTime = 0;

        var currentDistance = 0;

        // Common
        var sortByLeft = function(a,b){
            return parseInt(a.obj.css('left'))> parseInt(b.obj.css('left'));
        }
    
        var calculateDistance = function(pointA,pointB){

            if(pointA == null || pointB == null)
            {
                return;
            }

            return Math.sqrt(Math.pow(pointA.x-pointB.x,2)+Math.pow(pointA.y-pointB.y,2));
        }

        var fitToContainer = function(obj){
            var width = obj.parent().width();
            var height = obj.parent().height();

            obj.attr('width',width);
            obj.attr('height',height);
        }

        // Layout

        var setLayout = function () {

            // Set the size of Canvas, which should be equal to its parent
            fitToContainer(presentor);

            // btn
            $this.find('.get-data').click(function () {
                getAnswer();
            });

            $this.find('.start-play').click(function () {
                play();
            });

        };

        var generateAnswer = function(){

            wordItemsClone = wordItems.slice().sort(sortByLeft);

            var answerString = "";
            $.each(wordItemsClone, function (index, wordItem) {
                if(index == 0){
                    answerString = answerString + wordItem.obj.text();
                }
                else{
                    answerString = answerString + " " + wordItem.obj.text();
                }
            });
            answerString = answerString + ".";

            answerLabel.text(answerString);
        }


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
                        calculateWordItemPositions();
                    }
                }
            );
        };

        var getWordItemByTargetElementIndex = function(targetElementIndex){

            var selectedWordItem;
            $.each(wordItems,function(index,wordItem){
                if(wordItem.index == targetElementIndex){
                    selectedWordItem = wordItem;
                    return;
                }
            });
            return selectedWordItem;
        }

        var calculateWordItemPositions = function(){

            var currentMillisecondTimeSIM = 0;

            var currentActiveWordItemSIM;
            var tapBeganRelativePositionSIM;  
            
            var movementDTOCurrentIndexSIM = 0;

            currentMillisecondTimeSIM = 0;
            while(movementDTOCurrentIndexSIM <= movementDTOs.length - 1){

                while (movementDTOs[movementDTOCurrentIndexSIM].time <= currentMillisecondTimeSIM) {

                    var currentMovementDTO = movementDTOs[movementDTOCurrentIndexSIM];
    
                    if (currentMovementDTO.state == 0) {
                        // tap
                        currentActiveWordItemSIM = getWordItemByTargetElementIndex(currentMovementDTO.targetElement);
    
                        tapBeganRelativePositionSIM = new Point(currentMovementDTO.xPosition - parseInt(currentActiveWordItemSIM.obj.css('left')),currentMovementDTO.yPosition - parseInt(currentActiveWordItemSIM.obj.css('top')));
                    }
                    else if (currentMovementDTO.state == 1) {
                        // move

                        
                        var wordItemLeft = currentMovementDTO.xPosition - tapBeganRelativePositionSIM.x;
                        var wordItemTop = currentMovementDTO.yPosition - tapBeganRelativePositionSIM.y;
                    
                        currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovementDTO.time,wordItemLeft,wordItemTop));
    
                    }
                    else if (currentMovementDTO.state == 2) {
                        // end
                        currentActiveWordItemSIM = null;
                    }
    
                    movementDTOCurrentIndexSIM ++;
                    if(movementDTOCurrentIndexSIM > movementDTOs.length - 1){
                        break;
                    }            
                }

                currentMillisecondTimeSIM += 100;
            }

        }

        var generateWordItems = function () {
            var wordItemTemplate = $('<div class="word-item"><div class="word-item-background">here we are</div></div>');

            var splittedDivision = division.split('|');

            $.each(splittedDivision, function (index, word) {
                var wordItemObj = wordItemTemplate.clone();
                wordItemObj.find('.word-item-background').first().text(word);
                wordItemObj.appendTo(mainView);

                wordItems.push(new WordItem(index,wordItemObj,new Array()));
            });

        };

        var arrangeWordItems = function () {
            var horizontalPadding = 10.0;
            var verticalPadding = 10.0;

            // 26
            var wordItemHeight = wordItems[0].obj.innerHeight();

            var containerLength = mainView.width();
            var containerHeight = mainView.height();
            var lines = new Array();
            var currentLine = new Array();
            var currentLineLength = 0.0;

            $.each(wordItems, function (index, wordItem) {

                var wordItemWidth = wordItem.obj.innerWidth();

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

                    lines[i][j].obj.css({
                        left: currentXPosition + horizontalPadding,
                        top: containerHeight - (lines.length - i) * (wordItemHeight + verticalPadding)
                    });

                    currentXPosition = currentXPosition + horizontalPadding + lines[i][j].obj.innerWidth();
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

                    pointerBeganPositionXInWordItem = currentMovementDTO.xPosition - parseInt(currentActiveWordItem.obj.css('left'));
                    pointerBeganPositionYInWordItem = currentMovementDTO.yPosition - parseInt(currentActiveWordItem.obj.css('top'));

                    lastDrawPoint = null;

                }
                else if (currentMovementDTO.state == 1) {
                    // move

                    currentActiveWordItem.obj.css({
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
                presentorProxy.drawRect(presentorCtx,currentDrawPoint);
                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint,currentDrawPoint);

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

        var getClosestElementPosition = function(wordItem,time){

            var selectedElementPosition;
            $.each(wordItem.elementPositionCollection,function(index,elementPosition){
                if(time>=elementPosition.time){
                    selectedElementPosition = elementPosition;
                }
            });

            return selectedElementPosition;
        }

        var playMovementAnimation = function(){

            currentMillisecondTime += 100;

            while (movementDTOs[movementDTOCurrentIndex].time <= currentMillisecondTime) {
                var currentMovementDTO = movementDTOs[movementDTOCurrentIndex];

                $.each(wordItems,function(index,wordItem){
                    var elementPosition = getClosestElementPosition(wordItem,currentMillisecondTime);

                    if(elementPosition != null)
                    {
                        wordItem.obj.css({
                            left: elementPosition.left,
                            top: elementPosition.top
                        });
                    }
                });

                currentDrawPoint = new Point(currentMovementDTO.xPosition,currentMovementDTO.yPosition);

                if(currentMovementDTO.state == 0)
                {
                    lastDrawPoint = null;
                }

                if(lastDrawPoint == null){
                    lastDrawPoint = currentDrawPoint;
                }

                currentDistance += calculateDistance(currentDrawPoint,lastDrawPoint);

                movementDistance.text(currentDistance.toFixed(2));
                presentorProxy.drawRect(currentDrawPoint);
                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint,currentDrawPoint);

                // When finish drawing
                lastDrawPoint = currentDrawPoint;

                movementDTOCurrentIndex++;
            }
        }

        var play = function () {

            setInterval(playMovementAnimation, 100);
        };

        $(function () {
            //alert("init");

            //generateWordItems();
            //arrangeWordItems();

            setLayout();
        });

    };
})(jQuery);