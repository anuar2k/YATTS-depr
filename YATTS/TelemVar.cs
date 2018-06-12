using System;
using System.IO.MemoryMappedFiles;
using static System.BitConverter;

namespace YATTS {
    public abstract class TelemVar {
        public TelemVar(string ID, string Name, string Description, long Offset, int MaxArrayLength = 1) {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Offset = Offset;
            this.MaxArrayLength = MaxArrayLength;
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

        public virtual byte[] GetByteValue(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);
            return value;
        }

        public abstract string GetStringValue(MemoryMappedViewAccessor source);
    }

    public class BoolTelemVar : TelemVar {
        public BoolTelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override int ElementSize => 1;
        public override string TypeName => ArrayLength == 1 ? "bool" : "bool[]";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return $"{ToBoolean(value, 0).ToString()}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {ToBoolean(value, i)}\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return $"{ToUInt32(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {ToUInt32(value, i*ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class S32TelemVar : U32TelemVar {
        public S32TelemVar(string ID, string Name, string Description, long offset, int MaxArraySize = 1) : base(ID, Name, Description, offset, MaxArraySize) {

        }

        public override string TypeName => ArrayLength == 1 ? "int32_t" : "int32_t[]";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return $"{ToSingle(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {ToSingle(value, i * ElementSize)}\r\n";
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
                        return "float -> int32_t";
                    } else {
                        return "float[] -> int32_t[]";
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

        public override byte[] GetByteValue(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);

            if (doMultiply) {
                for (int i = 0; i < ArrayLength; i++) {
                    float temp = ToSingle(value, i * ElementSize);
                    temp *= Multiplier;
                    byte[] newValue = GetBytes(temp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }
            

            if (ConvertToInt) {
                for (int i = 0; i < ArrayLength; i++) {
                    float temp = ToSingle(value, i * ElementSize);
                    int convertedTemp = (int)Math.Round(temp);
                    byte[] newValue = GetBytes(convertedTemp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }

            return value;
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return ConvertToInt ? $"{ToInt32(value, 0)}\r\n" : $"{ToSingle(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += ConvertToInt ? $"{i}: {ToInt32(value, i * ElementSize)}\r\n" : $"{i}: {ToSingle(value, i * ElementSize)}\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return $"{ToUInt64(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {ToUInt64(value, i * ElementSize)}\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {ToSingle(value, 0)}\r\n";
                result += $"Y: {ToSingle(value, 4)}\r\n";
                result += $"Z: {ToSingle(value, 8)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {ToSingle(value, i * ElementSize)}\r\n";
                    result += $"Y: {ToSingle(value, i * ElementSize + 4)}\r\n";
                    result += $"Z: {ToSingle(value, i * ElementSize + 8)}\r\n\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {ToDouble(value, 0)}\r\n";
                result += $"Y: {ToDouble(value, 8)}\r\n";
                result += $"Z: {ToDouble(value, 16)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {ToDouble(value, i * ElementSize + 0)}\r\n";
                    result += $"Y: {ToDouble(value, i * ElementSize + 8)}\r\n";
                    result += $"Z: {ToDouble(value, i * ElementSize + 16)}\r\n\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {ToSingle(value, 0)}\r\n";
                result += $"Y: {ToSingle(value, 4)}\r\n";
                result += $"Z: {ToSingle(value, 8)}\r\n";
                result += $"Head: {ToSingle(value, 12)}\r\n";
                result += $"Pitch: {ToSingle(value, 16)}\r\n";
                result += $"Roll: {ToSingle(value, 20)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {ToSingle(value, i * ElementSize + 0)}\r\n";
                    result += $"Y: {ToSingle(value, i * ElementSize + 4)}\r\n";
                    result += $"Z: {ToSingle(value, i * ElementSize + 8)}\r\n";
                    result += $"Head: {ToSingle(value, i * ElementSize + 12)}\r\n";
                    result += $"Pitch: {ToSingle(value, i * ElementSize + 16)}\r\n";
                    result += $"Roll: {ToSingle(value, i * ElementSize + 20)}\r\n\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            string result = string.Empty;
            if (ArrayLength == 1) {
                result += $"X: {ToDouble(value, 0)}\r\n";
                result += $"Y: {ToDouble(value, 8)}\r\n";
                result += $"Z: {ToDouble(value, 16)}\r\n";
                result += $"Head: {ToSingle(value, 24)}\r\n";
                result += $"Pitch: {ToSingle(value, 28)}\r\n";
                result += $"Roll: {ToSingle(value, 32)}\r\n";
                return result;
            } else {
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"Value {i}:\r\n";
                    result += $"X: {ToDouble(value, i * ElementSize + 0)}\r\n";
                    result += $"Y: {ToDouble(value, i * ElementSize + 8)}\r\n";
                    result += $"Z: {ToDouble(value, i * ElementSize + 16)}\r\n";
                    result += $"Head: {ToSingle(value, i * ElementSize + 24)}\r\n";
                    result += $"Pitch: {ToSingle(value, i * ElementSize + 28)}\r\n";
                    result += $"Roll: {ToSingle(value, i * ElementSize + 32)}\r\n\r\n";
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

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            return System.Text.Encoding.ASCII.GetString(GetByteValue(source));
        }
    }

    public class UTF8TelemVar : ASCIITelemVar {
        public UTF8TelemVar(string ID, string Name, string Description, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, offset, MaxArrayLength) {

        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            return System.Text.Encoding.UTF8.GetString(GetByteValue(source));
        }
    }
}
