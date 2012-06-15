using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ViewGame
{
    class Program
    {
        static readonly string stHtml = @"
<!DOCTYPE HTML>
<html>
	<head>
		<title>AI Challenge - Game log visualiser</title>
		<style type=""text/css"">
			html, body
			{
				margin: 0px;
				width: 	100%;
				height: 100%;
			}
			#canvas
			{
				height: 100%;
				width: 100%;
				display:block;
			}
		</style>
		<script type=""text/javascript"" >
			var gameLog = ""$SOURCE$"";
            var srcPath = ""$VPATH$"";
		</script>
		<script type=""text/javascript"" src=""$VPATH$/scripts/tools.js"" ></script>
		<script type=""text/javascript"" src=""$VPATH$/scripts/agent.js"" ></script>
		<script type=""text/javascript"" src=""$VPATH$/scripts/gamestate.js"" ></script>
		<script type=""text/javascript"" src=""$VPATH$/scripts/visualiser.js"" ></script>
		<script type=""text/javascript"" src=""$VPATH$/scripts/main.js"" ></script>
	</head>
	<body onresize=""onResizeCanvas()"">
		<canvas id=""canvas"">
			Sorry! This app requires a browser that supports HTML5 Canvas.
		</canvas>
	</body>
</html>
";

        static void Main( string[] args )
        {
            if ( args.Length == 0 )
            {
                Console.WriteLine( "Expected a path to a game .log file.\nPress any key to close..." );
                Console.ReadKey();
                return;
            }

            String source = File.ReadAllText( args[ 0 ] );
            String vpath = Path.GetFullPath( "visualiser" ).Replace( '\\', '/' );

            String html = stHtml;
            html = html.Replace( "$SOURCE$", source );
            html = html.Replace( "$VPATH$", vpath );

            String outPath = args[ 0 ] + ".html";

            if ( args.Length > 1 )
                outPath = args[ 1 ];

            File.WriteAllText( outPath, html );

            Process.Start( outPath );
        }
    }
}
