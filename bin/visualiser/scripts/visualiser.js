function Visualiser()
{
	var myDragging = false;
	
	var myDragX = 0;
	var myDragY = 0;
	
	var m = 8;
	
	this.canvas = null;
	this.context = null;

	this.mouseX = 0;
	this.mouseY = 0;
	
	this.mapX = 0;
	this.mapY = 0;
	
	this.viewX = 0;
	this.viewY = 0;
	
	this.controlY = 0;
	
	this.viewWidth = 512;
	this.viewHeight = 384;
	
	this.controls = new Controls();
	
	this.initialize = function()
	{
		this.canvas = document.getElementById( "canvas" );
		this.context = this.canvas.getContext( "2d" );
		
		gameState.parse( gameLog );
		
		this.mapX = gameState.width / 2.0;
		this.mapY = gameState.height / 2.0;
		
		this.onResizeCanvas();
	}

	this.mouseMove = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
		
		if( myDragging )
		{
			this.mapX += ( myDragX - x ) / 16;
			this.mapY += ( myDragY - y ) / 16;
		
			myDragX = x;
			myDragY = y;
		}
		else
			this.controls.mouseMove( x - this.viewX, y - this.controlY );
	}

	this.mouseDown = function( x, y )
	{	
		this.mouseX = x;
		this.mouseY = y;
		
		myDragging |= (
			x >= this.viewX &&
			y >= this.viewY &&
			x < this.viewX + this.viewWidth &&
			y < this.viewY + this.viewHeight
		);
		
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
		
		if( !myDragging )
			this.controls.click( x - this.viewX, y - this.controlY );
	}

	this.keyDown = function( e )
	{
		
	}

	this.keyUp = function( e )
	{
	
	}

	this.onResizeCanvas = function()
	{
		this.canvas.width = window.innerWidth;
		this.canvas.height = window.innerHeight;
		
		this.viewX = Math.round( 0.5 * ( this.canvas.width - this.viewWidth ) );
		this.viewY = Math.round( 0.5 * ( this.canvas.height - ( this.viewHeight + m + 64 ) ) );
		
		this.controlY = this.viewY + m + this.viewHeight;
	}

	this.render = function()
	{		
		this.context.fillStyle = "#141414";
		this.context.fillRect( 0, 0, this.canvas.width, this.canvas.height );
	
		this.context.fillStyle = "#000000";
		this.context.fillRect( this.viewX - m, this.viewY - m, this.viewWidth + m * 2, this.viewHeight + 64 + m * 3 );
		
		gameState.render( this.context, this.viewX, this.viewY,
			this.viewWidth, this.viewHeight, this.mapX, this.mapY );
		
		this.controls.render( this.context, this.viewX, this.controlY, this.viewWidth, 64 );
	}

	this.run = function()
	{
		setInterval( this.mainLoop, 1000.0 / 60.0, this );
	}

	this.mainLoop = function( self )
	{
		if( !myDragging )
		{
			self.mapX += ( Math.round( self.mapX ) - self.mapX ) * 0.25;
			self.mapY += ( Math.round( self.mapY ) - self.mapY ) * 0.25;
		}
	
		self.render();
	}
}
