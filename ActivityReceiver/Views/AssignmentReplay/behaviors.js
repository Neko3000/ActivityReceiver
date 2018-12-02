(function ($) {

    $.fn.replayer = function () {
        var $this = $(this);
        var mainView = $this.find('.main-view');

        var wordItems = new Array();
        var movements = [
            {x:100,y:200},
            {x:100,y:210},
            {x:100,y:220},
            {x:100,y:230},
            {x:100,y:240},
        ];

        var counter = 0;

        var generateWordItems = function(){
            var wordItemTemplate = $('<div class="word-item">here we are</div>');
            var division = "here|is|my|greate|wallpaper";
            var splittedDivision = division.split('|');

            $.each(splittedDivision,function(index,word){
                var wordItem = wordItemTemplate.clone();
                wordItem.text(word);

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
            var point = $('<div class="point"></div>');

            point.css({
                left:movements[counter].x,
                top:movements[counter].y
            });

            mainView.append(point);

            counter ++;
        };

        var play = function(){
            setInterval(playAnimation,1000);
        };

        $(function () {
            //alert("init");

            generateWordItems();
            arrangeWordItems();

            play();
        });

    }
})(jQuery);