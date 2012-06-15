function Visualiser()
{
	var myDragging = false;
	
	var myDragX = 0;
	var myDragY = 0;
	
	this.canvas = null;
	this.context = null;

	this.mouseX = 0;
	this.mouseY = 0;
	
	this.viewX = 8;
	this.viewY = 8;
	
	this.viewWidth = 512;
	this.viewHeight = 384;
	
	this.controls = new Controls();
	
	this.initialize = function()
	{
		this.canvas = document.getElementById( "canvas" );
		this.context = this.canvas.getContext( "2d" );
		
		gameState.parse( gameLog );
		
		this.onResizeCanvas();
	}

	this.mouseMove = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
		
		if( myDragging )
		{
			this.viewX += ( myDragX - x ) / 16;
			this.viewY += ( myDragY - y ) / 16;
		
			myDragX = x;
			myDragY = y;
		}
	}

	this.mouseDown = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
		
		myDragging = true;
		
		myDragX = x;
		myDragY = y;
	}

	this.mouseUp = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
		
		myDragging = false;
	}

	this.click = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
	}

	this.keyDown = function( e )
	{
		++gameState.turn;
		if( gameState.turn == gameState.turnCount )
			gameState.turn = 0;
	}

	this.keyUp = function( e )
	{
	
	}

	this.onResizeCanvas = function()
	{
		this.canvas.width = window.innerWidth;
		this.canvas.height = window.innerHeight;
	}

	this.render = function()
	{
		var m = 8;
	
		var viewX = Math.round( 0.5 * ( this.canvas.width - this.viewWidth ) );
		var viewY = Math.round( 0.5 * ( this.canvas.height - ( this.viewHeight + m + 64 ) ) );
		
		this.context.fillStyle = "#141414";
		this.context.fillRect( 0, 0, this.canvas.width, this.canvas.height );
	
		this.context.fillStyle = "#000000";
		this.context.fillRect( viewX - m, viewY - m, this.viewWidth + m * 2, this.viewHeight + 64 + m * 3 );
		
		gameState.render( this.context, viewX, viewY,
			this.viewWidth, this.viewHeight, this.viewX, this.viewY
		);
		
		this.controls.render( this.context, viewX, viewY + m + this.viewHeight, this.viewWidth, 64 );
	}

	this.run = function()
	{
		setInterval( this.mainLoop, 1000.0 / 60.0, this );
	}

	this.mainLoop = function( self )
	{
		if( !myDragging )
		{
			self.viewX += ( Math.round( self.viewX ) - self.viewX ) * 0.25;
			self.viewY += ( Math.round( self.viewY ) - self.viewY ) * 0.25;
		}
	
		self.render();
	}
}
