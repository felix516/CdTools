using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Felix516.cdTools.Lib
{
    /// <summary>
    /// Class that wraps around various low level cdrom drive operations
    /// </summary>
    public static class CdTools
    {

        /// <summary>
        /// Reads the cdrom table of contents and generates a table of contents string consumable by
        /// music database services such as Musicbrainz and Gracenote
        /// </summary>
        /// <param name="driveLetter">the cdrom drive to read</param>
        /// <returns>a string containing the sector offset for each track on the cd + sector offset for the leadout</returns>
        public static string GetTocString(string driveLetter)
        {
            StringBuilder tocBuilder = new StringBuilder();

            if (IsDiskInDrive(driveLetter))
            {
                var handle = Win32Functions.CreateFile(createDriveFileString(driveLetter), Win32Functions.GENERIC_READ, Win32Functions.FILE_SHARE_READ, IntPtr.Zero, Win32Functions.OPEN_EXISTING, 0, IntPtr.Zero);
                var toc = new Win32Functions.CDROM_TOC();
                uint bytesRead = 0;

                //Get toc
                Win32Functions.DeviceIoControl(handle, Win32Functions.IOCTL_CDROM_READ_TOC, IntPtr.Zero, 0, toc, (uint)Marshal.SizeOf(toc), ref bytesRead, IntPtr.Zero);

                for (int i = toc.FirstTrack - 1; i < toc.LastTrack + 1; i++)
                {
                    //if (toc.TrackData[i - 1].Control == 0)
                   // {
                        var offset = GetStartSector(toc.TrackData[i]);
                        tocBuilder.Append(offset);

                        if (i != toc.LastTrack)
                        {
                            tocBuilder.Append(" ");
                        }
                    //}
                }
            }
            return tocBuilder.ToString();
        }

        /// <summary>
        /// Determines whether a CD is inserted into the given drive
        /// </summary>
        /// <param name="driveLetter">the drive to check for a cd</param>
        /// <returns>returns true if cd exists, false otherwise</returns>
        public static bool IsDiskInDrive(string driveLetter)
        {
            uint dummy = 0;
            var handle = Win32Functions.CreateFile(createDriveFileString(driveLetter), Win32Functions.GENERIC_READ, Win32Functions.FILE_SHARE_READ, IntPtr.Zero, Win32Functions.OPEN_EXISTING, 0, IntPtr.Zero);
            var check = Win32Functions.DeviceIoControl(handle, Win32Functions.IOCTL_STORAGE_CHECK_VERIFY, IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero);

            if (check != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a file string that is compatible with the Win32Functions.CreateFile function
        /// </summary>
        /// <param name="driveletter">the drive letter to be converted</param>
        /// <returns>a CreateFile compatible string</returns>
        private static string createDriveFileString(string driveletter)
        {
            StringBuilder filebuilder = new StringBuilder();

            filebuilder.Append("\\\\.\\");
            filebuilder.Append(driveletter);
            filebuilder.Append(":");

            return filebuilder.ToString();
        }

        /// <summary>
        /// Gets the start offset of a track given its track data
        /// </summary>
        /// <param name="data">the track data to read</param>
        /// <returns>the frame offset where the track begins</returns>
        private static int GetStartSector(Win32Functions.TRACK_DATA data)
        {
            return (data.Address_1 * 60 * 75 + data.Address_2 * 75 + data.Address_3);
        }

        /// <summary>
        /// Gets the first Cdrom drive on the system and returns its letter
        /// </summary>
        /// <returns>the first drive letter that corresponds to a cdrom drive</returns>
        public static string GetCdromDriveLetter()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.CDRom)
                {
                    return drive.Name[0].ToString();
                }
            }
            return string.Empty;
        }
    }
}
