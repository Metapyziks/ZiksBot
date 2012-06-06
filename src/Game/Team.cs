using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Game
{
    class Team
    {
        public readonly String ExePath;

        private Process myProcess;

        public bool ProgramRunning
        {
            get { return myProcess != null && !myProcess.HasExited; }
        }

        public bool HasError
        {
            get { return !myProcess.StandardError.EndOfStream; }
        }

        public Team( String exePath )
        {
            ExePath = exePath;
        }

        public void StartProgram()
        {
            if ( File.Exists( ExePath ) )
            {
                ProcessStartInfo info = new ProcessStartInfo( ExePath );
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.WorkingDirectory = Directory.GetCurrentDirectory();

                myProcess = Process.Start( info );
            }
            else
            {
                throw new FileNotFoundException( "Executable not found at " + ExePath );
            }
        }

        public String ReadLine()
        {
            return myProcess.StandardOutput.ReadLine();
        }

        public void WriteLine( params object[] values )
        {
            for ( int i = 0; i < values.Length; ++i )
            {
                String val = values[ i ].ToString() + ( i < values.Length - 1 ? " " : "" );
                myProcess.StandardInput.Write( val );
            }
            myProcess.StandardInput.WriteLine();
        }

        public void StopProgram()
        {
            if ( ProgramRunning )
                myProcess.Kill();
        }
    }
}
