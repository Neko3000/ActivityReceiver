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

        var accelerationX = $this.find('.acceleration-x');
        var accelerationY = $this.find('.acceleration-y');
        var accelerationZ = $this.find('.acceleration-z');

        var sentenceJP;
        var sentenceEN;
        var division;
        var answerDivision;
        var content;

        var wordItems = new Array();
    
        var movementDTOs;
        var movementDTOCurrentIndex = 0;

        var deviceAccelerationDTOs;
        var deviceAccelerationDTOCurrentIndex = 0;

        var currentDrawPoint;
        var lastDrawPoint;

        var currentMillisecondTime = 0;
        var totalMillisecondTime = 0;

        var currentDistance = 0;

        var TimerID;
        var animationFrequency = 20;

        // Common
        var sortByLeft = function(a,b){
            return parseInt(a.obj.css('left'))> parseInt(b.obj.css('left'));
        }

        var sortByTime = function(a,b){
            return parseInt(a.time > b.time);
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

        var adjustCanvasToTime = function(time){

            var currrnetDrawPointTemp;
            var lastDrawPointTemp;

            $.each(movementDTOs,function(index,currentMovementDTO){
                if(currentMovementDTO.time > time){
                    return;
                }

                currrnetDrawPointTemp = new Point(currentMovementDTO.xPosition,currentMovementDTO.yPosition);

                if(currentMovementDTO.state == 0)
                {
                    lastDrawPointTemp = currrnetDrawPointTemp;
                    currentColorIndex = (currentColorIndex + 1) % colorList.length;
                }

                presentorProxy.drawRect(currrnetDrawPointTemp,colorList[currentColorIndex]);
                presentorProxy.drawLineBetweenTwoPoints(lastDrawPointTemp,currrnetDrawPointTemp,colorList[currentColorIndex]);

                lastDrawPointTemp = currrnetDrawPointTemp;
            });
        }

        var adjustWordItemsToTime = function(time){
            
            $.each(wordItems,function(index,wordItem){
                var elementPosition = getClosestElementPosition(wordItem,time);

                if(elementPosition != null)
                {                    
                    wordItem.obj.css({
                        left: elementPosition.left,
                        top: elementPosition.top
                    });
                }
            });
        }

        // Layout
        var setLayout = function () {

            // Set the size of Canvas, which should be equal to its parent
            fitToContainer(presentor);

            // Slider
            progressSlider.ionRangeSlider({
                min: 0,
                max: 100,
                from: 10,

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

            textAnswer.text(content);
            textJP.text(sentenceJP);
            textEN.text(sentenceEN);

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

                        totalMillisecondTime = getMaxMillisecondTime();
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

            var currentActiveWordItemSIM;
          
            var movementDTOCurrentIndexSIM = 0;
            var tapBeganRelativePositionSIM;  

            var currentMillisecondTimeSIM = 0;
            while(movementDTOCurrentIndexSIM <= movementDTOs.length - 1){

                while (movementDTOs[movementDTOCurrentIndexSIM].time <= currentMillisecondTimeSIM){

                    var currentMovementDTO = movementDTOs[movementDTOCurrentIndexSIM];
    
                    if (currentMovementDTO.state == 0) {
                        // tap
                        currentActiveWordItemSIM = getWordItemByTargetElementIndex(currentMovementDTO.targetElement);

                        if(currentActiveWordItemSIM.elementPositionCollection == null){

                            currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovementDTO.time,currentActiveWordItemSIM.orgElementPosition.left,currentActiveWordItemSIM.orgElementPosition.top));
                        }

                        currentElementPositionSIM = getClosestElementPosition(currentActiveWordItemSIM,currentMovementDTO.time);

                        tapBeganRelativePositionSIM = new Point(currentMovementDTO.xPosition - currentElementPositionSIM.left,currentMovementDTO.yPosition - currentElementPositionSIM.top); 

                    }
                    else if (currentMovementDTO.state == 1) {
                        // move   
                        var wordItemLeft = currentMovementDTO.xPosition - tapBeganRelativePositionSIM.x;
                        var wordItemTop = currentMovementDTO.yPosition - tapBeganRelativePositionSIM.y;
                    
                        currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovementDTO.time,wordItemLeft,wordItemTop));
    
                    }
                    else if (currentMovementDTO.state == 2) {
                        // end
                        var wordItemLeft = currentMovementDTO.xPosition - tapBeganRelativePositionSIM.x;
                        var wordItemTop = currentMovementDTO.yPosition - tapBeganRelativePositionSIM.y;
                    
                        currentActiveWordItemSIM.elementPositionCollection.push(new ElementPosition(currentMovementDTO.time,wordItemLeft,wordItemTop));

                        currentActiveWordItemSIM = null;             
                    }
    
                    movementDTOCurrentIndexSIM ++;

                    if(movementDTOCurrentIndexSIM > movementDTOs.length - 1){
                        break;
                    }            
                }

                currentMillisecondTimeSIM += 1000/animationFrequency;
            }

        }

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

        var getMaxMillisecondTime = function(){
            var sortedMovementDTOs = movementDTOs.slice().sort(sortByTime);
            sortedMovementDTOs.reverse();
            var sortedDeviceAccelerationDTOs = deviceAccelerationDTOs.slice().sort(sortByTime);
            sortedDeviceAccelerationDTOs.reverse();

            maxTime = sortedDeviceAccelerationDTOs[0].time > sortedMovementDTOs[0].time ? sortedDeviceAccelerationDTOs[0].time:sortedMovementDTOs[0].time;

            return maxTime;
        }

        var playMovementAnimation = function(){

            if(movementDTOCurrentIndex > movementDTOs.length - 1){

                return;
            }

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
                    lastDrawPoint = currentDrawPoint;
                    currentColorIndex = (currentColorIndex + 1) % colorList.length;
                }

                currentDistance += calculateDistance(currentDrawPoint,lastDrawPoint);

                movementDistance.text(currentDistance.toFixed(2));
                presentorProxy.drawRect(currentDrawPoint,colorList[currentColorIndex]);
                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint,currentDrawPoint,colorList[currentColorIndex]);

                // When finish drawing
                lastDrawPoint = currentDrawPoint;

                movementDTOCurrentIndex++;
            }
        }

        var playAcceralationAnimation = function(){

            if(deviceAccelerationDTOCurrentIndex > deviceAccelerationDTOs.length - 1){
                return;
            }

            while (deviceAccelerationDTOs[deviceAccelerationDTOCurrentIndex].time <= currentMillisecondTime) {

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
        }

        var playAnimation = function () {

            currentMillisecondTime += 1000/animationFrequency;

            if(currentMillisecondTime > totalMillisecondTime){
                clearInterval();

                return;
            }

            playMovementAnimation();
            playAcceralationAnimation();

            generateAnswer();
            triggerPrograssSlider();
        };

        var stopAnimation = function(){

            clearInterval(TimerID);
        }

        var getClosestElementPosition = function(wordItem,time){

            var selectedElementPosition;

            selectedElementPosition = wordItem.orgElementPosition;
            $.each(wordItem.elementPositionCollection,function(index,elementPosition){

                if(time < elementPosition.time){
                    return;
                }

                selectedElementPosition = elementPosition;
            });

            return selectedElementPosition;
        }

        var triggerPrograssSlider = function(){
            //movePrograssSlider = progressSlider.noUiSlider.set(100 * currentMillisecondTime/totalMillisecondTime);

            progressSlider.data("ionRangeSlider").update({
                from: 100 * currentMillisecondTime/totalMillisecondTime,
            });
        }

        var play = function () {

            TimerID = setInterval(playAnimation, 1000/animationFrequency);
        };

        $(function () {

            setLayout();
            getAnswer();
        });

    };
})(jQuery);