using System;
using System.IO.MemoryMappedFiles;

namespace YATTS {
    public abstract class TelemVar {
        public TelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) {
            ArrayLength = MaxArrayLength;
        }

        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MaxArrayLength { get; private set; }

        private int _ArrayLength;
        public int ArrayLength {
            get {
                return _ArrayLength;
            }
            set {
                if (value > MaxArrayLength || value < 1) {
                    throw new ArgumentOutOfRangeException();
                }
                _ArrayLength = value;
                DataSize = value * ElementSize;
            }
        }
        public abstract int ElementSize { get; }
        public int DataSize { get; private set; }
        public long Offset { get; private set; }

        public abstract byte[] GetBytes(MemoryMappedViewAccessor source);
        public abstract string StringValue(MemoryMappedViewAccessor source);
    }

    public class BoolTelemVar : TelemVar {
        public BoolTelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override int ElementSize => 1;

        public override byte[] GetBytes(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);
            return value;
        }

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToBoolean(value, 0).ToString()}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToBoolean(value, i).ToString()}\r\n";
                }
                return result;
            }
        }
    }

    public class U32TelemVar : TelemVar {
        public U32TelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override int ElementSize => 4;

        public override byte[] GetBytes(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);
            return value;
        }

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToUInt32(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToUInt32(value, i*ElementSize).ToString()}\r\n";
                }
                return result;
            }
        }
    }

    public class S32TelemVar : U32TelemVar {
        public S32TelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToSingle(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToSingle(value, i * ElementSize).ToString()}\r\n";
                }
                return result;
            }
        }
    }
}
