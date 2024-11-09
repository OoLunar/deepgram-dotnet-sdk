// Copyright 2024 Deepgram .NET SDK contributors. All Rights Reserved.
// Use of this source code is governed by a MIT license that can be found in the LICENSE file.
// SPDX-License-Identifier: MIT

using System;
using Deepgram.Logger;
using Deepgram.Microphone;
using Deepgram.Models.Authenticate.v1;
using Deepgram.Models.Speak.v2.WebSocket;
using PortAudioSharp;

namespace SampleApp
{
    public static class Program
    {
        public static async Task Main()
        {
            // Initialize Library with default logging
            // Normal logging is "Info" level
            Deepgram.Library.Initialize();
            Deepgram.Microphone.Library.Initialize();

            // OR very chatty logging
            //Library.Initialize(LogLevel.Verbose); // LogLevel.Default, LogLevel.Debug, LogLevel.Verbose

            // use the client factory with a API Key set with the "DEEPGRAM_API_KEY" environment variable
            var speakClient = ClientFactory.CreateSpeakWebSocketClient(Environment.GetEnvironmentVariable("DEEPGRAM_API_KEY"));
            var speaker = new Speaker(rate: 48000, deviceIndex: PortAudio.DefaultOutputDevice, format: SampleFormat.Int16);

            await speakClient.Subscribe(new EventHandler<AudioResponse>((sender, e) =>
            {
                if (e.Stream is not null)
                {
                    speaker.Write(e.Stream.ToArray());
                }
            }));

            await speakClient.Subscribe(new EventHandler<MetadataResponse>((sender, e) =>
            {
                Console.WriteLine($"----> {e.Type} received");
                Console.WriteLine($"----> RequestId: {e.RequestId}");
            }));

            await speakClient.Subscribe(new EventHandler<CloseResponse>((sender, e) =>
            {
                speaker.Start();
                Console.WriteLine($"----> {e.Type} received");
            }));

            await speakClient.Subscribe(new EventHandler<OpenResponse>((sender, e) => Console.WriteLine($"\n\n----> {e.Type} received")));
            await speakClient.Subscribe(new EventHandler<FlushedResponse>((sender, e) => Console.WriteLine($"----> {e.Type} received")));
            await speakClient.Subscribe(new EventHandler<ClearedResponse>((sender, e) => Console.WriteLine($"----> {e.Type} received")));
            await speakClient.Subscribe(new EventHandler<WarningResponse>((sender, e) => Console.WriteLine($"----> {e.Type} received")));
            await speakClient.Subscribe(new EventHandler<UnhandledResponse>((sender, e) => Console.WriteLine($"----> {e.Type} received")));
            await speakClient.Subscribe(new EventHandler<ErrorResponse>((sender, e) => Console.WriteLine($"----> {e.Type} received. Error: {e.Message}")));

            // Start the connection
            var speakSchema = new SpeakSchema()
            {
                Encoding = "linear16",
                SampleRate = 48000,
            };

            if (!await speakClient.Connect(speakSchema))
            {
                Console.WriteLine("Failed to connect to the server");
                return;
            }

            // Send some Text to convert to audio
            speakClient.SpeakWithText("Hello World!");

            //Flush the audio
            speakClient.Flush();
            speakClient.Close();

            // Wait for the audio to be spoken
            await speaker.WaitForCompleteAsync();
            speaker.Stop();

            // Wait for the user to press a key
            Console.WriteLine("Press any key to stop and exit...");
            Console.ReadKey();

            // Terminate Libraries
            Deepgram.Library.Terminate();
            Deepgram.Microphone.Library.Terminate();
        }
    }
}
