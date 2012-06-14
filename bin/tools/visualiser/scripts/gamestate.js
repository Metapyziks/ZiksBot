var tile = new Object();
tile.empty = 0;
tile.wall  = 1;

function GameState()
{
	var PSNone = 0;
	var PSMap  = 1;
	var PSTurn = 2;
	
	var myMapImage = null;
	var myMapPattern = null;

	this.width  = 0;
	this.height = 0;
	
	this.tiles = null;
	
	this.parse = function( text )
	{
		var lines = text.split( ";" );
		var parseState = PSNone;
		
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
							this.tiles = new Array();
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
					
					break;
			}
		}
		
		myMapImage = document.createElement( "canvas" );
		myMapImage.width = this.width * 16;
		myMapImage.height = this.height * 16;

		var context = myMapImage.getContext( "2d" );
		
		for( var row = 0; row < this.height; ++row )
			for( var col = 0; col < this.width; ++col )
				if( this.tiles[ col ][ row ] == tile.wall )
					context.drawImage( images.wall, col * 16, row * 16 );
		
		var border = 2;
		var border2 = 2 * border;
		
		context.fillStyle = "#333333";
		for( var row = 0; row < this.height; ++row )
		{
			for( var col = 0; col < this.width; ++col )
			{
				if( this.tiles[ col ][ row ] != tile.wall )
				{
					context.fillRect( col * 16 - border, row * 16 - border, 16 + border2, 16 + border2 );
					
					if( col == this.width - 1 )
						context.fillRect( -16 - border, row * 16 - border, 16 + border2, 16 + border2 );
						
					if( row == this.height - 1 )
						context.fillRect( col * 16 - border, -16 - border, 16 + border2, 16 + border2 );
				}
			}
		}
		
		for( var row = 0; row < this.height; ++row )
			for( var col = 0; col < this.width; ++col )
				if( this.tiles[ col ][ row ] != tile.wall )
					context.drawImage( images.tile, col * 16, row * 16 );
	}
	
	this.render = function( context, left, top, width, height, vX, vY )
	{
		if( myMapPattern == null )
			myMapPattern = context.createPattern( myMapImage, "repeat" )
		
		var rectX = vX * 16 - 0.5 * width;
		var rectY = vY * 16 - 0.5 * height;
		
		if( rectX < 0 )
			rectX += this.width * 16;
			
		if( rectY < 0 )
			rectY += this.height * 16;
		
		var transX = left - rectX;
		var transY = top  - rectY;
		
		context.fillStyle = myMapPattern;
		context.translate( transX, transY );
		context.fillRect( rectX, rectY, width, height );
		context.translate( -transX, -transY );
		
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
