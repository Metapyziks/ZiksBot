function Visualiser()
{
	this.canvas = null;
	this.context = null;

	images.load( "tile.png" );
	images.load( "wall.png" );

	this.mouseX = 0;
	this.mouseY = 0;
	
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
	}

	this.mouseDown = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
	}

	this.mouseUp = function( x, y )
	{
		this.mouseX = x;
		this.mouseY = y;
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
			0.5 * ( this.canvas.width - this.viewWidth ),
			0.5 * ( this.canvas.height - this.viewHeight ),
			this.viewWidth, this.viewHeight, 8, 8
		);
	}

	this.run = function()
	{
		setInterval( this.mainLoop, 1000.0 / 60.0, this );
	}

	this.mainLoop = function( self )
	{
		self.render();
	}
}
