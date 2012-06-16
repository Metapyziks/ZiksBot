function Button( image, x, y, action )
{
	this.image = image;
	this.x = x;
	this.y = y;
	this.action = action;
	
	this.highlight = false;
	
	this.isIntersecting = function( sX, sY )
	{
		return sX >= this.x && sY >= this.y &&
			sX < this.x + this.image.width &&
			sY < this.y + this.image.height;
	}
	
	this.render = function( context, sX, sY )
	{
		context.drawImage( this.image, this.x + sX, this.y + sY );
		
		if( !this.highlight )
		{
			context.fillStyle = "#000000";
			context.globalAlpha = 0.33;
			context.fillRect( this.x + sX, this.y + sY, this.image.width, this.image.height );
			context.globalAlpha = 1.0;
		}
	}
}

function Controls()
{
	var myPlaying = false;
	var myPlayID = -1;
	
	var pause = function()
	{
		if( myPlaying )
		{
			myPlaying = false;
			clearInterval( myPlayID );
		}
	}

	this.btnPrev = new Button( images.load( "btnprev.png" ), 0, 24, function()
	{
		pause();
		gameState.prevTurn();
	} );
	this.btnPlay = new Button( images.load( "btnplay.png" ), 48, 24, function()
	{
		if( !myPlaying )
		{
			myPlaying = true;
			myPlayID = setInterval( function()
			{
				if( myPlaying )
					gameState.nextTurn();
			}, 1000.0 / 8.0 );
		}
	} );
	this.btnPause = new Button( images.load( "btnpause.png" ), 96, 24, function()
	{
		pause();
	} );
	this.btnStop = new Button( images.load( "btnstop.png" ), 144, 24, function()
	{
		pause();
		gameState.turn = 0;
	} );
	this.btnNext = new Button( images.load( "btnnext.png" ), 192, 24, function()
	{
		pause();
		gameState.nextTurn();
	} );

	this.buttons =
	[
		this.btnPrev, this.btnPlay, this.btnPause, this.btnStop, this.btnNext
	];
	
	this.mouseMove = function( x, y )
	{
		for( var i = 0; i < this.buttons.length; ++i )
		{
			var btn = this.buttons[ i ];
			btn.highlight = btn.isIntersecting( x, y );
		}
	}

	this.click = function( x, y )
	{
		for( var i = 0; i < this.buttons.length; ++i )
		{
			var btn = this.buttons[ i ];
			if( btn.isIntersecting( x, y ) )
			{
				btn.action();
				break;
			}
		}
	}
	
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
			context.fillRect( x + i * sMul, y, space * sMul, 16 );
		}
		context.globalAlpha = 1.0;
		
		for( var i = 0; i < this.buttons.length; ++i )
			this.buttons[ i ].render( context, x, y );
	}
}
