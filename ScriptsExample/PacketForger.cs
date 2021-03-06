using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BambooNetCode
{
    public enum ServerPackets
    {
        Welcome = 1,
        ClientJoinGroup,
        ClientLeftGroup,
        UpdateMatchMakingInfo,
        GroupLeftMatchMaking,
        PlayerLoadLevelScene,
        PlayerCharacterSpawn,
        PlayerCharacterUpdate,
        PlayerInfoUpdate,
        EntityTransformUpdate,
        GameInstanceTimerUpdate,
        MatchMakingGameOver,
        BoxMapPlayerEliminationBegin,
        BoxMapPlayerEliminationCanceled,
        BoxMapPlayerEliminated
    }
    public enum ClientPackets
    {
        WelcomeReceived = 1,
        ClientJoinGroupWithSerialCodeReceive,
        ClientLeftGroupReceive,
        GroupQueeMatchMakingReceive,
        GroupUnqueeMatchMakingReceive,
        ClientHasLoadedLevelScene,
        ClientInputsReceive,
        CharacterPlayerInfoUpdateReceive
    }
    public class PacketForger : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public PacketForger()
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="_id">The packet ID.</param>
        public PacketForger(int _id)
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0

            Write(_id); // Write packet id to the buffer
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public PacketForger(byte[] _data)
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0

            SetBytes(_data);
        }

        #region Functions
        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="_value">The int to insert.</param>
        public void InsertInt(int _value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Return the remaining length (unread)
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="_shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); // Clear buffer
                readableBuffer = null;
                readPos = 0; // Reset readPos
            }
            else
            {
                readPos -= 4; // "Unread" the last read int
            }
        }

        public static Vector3 ConvertToUnityVector3(System.Numerics.Vector3 _vector)
        {
            return new Vector3(_vector.X, _vector.Y, _vector.Z);
        }
        public static Quaternion ConvertFromToUnityQuaternion(System.Numerics.Quaternion _quaternion)
        {
            return new Quaternion(_quaternion.X, _quaternion.Y, _quaternion.Z, _quaternion.W);
        }
        public static System.Numerics.Vector3 ConvertToSNVector3(Vector3 _vector)
        {
            return new System.Numerics.Vector3(_vector.x, _vector.y, _vector.z);
        }
        public static System.Numerics.Quaternion ConvertToSNQuaternion(Quaternion _quaternion)
        {
            return new System.Numerics.Quaternion(_quaternion.x, _quaternion.y, _quaternion.z, _quaternion.w);
        }

        #endregion

        #region Write Data
        /// <summary>Adds a byte to the packet.</summary>
        /// <param name="_value">The byte to add.</param>
        public void Write(byte _value)
        {
            buffer.Add(_value);
        }
        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="_value">The byte array to add.</param>
        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }
        /// <summary>Adds a short to the packet.</summary>
        /// <param name="_value">The short to add.</param>
        public void Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds an int to the packet.</summary>
        /// <param name="_value">The int to add.</param>
        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds an int[] to the packet.</summary>
        /// <param name="_value">The int[] to add.</param>
        public void Write(int[] _value)
        {
            Write(_value.Length);
            for (int i = 0; i < _value.Length; i++)
                Write(_value[i]);
        }
        /// <summary>Adds an float[] to the packet.</summary>
        /// <param name="_value">The float[] to add.</param>
        public void Write(float[] _value)
        {
            Write(_value.Length);
            for (int i = 0; i < _value.Length; i++)
                Write(_value[i]);
        }
        /// <summary>Adds an bool[] to the packet.</summary>
        /// <param name="_value">The bool[] to add.</param>
        public void Write(bool[] _value)
        {
            Write(_value.Length);
            for (int i = 0; i < _value.Length; i++)
                Write(_value[i]);
        }
        /// <summary>Adds a long to the packet.</summary>
        /// <param name="_value">The long to add.</param>
        public void Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a float to the packet.</summary>
        /// <param name="_value">The float to add.</param>
        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a bool to the packet.</summary>
        /// <param name="_value">The bool to add.</param>
        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a string to the packet.</summary>
        /// <param name="_value">The string to add.</param>
        public void Write(string _value)
        {
            Write(_value.Length); // Add the length of the string to the packet
            buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
        }
        /// <summary>Adds a Vector3 to the packet.</summary>
        /// <param name="_value">The Vector3 to add.</param>
        public void Write(Vector3 _value)
        {
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
        }
        /// <summary>Adds a SN.Vector3 to the packet.</summary>
        /// <param name="_value">The SN.Vector3 to add.</param>
        public void Write(System.Numerics.Vector3 _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
        }
        /// <summary>Adds a Quaternion to the packet.</summary>
        /// <param name="_value">The Quaternion to add.</param>
        public void Write(Quaternion _value)
        {
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
            Write(_value.w);
        }
        /// <summary>Adds a Tranform to the packet.</summary>
        /// <param name="_value">The Transform to add.</param>
        public void Write(TransformNetwork _value)
        {
            Write(_value.position);
            Write(_value.rotation);
            Write(_value.scale);
        }
        /// <summary>Adds a Tranform[] to the packet.</summary>
        /// <param name="_value">The Transform to add.</param>
        public void Write(TransformNetwork[] _value)
        {
            int length = _value.Length;
            for (int i = 0; i < length; i++)
            {
                Write(_value);
            }
        }
        /// <summary>Adds a PlayerInfo to the packet.</summary>
        /// <param name="_value">The PlayerInfo to add.</param>
        public void Write(PlayerInfo _value)
        {
            Write(_value.clientID);
            Write(_value.groupID);
            Write(_value.username);
            Write(_value.headID);
            Write(_value.skinID);
        }
        /// <summary>Adds a PlayerInfo[] to the packet.</summary>
        /// <param name="_value">The PlayerInfo[] to add.</param>
        public void Write(PlayerInfo[] _value)
        {
            Write(_value.Length);
            for (int i = 0; i < _value.Length; i++)
                Write(_value[i]);
        }

        #endregion

        #region Read Data
        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte _value = readableBuffer[readPos]; // Get the byte at readPos' position
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="_length">The length of the byte array.</param>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Get the bytes at readPos' position with a range of _length
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += _length; // Increase readPos by _length
                }
                return _value; // Return the bytes
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                short _value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
                if (_moveReadPos)
                {
                    // If _moveReadPos is true and there are unread bytes
                    readPos += 2; // Increase readPos by 2
                }
                return _value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                int _value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return _value; // Return the int
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }
        /// <summary>Reads an int[] from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public int[] ReadIntArray(bool _moveReadPos = true)
        {
            int length = ReadInt(_moveReadPos);
            int[] tab = new int[length];
            for (int i = 0; i < length; i++)
                tab[i] = ReadInt();
            return tab;
        }

        /// <summary>Reads an float[] from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public float[] ReadFloatArray(bool _moveReadPos = true)
        {
            int length = ReadInt(_moveReadPos);
            float[] tab = new float[length];
            for (int i = 0; i < length; i++)
                tab[i] = ReadFloat();
            return tab;
        }
        /// <summary>Reads an bool[] from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool[] ReadBoolArray(bool _moveReadPos = true)
        {
            int length = ReadInt(_moveReadPos);
            bool[] tab = new bool[length];
            for (int i = 0; i < length; i++)
                tab[i] = ReadBool();
            return tab;
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                long _value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 8; // Increase readPos by 8
                }
                return _value; // Return the long
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                float _value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return _value; // Return the float
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the bool
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt(); // Get the length of the string
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
                if (_moveReadPos && _value.Length > 0)
                {
                    // If _moveReadPos is true string is not empty
                    readPos += _length; // Increase readPos by the length of the string
                }
                return _value; // Return the string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        /// <summary>Reads a Vector3 from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public Vector3 ReadVector3(bool _moveReadPos = true)
        {
            return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }

        /// <summary>Reads a Quaternion from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        /// <summary>Reads a Transform from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public TransformNetwork ReadTransform(bool _moveReadPos = true)
        {
            return new TransformNetwork(ReadVector3(_moveReadPos), ReadQuaternion(_moveReadPos), ReadVector3(_moveReadPos));
        }
        /// <summary>Reads a Transform[] from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public TransformNetwork[] ReadTransformArray(bool _moveReadPos = true)
        {
            List<TransformNetwork> tnList = new List<TransformNetwork>();
            int length = ReadInt();
            for(int i = 0; i< length; i++)
            {
                tnList.Add(ReadTransform());
            }
            return tnList.ToArray();
        }

        /// <summary>Reads a PlayerInfo from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public PlayerInfo ReadPlayerInfo(bool _moveReadPos = true)
        {
            return new PlayerInfo(ReadInt(), ReadInt(), ReadString() , ReadInt(), ReadInt());
        }
        /// <summary>Reads a PlayerInfo from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public PlayerInfo[] ReadPlayerInfoArray(bool _moveReadPos = true)
        {
            List<PlayerInfo> pI = new List<PlayerInfo>();
            int length = ReadInt();
            for (int i = 0; i < length; i++)
                pI.Add(ReadPlayerInfo());
            return pI.ToArray();
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
