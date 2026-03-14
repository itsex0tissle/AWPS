using System;
using System.Text;
using System.Collections;
using AWPS.IoT.Measuring;

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
        private static byte[] EnsureValidBuffer(byte[]? buffer, int offset, int length)
        {
            if(buffer is null)
            {
                if(offset is not 0)
                {
                    throw new ArgumentException("Offset must be 0, if buffer is null");
                }
                return new byte[length];
            }
            else
            {
                EnsureBufferLength(buffer, offset, length);
                return buffer;
            }
        }
        public static void WriteByte(byte[] buffer, ref int offset, byte value)
        {
            EnsureBufferLength(buffer, offset, sizeof(byte));
            buffer[offset] = value;
            offset += sizeof(byte);
        }
        public static void WriteSByte(byte[] buffer, ref int offset, sbyte value)
        {
            EnsureBufferLength(buffer, offset, sizeof(sbyte));
            buffer[offset] = (byte)value;
            offset += sizeof(sbyte);
        }
        public static void WriteShort(byte[] buffer, ref int offset, short value)
        {
            EnsureBufferLength(buffer, offset, sizeof(short));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(short);
        }
        public static void WriteUShort(byte[] buffer, ref int offset, ushort value)
        {
            EnsureBufferLength(buffer, offset, sizeof(ushort));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(ushort);
        }
        public static void WriteInt(byte[] buffer, ref int offset, int value)
        {
            EnsureBufferLength(buffer, offset, sizeof(int));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(int);
        }
        public static void WriteUInt(byte[] buffer, ref int offset, uint value)
        {
            EnsureBufferLength(buffer, offset, sizeof(uint));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(uint);
        }
        public static void WriteLong(byte[] buffer, ref int offset, long value)
        {
            EnsureBufferLength(buffer, offset, sizeof(long));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(long);
        }
        public static void WriteULong(byte[] buffer, ref int offset, ulong value)
        {
            EnsureBufferLength(buffer, offset, sizeof(ulong));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(ulong);
        }
        public static void WriteFloat(byte[] buffer, ref int offset, float value)
        {
            EnsureBufferLength(buffer, offset, sizeof(float));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(float);
        }
        public static void WriteDouble(byte[] buffer, ref int offset, double value)
        {
            EnsureBufferLength(buffer, offset, sizeof(double));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(double);
        }
        public static void WriteBool(byte[] buffer, ref int offset, bool value)
        {
            EnsureBufferLength(buffer, offset, sizeof(bool));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(bool);
        }
        public static void WriteChar(byte[] buffer, ref int offset, char value)
        {
            EnsureBufferLength(buffer, offset, sizeof(char));
            BitConverter.GetBytes(value).CopyTo(buffer, offset);
            offset += sizeof(char);
        }
        public static void WriteString(byte[] buffer, ref int offset, string value)
        {
            EnsureBufferLength(buffer, offset, value.Length + sizeof(int));
            WriteInt(buffer, ref offset, value.Length);
            Encoding.UTF8.GetBytes(value).CopyTo(buffer, offset);
            offset += value.Length;
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
            EnsureBufferLength(buffer, offset, sizeof(sbyte));
            var result = (sbyte)buffer[offset];
            offset += sizeof(sbyte);
            return result;
        }
        public static short ReadShort(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(short));
            short result = BitConverter.ToInt16(buffer, offset);
            offset += sizeof(short);
            return result;
        }
        public static ushort ReadUShort(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(ushort));
            ushort result = BitConverter.ToUInt16(buffer, offset);
            offset += sizeof(ushort);
            return result;
        }
        public static int ReadInt(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(int));
            int result = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            return result;
        }
        public static uint ReadUInt(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(uint));
            uint result = BitConverter.ToUInt32(buffer, offset);
            offset += sizeof(uint);
            return result;
        }
        public static long ReadLong(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(long));
            long result = BitConverter.ToInt64(buffer, offset);
            offset += sizeof(long);
            return result;
        }
        public static ulong ReadULong(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(ulong));
            ulong result = BitConverter.ToUInt64(buffer, offset);
            offset += sizeof(ulong);
            return result;
        }
        public static float ReadFloat(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(float));
            float result = BitConverter.ToSingle(buffer, offset);
            offset += sizeof(float);
            return result;
        }
        public static double ReadDouble(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(double));
            double result = BitConverter.ToDouble(buffer, offset);
            offset += sizeof(double);
            return result;
        }
        public static bool ReadBool(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(bool));
            bool result = BitConverter.ToBoolean(buffer, offset);
            offset += sizeof(bool);
            return result;
        }
        public static char ReadChar(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(char));
            char result = BitConverter.ToChar(buffer, offset);
            offset += sizeof(char);
            return result;
        }
        public static string ReadString(byte[] buffer, ref int offset)
        {
            EnsureBufferLength(buffer, offset, sizeof(int));
            int length = ReadInt(buffer, ref offset);
            EnsureBufferLength(buffer, offset, length);
            string result = Encoding.UTF8.GetString(buffer, offset, length);
            offset += length;
            return result;
        }
        public static byte[] SerializeEnumerable(IEnumerable records, byte[]? buffer = null, int offset = 0)
        {
            ArrayList list = new();
            int length = 0;
            foreach(object obj in records)
            {
                if(obj is not BinaryRecord record)
                {
                    throw new ArgumentException("All items in records must be 'BinaryRecord'");
                }
                length += record.ByteLength;
                list.Add(record);
            }
            buffer = EnsureValidBuffer(buffer, offset, length);
            foreach(BinaryRecord record in list)
            {
                record.Serialize(buffer, offset);
                offset += record.ByteLength;
            }
            return buffer;
        }
        public static ArrayList DeserializeEnumerable(byte[] buffer, int offset, int count)
        {
            if(buffer.Length - offset < 2)
            {
                return new ArrayList();
            }
            ArrayList list = new();
            while(offset < count)
            {
                BinaryRecord record;
                switch((BinaryRecordType)buffer[offset])
                {
                    case BinaryRecordType.MeasuringData:
                    {
                        record = new MeasuringDataRecord();
                        record.Deserialize(buffer, offset);
                        offset += record.ByteLength;
                        list.Add(record);
                        break;
                    }
                }
            }
            return list;
        }
        public static ArrayList DeserializeEnumerable(byte[] buffer, int offset = 0)
        {
            return DeserializeEnumerable(buffer, offset, buffer.Length);
        }
        #endregion

        #region Instance
        protected abstract int ChildByteLength { get; }
        public abstract BinaryRecordType Type { get; }
        public abstract byte Version { get; }

        public int ByteLength
        {
            get => ChildByteLength + 2;
        }

        protected abstract void SerializeChild(byte[] buffer, int offset);
        protected abstract void DeserializeChild(byte[] buffer, int offset);
        public byte[] Serialize(byte[]? buffer = null, int offset = 0)
        {
            buffer = EnsureValidBuffer(buffer, offset, ByteLength);
            buffer[offset] = (byte)Type;
            buffer[offset + 1] = Version;
            SerializeChild(buffer, offset + 2);
            return buffer;
        }
        public void Deserialize(byte[] buffer, int offset = 0)
        {
            EnsureBufferLength(buffer, offset, ByteLength);
            if(buffer[offset] != (byte)Type)
            {
                throw new ArgumentException($"Type in record buffer ({(BinaryRecordType)buffer[offset]}) is invalid. Expected type is '{Type}'");
            }
            if(buffer[offset + 1] != Version)
            {
                throw new ArgumentException($"Version in record buffer ({buffer[offset]}) is invalid. Expected version is '{Version}'");
            }
            DeserializeChild(buffer, offset + 2);
        }
        #endregion
    }
}