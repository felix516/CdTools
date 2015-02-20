using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Felix516.cdTools.Lib;


namespace Felix516.cdTools.ConsoleApp
{
    /// <summary>
    /// Basic Console Application that demonstates the functionality of the library
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The letter of your first cdrom drive is {0}",CdTools.GetCdromDriveLetter());
            if (CdTools.IsDiskInDrive(CdTools.GetCdromDriveLetter()))
            {
                Console.WriteLine("There is a disc in your cdrom drive!");
            }
            else
            {
                Console.WriteLine("There is not a disc in your cdrom drive!");
            }
            Console.WriteLine("The toc string is :\n{0}",CdTools.GetTocString(CdTools.GetCdromDriveLetter()));
            Console.Read();
        }
    }
}
