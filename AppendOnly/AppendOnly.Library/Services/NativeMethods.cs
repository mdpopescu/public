using System;
using System.Runtime.InteropServices;

namespace Renfield.AppendOnly.Library.Services
{
    internal static unsafe class NativeMethods
    {
        // Define the Windows system functions that are called by this class via COM Interop:
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr CreateFile
        (
            string fileName, // file name
            uint desiredAccess, // access mode
            uint shareMode, // share mode
            UIntPtr securityAttributes, // Security Attributes
            uint creationDisposition, // how to create
            uint flagsAndAttributes, // file attributes
            IntPtr hTemplateFile // handle to template file
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool ReadFile
        (
            IntPtr hFile, // handle to file
            void* pBuffer, // data buffer
            int numberOfBytesToRead, // number of bytes to read
            int* pNumberOfBytesRead, // number of bytes read
            IntPtr overlapped // overlapped buffer which is used for async I/O.  Not used here.
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool WriteFile
        (
            IntPtr handle, // handle to file
            void* pBuffer, // data buffer
            int numberOfBytesToWrite, // Number of bytes to write.
            int* pNumberOfBytesWritten, // Number of bytes that were written..
            IntPtr overlapped // Overlapped buffer which is used for async I/O.  Not used here.
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle
        (
            IntPtr hObject // handle to object
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool SetFilePointerEx
        (
            IntPtr hFile, // handle to file
            long liDistanceToMove, // no. of bytes to move the file pointer
            long* lpNewFilePointer, // a pointer to a variable to receive the new file pointer. If null, new file pointer is not returned
            uint dwMoveMethod // The starting point for the file pointer to move (FILE_BEGIN, FILE_CURRENT, FILE_END)
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool GetFileSizeEx
        (
            IntPtr hFile, // handle to file
            long* lpFileSize // a pointer to a large integer that will be set to the file size
        );
    }
}