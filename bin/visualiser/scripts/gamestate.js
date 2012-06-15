var tile = new Object();
tile.empty = 0;
tile.wall  = 1;

function GameState()
{
	images.load( "tile.png" );
	images.load( "wall.png" );
	
	images.load( "package.png" );
	
	images.load( "shadtl.png" );
	images.load( "shadct.png" );
	images.load( "shadt.png" );
	images.load( "shadcl.png" );
	images.load( "shadl.png" );
	images.load( "shadc.png" );
	
	var PSNone = 0;
	var PSMap  = 1;
	var PSTurn = 2;
	
	var myMapImage = null;
	var myMapPattern = null;

	this.width  = 0;
	this.height = 0;
	
	this.turn = 0;
	this.turnCount = 0;
	
	this.tiles = new Array();
	this.agents = new Array();
	this.packages = new Array();
	
	this.getTile = function( x, y )
	{
		x -= Math.floor( x / this.width ) * this.width;
		y -= Math.floor( y / this.height ) * this.height;
		
		return this.tiles[ y ][ x ];
	}
	
	this.parse = function( text )
	{
		var lines = text.split( ";" );
		var parseState = PSNone;
		var curTurn = 0;
		var self = this;
	
		var turnStart = function( turn )
		{
			parseState = PSTurn;
			curTurn = turn;
			self.turnCount = Math.max( curTurn + 1, self.turnCount );
			self.packages.push( new Array() );
		}
		
		for( var i = 0; i < lines.length; ++i )
		{
			var line = lines[ i ];
			
			if( line.length == 0 )
				continue;
			
			switch( parseState )
			{
				case PSNone:
					if( line.charAt( 0 ) == '#' )
						break;
				
					var args = line.split( " " );
					switch( args[ 0 ] )
					{
						case "width":
							this.width = parseInt( args[ 1 ] );
							break;
						case "height":
							this.height = parseInt( args[ 1 ] );
							break;
						case "map":
							parseState = PSMap;
							break;
						case "turn":
							turnStart( parseInt( args[ 1 ] ) );
							break;
					}
					break;
				case PSMap:				
					if( line === "end" )
					{
						parseState = PSNone;
						break;
					}
				
					var row = new Array();
					for( var j = 0; j < this.width; ++j )
					{
						if( line.charAt( j << 1 ) == '#' )
							row.push( tile.wall );
						else
							row.push( tile.empty );
					}
					this.tiles.push( row );
					break;
				case PSTurn:
					if( line.charAt( 0 ) == '#' )
						break;
				
					var args = line.split( " " );
					switch( args[ 0 ] )
					{
						case "turn":
							turnStart( parseInt( args[ 1 ] ) );
							break;
						case "a":
						case "d":
							var id = parseInt( args[ 1 ] );
							var team = parseInt( args[ 2 ] );
							var x = parseInt( args[ 3 ] );
							var y = parseInt( args[ 4 ] );
							var dir = args[ 5 ];
							
							while( id >= this.agents.length )
								this.agents.push( null );
							
							if( this.agents[ id ] == null )
								this.agents[ id ] = new Agent( id, team, curTurn, x, y );
							
							this.agents[ id ].positions.push( new Pos( x, y ) );
							this.agents[ id ].directions.push( dir );

							if( args[ 0 ] == "d" )
								this.agents[ id ].die( curTurn );
							break;
						case "p":
							var x = parseInt( args[ 1 ] );
							var y = parseInt( args[ 2 ] );
							this.packages[ curTurn ].push( new Pos( x, y ) );
							break;
						case "o":
							var id = parseInt( args[ 1 ] );
							var team = parseInt( args[ 2 ] );
							var o = args[ 3 ];
							this.agents[ id ].addMove( curTurn, o );
							break;
					}
					break;
			}
		}
		
		myMapImage = document.createElement( "canvas" );
		myMapImage.width = this.width * 16;
		myMapImage.height = this.height * 16;

		var context = myMapImage.getContext( "2d" );
		
		for( var row = 0; row < this.height; ++row )
			for( var col = 0; col < this.width; ++col )
				if( this.tiles[ row ][ col ] == tile.wall )
					context.drawImage( images.wall, col * 16, row * 16 );
		
		var border = 2;
		var border2 = 2 * border;
		
		context.fillStyle = "#333333";
		for( var row = 0; row < this.height; ++row )
		{
			for( var col = 0; col < this.width; ++col )
			{
				if( this.tiles[ row ][ col ] != tile.wall )
				{
					context.fillRect( col * 16 - border, row * 16 - border, 16 + border2, 16 + border2 );
					
					if( col == 0 )
						context.fillRect( this.width * 16 - border, row * 16 - border, 16 + border2, 16 + border2 );
						
					if( col == this.width - 1 )
						context.fillRect( -16 - border, row * 16 - border, 16 + border2, 16 + border2 );
						
					if( row == 0 )
						context.fillRect( col * 16 - border, this.height * 16 - border, 16 + border2, 16 + border2 );
						
					if( row == this.height - 1 )
						context.fillRect( col * 16 - border, -16 - border, 16 + border2, 16 + border2 );
				}
			}
		}
		
		for( var row = 0; row < this.height; ++row )
		{
			for( var col = 0; col < this.width; ++col )
			{
				if( this.tiles[ row ][ col ] != tile.wall )
				{
					context.drawImage( images.tile, col * 16, row * 16 );
					
					var c = this.getTile( col - 1, row - 1 ) == tile.wall;
					var l = this.getTile( col - 1, row ) == tile.wall;
					var t = this.getTile( col, row - 1 ) == tile.wall;
					
					if( t && l )
						context.drawImage( images.shadtl, col * 16, row * 16 );
					else if( c && t )
						context.drawImage( images.shadct, col * 16, row * 16 );
					else if( c && l )
						context.drawImage( images.shadcl, col * 16, row * 16 );
					else if( c )
						context.drawImage( images.shadc, col * 16, row * 16 );
					else if( t )
						context.drawImage( images.shadt, col * 16, row * 16 );
					else if( l )
						context.drawImage( images.shadl, col * 16, row * 16 );
				}
			}
		}
	}
	
	this.render = function( context, left, top, width, height, vX, vY )
	{
		vX = Math.round( vX * 8 ) / 8;
		vY = Math.round( vY * 8 ) / 8;
	
		if( myMapPattern == null )
			myMapPattern = context.createPattern( myMapImage, "repeat" );
		
		var rectX = vX * 16 - 0.5 * width;
		var rectY = vY * 16 - 0.5 * height;
		
		// if( rectX < 0 )
		//     rectX += this.width * 16;
			
		// if( rectY < 0 )
		// 	   rectY += this.height * 16;
		
		var transX = left - rectX;
		var transY = top  - rectY;
		
		context.fillStyle = myMapPattern;
		context.translate( transX, transY );
		context.fillRect( rectX, rectY, width, height );
		context.translate( -transX, -transY );
		
		var midX = left + 0.5 * width;
		var midY = top + 0.5 * height;
		
		for( var i = 0; i < this.agents.length; ++i )
			this.agents[ i ].render( context, this.turn, midX, midY, vX, vY );
			
		for( var i = 0; i < this.packages[ this.turn ].length; ++i )
		{
			var pos = this.packages[ this.turn ][ i ];
			var transX = midX + ( pos.x - vX ) * 16;
			var transY = midY + ( pos.y - vY ) * 16;
			context.drawImage( images.package, transX, transY );
		}
		
		var innerLeft = 0.5 * ( width - this.width * 16 );
		var innerTop = 0.5 * ( height - this.height * 16 );
		var innerRight = innerLeft + this.width * 16;
		var innerBottom = innerTop + this.height * 16;
		
		context.fillStyle = "#000000";
		context.globalAlpha = 0.5;
		context.fillRect( left, top, width, innerTop );
		context.fillRect( left, top + innerTop, innerLeft, innerBottom - innerTop );
		context.fillRect( left + innerRight, top + innerTop, width - innerRight, innerBottom - innerTop );
		context.fillRect( left, top + innerBottom, width, height - innerBottom );
		context.globalAlpha = 1.0;
	}
}
