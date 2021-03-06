﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Reloaded.Process;
using Reloaded_Mod_Template.ReloadedTemplate;
using WPF_Demo_Overlay;

namespace Reloaded_Mod_Template
{
    public static class Program
    {
        #region Reloaded Mod Template Stuff
        /// <summary>
        /// Holds the game process for us to manipulate.
        /// Allows you to read/write memory, perform pattern scans, etc.
        /// See libReloaded/GameProcess (folder)
        /// </summary>
        public static ReloadedProcess GameProcess;

        /// <summary>
        /// Stores the absolute executable location of the currently executing game or process.
        /// </summary>
        public static string ExecutingGameLocation;

        /// <summary>
        /// Specifies the full directory location that the current mod 
        /// is contained in.
        /// </summary>
        public static string ModDirectory;
        #endregion Reloaded Mod Template Stuff

        private static OverlayWindow WPFWindow;
        private static System.Windows.Application WPFApp;

        /// <summary>
        /// Your own user code starts here.
        /// If this is your first time, do consider reading the notice above.
        /// It contains some very useful information.
        /// </summary>
        public static void Init()
        {
            /*
                Reloaded Mod Loader Sample: Windows Presentation Foundation
                Architectures supported: X86, X64

                Creates an interactive Windows Presentation Foundation semi-transparent window inside
                the running process, demonstrating or giving an idea on how perhaps utility tools 
                could be developed with libReloaded or how WPF may be used as an alternative to Direct2D
                HUDs/Overlays.

                Note: In order to allow the use of WPF over the Reloaded Template, you need to add
                <ProjectTypeGuids>{60dc8134-eba5-43b8-bcc9-bb4bc16c2548};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
                to the first PropertyGroup in the Project's .csproj and reference PresentationFramework + System.Xaml.
            */

            /*
                We create our own thread and run it in the background because Reloaded-Loader explicitly waits
                for the mod's thread to return before continuing to load other mods and ultimately the game.

                Inside the thread, we initiate a regular WPF window to use for drawing.
            */

            /*
             * Generates a random sleep value ontop of the existing sleep value.
             * We want to reduce the amount of possible collisions of WPF overlays launching
             * at the same identical time, as Application.Current is not updated in real-time (seemingly).
            */
            Random randomNumberGenerator = new Random();
            int randomSleepValue = randomNumberGenerator.Next(0, 3000);
            Thread.Sleep(32); // Ensures different tick value for seeding, offsets overlay launch time to prevent collisions.

            Thread launchWpfThread = new Thread
            (
                () =>
                {
                    // Loop infinitely until a window handle is found.
                    while (GameProcess.Process.MainWindowHandle == IntPtr.Zero)
                    {
                        // Sleep the thread for a sensible amount of time.
                        Thread.Sleep(1000);
                    }

                    // Wait a fixed amount of time + random extra (to not try to launch different copies at once, minimize collisions).
                    Thread.Sleep(randomSleepValue);

                    // Run the WPF window.

                    // Note that our OverlayWindow comes from WPF-Demo-Overlay project (it's added in as a reference).
                    // To use WPF windows, you want to create another project which is a WPF UserControl library, remove the
                    // usercontrol and create a window, use that library as a reference in your Reloaded WPF overlays.

                    // While this isn't strictly necessary to use another library/project, it will save you a lot of hassle when it
                    // comes to interoperating with different overlays, this will also make your life easier in other ways, believe me.

                    // Another overlay already active.
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            WPFWindow = new OverlayWindow();
                            WPFWindow.Show();
                        });
                    }
                    // This is the first overlay.
                    else
                    {
                        try
                        {
                            // Create new Application Context.
                            Application WPFApp = new System.Windows.Application();

                            // Create our window
                            WPFWindow = new OverlayWindow();

                            // Instance the overlay.
                            WPFApp.Run(WPFWindow);
                        }
                        catch
                        {
                            // If it fails, fall back to second overlay anyway.
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                WPFWindow = new OverlayWindow();
                                WPFWindow.Show();
                            });
                        }
                    }
                }
            );
            launchWpfThread.SetApartmentState(ApartmentState.STA);
            launchWpfThread.Start();
        }
    }
}
