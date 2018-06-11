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

        public abstract string TypeName { get; }

        public virtual byte[] GetBytes(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);
            return value;
        }

        public abstract string StringValue(MemoryMappedViewAccessor source);
    }

    public class BoolTelemVar : TelemVar {
        public BoolTelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override int ElementSize => 1;
        public override string TypeName => ArrayLength == 1 ? "bool" : "bool[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToBoolean(value, 0).ToString()}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToBoolean(value, i)}\r\n";
                }
                return result;
            }
        }

    }

    public class U32TelemVar : TelemVar {
        public U32TelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override int ElementSize => 4;
        public override string TypeName => ArrayLength == 1 ? "uint32_t" : "uint32_t[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToUInt32(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToUInt32(value, i*ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class S32TelemVar : U32TelemVar {
        public S32TelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override string TypeName => ArrayLength == 1 ? "int32_t" : "int32_t[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToSingle(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToSingle(value, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class FloatTelemVar : U32TelemVar {
        public FloatTelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        private bool doMultiply = false;
        private float _Multiplier = 1.0f;
        public float Multiplier {
            get {
                return _Multiplier;
            }
            set {
                if (value == 1.0f) {
                    doMultiply = false;
                } else {
                    doMultiply = true;
                }
                _Multiplier = value;
            }
        }
        public bool ConvertToInt { get; set; } = false;

        public override string TypeName {
            get {
                if (ConvertToInt) {
                    if (ArrayLength == 1) {
                        return "int32_t";
                    } else {
                        return "int32_t[]";
                    }
                } else {
                    if (ArrayLength == 1) {
                        return "float";
                    } else {
                        return "float[]";
                    }
                }
            }
        }

        public override byte[] GetBytes(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);

            if (doMultiply) {
                for (int i = 0; i < ArrayLength; i++) {
                    float temp = BitConverter.ToSingle(value, i * ElementSize);
                    temp *= Multiplier;
                    byte[] newValue = BitConverter.GetBytes(temp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }

            if (ConvertToInt) {
                for (int i = 0; i < ArrayLength; i++) {
                    float temp = BitConverter.ToSingle(value, i * ElementSize);
                    int convertedTemp = (int)Math.Round(temp);
                    byte[] newValue = BitConverter.GetBytes(convertedTemp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }

            return value;
        }

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return ConvertToInt ? $"{BitConverter.ToInt32(value, 0)}\r\n" : $"{BitConverter.ToSingle(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += ConvertToInt ? $"{i}: {BitConverter.ToInt32(value, i * ElementSize)}\r\n" : $"{i}: {BitConverter.ToSingle(value, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class U64TelemVar : TelemVar {
        public U64TelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override int ElementSize => 8;
        public override string TypeName => ArrayLength == 1 ? "uint64_t" : "uint64_t[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            if (ArrayLength == 1) {
                return $"{BitConverter.ToUInt64(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {BitConverter.ToUInt64(value, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class FVectorTelemVar : TelemVar {
        public FVectorTelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override int ElementSize => 12;
        public override string TypeName => ArrayLength == 1 ? "fvector" : "fvector[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {BitConverter.ToSingle(value, 0)}\r\n";
                result += $"Y: {BitConverter.ToSingle(value, 4)}\r\n";
                result += $"Z: {BitConverter.ToSingle(value, 8)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {BitConverter.ToSingle(value, i * ElementSize)}\r\n";
                    result += $"Y: {BitConverter.ToSingle(value, i * ElementSize + 4)}\r\n";
                    result += $"Z: {BitConverter.ToSingle(value, i * ElementSize + 8)}\r\n\r\n";
                }
                return result;
            }
        }
    }

    public class DVectorTelemVar : TelemVar {
        public DVectorTelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override int ElementSize => 24;
        public override string TypeName => ArrayLength == 1 ? "dvector" : "dvector[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {BitConverter.ToDouble(value, 0)}\r\n";
                result += $"Y: {BitConverter.ToDouble(value, 8)}\r\n";
                result += $"Z: {BitConverter.ToDouble(value, 16)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {BitConverter.ToDouble(value, i * ElementSize + 0)}\r\n";
                    result += $"Y: {BitConverter.ToDouble(value, i * ElementSize + 8)}\r\n";
                    result += $"Z: {BitConverter.ToDouble(value, i * ElementSize + 16)}\r\n\r\n";
                }
                return result;
            }
        }
    }

    public class FPlacementTelemVar : TelemVar {
        public FPlacementTelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override int ElementSize => 24;
        public override string TypeName => ArrayLength == 1 ? "fplacement" : "fplacement[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {BitConverter.ToSingle(value, 0)}\r\n";
                result += $"Y: {BitConverter.ToSingle(value, 4)}\r\n";
                result += $"Z: {BitConverter.ToSingle(value, 8)}\r\n";
                result += $"Head: {BitConverter.ToSingle(value, 12)}\r\n";
                result += $"Pitch: {BitConverter.ToSingle(value, 16)}\r\n";
                result += $"Roll: {BitConverter.ToSingle(value, 20)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {BitConverter.ToSingle(value, i * ElementSize + 0)}\r\n";
                    result += $"Y: {BitConverter.ToSingle(value, i * ElementSize + 4)}\r\n";
                    result += $"Z: {BitConverter.ToSingle(value, i * ElementSize + 8)}\r\n";
                    result += $"Head: {BitConverter.ToSingle(value, i * ElementSize + 12)}\r\n";
                    result += $"Pitch: {BitConverter.ToSingle(value, i * ElementSize + 16)}\r\n";
                    result += $"Roll: {BitConverter.ToSingle(value, i * ElementSize + 20)}\r\n\r\n";
                }
                return result;
            }
        }
    }

    public class DPlacementTelemVar : TelemVar {
        public DPlacementTelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override int ElementSize => 40; //36 + 4 byte explicit padding -> see scssdk_value.h
        public override string TypeName => ArrayLength == 1 ? "dplacement" : "dplacement[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetBytes(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {BitConverter.ToDouble(value, 0)}\r\n";
                result += $"Y: {BitConverter.ToDouble(value, 8)}\r\n";
                result += $"Z: {BitConverter.ToDouble(value, 16)}\r\n";
                result += $"Head: {BitConverter.ToSingle(value, 24)}\r\n";
                result += $"Pitch: {BitConverter.ToSingle(value, 28)}\r\n";
                result += $"Roll: {BitConverter.ToSingle(value, 32)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {BitConverter.ToDouble(value, i * ElementSize + 0)}\r\n";
                    result += $"Y: {BitConverter.ToDouble(value, i * ElementSize + 8)}\r\n";
                    result += $"Z: {BitConverter.ToDouble(value, i * ElementSize + 16)}\r\n";
                    result += $"Head: {BitConverter.ToSingle(value, i * ElementSize + 24)}\r\n";
                    result += $"Pitch: {BitConverter.ToSingle(value, i * ElementSize + 28)}\r\n";
                    result += $"Roll: {BitConverter.ToSingle(value, i * ElementSize + 32)}\r\n\r\n";
                }
                return result;
            }
        }
    }

    public class ASCIITelemVar : TelemVar {
        public ASCIITelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override int ElementSize => 64;

        public override string TypeName => "char[]";

        public override string StringValue(MemoryMappedViewAccessor source) {
            return System.Text.Encoding.ASCII.GetString(GetBytes(source));
        }
    }

    public class UTF8TelemVar : ASCIITelemVar {
        public UTF8TelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override string StringValue(MemoryMappedViewAccessor source) {
            return System.Text.Encoding.UTF8.GetString(GetBytes(source));
        }
    }
}
