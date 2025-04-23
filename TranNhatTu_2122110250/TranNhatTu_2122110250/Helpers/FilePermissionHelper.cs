using System;
using System.IO;
using System.Text;

namespace TranNhatTu_2122110250.Helpers
{
    public static class FilePermissionHelper
    {
        public static bool HasWritePermission(string path)
        {
            try
            {
                string testFile = Path.Combine(path, Path.GetRandomFileName());
                using (var fs = File.Create(testFile))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("x");
                    fs.Write(info, 0, info.Length);
                }

                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
