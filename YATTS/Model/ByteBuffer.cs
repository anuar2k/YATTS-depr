using System;

namespace YATTS {
    public class ByteBuffer {
        public ByteBuffer(int length) {
            buffer = new byte[length];
        }

        private byte[] buffer;
        public int Position { get; private set; } = 0;
        public int Length {
            get {
                return buffer.Length;
            }
            set {
                if (value != buffer.Length) {
                    buffer = new byte[value];
                    Position = 0;
                }
            }
        }

        public void Append(byte[] data) {
            Array.Copy(data, 0, buffer, Position, data.Length);
            Position += data.Length;
        }

        public byte[] Flush() {
            byte[] result = new byte[Position];
            Array.Copy(buffer, result, Position);
            Position = 0;
            return result;
        }
    }
}
