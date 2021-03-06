/* Classes*/
// Basic
class Point{
    constructor(x, y) {
      this.x = x;
      this.y = y;
    }
}

// WordItem
class ElementState{
    constructor(time,isActive,x,y) {
        this.time = time;
        this.isActive = isActive;

        this.x = x;
        this.y = y;
      }
}

class WordItem{
    
    constructor(index,obj,elementStateCollection) {
        this.index = index;
        this.obj = obj;

        this.elementStateCollection = elementStateCollection;
      }

      setPosition(x,y){

        this.obj.css({
            left: x,
            top: y
        });
      }

      toActive(){

          this.obj.find('.word-item-background').css({
            "background-color":"#68C7B9"
        });
      }

      cancelActive(){
          this.obj.find('.word-item-background').css({
            "background-color":"#222a41"
        });
      }
}

// For canvas
class PresentorPoint{
    constructor(time, state, x, y, isAbnormal) {

        this.time = time;
        // As the state in ios begain/move/end
        this.state = state;

        this.x = x;
        this.y = y;

        this.isAbnormal = isAbnormal;
      }
}

// For RectSelectionView
class Rectangle {
    constructor(x, y, width, height) {
        this.x = x;
        this.y = y;

        this.width = width;
        this.height = height;
    }

    static getRectFromTwoPoints(point1, point2) {
        var orgX = point1.x < point2.x ? point1.x : point2.x;
        var orgY = point1.y < point2.y ? point1.y : point2.y;

        var width = Math.abs(point1.x - point2.x);
        var height = Math.abs(point1.y - point2.y);

        return new Rectangle(orgX, orgY, width, height);
    }
}
class RectSelectionViewState {
    constructor(time, state, rect) {

        this.time = time;
        // As the state in ios begain/move/end
        this.state = state;

        this.rect = rect;
    }
}
class RectSelectionView {
    constructor(obj) {
        this.obj = obj;
    }

    setFrame(rect) {
        this.obj.css({
            "left": rect.x,
            "top": rect.y,
            "width": rect.width,
            "height": rect.height
        });
    }
    show() {
        this.obj.css({
            "display": "block"
        });
    }
    hide() {
        this.obj.css({
            "display": "none"
        });
    }
}
// Presentor
class PresentorProxy{
    constructor(canvas){
        this.canvas = canvas;
        this.context = this.canvas.getContext("2d");  
    }

    drawRect(point,color) {

        this.context.fillStyle = color;
        // Calculate the center position
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

        var colorList = ['#34bfa3', '#ffb822', '#f4516c', '#36a3f7', '#5867dd'];
        var currentColorIndex = 0;

        var lightGrey = '#b2b2b2';
        var standardRed = '#da3f5e';

        var mainView = $this.find('.main-view');
        var sentenceJPLabel = $this.find('.sentence-jp');
        var answerLabel = $this.find('.answer');

        var rectSelectionView = new RectSelectionView($this.find('.rect-selection-view')); 

        var progressSlider = $this.find('.progress-slider');

        var accelerationX = $this.find('.acceleration-x');
        var accelerationY = $this.find('.acceleration-y');
        var accelerationZ = $this.find('.acceleration-z');

        // Show in replayer
        var sentenceJP;
        var division;

        var startDate;
        var endDate;

        var wordItemCollection = [];
        var presentorPointCollection = [];
        var presentorPointCollectionCurrentIndex = 0;

        var rectSelectionViewStateCollection = [];
        var rectSelectionViewStateCollectionCurrentIndex = 0;

        var currentDrawPoint;
        var lastDrawPoint;
    
        var movementCollection;
        var movementAnimationCurrentIndex = 0;

        var movementSupervisedCollection;
        var movementSupervisedAnimationCurrentIndex = 0;

        var deviceAccelerationCollection;
        var deviceAccelerationAnimationCurrentIndex = 0;

        var currentMillisecondTime = 0;
        var totalMillisecondTime = 0;

        var currentDistance = 0;

        var TimerID;
        var animationFrequency = 20;

        var presentorPointHintAbnormalBegin = $('<div class="presentor-point-hint">begin</div>');
        var presentorPointHintAbnormalEnd = $('<div class="presentor-point-hint">end</div>');

        var arrowBottom = $('<div class="arrow-bottom"></div>');

        var isDrawingAbnormalNow = false;

        // Sorts
        var sortByLeft = function (a, b) {
            return parseInt(a.obj.css('left')) > parseInt(b.obj.css('left'));
        };

        var sortByTime = function (a, b) {
            return parseInt(a.time) > parseInt(b.time);
        };

        var sortByIndex = function (a, b) {
            return parseInt(a.index) > parseInt(b.index);
        };
    
        var calculateDistance = function (pointA, pointB) {

            if (pointA == null || pointB == null) {
                return;
            }

            return Math.sqrt(Math.pow(pointA.x - pointB.x, 2) + Math.pow(pointA.y - pointB.y, 2));
        };

        var fitToContainer = function (obj) {
            var width = obj.parent().css('width');
            var height = obj.parent().css('height');

            obj.attr('width', width);
            obj.attr('height', height);
        };

        var adjustMainViewSizeToResolution = function (resolution) {
            var splittedResoultion = resolution.split('x');

            var width = splittedResoultion[0];
            var height = splittedResoultion[1];

            mainView.css({
                width: width,
                height: height
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

        };

        var generateAnswer = function () {

            wordItemsClone = wordItemCollection.slice().sort(sortByLeft);

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

        var getAnswerRecord = function () {

            $.ajax(
                {
                    url: "/AnswerRecordReplay/GetAnswerRecord?id=" + id.toString(),
                    type: "get",
                    dataType: "json", deviceAccelerationCollection,
                    async: false,
                    success: function (answer) {

                        sentenceJP = answer.sentenceJP;
                        division = answer.division;

                        startDate = answer.startDate;
                        endDate = answer.endDate;

                        movementCollection = answer.movementSupervisedCollection.slice().sort(sortByIndex);
                        //movementSupervisedCollection = answer.movementSupervisedCollection.slice().sort(sortByIndex);
                        deviceAccelerationCollection = answer.deviceAccelerationCollection.slice().sort(sortByIndex);

                        adjustMainViewSizeToResolution(answer.resolution);

                        showQuestion();

                        generateWordItems();
                        arrangeWordItems();

                        totalMillisecondTime = getMaxMillisecondTime();
                        simulateWordItemState();

                    }
                }
            );
        };

        var getWordItemByTargetElement = function(targetElement){

            var splittedTargetElement = targetElement.split('#');

            var selectedWordItem = [];
            for(var i = 0; i <= splittedTargetElement.length - 1 ; i++)
            {
                for(var j = 0; j<= wordItemCollection.length - 1 ; j++)
                {
                    if(wordItemCollection[j].index == parseInt(splittedTargetElement[i]))
                    {
                        selectedWordItem.push(wordItemCollection[j]);
                        break;
                    }
                }
            }

            return selectedWordItem;
        } 

        var simulateWordItemState = function () {

            // Variables
            var currentMovement;
            var targetElementCollection;
            var pointerLastPosition, pointerCurrentPosition;
            var potinerRectSelectionViewBeginPosition;
            var offsetX, offsetY;

            var index = 0;
            while (index < movementCollection.length) {

                currentMovement = movementCollection[index];
                console.log(currentMovement.index + "/" + currentMovement.time);
                switch (currentMovement.state) {

                    // Drag Single/Group Begain
                    case 0:
                    case 6:

                        targetElementCollection = getWordItemByTargetElement(currentMovement.targetElement);

                        pointerCurrentPosition = new Point(currentMovement.xPosition, currentMovement.yPosition);

                        presentorPointCollection.push(new PresentorPoint(currentMovement.time, 0, pointerCurrentPosition.x, pointerCurrentPosition.y, currentMovement.isAbnormal));

                        targetElementCollection.forEach(function (wordItem, index) {

                            wordItem.elementStateCollection.push(new ElementState(currentMovement.time, true, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y));

                        });

                        pointerLastPosition = pointerCurrentPosition;

                        index++;

                        while (movementCollection[index].state == 1 || movementCollection[index].state == 7) {
                            currentMovement = movementCollection[index];
                            targetElementCollection = getWordItemByTargetElement(currentMovement.targetElement);

                            pointerCurrentPosition = new Point(currentMovement.xPosition, currentMovement.yPosition);

                            presentorPointCollection.push(new PresentorPoint(currentMovement.time, 1, pointerCurrentPosition.x, pointerCurrentPosition.y, currentMovement.isAbnormal));

                            offsetX = pointerCurrentPosition.x - pointerLastPosition.x;
                            offsetY = pointerCurrentPosition.y - pointerLastPosition.y;

                            targetElementCollection.forEach(function (wordItem, index) {

                                wordItem.elementStateCollection.push(new ElementState(currentMovement.time, true, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x + offsetX, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y + offsetY));

                            });

                            pointerLastPosition = pointerCurrentPosition;

                            index++;
                        }

                        currentMovement = movementCollection[index];

                        pointerCurrentPosition = new Point(currentMovement.xPosition, currentMovement.yPosition);

                        presentorPointCollection.push(new PresentorPoint(currentMovement.time, 2, pointerCurrentPosition.x, pointerCurrentPosition.y, currentMovement.isAbnormal));

                        offsetX = pointerCurrentPosition.x - pointerLastPosition.x;
                        offsetY = pointerCurrentPosition.y - pointerLastPosition.y;

                        targetElementCollection.forEach(function (wordItem, index) {

                            wordItem.elementStateCollection.push(new ElementState(currentMovement.time, false, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x + offsetX, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y + offsetY));

                        });

                        index++;

                        break;

                    // Make Group Begin
                    case 3:

                        targetElementCollection = getWordItemByTargetElement(currentMovement.targetElement);

                        pointerCurrentPosition = new Point(currentMovement.xPosition, currentMovement.yPosition);

                        presentorPointCollection.push(new PresentorPoint(currentMovement.time, 0, pointerCurrentPosition.x, pointerCurrentPosition.y, currentMovement.isAbnormal));

                        potinerRectSelectionViewBeginPosition = pointerCurrentPosition;

                        rectSelectionViewStateCollection.push(new RectSelectionViewState(currentMovement.time, 0, new Rectangle(currentMovement.xPosition, currentMovement.xPosition,0,0)));

                        targetElementCollection.forEach(function (wordItem, index) {

                            wordItem.elementStateCollection.push(new ElementState(currentMovement.time, true, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y));

                        });

                        index++;

                        while (movementCollection[index].state == 4) {
                            currentMovement = movementCollection[index];
                            targetElementCollection = getWordItemByTargetElement(currentMovement.targetElement);

                            pointerCurrentPosition = new Point(currentMovement.xPosition, currentMovement.yPosition);

                            presentorPointCollection.push(new PresentorPoint(currentMovement.time, 1, pointerCurrentPosition.x, pointerCurrentPosition.y, currentMovement.isAbnormal));

                            rectSelectionViewStateCollection.push(new RectSelectionViewState(currentMovement.time, 1, Rectangle.getRectFromTwoPoints(pointerCurrentPosition, potinerRectSelectionViewBeginPosition)));

                            targetElementCollection.forEach(function (wordItem, index) {

                                wordItem.elementStateCollection.push(new ElementState(currentMovement.time, true, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y));

                            });

                            index++;
                        }

                        currentMovement = movementCollection[index];

                        pointerCurrentPosition = new Point(currentMovement.xPosition, currentMovement.yPosition);

                        presentorPointCollection.push(new PresentorPoint(currentMovement.time, 2, pointerCurrentPosition.x, pointerCurrentPosition.y, currentMovement.isAbnormal));

                        rectSelectionViewStateCollection.push(new RectSelectionViewState(currentMovement.time, 2, Rectangle.getRectFromTwoPoints(pointerCurrentPosition, potinerRectSelectionViewBeginPosition)));

                        targetElementCollection.forEach(function (wordItem, index) {

                            wordItem.elementStateCollection.push(new ElementState(currentMovement.time, true, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y));

                        });

                        index++;

                        break;

                    // Cancel Group
                    case 9:

                        wordItemCollection.forEach(function (wordItem, index) {

                            wordItem.elementStateCollection.push(new ElementState(currentMovement.time, false, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].x, wordItem.elementStateCollection[wordItem.elementStateCollection.length - 1].y));

                        });

                        index++;

                        break;

                    default:

                        break;
                }
            }
        };

        var generateWordItems = function () {
            var wordItemTemplate = $('<div class="word-item"><div class="word-item-background">here we are</div></div>');

            var splittedDivision = division.split('|');

            $.each(splittedDivision, function (index, word) {
                var wordItemObj = wordItemTemplate.clone();
                wordItemObj.find('.word-item-background').first().text(word);
                wordItemObj.appendTo(mainView);

                wordItemCollection.push(new WordItem(index,wordItemObj,new Array()));
            });

        };

        var arrangeWordItems = function () {
            var horizontalPadding = 10.0;
            var verticalPadding = 10.0;

            // 26
            var wordItemHeight = wordItemCollection[0].obj.innerHeight();

            var containerLength = mainView.width();
            var containerHeight = mainView.height();
            var lines = new Array();
            var currentLine = new Array();
            var currentLineLength = 0.0;

            $.each(wordItemCollection, function (index, wordItem) {

                var wordItemWidth = wordItem.obj.innerWidth();

                if (currentLineLength + wordItemWidth + horizontalPadding > containerLength) {

                    lines.push(currentLine);
                    currentLine = new Array();
                    currentLineLength = 0.0;
                }

                currentLine.push(wordItem);
                currentLineLength = currentLineLength + horizontalPadding + wordItemWidth;

                // when over
                if (index >= wordItemCollection.length - 1) {
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

            wordItemCollection.forEach(function (wordItem, index) {
                
                wordItem.elementStateCollection.push(new ElementState(0, false, parseInt(wordItem.obj.css('left')), parseInt(wordItem.obj.css('top'))));              
            });
        };

        var getMaxMillisecondTime = function () {
            var sortedMovementCollection = movementCollection.slice().sort(sortByTime);
            sortedMovementCollection.reverse();
            var sortedDeviceAccelerationCollection = deviceAccelerationCollection.slice().sort(sortByTime);
            sortedDeviceAccelerationCollection.reverse();

            maxTime = sortedDeviceAccelerationCollection[0].time > sortedMovementCollection[0].time ? sortedDeviceAccelerationCollection[0].time : sortedMovementCollection[0].time;

            return maxTime;
        };

        var insertPointHintAbnormal = function (isBegin, x, y) {

            var presentorPointHintAbnormalClone;

            if (isBegin) {
                presentorPointHintAbnormalClone = presentorPointHintAbnormalBegin.clone();
            }
            else {
                presentorPointHintAbnormalClone = presentorPointHintAbnormalEnd.clone();
            }

            presentorPointHintAbnormalClone.css({
                left: x - 25,
                top: y - 30
            });
            mainView.append(presentorPointHintAbnormalClone);
        };

        var insertArrowBottom = function (x, y) {

            var arrowBottomClone = arrowBottom.clone();

            arrowBottomClone.css({
                left: x - 6,
                top: y - 8
            });
            mainView.append(arrowBottomClone);
        };

        var playMovementAnimation = function () {

            if (currentMillisecondTime > totalMillisecondTime) {

                return;
            }    

            // Draw point
            if (presentorPointCollectionCurrentIndex < presentorPointCollection.length) {

                while (presentorPointCollection[presentorPointCollectionCurrentIndex].time <= currentMillisecondTime) {

                    currentDrawPoint = new Point(presentorPointCollection[presentorPointCollectionCurrentIndex].x, presentorPointCollection[presentorPointCollectionCurrentIndex].y);

                    switch (presentorPointCollection[presentorPointCollectionCurrentIndex].state) {

                        case 0:
                            if (presentorPointCollection[presentorPointCollectionCurrentIndex].isAbnormal) {
                                presentorProxy.drawRect(currentDrawPoint, standardRed);

                                if (!isDrawingAbnormalNow) {
                                    isDrawingAbnormalNow = true;

                                    insertArrowBottom(presentorPointCollection[presentorPointCollectionCurrentIndex].x, presentorPointCollection[presentorPointCollectionCurrentIndex].y);
                                }
                            }
                            else {
                                presentorProxy.drawRect(currentDrawPoint, lightGrey);

                                if (isDrawingAbnormalNow) {
                                    isDrawingAbnormalNow = false;

                                    insertArrowBottom(lastDrawPoint.x, lastDrawPoint.y);
                                }
                            }

                            lastDrawPoint = currentDrawPoint;

                            break;

                        case 1:
                            if (presentorPointCollection[presentorPointCollectionCurrentIndex].isAbnormal) {
                                presentorProxy.drawRect(currentDrawPoint, standardRed);
                                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint, currentDrawPoint, standardRed);

                                if (!isDrawingAbnormalNow) {
                                    isDrawingAbnormalNow = true;

                                    insertArrowBottom(presentorPointCollection[presentorPointCollectionCurrentIndex].x, presentorPointCollection[presentorPointCollectionCurrentIndex].y);
                                }
                            }
                            else {
                                presentorProxy.drawRect(currentDrawPoint, lightGrey);
                                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint, currentDrawPoint, lightGrey);

                                if (isDrawingAbnormalNow) {
                                    isDrawingAbnormalNow = false;

                                    insertArrowBottom(lastDrawPoint.x, lastDrawPoint.y);
                                }
                            }

                            lastDrawPoint = currentDrawPoint;

                            break;

                        case 2:
                            if (presentorPointCollection[presentorPointCollectionCurrentIndex].isAbnormal) {
                                presentorProxy.drawRect(currentDrawPoint, standardRed);
                                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint, currentDrawPoint, standardRed);

                                if (!isDrawingAbnormalNow) {
                                    isDrawingAbnormalNow = true;

                                    insertArrowBottom(presentorPointCollection[presentorPointCollectionCurrentIndex].x, presentorPointCollection[presentorPointCollectionCurrentIndex].y);
                                }
                            }
                            else {
                                presentorProxy.drawRect(currentDrawPoint, lightGrey);
                                presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint, currentDrawPoint, lightGrey);

                                if (isDrawingAbnormalNow) {
                                    isDrawingAbnormalNow = false;

                                    insertArrowBottom(lastDrawPoint.x, lastDrawPoint.y);
                                }
                            }

                            lastDrawPoint = currentDrawPoint;

                            break;

                        default:
                            break;
                    }

                    presentorPointCollectionCurrentIndex++;

                    if (presentorPointCollectionCurrentIndex >= presentorPointCollection.length) {
                        break;
                    }
                }

            }

            if (rectSelectionViewStateCollectionCurrentIndex < rectSelectionViewStateCollection.length) {
                while (rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndex].time <= currentMillisecondTime) {

                    switch (rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndex].state) {

                        case 0:
                            rectSelectionView.show();
                            rectSelectionView.setFrame(rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndex].rect);

                            break;

                        case 1:
                            rectSelectionView.setFrame(rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndex].rect);

                            break;

                        case 2:
                            rectSelectionView.setFrame(rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndex].rect);
                            rectSelectionView.hide();

                            break;

                        default:
                            break;
                    }

                    rectSelectionViewStateCollectionCurrentIndex++;

                    if (rectSelectionViewStateCollectionCurrentIndex >= rectSelectionViewStateCollection.length) {
                        break;
                    }
                }
            }

            // Adjust position
            wordItemCollection.forEach(function(wordItem,index){

                var elementState = getClosestElementState(wordItem,currentMillisecondTime);

                wordItem.setPosition(elementState.x,elementState.y);

                if(elementState.isActive){
                    wordItem.toActive();
                }
                else{
                    wordItem.cancelActive();
                }

            });
            
        };

        var getClosestElementState = function (wordItem, time) {

            var selectedElementState  = wordItem.elementStateCollection[0];

            var index = 0;
            while(index < wordItem.elementStateCollection.length)
            {
                if(wordItem.elementStateCollection[index].time > time){
                    break;
                }

                if(selectedElementState.time <= wordItem.elementStateCollection[index].time)
                {
                    selectedElementState = wordItem.elementStateCollection[index];
                }
                
                index ++;
            }

            return selectedElementState;
        };

        var adjustCanvasToTime = function (time) {

            var presentorPointCollectionCurrentIndexSIM = 0;
            var currentColorIndexSIM = 0;

            var rectSelectionViewStateCollectionCurrentIndexSIM = 0;
            
            var currentMillisecondTimeSIM = 0;

            presentorProxy.clearAll();
            while(currentMillisecondTimeSIM < time)
            {
                // Draw point
                while(presentorPointCollection[presentorPointCollectionCurrentIndexSIM].time <= currentMillisecondTimeSIM){

                    currentDrawPoint = new Point(presentorPointCollection[presentorPointCollectionCurrentIndexSIM].x,presentorPointCollection[presentorPointCollectionCurrentIndexSIM].y);

                    switch(presentorPointCollection[presentorPointCollectionCurrentIndexSIM].state){

                        case 0:
                        presentorProxy.drawRect(currentDrawPoint,lightGrey);

                        lastDrawPoint = currentDrawPoint;

                        break;
                
                        case 1:
                        presentorProxy.drawRect(currentDrawPoint, lightGrey);
                        presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint,currentDrawPoint,lightGrey);

                        lastDrawPoint = currentDrawPoint;

                        break;

                        case 2:
                        presentorProxy.drawRect(currentDrawPoint,lightGrey);
                        presentorProxy.drawLineBetweenTwoPoints(lastDrawPoint,currentDrawPoint,lightGrey);

                        lastDrawPoint = currentDrawPoint;


                        break;

                        default:
                        break;
                    }
    
                    presentorPointCollectionCurrentIndexSIM ++;
                }

                
                currentMillisecondTimeSIM = currentMillisecondTimeSIM + 1000.0/animationFrequency;         
            }
            presentorPointCollectionCurrentIndex = presentorPointCollectionCurrentIndexSIM;

            // Adjust rectSelectionView
            rectSelectionView.hide();
            while (rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndexSIM].time <= currentMillisecondTimeSIM) {

                switch (rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndexSIM].state) {

                    case 0:
                        rectSelectionView.show();
                        rectSelectionView.setFrame(rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndexSIM].rect);

                        break;

                    case 1:
                        rectSelectionView.setFrame(rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndexSIM].rect);

                        break;

                    case 2:
                        rectSelectionView.setFrame(rectSelectionViewStateCollection[rectSelectionViewStateCollectionCurrentIndexSIM].rect);
                        rectSelectionView.hide();

                        break;

                    default:
                        break;
                }

                rectSelectionViewStateCollectionCurrentIndexSIM++;
            }
            rectSelectionViewStateCollectionCurrentIndex = rectSelectionViewStateCollectionCurrentIndexSIM;

            // Adjust position
            wordItemCollection.forEach(function(wordItem,index){

                var elementState = getClosestElementState(wordItem,time);

                wordItem.setPosition(elementState.x,elementState.y);

                if(elementState.isActive){
                    wordItem.toActive();
                }
                else{
                    wordItem.cancelActive();
                }

            });


        };

        // Generic
        var getClosestObjectByTime = function (objectArray, time) {

            var selectedObject  = objectArray[0];

            var index = 0;
            while(index < objectArray.length)
            {
                if(objectArray[index].time > time){
                    break;
                }

                if(selectedObject.time < objectArray[index].time)
                {
                    selectedObject = objectArray[index];
                }
                
                index ++;
            }

            return selectedObject;
        };

        var playAcceralationAnimation = function () {

            if (deviceAccelerationAnimationCurrentIndex > deviceAccelerationCollection.length - 1) {
                return;
            }

            while (deviceAccelerationCollection[deviceAccelerationAnimationCurrentIndex].time <= currentMillisecondTime) {

                accelerationX.css({
                    width: (Math.abs(deviceAccelerationCollection[deviceAccelerationAnimationCurrentIndex].x / 3.0 * 100)).toString() + "%"
                });

                accelerationY.css({
                    width: (Math.abs(deviceAccelerationCollection[deviceAccelerationAnimationCurrentIndex].y / 3.0 * 100)).toString() + "%"
                });

                accelerationZ.css({
                    width: (Math.abs(deviceAccelerationCollection[deviceAccelerationAnimationCurrentIndex].z / 3.0 * 100)).toString() + "%"
                });

                deviceAccelerationAnimationCurrentIndex++;

                if (deviceAccelerationAnimationCurrentIndex >= deviceAccelerationCollection.length) {
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

        var triggerPrograssSlider = function () {

            progressSlider.data("ionRangeSlider").update({
                from: 100 * currentMillisecondTime / totalMillisecondTime,
            });
        };

        var play = function () {

            TimerID = setInterval(playAnimation, 1000/animationFrequency);
        };

        $(function () {

            setLayout();
            getAnswerRecord();
        });

    };
})(jQuery);