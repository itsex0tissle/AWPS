using System;
using System.Text;
using System.Collections;
using AWPS.IoT.BinaryFiles;

namespace AWPS.IoT.BinaryRecords
{
    public abstract class BinaryRecord
    {
        #region Static
        private static void EnsureBufferLength(byte[] buffer, int offset, int length)
        {
            if(buffer.Length - offset < length)
            {
                throw new ArgumentException($"Length of buffer ({buffer.Length - offset}) less than required ({length})");
            }
        }
        public static void WriteByte(byte[] buffer, ref int offset, byte value)
        {
            EnsureBufferLength(buffer, offset, sizeof(byte));
            buffer[offset++] = value;
        }
        public static void WriteSByte(byte[] buffer, ref int offset, sbyte value)
        {
            WriteByte(buffer, ref offset, (byte)value);
        }
        public static void WriteShort(byte[] buffer, ref int offset, short value)
        {
            EnsureBufferLength(buffer, offset, sizeof(short));
            buffer[offset++] = (byte)(value >> 0);
            buffer[offset++] = (byte)(value >> 8);
        }
        public static void WriteUShort(byte[] buffer, ref int offset, ushort value)
        {
            WriteShort(buffer, ref offset, (short)value);
        }
        public static void WriteInt(byte[] buffer, ref int offset, int value)
        {
            EnsureBufferLength(buffer, offset, sizeof(int));
            buffer[offset++] = (byte)(value >> 0);
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
        }
        public static void WriteUInt(byte[] buffer, ref int offset, uint value)
        {
            WriteInt(buffer, ref offset, (int)value);
        }
        public static void WriteLong(byte[] buffer, ref int offset, long value)
        {
            EnsureBufferLength(buffer, offset, sizeof(long));
            buffer[offset++] = (byte)(value >> 0);
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
            buffer[offset++] = (byte)(value >> 32);
            buffer[offset++] = (byte)(value >> 40);
            buffer[offset++] = (byte)(value >> 48);
            buffer[offset++] = (byte)(value >> 56);
        }
        public static void WriteULong(byte[] buffer, ref int offset, ulong value)
        {
            WriteLong(buffer, ref offset, (long)value);
        }
        public static void WriteFloat(byte[] buffer, ref int offset, float value)
        {
            WriteDouble(buffer, ref offset, value);
        }
        public static void WriteDouble(byte[] buffer, ref int offset, double value)
        {
            WriteLong(buffer, ref offset, BitConverter.DoubleToInt64Bits(value));
        }
        public static void WriteBool(byte[] buffer, ref int offset, bool value)
        {
            var b = (byte)(value is false ? 0 : 1);
            WriteByte(buffer, ref offset, b);
        }
        public static void WriteChar(byte[] buffer, ref int offset, char value)
        {
            WriteShort(buffer, ref offset, (short)value);
        }
        public static void WriteString(byte[] buffer, ref int offset, string value)
        {
            EnsureBufferLength(buffer, offset, SizeOfString(value, out byte[] data));
            WriteInt(buffer, ref offset, data.Length);
            data.CopyTo(buffer, offset);
            offset += data.Length;
        }
        public static void WriteRecord(byte[] buffer, ref int offset, BinaryRecord value)
        {
            value.Serialize(buffer, ref offset);
        }
        public static void WriteObject(byte[] buffer, ref int offset, object value)
        {
            switch(value)
            {
                case null:
                {
                    throw new NotSupportedException("Null object is not supported");
                }
                case byte b:
                {
                    WriteByte(buffer, ref offset, b);
                    break;
                }
                case sbyte sb:
                {
                    WriteSByte(buffer, ref offset, sb);
                    break;
                }
                case short s:
                {
                    WriteShort(buffer, ref offset, s);
                    break;
                }
                case ushort us:
                {
                    WriteUShort(buffer, ref offset, us);
                    break;
                }
                case int i:
                {
                    WriteInt(buffer, ref offset, i);
                    break;
                }
                case uint ui:
                {
                    WriteUInt(buffer, ref offset, ui);
                    break;
                }
                case long l:
                {
                    WriteLong(buffer, ref offset, l);
                    break;
                }
                case ulong ul:
                {
                    WriteULong(buffer, ref offset, ul);
                    break;
                }
                case float f:
                {
                    WriteFloat(buffer, ref offset, f);
                    break;
                }
                case double d:
                {
                    WriteDouble(buffer, ref offset, d);
                    break;
                }
                case bool boolean:
                {
                    WriteBool(buffer, ref offset, boolean);
                    break;
                }
                case char c:
                {
                    WriteChar(buffer, ref offset, c);
                    break;
                }
                case string str:
                {
                    WriteString(buffer, ref offset, str);
                    break;
                }
                case BinaryRecord record:
                {
                    WriteRecord(buffer, ref offset, record);
                    break;
                }
                case ICollection collection:
                {
                    WriteCollection(buffer, ref offset, collection);
                    break;
                }
                default:
                {
                    throw new NotSupportedException($"Type '{value.GetType()}' is not supported");
                }
            }
        }
        public static void WriteNullable(byte[] buffer, ref int offset, object? value)
        {
            if(value is null)
            {
                WriteBool(buffer, ref offset, false);
                return;
            }
            WriteBool(buffer, ref offset, true);
            WriteObject(buffer, ref offset, value);
        }
        public static void WriteCollection(byte[] buffer, ref int offset, ICollection value)
        {
            WriteInt(buffer, ref offset, value.Count);
            foreach(object item in value)
            {
                WriteObject(buffer, ref offset, item);
            }
        }
        public static byte ReadByte(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(byte));
            byte result = buffer[offset];
            offset += sizeof(byte);
            return result;
        }
        public static sbyte ReadSByte(byte[] buffer, ref int offset)
        {
            return (sbyte)ReadByte(buffer, ref offset);
        }
        public static short ReadShort(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(short));
            return (short)(
                (buffer[offset++] << 0) |
                (buffer[offset++] << 8)
            );
        }
        public static ushort ReadUShort(byte[] buffer, ref int offset)
        {
            return (ushort)ReadShort(buffer, ref offset);
        }
        public static int ReadInt(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(int));
            return (
                (buffer[offset++] << 0)  |
                (buffer[offset++] << 8)  |
                (buffer[offset++] << 16) |
                (buffer[offset++] << 24)
            );
        }
        public static uint ReadUInt(byte[] buffer, ref int offset)
        {
            return (uint)ReadInt(buffer, ref offset);
        }
        public static long ReadLong(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(int));
            return (
                ((long)buffer[offset++] << 0)  |
                ((long)buffer[offset++] << 8)  |
                ((long)buffer[offset++] << 16) |
                ((long)buffer[offset++] << 24) |
                ((long)buffer[offset++] << 32) |
                ((long)buffer[offset++] << 40) |
                ((long)buffer[offset++] << 48) |
                ((long)buffer[offset++] << 56)
            );
        }
        public static ulong ReadULong(byte[] buffer, ref int offset)
        {
            return (ulong)ReadLong(buffer, ref offset);
        }
        public static float ReadFloat(byte[] buffer, ref int offset)
        {
            return (float)ReadDouble(buffer, ref offset);
        }
        public static double ReadDouble(byte[] buffer, ref int offset)
        {
            return BitConverter.Int64BitsToDouble(ReadLong(buffer, ref offset));
        }
        public static bool ReadBool(byte[] buffer, ref int offset)
        {
            return ReadByte(buffer, ref offset) is not 0;
        }
        public static char ReadChar(byte[] buffer, ref int offset)
        {
            return (char)ReadShort(buffer, ref offset);
        }
        public static string ReadString(byte[] buffer, ref int offset)
        {
            int length = ReadInt(buffer, ref offset);
            EnsureBufferLength(buffer, offset, length);
            string str = Encoding.UTF8.GetString(buffer, offset, length);
            offset += length;
            return str;
        }
        public static BinaryRecord ReadRecord(byte[] buffer, ref int offset)
        {
            int copy_offset = offset;
            switch((BinaryRecord.Type)ReadByte(buffer, ref copy_offset))
            {
                case BinaryRecord.Type.SensorsData:
                {
                    return SensorsDataRecord.Deserialize(buffer, ref offset);
                }
                case BinaryRecord.Type.TimestampMessage:
                {
                    return TimestampMessageRecord.Deserialize(buffer, ref offset);
                }
                case BinaryRecord.Type.GetWifiStatusResponse:
                {
                    return GetWifiStatusResponseRecord.Deserialize(buffer, ref offset);
                }
                case BinaryRecord.Type.PostWifiRequest:
                {
                    return PostWifiRequestRecord.Deserialize(buffer, ref offset);
                }
                case BinaryRecord.Type.PostWifiResponse:
                {
                    return PostWifiResponseRecord.Deserialize(buffer, ref offset);
                }
                case BinaryRecord.Type.MainDataFile:
                {
                    return MainDataFile.Deserialize(buffer, ref offset);
                }
            }
            throw new ArgumentException($"Can`t deserialize the record");
        }
        public static object ReadObject(byte[] buffer, ref int offset, params System.Type[] types)
        {
            if(types.Length is 0)
            {
                throw new ArgumentNullException("Must be provided at least one type");
            }
            System.Type type = types[0];
            if (type == typeof(byte))
            {
                return ReadByte(buffer, ref offset);
            }
            if (type == typeof(sbyte))
            {
                return ReadSByte(buffer, ref offset);
            }
            if (type == typeof(short))
            {
                return ReadShort(buffer, ref offset);
            }
            if (type == typeof(ushort))
            {
                return ReadUShort(buffer, ref offset);
            }
            if (type == typeof(int))
            {
                return ReadInt(buffer, ref offset);
            }
            if (type == typeof(uint))
            {
                return ReadUInt(buffer, ref offset);
            }
            if (type == typeof(long))
            {
                return ReadLong(buffer, ref offset);
            }
            if (type == typeof(ulong))
            {
                return ReadULong(buffer, ref offset);
            }
            if (type == typeof(float))
            {
                return ReadFloat(buffer, ref offset);
            }
            if (type == typeof(double))
            {
                return ReadDouble(buffer, ref offset);
            }
            if (type == typeof(bool))
            {
                return ReadBool(buffer, ref offset);
            }
            if (type == typeof(char))
            {
                return ReadChar(buffer, ref offset);
            }
            if (type == typeof(string))
            {
                return ReadString(buffer, ref offset);
            }
            if (type == typeof(BinaryRecord))
            {
                return ReadRecord(buffer, ref offset);
            }
            if (type == typeof(ICollection))
            {
                System.Type[] nested_types = new System.Type[types.Length - 1];
                Array.Copy(types, 1, nested_types, 0, nested_types.Length);
                return ReadCollection(buffer, ref offset, nested_types);
            }
            throw new NotSupportedException($"Type '{type}' is not supported");
        }
        public static object? ReadNullable(byte[] buffer, ref int offset, params System.Type[] types)
        {
            if(ReadBool(buffer, ref offset) is false)
            {
                return null;
            }
            return ReadObject(buffer, ref offset, types);
        }
        public static ArrayList ReadCollection(byte[] buffer, ref int offset, params System.Type[] types)
        {
            if(types.Length is 0)
            {
                throw new ArgumentNullException("Must be provided at least one type");
            }
            int count = ReadInt(buffer, ref offset);
            ArrayList result = new();
            for(int index = 0; index < count; index++)
            {
                result.Add(ReadObject(buffer, ref offset, types));
            }
            return result;
        }
        public static int SizeOfString(string value)
        {
            return SizeOfString(value, out _);
        }
        public static int SizeOfString(string value, out byte[] bytes)
        {
            bytes = Encoding.UTF8.GetBytes(value);
            return sizeof(int) + bytes.Length;
        }
        public static int SizeOfObject(object value)
        {
            switch(value)
            {
                case null:
                {
                    throw new NotSupportedException("Null object is not supported");
                }
                case byte:
                {
                    return sizeof(byte);
                }
                case sbyte:
                {
                    return sizeof(sbyte);
                }
                case short:
                {
                    return sizeof(short);
                }
                case ushort:
                {
                    return sizeof(ushort);
                }
                case int:
                {
                    return sizeof(int);
                }
                case uint:
                {
                    return sizeof(uint);
                }
                case long:
                {
                    return sizeof(long);
                }
                case ulong:
                {
                    return sizeof(ulong);
                }
                case float:
                {
                    return sizeof(float);
                }
                case double:
                {
                    return sizeof(double);
                }
                case bool:
                {
                    return sizeof(bool);
                }
                case char:
                {
                    return sizeof(char);
                }
                case string str:
                {
                    return SizeOfString(str);
                }
                case BinaryRecord record:
                {
                    return record.ByteLength;
                }
                case ICollection collection:
                {
                    return SizeOfCollection(collection);
                }
                default:
                {
                    throw new NotSupportedException($"Type '{value.GetType()}' is not supported");
                }
            }
        }
        public static int SizeOfNullable(object? value)
        {
            if(value is null)
            {
                return sizeof(bool);
            }
            return sizeof(bool) + SizeOfObject(value);
        }
        public static int SizeOfCollection(ICollection value)
        {
            int size = sizeof(int);
            foreach(object item in value)
            {
                size += SizeOfObject(item);
            }
            return size;
        }
        #endregion

        #region Instance
        protected abstract int ChildByteLength { get; }
        public abstract BinaryRecord.Type RecordType { get; }
        public abstract byte Version { get; }
        public int ByteLength
        {
            get => ChildByteLength + sizeof(BinaryRecord.Type) + sizeof(byte);
        }

        protected abstract void SerializeChild(byte[] buffer, ref int offset);
        protected abstract void DeserializeChild(byte[] buffer, ref int offset);
        public void Serialize(byte[] buffer, ref int offset)
        {
            BinaryRecord.WriteByte(buffer, ref offset, (byte)RecordType);
            BinaryRecord.WriteByte(buffer, ref offset, Version);
            SerializeChild(buffer, ref offset);
        }
        public void Serialize(byte[] buffer, int offset = 0)
        {
            Serialize(buffer, ref offset);
        }
        public byte[] Serialize()
        {
            byte[] buffer = new byte[ByteLength];
            Serialize(buffer);
            return buffer;
        }
        public void Deserialize(byte[] buffer, ref int offset)
        {
            if(BinaryRecord.ReadByte(buffer, ref offset) != (byte)RecordType)
            {
                throw new InvalidOperationException("Invalid record type in buffer");
            }
            if(BinaryRecord.ReadByte(buffer, ref offset) != Version)
            {
                throw new InvalidOperationException("Invalid record version in buffer");
            }
            DeserializeChild(buffer, ref offset);
        }
        public void Deserialize(byte[] buffer, int offset = 0)
        {
            Deserialize(buffer, ref offset);
        }
        #endregion

        #region Nested
        public enum Type
        {
            Unknown,
            SensorsData,
            TimestampMessage,
            GetWifiStatusResponse,
            PostWifiRequest,
            PostWifiResponse,
            MainDataFile,
        }
        #endregion
    }
}