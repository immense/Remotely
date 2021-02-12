using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public class AppendableStream : Stream
    {
        private readonly byte[] _bytesToAppend;
        private readonly Stream _underlyingStream;
        private bool _bytesAppended;

        public AppendableStream(Stream underlyingStream, byte[] bytesToAppend)
        {
            _underlyingStream = underlyingStream;
            _bytesToAppend = bytesToAppend;
        }

        public override bool CanRead => _underlyingStream.CanRead;

        public override bool CanSeek => _underlyingStream.CanSeek;

        public override bool CanTimeout => _underlyingStream.CanTimeout;

        public override bool CanWrite => _underlyingStream.CanWrite;

        public override long Length => _underlyingStream.Length + _bytesToAppend.Length;

        public override long Position
        {
            get => _underlyingStream.Position;
            set => _underlyingStream.Position = value;
        }

        public override int ReadTimeout
        {
            get => _underlyingStream.ReadTimeout;
            set => _underlyingStream.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => _underlyingStream.WriteTimeout;
            set => _underlyingStream.WriteTimeout = value;
        }

        public override void Close()
        {
            _underlyingStream.Close();
            base.Close();
        }


        public override ValueTask DisposeAsync()
        {
            return _underlyingStream.DisposeAsync();
        }

        public override void Flush()
        {
            _underlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_underlyingStream.Position == _underlyingStream.Length && !_bytesAppended)
            {
                _bytesToAppend.CopyTo(buffer, 0);
                _bytesAppended = true;
                return _bytesToAppend.Length;
            }
            return _underlyingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _underlyingStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _underlyingStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            _underlyingStream.Dispose();
            base.Dispose(disposing);
        }
    }
}
