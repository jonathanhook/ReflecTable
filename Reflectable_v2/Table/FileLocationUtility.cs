using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Table
{
    public static class FileLocationUtility
    {
        private const string SUBFOLDER = "ReflectTable";

        public static string GetPathInVideoFolderLocation(string file)
        {
            string folder = GetVideoFolderLoctation();
            return Path.Combine(folder, file); ;
        }

        public static string GetVideoFolderLoctation()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), SUBFOLDER);
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
