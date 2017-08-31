using System;
using System.ComponentModel;
using Renfield.AppendOnly.Library.Models;

// based on https://github.com/JoshKeegan/PiSearch/blob/master/StringSearch/IO/WinFileIO.cs

namespace Renfield.AppendOnly.Library.Services
{
    public unsafe class WinFileIO : IDisposable
    {
        // This class provides the capability to utilize the ReadFile and Writefile windows IO functions.  These functions
        // are the most efficient way to perform file I/O from C# or even C++.  The constructor with the buffer and buffer
        // size should usually be called to init this class.  PinBuffer is provided as an alternative.  The reason for this
        // is because a pointer needs to be obtained before the ReadFile or WriteFile functions are called.
        //
        // Error handling - In each public function of this class where an error can occur, an ApplicationException is
        // thrown with the Win32Exception message info if an error is detected.  If no exception is thrown, then a normal
        // return is considered success.
        // 
        // This code is not thread safe.  Thread control primitives need to be added if running this in a multi-threaded
        // environment.
        //
        // The recommended and fastest function for reading from a file is to call the ReadBlocks method.
        // The recommended and fastest function for writing to a file is to call the WriteBlocks method.
        //
        // License and disclaimer:
        // This software is free to use by any individual or entity for any endeavor for profit or not.
        // Even though this code has been tested and automated unit tests are provided, there is no guarantee that
        // it will run correctly with your system or environment.  I am not responsible for any failure and you agree
        // that you accept any and all risk for using this software.
        //
        //
        // Written by Robert G. Bryan in Feb, 2011.
        //
        // Constants required to handle file I/O:
        private const uint GENERIC_READ = 0x80000000;

        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;

        // ReSharper disable UnusedMember.Local
        private const uint CREATE_NEW = 1;

        private const uint CREATE_ALWAYS = 2;
        private const uint OPEN_EXISTING = 3;
        private const uint OPEN_ALWAYS = 4;
        private const uint TRUNCATE_EXISTING = 5;

        private const uint FILE_BEGIN = 0;
        private const uint FILE_CURRENT = 1;

        private const uint FILE_END = 2;
        // ReSharper restore UnusedMember.Local

        private IntPtr pHandle;
        private long position;
        private long length = -1;

        public long Length
        {
            get
            {
                //If we have already fetched the length of the file open, use that
                if (length != -1)
                    return length;

                //If no file has been opened, throw an exception
                if (pHandle == IntPtr.Zero)
                    throw new ApplicationException("WinFileIO:Length - No file open");

                long fileSize;
                if (!NativeMethods.GetFileSizeEx(pHandle, &fileSize))
                {
                    var we = new Win32Exception();
                    throw new ArgumentException($"WinFileIO:Length - Error occurred whilst reading file size. - {we.Message}");
                }

                length = fileSize;
                return length;
            }
        }

        public long Position
        {
            get => position;
            set
            {
                //Validation
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Position must be >= 0");

                if (pHandle != IntPtr.Zero)
                {
                    if (!NativeMethods.SetFilePointerEx(pHandle, value, null, FILE_BEGIN))
                    {
                        var we = new Win32Exception();
                        throw new ApplicationException($"WinFileIO:Position - Error occurred seeking. - {we.Message}");
                    }
                }

                position = value;
            }
        }

        public WinFileIO()
        {
            pHandle = IntPtr.Zero;
        }

        public void Dispose()
        {
            // This method should be called to clean everything up.
            Dispose(true);
            // Tell the GC not to finalize since clean up has already been done.
            GC.SuppressFinalize(this);
        }

        ~WinFileIO()
        {
            // Finalizer gets called by the garbage collector if the user did not call Dispose.
            Dispose(false);
        }

        //

        protected void Dispose(bool disposing)
        {
            // This function frees up the unmanaged resources of this class.
            Close();
        }

        //

        public void OpenForReading(string fileName)
        {
            OpenForReading(fileName, 0);
        }

        public void OpenForReading(string fileName, WinFileFlagsAndAttributes flagsAndAttributes)
        {
            // This function uses the Windows API CreateFile function to open an existing file for reading.
            // A return value of true indicates success.
            // It allows other processes to read the file
            Close();

            pHandle = NativeMethods.CreateFile(fileName, GENERIC_READ, FILE_SHARE_READ, UIntPtr.Zero, OPEN_EXISTING, (uint) flagsAndAttributes, IntPtr.Zero);
            position = 0;

            if (pHandle != IntPtr.Zero)
                return;

            var we = new Win32Exception();
            throw new ApplicationException($"WinFileIO:OpenForReading - Could not open file {fileName} - {we.Message}");
        }

        public void OpenForWriting(string fileName)
        {
            OpenForWriting(fileName, 0);
        }

        public void OpenForWriting(string fileName, WinFileFlagsAndAttributes flagsAndAttributes)
        {
            // This function uses the Windows API CreateFile function to open a file for writing.
            // If it doesn't exist, it will be created.
            // If it does exist, it will be loaded.
            // It does not allow other processes to access the file
            Close();

            pHandle = NativeMethods.CreateFile(fileName, GENERIC_WRITE, 0, UIntPtr.Zero, OPEN_ALWAYS, (uint) flagsAndAttributes, IntPtr.Zero);
            position = 0;

            if (pHandle != IntPtr.Zero)
                return;

            var we = new Win32Exception();
            throw new ApplicationException($"WinFileIO:OpenForWriting - Could not open file {fileName} - {we.Message}");
        }

        public void OpenForReadingWriting(string fileName)
        {
            OpenForReadingWriting(fileName, 0);
        }

        public void OpenForReadingWriting(string fileName, WinFileFlagsAndAttributes flagsAndAttributes)
        {
            // This function uses the Windows API CreateFile function to open a file for reading and writing.
            // If it doesn't exist, it will be created.
            // If it does exist, it will be loaded.
            // It does not allow other processes to access the file
            Close();

            pHandle = NativeMethods.CreateFile(fileName, GENERIC_READ | GENERIC_WRITE, 0, UIntPtr.Zero, OPEN_ALWAYS, (uint) flagsAndAttributes, IntPtr.Zero);
            position = 0;

            if (pHandle != IntPtr.Zero)
                return;

            var we = new Win32Exception();
            throw new ApplicationException($"WinFileIO:OpenForReadingWriting - Could not open file {fileName} - {we.Message}");
        }

        public int Read(byte[] buffer)
        {
            // This function reads in a file up to BytesToRead using the Windows API function ReadFile.  The return value
            // is the number of bytes read.

            fixed (byte* pBuffer = buffer)
            {
                var bytesRead = 0;
                if (NativeMethods.ReadFile(pHandle, pBuffer, buffer.Length, &bytesRead, IntPtr.Zero))
                    return bytesRead;

                var we = new Win32Exception();
                throw new ApplicationException($"WinFileIO:Read - Error occurred reading a file. - {we.Message}");
            }
        }

        public int Write(byte[] buffer)
        {
            // Writes out the file in one swoop using the Windows WriteFile function.
            fixed (byte* pBuffer = buffer)
            {
                int numberOfBytesWritten;
                if (NativeMethods.WriteFile(pHandle, pBuffer, buffer.Length, &numberOfBytesWritten, IntPtr.Zero))
                    return numberOfBytesWritten;

                var we = new Win32Exception();
                throw new ApplicationException($"WinFileIO:Write - Error occurred writing a file. - {we.Message}");
            }
        }

        public bool Close()
        {
            // This function closes the file handle.

            //If read and write are using the same handle, require both to be closed at once
            if (pHandle == IntPtr.Zero)
                return true;

            var success = NativeMethods.CloseHandle(pHandle);
            pHandle = IntPtr.Zero;
            length = -1;

            return success;
        }
    }
}