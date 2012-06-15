function Controls()
{
	this.render = function( context, x, y, width, height )
	{
		context.fillStyle = "#aed771";
		context.fillRect( x, y, width, 16 );
		
		var ratio = gameState.turnCount <= 1 ? 1 : gameState.turn / ( gameState.turnCount - 1 );
		
		context.fillStyle = "#78a930";
		context.fillRect( x, y, width * ratio, 16 );
		
		var sMul = width / ( gameState.turnCount - 1 );
		var space = 1;
		while( space * sMul < 4 )
			space *= 2;
			
		context.fillStyle = "#000000";
		context.globalAlpha = 0.1;
		for( var i = 0; i < gameState.turnCount; i += 2 * space )
		{
			context.fillRect( x + i * space * sMul, y, space * sMul, 16 );
		}
		context.globalAlpha = 1.0;
	}
}
