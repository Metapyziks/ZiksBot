Array.prototype.contains = function( obj )
{
    var i = this.length;
    while ( --i >= 0 )
        if ( this[ i ] === obj )
            return true;

    return false;
}

Array.prototype.containsEqual = function( obj )
{
    var i = this.length;
    while ( --i >= 0 )
        if ( this[ i ].equals( obj ) )
            return true;

    return false;
}

var images = new Object();
images.myToLoadCount = 0;
images.onAllLoaded = function(){}

images.onImageLoad = function()
{	
	--images.myToLoadCount;

	if( images.myToLoadCount == 0 )
		images.onAllLoaded();
}

images.load = function( path )
{
	++images.myToLoadCount;
	var img = new Image();
	img.src = srcPath + "images/" + path;

	img.onload = images.onImageLoad;

	images[ path.substring( 0, path.length - 4 ) ] = img;
	return img;
}

images.allImagesLoaded = function()
{
	return ( images.myToLoadCount == 0 );
}

function Pos( x, y )
{
	this.x = x;
	this.y = y;

	this.add = function( pos )
	{
		return new Pos( this.x + pos.x, this.y + pos.y );
	}

	this.sub = function( pos )
	{
		return new Pos( this.x - pos.x, this.y - pos.y );
	}

	this.equals = function( pos )
	{
		return this.x == pos.x && this.y == pos.y;
	}

	this.toString = function()
	{
		return "(" + this.x + "," + this.y + ")";
	}
}
