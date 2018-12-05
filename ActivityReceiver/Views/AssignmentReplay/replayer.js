(function ($) {

    $.fn.replayer = function () {
        var $this = $(this);
        var mainView = $this.find('.main-view');

        var division;
        var movementDTOs;
        var wordItems = new Array();

        var currentActiveWordItem;
        var pointerBeganPositionXInWordItem;
        var pointerBeganPositionYInWordItem;

        var currentMovementDTOIndex = 0;
        var currentMillisecondTime = 0;

        var setLayout = function(){

            // get-data btn
            $this.find('.get-data').click(function(){
                getAnswer();
            });

            $this.find('.start-play').click(function(){
                play();
            });

        }

        var getAnswer = function(){
            var id = 6;
            $.ajax(
            {
                url: "http://118.25.44.137/AnswerReplay/GetAnswer?id=" +id.toString(),
                type: "get",
                dataType: "json",
                async: false,
                success: function (answer) {

                    division = answer.division;
                    movementDTOs = answer.movementDTOs;

                    generateWordItems();
                    arrangeWordItems();
                }
            }
        );
        }

        var generateWordItems = function(){
            var wordItemTemplate = $('<div class="word-item"><div class="word-item-background">here we are</div></div>');

            var splittedDivision = division.split('|');

            $.each(splittedDivision,function(index,word){
                var wordItem = wordItemTemplate.clone();
                wordItem.find('.word-item-background').first().text(word);

                wordItems.push(wordItem);
                wordItem.appendTo(mainView);
            });

        };

        var arrangeWordItems = function(){
            var horizontalPadding = 10.0;
            var verticalPadding = 10.0;

            // 26
            var wordItemHeight = wordItems[0].innerHeight();

            var containerLength = mainView.width();
            var containerHeight = mainView.height();
            var lines = new Array();
            var currentLine = new Array();
            var currentLineLength = 0.0;

            $.each(wordItems,function(index,wordItem){

                var wordItemWidth = wordItem.innerWidth();

                if(currentLineLength + wordItemWidth + horizontalPadding > containerLength){

                    lines.push(currentLine);
                    currentLine = new Array();
                    currentLineLength = 0.0;
                }

                currentLine.push(wordItem);
                currentLineLength = currentLineLength + horizontalPadding + wordItemWidth;

                // when over
                if(index>=wordItems.length - 1){
                    lines.push(currentLine);
                    currentLine = new Array();
                    currentLineLength = 0.0;
                }

                
            });

            for(i=0;i<=lines.length-1;i++){
                var currentXPosition = 0.0;

                for(j=0;j<=lines[i].length-1;j++){

                    lines[i][j].css({
                        left:currentXPosition + horizontalPadding,
                        top:containerHeight - (lines.length - i) * (wordItemHeight + verticalPadding)
                    });

                    currentXPosition = currentXPosition + horizontalPadding + lines[i][j].innerWidth();
                }
            }

        };

        var playAnimation = function(){

            currentMillisecondTime += 100;

            if(currentMovementDTOIndex >= movementDTOs.length){

                clearInterval();
                return;
            }

            while(movementDTOs[currentMovementDTOIndex].time<=currentMillisecondTime)
            {
                var currentMovementDTO = movementDTOs[currentMovementDTOIndex];

                
                if(currentMovementDTO.state == 0){
                    // tap

                    currentActiveWordItem = wordItems[currentMovementDTO.targetElement];
                    
                    pointerBeganPositionXInWordItem = currentMovementDTO.xPosition - parseInt(currentActiveWordItem.css('left'));
                    pointerBeganPositionYInWordItem = currentMovementDTO.yPosition - parseInt(currentActiveWordItem.css('top'));        
                
                }
                else if(currentMovementDTO.state == 1){
                    // move

                    currentActiveWordItem.css({
                        left:currentMovementDTO.xPosition - pointerBeganPositionXInWordItem,
                        top:currentMovementDTO.yPosition - pointerBeganPositionYInWordItem
                    });

                }
                else if(currentMovementDTO.state == 2){
                    // end

                    currentActiveWordItem = null;
                }

                var point = $('<div class="point"></div>');



                point.css({
                    left:currentMovementDTO.xPosition,
                    top:currentMovementDTO.yPosition
                });
    
                mainView.append(point);
    
                currentMovementDTOIndex ++;
            }

        };

        var play = function(){

            setInterval(playAnimation,100);
        };

        $(function () {
            //alert("init");

            //generateWordItems();
            //arrangeWordItems();

            setLayout();
        });

    }
})(jQuery);