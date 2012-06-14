function Visualiser()
{
	this.canvas = null;
	this.context = null;

	this.mouseX = 0;
	this.mouseY = 0;

	this.initialize = function()
	{
		this.canvas = document.getElementById( "canvas" );
		this.context = this.canvas.getContext( "2d" );
		
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
