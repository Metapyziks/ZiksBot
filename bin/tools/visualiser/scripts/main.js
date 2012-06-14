visualiser = new Visualiser();

window.onload = function( e )
{
	if( !images.allImagesLoaded() )
	{
		images.onAllLoaded = function()
		{
			visualiser.initialize();
			visualiser.run();
		}
	}
	else
	{
		visualiser.initialize();
		visualiser.run();
	}
}

window.onmousemove = function( e )
{
	if( e )
		visualiser.mouseMove( e.pageX, e.pageY );
	else
		visualiser.mouseMove( window.event.clientX, window.event.clientY );
}

window.onmousedown = function( e )
{
	if( e )
		visualiser.mouseDown( e.pageX, e.pageY );
	else
		visualiser.mouseDown( window.event.clientX, window.event.clientY );
}

window.onmouseup = function( e )
{
	if( e )
		visualiser.mouseUp( e.pageX, e.pageY );
	else
		visualiser.mouseUp( window.event.clientX, window.event.clientY );
}

window.onclick = function( e )
{
	if( e )
		visualiser.click( e.pageX, e.pageY );
	else
		visualiser.click( window.event.clientX, window.event.clientY );
}

window.onkeydown = function( e )
{
	visualiser.keyDown( e );
}

window.onkeyup = function( e )
{
	visualiser.keyUp( e );
}

function onResizeCanvas()
{
	visualiser.onResizeCanvas();
}
