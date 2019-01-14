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
    constructor(index,obj,orgElementPosition,elementPositionCollection) {
        this.index = index
        this.obj = obj

        this.orgElementPosition = orgElementPosition;
        this.elementPositionCollection = elementPositionCollection
      }
}
class PresentorProxy{
    constructor(canvas){
        this.canvas = canvas;
        this.context = this.canvas.getContext("2d");

        
    }

    drawRect(point,color) {

        // Calculate the center position
        this.context.fillStyle = color;
        this.context.fillRect(point.x - 2,point.y - 2,4,4);
    }

    drawLineBetweenTwoPoints(pointA,pointB,color){

        if(pointA == null || pointB == null)
        {
            return;
        }

        this.context.beginPath();
        this.context.moveTo(pointA.x,pointA.y);
        this.context.lineTo(pointB.x,pointB.y);
        this.context.strokeStyle = color;
        this.context.stroke();
    }

    clearAll(){
        this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }
}

(function ($) {

    $.fn.replayer = function (id) {
        var $this = $(this);

        // Get the dom object instead of warpped jQuery object

        var presentor = $this.find('.presentor');
        var presentorProxy = new PresentorProxy(presentor[0]);
        var colorList = ['#34bfa3','#ffb822','#f4516c','#36a3f7','#5867dd'];
        var currentColorIndex = 0;

        var mainView = $this.find('.main-view');
        var sentenceJPLabel = $this.find('.sentence-jp');
        var answerLabel = $this.find('.answer');

        var progressSlider = $this.find('.progress-slider');

        var textAnswer = $this.find('.text-answer .info-value');
        var textJP = $this.find('.text-jp .info-value');
        var textEN = $this.find('.text-en .info-value');

        var movementDistance = $this.find('.movement-distance .info-value');
        var movementTotalDistance = $this.find('.movement-total-distance .info-value');
        var movementTime = $this.find('.movement-time .info-value');
        var movementDD = $this.find('.movement-dd .info-value');

        var accelerationX = $this.find('.acceleration-x');
        var accelerationY = $this.find('.acceleration-y');
        var accelerationZ = $this.find('.acceleration-z');

        var sentenceJP;
        var sentenceEN;
        var division;
        var standardAnswerDivision;
        var answerDivision;

        var startDate;
        var endDate;

        var wordItems = new Array();
    
        var movementCollection;
        var movementCurrentIndex = 0;

        var deviceAccelerationCollection;
        var deviceAccelerationCurrentIndex = 0;

        var currentDrawPoint;
        var lastDrawPoint;

        var currentMillisecondTime = 0;
        var totalMillisecondTime = 0;

        var currentDistance = 0;

        var TimerID;
        var animationFrequency = 20;

        // Calculation

        var getMaxDD = function () {
            var maxDD = 0;
            var lastMovementEnd;

            $.each(movementCollection, function (index, movement) {
                if (movement.state == 1) {
                    if (lastMovementEnd != null) {
                        var diffence = movement.time - lastMovementEnd.time
                        maxDD = maxDD < diffence ? diffence : maxDD;
                    }
                }
                else if (movement.state == 2) {
                    lastMovementEnd = movement
                }
            });
            return maxDD;
        };

        var getTotalDistance = function () {
            var totalDistance = 0;
            var currentPoint, lastPoint;

            $.each(movementCollection, function (index, movement) {
                if (movement.state == 0) {
                    lastPoint = new Point(movement.xPosition, movement.yPosition);
                }
                else if (movement.state == 1) {
                    currentPoint = new Point(movement.xPosition, movement.yPosition);
                    totalDistance += calculateDistance(lastPoint, currentPoint);

                    lastPoint = currentPoint;
                }
                else if (movement.state == 2) {
                    currentPoint = new Point(movement.xPosition, movement.yPosition);
                    totalDistance += calculateDistance(lastPoint, currentPoint);

                    lastPoint = null;
                }

            });

            return totalDistance;
        };

        var getTimeDifference = function (time1, time2) {

            var millionSecondTime1 = new Date(time1).getTime();
            var millionSecondTime2 = new Date(time2).getTime();

            return millionSecondTime2 - millionSecondTime1;
        };

        // Common
        var sortByLeft = function (a, b) {
            return parseInt(a.obj.css('left')) > parseInt(b.obj.css('left'));
        };

        var sortByTime = function (a, b) {
            return parseInt(a.time) > parseInt(b.time);
        };
    
        var calculateDistance = function (pointA, pointB) {

            if (pointA == null || pointB == null) {
                return;
            }

            return Math.sqrt(Math.pow(pointA.x - pointB.x, 2) + Math.pow(pointA.y - pointB.y, 2));
        };

        var fitToContainer = function (obj) {
            var width = obj.parent().width();
            var height = obj.parent().height();

            obj.attr('width', width);
            obj.attr('height', height);
        };

        var adjustCanvasToTime = function (time) {

            var currrnetDrawPointTemp;
            var lastDrawPointTemp;

            $.each(movementCollection, function (index, currentMovement) {
                if (currentMovement.time > time) {
                    return;
                }

                currrnetDrawPointTemp = new Point(currentMovement.xPosition, currentMovement.yPosition);

                if (currentMovement.state == 0) {
                    lastDrawPointTemp = currrnetDrawPointTemp;
                    currentColorIndex = (currentColorIndex + 1) % colorList.length;
                }

                presentorProxy.drawRect(currrnetDrawPointTemp, colorList[currentColorIndex]);
                presentorProxy.drawLineBetweenTwoPoints(lastDrawPointTemp, currrnetDrawPointTemp, colorList[currentColorIndex]);

                lastDrawPointTemp = currrnetDrawPointTemp;
            });
        };

        var adjustWordItemsToTime = function (time) {

            $.each(wordItems, function (index, wordItem) {
                var elementPosition = getClosestElementPosition(wordItem, time);

                if (elementPosition != null) {
                    wordItem.obj.css({
                        left: elementPosition.left,
                        top: elementPosition.top
                    });
                }
            });
        };

        // Layout
        var setLayout = function () {

            // Set the size of Canvas, which should be equal to its parent
            fitToContainer(presentor);

            // Slider
            progressSlider.ionRangeSlider({
                min: 0,
                max: 100,
                from: 0,

                onStart: function () {
                    // Called right after range slider instance initialised
                    stopAnimation();
                },
                onChange: function (data) {
                    // Called every time handle position is changed           
                    presentorProxy.clearAll();

                    currentMillisecondTime = data.from / 100 * totalMillisecondTime;

                    adjustCanvasToTime(currentMillisecondTime);
                    adjustWordItemsToTime(currentMillisecondTime);
                },
            });

            // btn
            $this.find('.pause-btn').click(function () {
                stopAnimation();
            });

            $this.find('.start-play-btn').click(function () {
                play();
            });


        };

        var showQuestion = function () {

            sentenceJPLabel.text(sentenceJP);

            textAnswer.text(answerDivision);
            textJP.text(sentenceJP);
            textEN.text(sentenceEN);

        };

        var generateAnswer = function () {

            wordItemsClone = wordItems.slice().sort(sortByLeft);

            var answerString = "";
            $.each(wordItemsClone, function (index, wordItem) {
                if (index == 0) {
                    answerString = answerString + wordItem.obj.text();
                }
                else {
                    answerString = answerString + " " + wordItem.obj.text();
                }
            });
            answerString = answerString + ".";

            answerLabel.text(answerString);
        };

        var getAnswer = function () {

            $.ajax(
                {
                    url: "/AnswerReplay/GetAnswerRecord?id=" + id.toString(),
                    type: "get",
                    dataType: "json", deviceAccelerationCollection,
                    async: false,
                    success: function (answer) {

                        sentenceJP = answer.sentenceJP;
                        sentenceEN = answer.sentenceEN;
                        division = answer.division;
                        standardAnswerDivision = answer.standardAnswerDivision;
                        answerDivision = answer.answerDivision;

                        startDate = answer.startDate;
                        endDate = answer.endDate;

                        movementCollection = answer.movementCollection.slice().sort(sortByTime);
                        deviceAccelerationCollection = answer.deviceAccelerationCollection.slice().sort(sortByTime);

                        showQuestion();

                        generateWordItems();
                        arrangeWordItems();

                        totalMillisecondTime = getMaxMillisecondTime();
                        calculateWordItemPositions();

                        movementTotalDistance.text(getTotalDistance().toFixed(2));
                        movementTime.text(getTimeDifference(startDate,endDate));
                        movementDD.text(getMaxDD());
                    }
                }
            );
        };

        var getWordItemByTargetElementIndex = function (targetElementIndex) {

            var selectedWordItem;
            $.each(wordItems, function (index, wordItem) {
                if (wordItem.index == targetElementIndex) {
                    selectedWordItem = wordItem;
                    return;
                }
            });
            return selectedWordItem;
        };

        var calculateWordItemPositions = function () {

            var currentActiveWordItemSIM;

            var movementCurrentIndexSIM = 0;
            var tapBeganRelativePositionSIM;

            var currentMillisecondTimeSIM = 0;
            while (movementCurrentIndexSIM <= movementCollection.length - 1) {

                while (movementCollection[movementCurrentIndexSIM].time <= currentMillisecondTimeSIM) {

                    var currentMovement = movementCollection[movementCurrentIndexSIM];

                    if (currentMovement.state == 0) {
                        // tap
                        currentActiveWordItemSIM = getWordItemByTargetElementIndex(currentMovement.targetElement);

                        if (currentActiveWordItemSIM.elementPositionCollection == null) {

                            currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovement.time, currentActiveWordItemSIM.orgElementPosition.left, currentActiveWordItemSIM.orgElementPosition.top));
                        }

                        currentElementPositionSIM = getClosestElementPosition(currentActiveWordItemSIM, currentMovement.time);

                        tapBeganRelativePositionSIM = new Point(currentMovement.xPosition - currentElementPositionSIM.left, currentMovement.yPosition - currentElementPositionSIM.top);

                    }
                    else if (currentMovement.state == 1) {
                        // move   
                        var wordItemLeft = currentMovement.xPosition - tapBeganRelativePositionSIM.x;
                        var wordItemTop = currentMovement.yPosition - tapBeganRelativePositionSIM.y;

                        currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovement.time, wordItemLeft, wordItemTop));

                    }
                    else if (currentMovement.state == 2) {
                        // end
                        var wordItemLeft = currentMovement.xPosition - tapBeganRelativePositionSIM.x;
                        var wordItemTop = currentMovement.yPosition - tapBeganRelativePositionSIM.y;

                        currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovement.time, wordItemLeft, wordItemTop));

                        currentActiveWordItemSIM = null;
                    }

                    movementCurrentIndexSIM++;

                    if (movementCurrentIndexSIM > movementCollection.length - 1) {
                        break;
                    }
                }

                currentMillisecondTimeSIM += 1000 / animationFrequency;
            }

        };

        var generateWordItems = function () {
            var wordItemTemplate = $('<div class="word-item"><div class="word-item-background">here we are</div></div>');

            var splittedDivision = division.split('|');

            $.each(splittedDivision, function (index, word) {
                var wordItemObj = wordItemTemplate.clone();
                wordItemObj.find('.word-item-background').first().text(word);
                wordItemObj.appendTo(mainView);

                wordItems.push(new WordItem(index,wordItemObj,null,new Array()));
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


            $.each(wordItems,function(index,wordItem){
                wordItem.orgElementPosition = new ElementPosition(0,parseInt(wordItem.obj.css('left')),parseInt(wordItem.obj.css('top')));
            });
        };

        var getMaxMillisecondTime = function () {
            var sortedmovementCollection = movementCollection.slice().sort(sortByTime);
            sortedmovementCollection.reverse();
            var sorteddeviceAccelerationCollection = deviceAccelerationCollection.slice().sort(sortByTime);
            sorteddeviceAccelerationCollection.reverse();

            maxTime = sorteddeviceAccelerationCollection[0].time > sortedmovementCollection[0].time ? sorteddeviceAccelerationCollection[0].time : sortedmovementCollection[0].time;

            return maxTime;
        };

        var playMovementAnimation = function () {

            if (movementCurrentIndex > movementCollection.length - 1) {

                return;
            }

            while (movementCollection[movementCurrentIndex].time <= currentMillisecondTime) {
                var currentMovement = movementCollection[movementCurrentIndex];

                $.each(wordItems, function (index, wordItem) {
                    var elementPosition = getClosestElementPosition(wordItem, currentMillisecondTime);

                    if (elementPosition != null) {
                        wordItem.obj.css({
                            left: elementPosition.left,
                            top: elementPosition.top
                        });
                    }
                });

                currentDrawPoint = new Point(currentMovement.xPosition, currentMovement.yPosition);

                if (currentMovement.state == 0) {
                    lastDrawPoint = currentDrawPoint;
                    currentColorIndex = (currentColorIndex + 1) % colorList.length;
                }

                currentDistance += calculateDistance(currentDrawPoint, lastDrawPoint);

                movementDistance.text(currentDistance.toFixed(2));
                presentorProxy.drawRect(currentDrawPoint, colorList[currentColorIndex]);
                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint, currentDrawPoint, colorList[currentColorIndex]);

                // When finish drawing
                lastDrawPoint = currentDrawPoint;

                movementCurrentIndex++;

                if (movementCurrentIndex >= movementCollection.length) {
                    break;
                }
            }
        };

        var playAcceralationAnimation = function () {

            if (deviceAccelerationCurrentIndex > deviceAccelerationCollection.length - 1) {
                return;
            }

            while (deviceAccelerationCollection[deviceAccelerationCurrentIndex].time <= currentMillisecondTime) {

                accelerationX.css({
                    width: (Math.abs(deviceAccelerationCollection[deviceAccelerationCurrentIndex].x / 3.0 * 100)).toString() + "%"
                });

                accelerationY.css({
                    width: (Math.abs(deviceAccelerationCollection[deviceAccelerationCurrentIndex].y / 3.0 * 100)).toString() + "%"
                });

                accelerationZ.css({
                    width: (Math.abs(deviceAccelerationCollection[deviceAccelerationCurrentIndex].z / 3.0 * 100)).toString() + "%"
                });

                deviceAccelerationCurrentIndex++;

                if (deviceAccelerationCurrentIndex >= deviceAccelerationCollection.length) {
                    break;
                }
            }
        };

        var playAnimation = function () {

            currentMillisecondTime += 1000/animationFrequency;

            if(currentMillisecondTime > totalMillisecondTime){
                stopAnimation();

                return;
            }

            playMovementAnimation();
            playAcceralationAnimation();

            generateAnswer();
            triggerPrograssSlider();
        };

        var stopAnimation = function () {

            clearInterval(TimerID);
        };

        var getClosestElementPosition = function (wordItem, time) {

            var selectedElementPosition;

            selectedElementPosition = wordItem.orgElementPosition;
            $.each(wordItem.elementPositionCollection, function (index, elementPosition) {

                if (time < elementPosition.time) {
                    return;
                }

                selectedElementPosition = elementPosition;
            });

            return selectedElementPosition;
        };

        var triggerPrograssSlider = function () {
            //movePrograssSlider = progressSlider.noUiSlider.set(100 * currentMillisecondTime/totalMillisecondTime);

            progressSlider.data("ionRangeSlider").update({
                from: 100 * currentMillisecondTime / totalMillisecondTime,
            });
        };

        var play = function () {

            TimerID = setInterval(playAnimation, 1000/animationFrequency);
        };

        $(function () {

            setLayout();
            getAnswer();
        });

    };
})(jQuery);