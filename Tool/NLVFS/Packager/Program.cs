using System;
using System.Collections.Generic;
using System.Text;

namespace Packager
{
    class Program
    {
        static void Main(string[] args)
        {
          //  var startPath = @"F:\TOMClassic\Trunk\PicResource";
            var startPath = args[0];
            var targetFile = args[1];

            var dt = DateTime.Now;
            Console.WriteLine(DateTime.Now + " start");
            NLVFS.NLVFS.MakeVfsFile(startPath, targetFile);
            Console.WriteLine(DateTime.Now + " all fin");
        }
    }
}
