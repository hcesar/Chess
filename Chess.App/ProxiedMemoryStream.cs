using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App
{
    internal class ProxiedMemoryStream : MemoryStream
    {
        private Stream realStream;
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public ProxiedMemoryStream(Stream realStream)
        {
            this.realStream = realStream;
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            realStream.Write(buffer, offset, count);
            base.Write(buffer, offset, count);
            base.Seek(0, SeekOrigin.Begin);
        }

        public override void WriteByte(byte value)
        {
            realStream.WriteByte(value);
            base.WriteByte(value);
            base.Seek(0, SeekOrigin.Begin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            while (base.Position - base.Length < count)
                System.Threading.Thread.Sleep(1);
            return base.Read(buffer, offset, count);
        }
    }
}
