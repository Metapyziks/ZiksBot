function Visualiser()
{
	images.load( "tile.png" );
	images.load( "wall.png" );

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

	this.gameState = new GameState();
	
	this.initialize = function()
	{
		this.canvas = document.getElementById( "canvas" );
		this.context = this.canvas.getContext( "2d" );
		
		this.gameState.parse( gameLog );
		
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
		this.context.fillStyle = "#141414";
		this.context.fillRect( 0, 0, this.canvas.width, this.canvas.height );
		
		this.gameState.render( this.context,
			Math.round( 0.5 * ( this.canvas.width - this.viewWidth ) ),
			Math.round( 0.5 * ( this.canvas.height - this.viewHeight ) ),
			this.viewWidth, this.viewHeight, this.viewX, this.viewY
		);
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
