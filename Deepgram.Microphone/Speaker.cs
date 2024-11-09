using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Deepgram.Microphone
{
    /// <summary>
    /// A one-time use class to play audio data.
    /// </summary>
    public class Speaker : IDisposable
    {
        private readonly ConcurrentQueue<byte[]> _audioQueue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly TaskCompletionSource<object?> _taskCompletionSource = new();
        private readonly PortAudioSharp.Stream _stream;

        // Track partially played buffer and its current offset
        private byte[]? _lastBuffer;
        private int _lastOffset;

        /// <summary>
        /// Creates a new instance of the <see cref="Speaker"/> class.
        /// </summary>
        /// <param name="rate">The sample rate in Hz.</param>
        /// <param name="framesPerBuffer">The number of frames per buffer.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="deviceIndex">The index of the audio output device.</param>
        /// <param name="format">The sample format of the incoming audio data.</param>
        public Speaker(
            int rate = Defaults.RATE,
            uint framesPerBuffer = Defaults.CHUNK_SIZE,
            int channels = Defaults.CHANNELS,
            int deviceIndex = Defaults.DEVICE_INDEX,
            SampleFormat format = Defaults.SAMPLE_FORMAT)
        {
            _stream = new PortAudioSharp.Stream(
                inParams: null,
                outParams: new StreamParameters()
                {
                    device = deviceIndex,
                    channelCount = channels,
                    sampleFormat = format,
                    suggestedLatency = PortAudio.GetDeviceInfo(deviceIndex).defaultLowInputLatency,
                    hostApiSpecificStreamInfo = IntPtr.Zero
                },
                sampleRate: rate,
                framesPerBuffer: framesPerBuffer,
                streamFlags: StreamFlags.ClipOff,
                callback: StreamCallback,
                userData: IntPtr.Zero
            );
        }

        /// <summary>
        /// Starts playing the queued audio data.
        /// </summary>
        public void Start()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("Speaker has already been used, please create a new instance!");
            }

            _stream.Start();
        }

        /// <summary>
        /// Stops and clears the audio queue.
        /// </summary>
        public void Stop()
        {
            if (!_taskCompletionSource.Task.IsCompleted)
            {
                _taskCompletionSource.SetResult(null);
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _audioQueue.Clear();
                _stream.Stop();
                _stream.Dispose();
            }
        }

        /// <summary>
        /// Adds audio data to the queue.
        /// </summary>
        /// <param name="data">The audio data to add to the queue.</param>
        public void Write(params byte[] data) => _audioQueue.Enqueue(data);

        /// <summary>
        /// Blocks until all audio data has been played.
        /// </summary>
        public Task WaitForCompleteAsync() => _taskCompletionSource.Task;

        private StreamCallbackResult StreamCallback(IntPtr input, IntPtr output, uint frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData)
        {
            if (_cancellationTokenSource.IsCancellationRequested || _stream.outputParameters is null)
            {
                Stop();
                return StreamCallbackResult.Abort;
            }

            int dataSize = _stream.outputParameters.Value.sampleFormat switch
            {
                SampleFormat.Int8 => Marshal.SizeOf<sbyte>(),
                SampleFormat.UInt8 => Marshal.SizeOf<byte>(),
                SampleFormat.Int16 => Marshal.SizeOf<short>(),
                SampleFormat.Int24 => Marshal.SizeOf<short>() + Marshal.SizeOf<byte>(),
                SampleFormat.Int32 => Marshal.SizeOf<int>(),
                SampleFormat.Float32 => Marshal.SizeOf<float>(),
                _ => throw new NotSupportedException($"Sample format '{_stream.outputParameters.Value.sampleFormat}' is not supported")
            };

            int offset = 0;
            int outputLength = (int)frameCount * dataSize * _stream.outputParameters.Value.channelCount;
            while (offset < outputLength)
            {
                // Load the next buffer if we finished the last one
                if (_lastBuffer is null || _lastOffset >= _lastBuffer.Length)
                {
                    if (!_audioQueue.TryDequeue(out _lastBuffer))
                    {
                        // No more data available
                        break;
                    }

                    _lastOffset = 0;
                }

                int remainingBufferLength = _lastBuffer.Length - _lastOffset;
                int remainingOutputLength = outputLength - offset;

                // Copy as much data as possible from the current buffer
                int copyLength = Math.Min(remainingBufferLength, remainingOutputLength);
                Marshal.Copy(_lastBuffer, _lastOffset, IntPtr.Add(output, offset), copyLength);

                _lastOffset += copyLength;
                offset += copyLength;
            }

            // Zero-fill if there’s not enough data
            if (offset < outputLength)
            {
                Marshal.Copy(new byte[outputLength - offset], 0, IntPtr.Add(output, offset), outputLength - offset);
            }

            // Signal completion if queue is empty and there's no partial buffer left
            if (!_audioQueue.IsEmpty || (_lastBuffer is not null && _lastOffset < _lastBuffer.Length))
            {
                return StreamCallbackResult.Continue;
            }

            Stop();
            return StreamCallbackResult.Complete;
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
