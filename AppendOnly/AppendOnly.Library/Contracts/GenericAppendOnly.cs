using System.Collections.Generic;

namespace Renfield.AppendOnly.Library.Contracts
{
    public interface GenericAppendOnly<T>
    {
        long[] GetIndex();

        /// <summary>
        ///   Adds a record to the file
        /// </summary>
        /// <param name="record">Record to add</param>
        void Append(T record);

        /// <summary>
        ///   Returns the i-th record
        /// </summary>
        /// <param name="i">The record number (0-based)</param>
        /// <returns>The contents of the record</returns>
        T Read(int i);

        /// <summary>
        ///   Returns all records from the i-th on
        /// </summary>
        /// <param name="i">The record number (0-based) to start reading from</param>
        /// <returns>The records from the i-th on</returns>
        IEnumerable<T> ReadFrom(int i);
    }
}