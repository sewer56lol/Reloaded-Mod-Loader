﻿/*
    [Reloaded] Mod Loader Mini-Wrapper Utility
    Loads the Any CPU Compiled Mod Loader, Loader executable in x64 mode
    by compiling self as x64 and invoking the Reloaded Loader Executable.
    Copyright (C) 2018  Sewer. Sz (Sewer56)

    [Reloaded] is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    [Reloaded] is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Reloaded_Wrapper_x64
{
    /*
     * Based on an idea by Gabriel Schenker:
     * http://lostechies.com/gabrielschenker/2009/10/21/force-net-application-to-run-in-32bit-process-on-64bit-os/
    */

    /// <summary>
    /// Mini Wrapper Program to load the Any CPU Compiled Mod Loader, 
    /// Loader executable in x64 mode by invoking the Reloaded Loader Executable in x64 mode.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Load the assembly into this process.
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyName = Path.Combine(directory, "Reloaded-Loader.exe");
            Assembly assembly = Assembly.LoadFile(assemblyName);

            // Find the entry point of the assembly.
            Type mainMethodType = null;
            MethodInfo methodInfo = null;

            // Get Main method.
            foreach (Type t in assembly.GetTypes())
            {
                if ((methodInfo = t.GetMethod("Main")) != null)
                {
                    mainMethodType = t;
                    break;
                }
            }

            // Invoke the Mod Loader Loader
            methodInfo.Invoke(mainMethodType, new object[] { args });
        }
    }
}
