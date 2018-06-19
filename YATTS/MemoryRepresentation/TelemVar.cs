using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using static System.BitConverter;

namespace YATTS {
    public abstract class TelemVar : INotifyPropertyChanged {
        public TelemVar(string ID, string Name, string Description, string Category, long Offset, int MaxArrayLength = 1) {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Category = Category;
            this.Offset = Offset;
            this.MaxArrayLength = MaxArrayLength;
            ArrayLength = MaxArrayLength;
        }

        private bool _Streamed = false;
        public bool Streamed {
            get {
                return _Streamed;
            }
            set {
                if (value != _Streamed) {
                    _Streamed = value;
                    OnPropertyChanged(nameof(Streamed));
                }
            }
        }

        public string ID { get; }
        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
        public int MaxArrayLength { get; }

        private int _ArrayLength;
        public int ArrayLength {
            get {
                return _ArrayLength;
            }
            set {
                if (value > MaxArrayLength || value < 1) {
                    throw new ArgumentOutOfRangeException();
                }
                if (value != _ArrayLength) {
                    _ArrayLength = value;
                    OnPropertyChanged(nameof(ArrayLength));

                    DataSize = value * ElementSize;
                    OnPropertyChanged(nameof(TypeName));
                }
            }
        }
        public abstract int ElementSize { get; }
        private int _DataSize;
        public int DataSize {
            get {
                return _DataSize;
            }
            set {
                if (value != _DataSize) {
                    _DataSize = value;
                    OnPropertyChanged(nameof(DataSize));
                }
            }
        }
        public long Offset { get; }

        public abstract string TypeName { get; }

        public virtual byte[] GetByteValue(MemoryMappedViewAccessor source) {
            byte[] value = new byte[DataSize];
            source.ReadArray(Offset, value, 0, DataSize);
            return value;
        }

        public abstract string GetStringValue(MemoryMappedViewAccessor source);

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string sender) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }

    public class BoolTelemVar : TelemVar {
        public BoolTelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArraySize = 1) : base(ID, Name, Description, Category, offset, MaxArraySize) {

        }

        public override int ElementSize => 1;
        public override string TypeName => ArrayLength == 1 ? "bool" : "bool[]";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return $"{ToBoolean(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {ToBoolean(value, i)}\r\n";
                }
                return result;
            }
        }

    }

    public class U8TelemVar : TelemVar {
        public U8TelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArraySize = 1) : base(ID, Name, Description, Category, offset, MaxArraySize) {

        }

        public override int ElementSize => 1;
        public override string TypeName => ArrayLength == 1 ? "byte" : "byte[]";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return $"{value[0]}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += $"{i}: {value[i]}\r\n";
                }
                return result;
            }
        }
    }

    public class U32TelemVar : TelemVar {
        public U32TelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArraySize = 1) : base(ID, Name, Description, Category, offset, MaxArraySize) {

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

    public class S32TelemVar : TelemVar {
        public S32TelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArraySize = 1) : base(ID, Name, Description, Category, offset, MaxArraySize) {

        }

        public override int ElementSize => 4;
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

    public class FloatTelemVar : TelemVar {
        public FloatTelemVar(string ID, string Name, string Description, string Category, long offset, Unit Unit, int MaxArraySize = 1) : base(ID, Name, Description, Category, offset, MaxArraySize) {
            this.Unit = Unit;
        }

        private float? _Multiplier = null;
        public float? Multiplier {
            get {
                return _Multiplier;
            }
            set {
                if (value != _Multiplier) {
                    _Multiplier = value;
                    OnPropertyChanged(nameof(TypeName));
                }
            }
        }

        public Unit Unit { get; }

        private Func<float, float> converterFunc;
        private Unit _TargetUnit = Unit.NULL;
        public Unit TargetUnit {
            get {
                return _TargetUnit;
            }
            set {
                if (value != _TargetUnit) {
                    _TargetUnit = value;
                    OnPropertyChanged(nameof(TargetUnit));
                    if (value == Unit.NULL || value == Unit.NONE) {
                        converterFunc = null;
                    } else {
                        converterFunc = Converters.ConverterDictionary[Unit][value];
                    }
                }
            }
        }

        private ConvertMode _ConvertMode = ConvertMode.NONE;
        public ConvertMode ConvertMode {
            get {
                return _ConvertMode;
            }
            set {
                if (value != _ConvertMode) {
                    _ConvertMode = value;
                    OnPropertyChanged(nameof(ConvertMode));
                    if (value != ConvertMode.CHANGE_UNIT) {
                        TargetUnit = Unit.NULL;
                    }
                    if (value != ConvertMode.MULTIPLY) {
                        Multiplier = null;
                    }
                }
            }
        }

        private CastMode _CastToInt = CastMode.NONE;
        public CastMode CastToInt {
            get {
                return _CastToInt;
            }
            set {
                if (value != _CastToInt) {
                    _CastToInt = value;
                    OnPropertyChanged(nameof(TypeName));
                }
            }
        }

        public override int ElementSize => 4;
        public override string TypeName {
            get {
                if (CastToInt != CastMode.NONE) {
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

            if (_ConvertMode != ConvertMode.NONE) {
                for (int i = 0; i < ArrayLength; i++) {
                    float temp = ToSingle(value, i * ElementSize);
                    if (_ConvertMode == ConvertMode.MULTIPLY) {
                        temp *= (float)_Multiplier;
                    }
                    if (_ConvertMode == ConvertMode.CHANGE_UNIT) {
                        temp = converterFunc(temp);
                    }
                    byte[] newValue = GetBytes(temp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }
            

            if (_CastToInt != CastMode.NONE) {
                for (int i = 0; i < ArrayLength; i++) {
                    float temp = ToSingle(value, i * ElementSize);
                    int castedTemp = 0;
                    switch (_CastToInt) {
                        case CastMode.FLOOR:
                            castedTemp = (int)Math.Floor(temp);
                            break;
                        case CastMode.ROUND:
                            castedTemp = (int)Math.Round(temp);
                            break;
                        case CastMode.CEIL:
                            castedTemp = (int)Math.Ceiling(temp);
                            break;
                    }
                    byte[] newValue = GetBytes(castedTemp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }

            return value;
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source);
            if (ArrayLength == 1) {
                return CastToInt != CastMode.NONE ? $"{ToInt32(value, 0)}\r\n" : $"{ToSingle(value, 0)}\r\n";
            } else {
                string result = string.Empty;
                for (int i = 0; i < ArrayLength; i++) {
                    result += CastToInt != CastMode.NONE ? $"{i}: {ToInt32(value, i * ElementSize)}\r\n" : $"{i}: {ToSingle(value, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class U64TelemVar : TelemVar {
        public U64TelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

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
        public FVectorTelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

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
        public DVectorTelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

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
        public FPlacementTelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

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
        public DPlacementTelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

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
        public ASCIITelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

        }

        public override int ElementSize => 64;
        public override string TypeName => "char[]";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            return System.Text.Encoding.ASCII.GetString(GetByteValue(source));
        }
    }

    public class UTF8TelemVar : TelemVar {
        public UTF8TelemVar(string ID, string Name, string Description, string Category, long offset, int MaxArrayLength = 1) : base(ID, Name, Description, Category, offset, MaxArrayLength) {

        }

        public override int ElementSize => 64;
        public override string TypeName => "char[]";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            return System.Text.Encoding.UTF8.GetString(GetByteValue(source));
        }
    }
}
