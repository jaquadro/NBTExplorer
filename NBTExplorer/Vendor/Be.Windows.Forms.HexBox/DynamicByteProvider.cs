using System;
using System.Collections.Generic;

namespace Be.Windows.Forms
{
    /// <summary>
    ///     Byte provider for a small amount of data.
    /// </summary>
    public class DynamicByteProvider : IByteProvider
    {
        /// <summary>
        ///     Contains a byte collection.
        /// </summary>
        private readonly List<byte> _bytes;

        /// <summary>
        ///     Contains information about changes.
        /// </summary>
        private bool _hasChanges;

        /// <summary>
        ///     Initializes a new instance of the DynamicByteProvider class.
        /// </summary>
        /// <param name="data"></param>
        public DynamicByteProvider(byte[] data) : this(new List<byte>(data))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the DynamicByteProvider class.
        /// </summary>
        /// <param name="bytes"></param>
        public DynamicByteProvider(List<byte> bytes)
        {
            _bytes = bytes;
        }

        /// <summary>
        ///     Gets the byte collection.
        /// </summary>
        public List<byte> Bytes => _bytes;

        /// <summary>
        ///     Raises the Changed event.
        /// </summary>
        private void OnChanged(EventArgs e)
        {
            _hasChanges = true;

            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        ///     Raises the LengthChanged event.
        /// </summary>
        private void OnLengthChanged(EventArgs e)
        {
            if (LengthChanged != null)
                LengthChanged(this, e);
        }

        #region IByteProvider Members

        /// <summary>
        ///     True, when changes are done.
        /// </summary>
        public bool HasChanges()
        {
            return _hasChanges;
        }

        /// <summary>
        ///     Applies changes.
        /// </summary>
        public void ApplyChanges()
        {
            _hasChanges = false;
        }

        /// <summary>
        ///     Occurs, when the write buffer contains new changes.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        ///     Occurs, when InsertBytes or DeleteBytes method is called.
        /// </summary>
        public event EventHandler LengthChanged;

        /// <summary>
        ///     Reads a byte from the byte collection.
        /// </summary>
        /// <param name="index">the index of the byte to read</param>
        /// <returns>the byte</returns>
        public byte ReadByte(long index)
        {
            return _bytes[(int)index];
        }

        /// <summary>
        ///     Write a byte into the byte collection.
        /// </summary>
        /// <param name="index">the index of the byte to write.</param>
        /// <param name="value">the byte</param>
        public void WriteByte(long index, byte value)
        {
            _bytes[(int)index] = value;
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Deletes bytes from the byte collection.
        /// </summary>
        /// <param name="index">the start index of the bytes to delete.</param>
        /// <param name="length">the length of bytes to delete.</param>
        public void DeleteBytes(long index, long length)
        {
            var internal_index = (int)Math.Max(0, index);
            var internal_length = (int)Math.Min((int)Length, length);
            _bytes.RemoveRange(internal_index, internal_length);

            OnLengthChanged(EventArgs.Empty);
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Inserts byte into the byte collection.
        /// </summary>
        /// <param name="index">the start index of the bytes in the byte collection</param>
        /// <param name="bs">the byte array to insert</param>
        public void InsertBytes(long index, byte[] bs)
        {
            _bytes.InsertRange((int)index, bs);

            OnLengthChanged(EventArgs.Empty);
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Gets the length of the bytes in the byte collection.
        /// </summary>
        public long Length => _bytes.Count;

        /// <summary>
        ///     Returns true
        /// </summary>
        public virtual bool SupportsWriteByte()
        {
            return true;
        }

        /// <summary>
        ///     Returns true
        /// </summary>
        public virtual bool SupportsInsertBytes()
        {
            return true;
        }

        /// <summary>
        ///     Returns true
        /// </summary>
        public virtual bool SupportsDeleteBytes()
        {
            return true;
        }

        #endregion IByteProvider Members
    }
}