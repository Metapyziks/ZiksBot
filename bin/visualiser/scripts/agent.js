var order = new Object();
order.none = 'n';
order.moveForward = 'f';
order.turnLeft  = 'l';
order.turnRight = 'r';

var direction = new Object();
direction.north = 'n';
direction.east  = 'e';
direction.south = 's';
direction.west  = 'w';

var teamColours =
[
	[ "#ff9999", "#ff6666" ],
	[ "#9999ff", "#6666ff" ],
	[ "#99ff99", "#66ff66" ],
	[ "#ffff99", "#ffff66" ],
	[ "#99ffff", "#66ffff" ],
	[ "#ff99ff", "#ff66ff" ],
	[ "#ffffff", "#ffffff" ],
	[ "#999999", "#666666" ]
];

var arrowCoords = new Object();
arrowCoords.n =
[
	[ -1, -8 ],
	[ -1, -12 ],
	[ -4, -12 ],
	[ 0, -20 ],
	[ 4, -12 ],
	[ 1, -12 ],
	[ 1, -8 ]
];
arrowCoords.s =
[
	[ -arrowCoords.n[ 0 ][ 0 ], -arrowCoords.n[ 0 ][ 1 ] ],
	[ -arrowCoords.n[ 1 ][ 0 ], -arrowCoords.n[ 1 ][ 1 ] ],
	[ -arrowCoords.n[ 2 ][ 0 ], -arrowCoords.n[ 2 ][ 1 ] ],
	[ -arrowCoords.n[ 3 ][ 0 ], -arrowCoords.n[ 3 ][ 1 ] ],
	[ -arrowCoords.n[ 4 ][ 0 ], -arrowCoords.n[ 4 ][ 1 ] ],
	[ -arrowCoords.n[ 5 ][ 0 ], -arrowCoords.n[ 5 ][ 1 ] ],
	[ -arrowCoords.n[ 6 ][ 0 ], -arrowCoords.n[ 6 ][ 1 ] ]
];
arrowCoords.e =
[
	[ -arrowCoords.n[ 0 ][ 1 ], arrowCoords.n[ 0 ][ 0 ] ],
	[ -arrowCoords.n[ 1 ][ 1 ], arrowCoords.n[ 1 ][ 0 ] ],
	[ -arrowCoords.n[ 2 ][ 1 ], arrowCoords.n[ 2 ][ 0 ] ],
	[ -arrowCoords.n[ 3 ][ 1 ], arrowCoords.n[ 3 ][ 0 ] ],
	[ -arrowCoords.n[ 4 ][ 1 ], arrowCoords.n[ 4 ][ 0 ] ],
	[ -arrowCoords.n[ 5 ][ 1 ], arrowCoords.n[ 5 ][ 0 ] ],
	[ -arrowCoords.n[ 6 ][ 1 ], arrowCoords.n[ 6 ][ 0 ] ]
];
arrowCoords.w =
[
	[ arrowCoords.n[ 0 ][ 1 ], -arrowCoords.n[ 0 ][ 0 ] ],
	[ arrowCoords.n[ 1 ][ 1 ], -arrowCoords.n[ 1 ][ 0 ] ],
	[ arrowCoords.n[ 2 ][ 1 ], -arrowCoords.n[ 2 ][ 0 ] ],
	[ arrowCoords.n[ 3 ][ 1 ], -arrowCoords.n[ 3 ][ 0 ] ],
	[ arrowCoords.n[ 4 ][ 1 ], -arrowCoords.n[ 4 ][ 0 ] ],
	[ arrowCoords.n[ 5 ][ 1 ], -arrowCoords.n[ 5 ][ 0 ] ],
	[ arrowCoords.n[ 6 ][ 1 ], -arrowCoords.n[ 6 ][ 0 ] ]
];

function Agent( id, team, turn, x, y, dir )
{
	this.id = id;
	this.team = team;

	this.spawnTurn = turn;
	this.deathTurn = -1;
	
	this.positions = new Array();
	this.directions = new Array();
	this.moves = new Array();
	
	this.addMove = function( turn, move )
	{
		while( this.moves.length < turn - this.spawnTurn )
			this.moves.push( order.none );
		
		this.moves.push( move );
	}
	
	this.die = function( turn )
	{
		this.deathTurn = turn;
	}
	
	this.isVisible = function( turn )
	{
		return turn >= this.spawnTurn && ( this.deathTurn == -1 || turn <= this.deathTurn );
	}
	
	this.getPosition = function( turn )
	{
		return this.positions[ turn - this.spawnTurn ];
	}
	
	this.render = function( context, turn, sX, sY, vX, vY )
	{
		var pos = this.positions[ turn - this.spawnTurn ];
		var dir = this.directions[ turn - this.spawnTurn ];
		
		var transX = sX + ( pos.x - vX + 0.5 ) * 16;
		var transY = sY + ( pos.y - vY + 0.5 ) * 16;
		
		context.translate( transX, transY );
		
		if( turn == this.deathTurn )
			context.globalAlpha = 0.5;
		
		context.fillStyle   = teamColours[ this.team ][ 0 ];
		context.strokeStyle = teamColours[ this.team ][ 1 ];
		context.lineWidth 	= 2;
		context.beginPath();
		context.arc( 0, 0, 7, 0, Math.PI * 2, true );
		context.closePath();
		context.fill();
		context.stroke();
		
		var coords = arrowCoords[ dir ];
		
		context.fillStyle   = teamColours[ this.team ][ 1 ];
		context.beginPath();
		context.moveTo( coords[ 0 ][ 0 ], coords[ 0 ][ 1 ] );
		
		for( var i = 1; i < coords.length; ++i )
			context.lineTo( coords[ i ][ 0 ], coords[ i ][ 1 ] );
			
		context.closePath();
		context.fill();
		context.translate( -transX, -transY );
		context.globalAlpha = 1.0;
	}
}
