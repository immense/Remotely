using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities;

public class RewritableStream : Stream
{
    private readonly Stream _underlyingStream;
    private readonly Dictionary<long, byte> _rewriteMap;

    public RewritableStream(Stream underlyingStream, Dictionary<long, byte> rewriteMap)
    {
        _underlyingStream = underlyingStream;
        _rewriteMap = rewriteMap;
    }

    public override bool CanRead => _underlyingStream.CanRead;

    public override bool CanSeek => _underlyingStream.CanSeek;

    public override bool CanTimeout => _underlyingStream.CanTimeout;

    public override bool CanWrite => _underlyingStream.CanWrite;

    public override long Length => _underlyingStream.Length;

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
        GC.SuppressFinalize(this);
        return _underlyingStream.DisposeAsync();
    }

    public override void Flush()
    {
        _underlyingStream.Flush();
    }

    
    public override int Read(byte[] buffer, int offset, int count)
    {
        var i = 0;
        for (; i < count; i++)
        {
            if (_rewriteMap.TryGetValue(Position, out var newValue))
            {
                buffer[offset + i] = newValue;
                Seek(1, SeekOrigin.Current);
            }
            else
            {
                var current = _underlyingStream.ReadByte();
                if (current == -1)
                {
                    break;
                }
                buffer[offset + i] = current == -1 ? (byte)0 : (byte)current;
            }
        }
        return i;
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
