﻿using System.Security.AccessControl;
using System.Security.Principal;
using KioskBrowser.Core.Service;
using NAudio.Utils;
using NAudio.Wave;

var filePath = string.Join(' ', args);

if (filePath is null)
{
    Console.WriteLine("Please specify a file path.");
    Environment.Exit(0);
}

var singleInstanceService = new SingleInstanceService("KioskBrowser.SoundPlayer");

singleInstanceService.OnAnotherInstanceDetected +=
    () => Console.WriteLine("Another instance is already running. Signaling it to stop...");

singleInstanceService.OnThisNeedToShutdown += () =>
{
    Console.WriteLine("");
    Console.WriteLine("Another instance is already running. Stopping...");
    Environment.Exit(0);
};

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("");
    Console.WriteLine("Exiting...");
    singleInstanceService.Dispose();
    Environment.Exit(0);
};

singleInstanceService.Start();

Console.WriteLine("Playing file: " + filePath);
try
{
    using (var audioFile = new AudioFileReader(filePath))
    using (var outputDevice = new WaveOutEvent())
    {
        outputDevice.Init(audioFile);
        outputDevice.Play();

        // Subscribe to the PlaybackStopped event
        outputDevice.PlaybackStopped += (sender, e) =>
        {
            Console.WriteLine("");
            Console.WriteLine("Sound played 100% exiting...");
            singleInstanceService.Dispose();
            Environment.Exit(0);
        };

        // Keep the application running while the audio is playing
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            var position = audioFile.CurrentTime;
            var total = audioFile.TotalTime;
            var percentage = Math.Round(position.TotalMilliseconds / total.TotalMilliseconds * 100);
            Console.Write(percentage + "% Current Position: " + position + " Total lenght: " + total + "          ");
            Console.SetCursorPosition(0, Console.CursorTop);

            if (Console.KeyAvailable)
            {
                // Read the key without blocking the thread
                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    // Check if the key pressed is 'q'
                    case ConsoleKey.LeftArrow:
                    {
                        // Seek backward by 1 second
                        var newTime = position.Subtract(TimeSpan.FromSeconds(10));
                        if (newTime < TimeSpan.Zero)
                            newTime = TimeSpan.Zero;

                        audioFile.CurrentTime = newTime;
                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        // Seek backward by 1 second
                        var newTime = position.Add(TimeSpan.FromSeconds(10));
                        if (newTime > total)
                            newTime = total;

                        audioFile.CurrentTime = newTime;
                        break;
                    }
                }
            }

            Thread.Sleep(100);
        }
    }
}
catch (Exception e)
{
    //Console.WriteLine(e);
}
finally
{
    singleInstanceService.Dispose();
}