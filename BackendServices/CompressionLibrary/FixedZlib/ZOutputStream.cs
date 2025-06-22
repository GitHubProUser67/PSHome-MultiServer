// Copyright (c) 2006, ComponentAce
// http://www.componentace.com
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 
// Neither the name of ComponentAce nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission. 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

/*
Copyright (c) 2000,2001,2002,2003 ymnk, JCraft,Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright 
notice, this list of conditions and the following disclaimer in 
the documentation and/or other materials provided with the distribution.

3. The names of the authors may not be used to endorse or promote products
derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
/*
* This program is based on zlib-1.1.3, so all credit should go authors
* Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
* and contributors of zlib.
*/
using System.IO;
using System;

namespace ComponentAce.Compression.Libs.zlib
{
    public class ZOutputStream : Stream
    {
        protected internal ZStream z = new ZStream();

        protected internal int bufsize = 4096;

        protected internal int flush_Renamed_Field;

        protected internal byte[] buf;

        protected internal byte[] buf1 = new byte[1];

        protected internal bool compress;

        protected bool closed = false;

        private Stream out_Renamed;

        public virtual int FlushMode
        {
            get
            {
                return flush_Renamed_Field;
            }
            set
            {
                flush_Renamed_Field = value;
            }
        }

        public virtual long TotalIn => z != null ? z.total_in : 0L;

        public virtual long TotalOut => z != null ? z.total_out : 0L;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => 0L;

        public override long Position
        {
            get
            {
                return 0L;
            }
            set
            {
            }
        }

        private void InitBlock()
        {
            flush_Renamed_Field = 0;
            buf = new byte[bufsize];
        }

        private void ImplDisposing(bool disposeOutput)
        {
            if (!closed)
            {
                try
                {
                    try
                    {
                        Finish();
                    }
                    catch (IOException)
                    {
                        // Ignore
                    }
                }
                finally
                {
                    closed = true;
                    End();
                    if (disposeOutput)
                        out_Renamed.Dispose();
                    out_Renamed = null;
                }
            }
        }

        protected void Detach(bool disposing)
        {
            if (disposing)
                ImplDisposing(disposeOutput: false);
            base.Dispose(disposing);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                ImplDisposing(disposeOutput: true);
            base.Dispose(disposing);
        }

        public ZOutputStream(Stream out_Renamed, bool NoHeader)
        {
            InitBlock();
            this.out_Renamed = out_Renamed;
            if (NoHeader)
                z.inflateInit(-15);
            else
                z.inflateInit();
            compress = false;
        }

        public ZOutputStream(Stream out_Renamed, int level, bool NoHeader)
        {
            InitBlock();
            this.out_Renamed = out_Renamed;
            if (NoHeader)
                z.deflateInit(level, -15);
            else
                z.deflateInit(level);
            compress = true;
        }

        public void WriteByte(int b)
        {
            buf1[0] = (byte)b;
            Write(buf1, 0, 1);
        }

        public override void WriteByte(byte b)
        {
            WriteByte(b);
        }

        public override void Write(byte[] b1, int off, int len)
        {
            if (len == 0)
                return;
            byte[] array = new byte[b1.Length];
            Array.Copy(b1, 0, array, 0, b1.Length);
            z.next_in = array;
            z.next_in_index = off;
            z.avail_in = len;
            int num;
            do
            {
                z.next_out = buf;
                z.next_out_index = 0;
                z.avail_out = bufsize;
                num = (!compress) ? z.inflate(flush_Renamed_Field) : z.deflate(flush_Renamed_Field);
                if (num != 0 && num != 1)
                    throw new ZStreamException((compress ? "de" : "in") + "flating: " + z.msg);
                out_Renamed?.Write(buf, 0, bufsize - z.avail_out);
            }
            while ((z.avail_in > 0 || z.avail_out == 0) && num == 0);
        }

        public virtual void Finish()
        {
            int num;
            do
            {
                z.next_out = buf;
                z.next_out_index = 0;
                z.avail_out = bufsize;
                num = (!compress) ? z.inflate(4) : z.deflate(4);
                if (num != 1 && num != 0)
                    throw new ZStreamException((compress ? "de" : "in") + "flating: " + z.msg);
                if (bufsize - z.avail_out > 0)
                    out_Renamed?.Write(buf, 0, bufsize - z.avail_out);
            }
            while ((z.avail_in > 0 || z.avail_out == 0) && num == 0);

            Flush();
        }

        public virtual void End()
        {
            if (compress)
                z.deflateEnd();
            else
                z.inflateEnd();
            z.free();
            z = null;
        }

        public override void Close()
        {
            if (!closed)
            {
                try
                {
                    try
                    {
                        Finish();
                    }
                    catch
                    {
                        
                    }
                }
                finally
                {
                    closed = true;
                    End();
                    out_Renamed.Close();
                    out_Renamed = null;
                }
            }
        }

        public override void Flush()
        {
            out_Renamed?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0L;
        }
    }

    public class ZOutputStreamLeaveOpen
        : ZOutputStream
    {
        public ZOutputStreamLeaveOpen(Stream output)
            : base(output, false)
        {
        }

        public ZOutputStreamLeaveOpen(Stream output, bool NoHeader)
            : base(output, NoHeader)
        {
        }

        public ZOutputStreamLeaveOpen(Stream output, int level, bool NoHeader)
            : base(output, level, NoHeader)
        {
        }

        public override void Close()
        {
            Detach(true);
        }

        protected override void Dispose(bool disposing)
        {
            Detach(disposing);
        }
    }
}