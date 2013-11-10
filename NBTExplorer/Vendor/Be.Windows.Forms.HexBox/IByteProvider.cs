using System;

namespace Be.Windows.Forms
{
	/// <summary>
	/// Defines a byte provider for HexBox control
	/// </summary>
	public interface IByteProvider
	{
		/// <summary>
		/// Reads a byte from the provider
		/// </summary>
		/// <param name="index">the index of the byte to read</param>
		/// <returns>the byte to read</returns>
		byte ReadByte(long index);
		/// <summary>
		/// Writes a byte into the provider
		/// </summary>
		/// <param name="index">the index of the byte to write</param>
		/// <param name="value">the byte to write</param>
		void WriteByte(long index, byte value);
		/// <summary>
		/// Inserts bytes into the provider
		/// </summary>
		/// <param name="index"></param>
		/// <param name="bs"></param>
		/// <remarks>This method must raise the LengthChanged event.</remarks>
		void InsertBytes(long index, byte[] bs);
		/// <summary>
		/// Deletes bytes from the provider
		/// </summary>
		/// <param name="index">the start index of the bytes to delete</param>
		/// <param name="length">the length of the bytes to delete</param>
		/// <remarks>This method must raise the LengthChanged event.</remarks>
		void DeleteBytes(long index, long length);

		/// <summary>
		/// Returns the total length of bytes the byte provider is providing.
		/// </summary>
        long Length { get; }
		/// <summary>
		/// Occurs, when the Length property changed.
		/// </summary>
		event EventHandler LengthChanged;

		/// <summary>
		/// True, when changes are done.
		/// </summary>
		bool HasChanges();
		/// <summary>
		/// Applies changes.
		/// </summary>
		void ApplyChanges();
		/// <summary>
		/// Occurs, when bytes are changed.
		/// </summary>
		event EventHandler Changed;

		/// <summary>
		/// Returns a value if the WriteByte methods is supported by the provider.
		/// </summary>
		/// <returns>True, when it´s supported.</returns>
		bool SupportsWriteByte();
		/// <summary>
		/// Returns a value if the InsertBytes methods is supported by the provider.
		/// </summary>
		/// <returns>True, when it´s supported.</returns>
		bool SupportsInsertBytes();
		/// <summary>
		/// Returns a value if the DeleteBytes methods is supported by the provider.
		/// </summary>
		/// <returns>True, when it´s supported.</returns>
		bool SupportsDeleteBytes();
	}
}
