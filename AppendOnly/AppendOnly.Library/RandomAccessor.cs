namespace Renfield.AppendOnly.Library
{
  public interface RandomAccessor
  {
    /// <summary>
    /// Returns the length of the available data
    /// </summary>
    /// <returns>The length of the available data</returns>
    long get_length();

    /// <summary>
    /// Reads a <c>long</c> from the given position
    /// </summary>
    /// <param name="position">The position to start reading from</param>
    /// <returns>A <c>long</c></returns>
    long read_long(long position);

    /// <summary>
    /// Reads a <c>byte</c> array from the given position
    /// </summary>
    /// <param name="position">The position to start reading from</param>
    /// <param name="size">The size of the data to read</param>
    /// <returns>A <c>byte</c> array of the given size (or less)</returns>
    byte[] read_bytes(long position, long size);

    /// <summary>
    /// Writes a <c>long</c> at the given position
    /// </summary>
    /// <param name="position">Position to write at</param>
    /// <param name="value">The value to write</param>
    void write_long(long position, long value);

    /// <summary>
    /// Writes a <c>byte</c> array at the given position
    /// </summary>
    /// <param name="position">Position to write at</param>
    /// <param name="value">The value to write</param>
    void write_bytes(long position, byte[] value);
  }
}