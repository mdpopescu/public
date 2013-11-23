function Game(gameId, canvasWidth, canvasHeight) {
    // private fields
    var canvas = $("#hexCanvas")[0];
    var ctx = canvas.getContext('2d');
    var grid = null;
    var game = null;

    // private methods
    function init() {
        // magic constants - I like the way the result looks with them
        const f1 = 400 / 70;
        const f2 = 320 / 60;
        const f3 = 320 / 30;

        setWHS(canvasWidth / f1, canvasHeight / f2, canvasHeight / f3);
        grid = drawHexGrid();

        $('#hexCanvas').on('click', function (e) {
            var posX = $(this).position().left;
            var posY = $(this).position().top;
            var p = new HT.Point(e.pageX - posX, e.pageY - posY);
            var hex = grid.GetHexAt(p);

            game.server.select(hex.Id);
        });
    }

    function connect(gameId) {
        game = $.connection.gameHub;

        game.client.select = select;
        game.client.selectMany = selectMany;

        $.connection.hub.start().done(function () {
            game.server.init(gameId);
        });
    }

    function select(id) {
        var hex = grid.GetHexById(id);
        hex.selected = !hex.selected;
        hex.draw(ctx);
    }

    function selectMany(ids) {
        for (var i = 0; i < ids.length; i++)
            select(ids[i]);
    }

    // public methods
    this.Start = function () {
        init();
        connect(gameId);
    }
}