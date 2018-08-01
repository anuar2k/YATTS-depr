using System;
using System.ComponentModel;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Linq;
using static System.BitConverter;
using static System.Text.Encoding;

namespace YATTS {
    public abstract class TelemVar : INotifyPropertyChanged {
        public TelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) {
            if (MaxArrayLength < 1) {
                throw new ArgumentOutOfRangeException("Array length cannot be lower than 1");
            }
            if (Position < 0) {
                throw new ArgumentOutOfRangeException("The position cannot be lower than 0");
            }

            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Category = Category;
            this.Position = Position;
            this.MaxArrayLength = MaxArrayLength;
            ArrayLength = MaxArrayLength;
            MaxDataSize = MaxArrayLength * ElementSize;
        }

        public string ID { get; }
        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
        public long Position { get; }
        public int MaxArrayLength { get; }
        public int MaxDataSize { get; }

        public abstract int ElementSize { get; }
        public abstract string BasicTypeName { get; }

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

        private int _ArrayLength;
        public virtual int ArrayLength {
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

        private int _DataSize;
        public int DataSize {
            get {
                return _DataSize;
            }
            private set {
                if (value != _DataSize) {
                    _DataSize = value;
                    OnPropertyChanged(nameof(DataSize));
                }
            }
        }

        public virtual string TypeName {
            get {
                if (MaxArrayLength == 1) {
                    return BasicTypeName;
                }
                else {
                    if (ArrayLength == 1) {
                        return $"{BasicTypeName}[{MaxArrayLength}] -> {BasicTypeName}";
                    }
                    else {
                        return $"{BasicTypeName}[{MaxArrayLength}] -> {BasicTypeName}[{ArrayLength}]";
                    }
                }
            }
        }

        public virtual byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray) {
            int bytesToRead = wholeArray ? MaxDataSize : DataSize;
            byte[] value = new byte[bytesToRead];
            source.ReadArray(Position, value, 0, bytesToRead);
            return value;

        }

        public abstract string GetStringValue(MemoryMappedViewAccessor source);

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string sender) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }

    //TODO: TAKE CARE OF STRINGVALUE IN ALL INHERENTS
    public abstract class StringableTelemVar : TelemVar {
        public StringableTelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) : base(ID, Name, Description, Category, Position, MaxArrayLength) {

        }

        private bool _Stringify = false;
        public bool Stringify {
            get {
                return _Stringify;
            }
            set {
                if (value != _Stringify) {
                    _Stringify = value;
                    if (value) {
                        GenerateStringifyBuffer();
                    }
                    else {
                        StringifyBuffer = null;
                    }
                    OnPropertyChanged(nameof(TypeName));
                }
            }
        }

        protected abstract int MaxStringifiedElementLength { get; }

        protected ByteBuffer StringifyBuffer { get; private set; }
        private void GenerateStringifyBuffer() {
            int length = MaxStringifiedElementLength * ArrayLength;
            if (StringifyBuffer != null) {
                StringifyBuffer.Length = length;
            } else {
                StringifyBuffer = new ByteBuffer(length);
            }
        }

        public override int ArrayLength {
            get {
                return base.ArrayLength;
            }
            set {
                if (value != base.ArrayLength) {
                    base.ArrayLength = value;
                    if (Stringify) {
                        GenerateStringifyBuffer();
                    }
                }
            }
        }

        public override string TypeName {
            get {
                if (Stringify) {
                    if (MaxArrayLength == 1) {
                        return $"{BasicTypeName} -> char[]";
                    }
                    else {
                        if (ArrayLength == 1) {
                            return $"{BasicTypeName}[{MaxArrayLength}] -> char[]";
                        }
                        else {
                            return $"{BasicTypeName}[{MaxArrayLength}] -> char[{ArrayLength}][]";
                        }
                    }
                }
                else {
                    return base.TypeName;
                }
            }
        }
    }

    public class BoolTelemVar : TelemVar {
        public BoolTelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArraySize = 1) : base(ID, Name, Description, Category, Position, MaxArraySize) {

        }

        public override int ElementSize => 1;
        public override string BasicTypeName => "bool";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source, true);

            if (MaxArrayLength == 1) {
                return $"{ToBoolean(value, 0)}\r\n";
            }
            else {
                string result = string.Empty;
                for (int i = 0; i < MaxArrayLength; i++) {
                    result += $"{i}: {ToBoolean(value, i)}\r\n";
                }
                return result;
            }
        }

    }

    public class U8TelemVar : StringableTelemVar {
        public U8TelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArraySize = 1) : base(ID, Name, Description, Category, Position, MaxArraySize) {

        }

        public override int ElementSize => 1;
        public override string BasicTypeName => "byte";
        protected override int MaxStringifiedElementLength => 4; //1 length + 3 data

        public override byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray) {
            byte[] value = base.GetByteValue(source, wholeArray);

            if (Stringify) {
                int elemsToRead = wholeArray ? MaxArrayLength : ArrayLength;

                for (int i = 0; i < elemsToRead; i++) {
                    string stringified = value[i].ToString(CultureInfo.InvariantCulture);
                    byte[] buffer = new byte[stringified.Length + 1];
                    buffer[0] = (byte)stringified.Length;
                    ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);

                    StringifyBuffer.Append(buffer);
                }

                return StringifyBuffer.Flush();
            }
            else {
                return value;
            }
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] baseValue = base.GetByteValue(source, true);

            if (MaxArrayLength == 1) {
                return $"{baseValue[0]}\r\n";
            }
            else {
                string result = string.Empty;
                for (int i = 0; i < MaxArrayLength; i++) {
                    result += $"{i}: {baseValue[i]}\r\n";
                }
                return result;
            }
        }
    }

    public class U32TelemVar : StringableTelemVar {
        public U32TelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArraySize = 1) : base(ID, Name, Description, Category, Position, MaxArraySize) {

        }

        public override int ElementSize => 4;
        public override string BasicTypeName => "uint32_t";
        protected override int MaxStringifiedElementLength => 11; //1 length + 10 data

        public override byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray) {
            byte[] value = base.GetByteValue(source, wholeArray);

            if (Stringify) {
                int elemsToRead = wholeArray ? MaxArrayLength : ArrayLength;
                for (int i = 0; i < elemsToRead; i++) {
                    string stringified = ToUInt32(value, i * ElementSize).ToString(CultureInfo.InvariantCulture);
                    byte[] buffer = new byte[stringified.Length + 1];
                    buffer[0] = (byte)stringified.Length;
                    ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);

                    StringifyBuffer.Append(buffer);
                }
                return StringifyBuffer.Flush();
            }
            else {
                return value;
            }
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] baseValue = base.GetByteValue(source, true);

            if (MaxArrayLength == 1) {
                return $"{ToUInt32(baseValue, 0)}\r\n";
            }
            else {
                string result = string.Empty;
                for (int i = 0; i < MaxArrayLength; i++) {
                    result += $"{i}: {ToUInt32(baseValue, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class S32TelemVar : StringableTelemVar {
        public S32TelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArraySize = 1) : base(ID, Name, Description, Category, Position, MaxArraySize) {

        }

        public override int ElementSize => 4;
        public override string BasicTypeName => "int32_t";
        protected override int MaxStringifiedElementLength => 12; //1 length + 11 data

        public override byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray) {
            byte[] value = base.GetByteValue(source, wholeArray);

            if (Stringify) {
                int elemsToRead = wholeArray ? MaxArrayLength : ArrayLength;
                for (int i = 0; i < elemsToRead; i++) {
                    string stringified = ToInt32(value, i * ElementSize).ToString(CultureInfo.InvariantCulture);
                    byte[] buffer = new byte[stringified.Length + 1];
                    buffer[0] = (byte)stringified.Length;
                    ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);

                    StringifyBuffer.Append(buffer);
                }
                return StringifyBuffer.Flush();
            }
            else {
                return value;
            }
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] baseValue = base.GetByteValue(source, true);

            if (MaxArrayLength == 1) {
                return $"{ToInt32(baseValue, 0)}\r\n";
            }
            else {
                string result = string.Empty;
                for (int i = 0; i < MaxArrayLength; i++) {
                    result += $"{i}: {ToSingle(baseValue, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class FloatTelemVar : StringableTelemVar {
        public FloatTelemVar(string ID, string Name, string Description, string Category, long Position, Unit Unit, int MaxArraySize = 1) : base(ID, Name, Description, Category, Position, MaxArraySize) {
            this.Unit = Unit;
            UpdateStringifyFormat();
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
        private Unit _TargetUnit = Unit.NONE;
        public Unit TargetUnit {
            get {
                return _TargetUnit;
            }
            set {
                if (value != _TargetUnit) {
                    _TargetUnit = value;
                    OnPropertyChanged(nameof(TargetUnit));

                    if (value == Unit.NONE) {
                        converterFunc = null;
                    }
                    else {
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
                        TargetUnit = Unit.NONE;
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
        public override string BasicTypeName => "float";
        protected override int MaxStringifiedElementLength => 21; //1 length + 1 sign + 10 integer data + 1 length + 8 decimal fraction = 21
        public override string TypeName {
            get {
                if (Stringify || CastToInt == CastMode.NONE) {
                    return base.TypeName;
                } else {
                    if (MaxArrayLength == 1) {
                        return "float -> int32_t";
                    }
                    else {
                        if (ArrayLength == 1) {
                            return $"float[{MaxArrayLength}] -> int32_t";
                        }
                        else {
                            return $"float[{MaxArrayLength}] -> int32_t[{ArrayLength}]";
                        }
                    }
                }
            }
        }

        private string StringifyFormat;
        private void UpdateStringifyFormat() {
            string format = "0.";
            for (int i = 0; i < _DecimalLength; i++) {
                format += '#';
            }
            StringifyFormat = format;
        }

        private int _DecimalLength = 2;
        public int DecimalLength {
            get {
                return _DecimalLength;
            }
            set {
                if (value < 0 || value > 8) {
                    throw new ArgumentOutOfRangeException("DecimalLength must be between 0 and 8");
                }
                if (value != _DecimalLength) {
                    _DecimalLength = value;
                    OnPropertyChanged(nameof(DecimalLength));
                    UpdateStringifyFormat();
                }
            }
        }

        private byte[] GetConvertedValue(MemoryMappedViewAccessor source, bool wholeArray = false) {
            int bytesToRead = wholeArray ? MaxDataSize : DataSize;
            int elemsToRead = wholeArray ? MaxArrayLength : ArrayLength;

            byte[] value = new byte[bytesToRead];
            source.ReadArray(Position, value, 0, bytesToRead);

            if (_ConvertMode != ConvertMode.NONE) {
                for (int i = 0; i < elemsToRead; i++) {
                    float temp = ToSingle(value, i * ElementSize);
                    if (_ConvertMode == ConvertMode.MULTIPLY) {
                        if (_Multiplier != null) {
                            temp *= (float)_Multiplier;
                        }
                    }
                    if (_ConvertMode == ConvertMode.CHANGE_UNIT) {
                        if (converterFunc != null) {
                            temp = converterFunc(temp);
                        }
                    }
                    byte[] newValue = GetBytes(temp);
                    Array.Copy(newValue, 0, value, i * ElementSize, ElementSize);
                }
            }

            if (_CastToInt != CastMode.NONE) {
                for (int i = 0; i < elemsToRead; i++) {
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

        public override byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray = false) {
            byte[] value = GetConvertedValue(source, wholeArray);

            if (Stringify) {
                int elemsToRead = wholeArray ? MaxArrayLength : ArrayLength;

                for (int i = 0; i < elemsToRead; i++) {
                    byte[] buffer;
                    if (_CastToInt == CastMode.NONE) {
                        string stringified = ToSingle(value, i * ElementSize).ToString(StringifyFormat, CultureInfo.InvariantCulture);
                        if (_DecimalLength > 0) {
                            if (stringified.IndexOf('.') != -1) {
                                string[] parts = stringified.Split('.');
                                if (parts[0].Length > 11 || parts[1].Length > 8) {
                                    buffer = ASCII.GetBytes(new string('#', 21));
                                    buffer[0] = 11;
                                    buffer[12] = 8;
                                }
                                else {
                                    buffer = new byte[parts[0].Length + parts[1].Length + 2];

                                    buffer[0] = (byte)parts[0].Length;
                                    ASCII.GetBytes(parts[0], 0, parts[0].Length, buffer, 1);

                                    buffer[parts[0].Length + 1] = (byte)parts[1].Length;
                                    ASCII.GetBytes(parts[1], 0, parts[1].Length, buffer, parts[0].Length + 2);
                                }
                            }
                            else {
                                if (stringified.Length > 11) {
                                    buffer = ASCII.GetBytes(new string('#', 13));

                                    buffer[0] = 11;
                                    buffer[12] = 0;
                                }
                                else {
                                    buffer = new byte[stringified.Length + 2];

                                    buffer[0] = (byte)stringified.Length;
                                    buffer[stringified.Length + 1] = 0;
                                    ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);
                                }
                            }
                        }
                        else {
                            if (stringified.Length > 11) {
                                buffer = ASCII.GetBytes(new string('#', 12));

                                buffer[0] = 11;
                            }
                            else {
                                buffer = new byte[stringified.Length + 1];

                                buffer[0] = (byte)stringified.Length;
                                ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);
                            }
                        }
                    }
                    else {
                        string stringified = ToInt32(value, i * ElementSize).ToString(CultureInfo.InvariantCulture);
                        buffer = new byte[stringified.Length + 1];
                        buffer[0] = (byte)stringified.Length;
                        ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);
                    }

                    StringifyBuffer.Append(buffer);
                }

                return StringifyBuffer.Flush();
            }
            else {
                return value;
            }
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            if (_ConvertMode == ConvertMode.NONE && _CastToInt == CastMode.NONE && !Stringify) {
                byte[] value = base.GetByteValue(source, true);

                if (MaxArrayLength == 1) {
                    return $"{ToSingle(value, 0)}\r\n";
                }
                else {
                    string result = string.Empty;
                    for (int i = 0; i < MaxArrayLength; i++) {
                        result += $"{i}: {ToSingle(value, i * ElementSize)}\r\n";
                    }
                    return result;
                }
            }
            else {
                byte[] baseValue = base.GetByteValue(source, true);
                byte[] convertedValue = GetConvertedValue(source, true);

                string result = string.Empty;
                if (MaxArrayLength == 1) {
                    result += $"Orig: {ToSingle(baseValue, 0)}\r\n";
                    result += _CastToInt == CastMode.NONE ? $"Conv: {ToSingle(convertedValue, 0)}" : $"Conv: {ToInt32(convertedValue, 0)}";
                }
                else {
                    for (int i = 0; i < MaxArrayLength; i++) {
                        result += $"Orig[{i}]: {ToSingle(baseValue, i * ElementSize)}\r\n";
                        result += _CastToInt == CastMode.NONE ? $"Conv[{i}]: {ToSingle(convertedValue, i * ElementSize)}\r\n\r\n" : $"Conv[{i}]: {ToInt32(convertedValue, i * ElementSize)}\r\n\r\n";
                    }
                }
                return result;
            }
        }
    }

    public class U64TelemVar : StringableTelemVar {
        public U64TelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) : base(ID, Name, Description, Category, Position, MaxArrayLength) {

        }

        public override int ElementSize => 8;
        public override string BasicTypeName => "uint64_t";
        protected override int MaxStringifiedElementLength => 20;

        public override byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray) {
            byte[] value = base.GetByteValue(source, wholeArray);

            if (Stringify) {
                int elemsToRead = wholeArray ? MaxArrayLength : ArrayLength;
                for (int i = 0; i < elemsToRead; i++) {
                    string stringified = ToUInt64(value, i * ElementSize).ToString(CultureInfo.InvariantCulture);
                    byte[] buffer = new byte[stringified.Length + 1];
                    buffer[0] = (byte)stringified.Length;
                    ASCII.GetBytes(stringified, 0, stringified.Length, buffer, 1);
                    StringifyBuffer.Append(buffer);
                }

                return StringifyBuffer.Flush();
            }
            else {
                return value;
            }
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] baseValue = base.GetByteValue(source, true);

            if (MaxArrayLength == 1) {
                return $"{ToUInt64(baseValue, 0)}\r\n";
            }
            else {
                string result = string.Empty;
                for (int i = 0; i < MaxArrayLength; i++) {
                    result += $"{i}: {ToUInt64(baseValue, i * ElementSize)}\r\n";
                }
                return result;
            }
        }
    }

    public class FVectorTelemVar : TelemVar {
        public FVectorTelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) : base(ID, Name, Description, Category, Position, MaxArrayLength) {

        }

        public override int ElementSize => 12;
        public override string BasicTypeName => "fvector";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source, true);
            string result = string.Empty;

            if (MaxArrayLength == 1) {
                result += $"X: {ToSingle(value, 0)}\r\n";
                result += $"Y: {ToSingle(value, 4)}\r\n";
                result += $"Z: {ToSingle(value, 8)}\r\n";
                return result;
            }
            else {
                for (int i = 0; i < MaxArrayLength; i++) {
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
        public DVectorTelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) : base(ID, Name, Description, Category, Position, MaxArrayLength) {

        }

        public override int ElementSize => 24;
        public override string BasicTypeName => "dvector";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source, true);
            string result = string.Empty;

            if (MaxArrayLength == 1) {
                result += $"X: {ToDouble(value, 0)}\r\n";
                result += $"Y: {ToDouble(value, 8)}\r\n";
                result += $"Z: {ToDouble(value, 16)}\r\n";
                return result;
            }
            else {
                for (int i = 0; i < MaxArrayLength; i++) {
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
        public FPlacementTelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) : base(ID, Name, Description, Category, Position, MaxArrayLength) {

        }

        public override int ElementSize => 24;
        public override string BasicTypeName => "fplacement";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source, true);
            string result = string.Empty;

            if (MaxArrayLength == 1) {
                result += $"X: {ToSingle(value, 0)}\r\n";
                result += $"Y: {ToSingle(value, 4)}\r\n";
                result += $"Z: {ToSingle(value, 8)}\r\n";
                result += $"Head: {ToSingle(value, 12)}\r\n";
                result += $"Pitch: {ToSingle(value, 16)}\r\n";
                result += $"Roll: {ToSingle(value, 20)}\r\n";
                return result;
            }
            else {
                for (int i = 0; i < MaxArrayLength; i++) {
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
        public DPlacementTelemVar(string ID, string Name, string Description, string Category, long Position, int MaxArrayLength = 1) : base(ID, Name, Description, Category, Position, MaxArrayLength) {

        }

        public override int ElementSize => 40; //36 + 4 byte explicit padding -> see scssdk_value.h
        public override string BasicTypeName => "dplacement";

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            byte[] value = GetByteValue(source, true);
            string result = string.Empty;
            if (MaxArrayLength == 1) {
                result += $"X: {ToDouble(value, 0)}\r\n";
                result += $"Y: {ToDouble(value, 8)}\r\n";
                result += $"Z: {ToDouble(value, 16)}\r\n";
                result += $"Head: {ToSingle(value, 24)}\r\n";
                result += $"Pitch: {ToSingle(value, 28)}\r\n";
                result += $"Roll: {ToSingle(value, 32)}\r\n";
                return result;
            }
            else {
                for (int i = 0; i < MaxArrayLength; i++) {
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

    //convert to stringified
    public class StringTelemVar : StringableTelemVar {
        public StringTelemVar(string ID, string Name, string Description, string Category, long Position) : base(ID, Name, Description, Category, Position) {

        }

        public override int ElementSize => 64;
        public override string BasicTypeName => "char[64]";
        protected override int MaxStringifiedElementLength => 64;

        public override byte[] GetByteValue(MemoryMappedViewAccessor source, bool wholeArray) {
            byte[] value = base.GetByteValue(source, wholeArray);

            if (Stringify) {
                string valueStr = UTF8.GetString(value).TrimEnd('\0');
                byte[] newValue = new byte[valueStr.Length + 1];
                newValue[0] = (byte)valueStr.Length;
                UTF8.GetBytes(valueStr, 0, valueStr.Length, newValue, 1);
                return newValue;
            }
            else {
                return value;
            }
        }

        public override string GetStringValue(MemoryMappedViewAccessor source) {
            return UTF8.GetString(base.GetByteValue(source, false)).TrimEnd('\0');
        }
    }
}
