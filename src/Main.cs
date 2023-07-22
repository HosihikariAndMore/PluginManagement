using System.Runtime.InteropServices;

namespace Loader;

public static class Main
{
    [UnmanagedCallersOnly()]
    public static void Initialize()
    {
        DirectoryInfo directoryInfo = new("plugins");
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (FileInfo file in files)
        {
            try
            {
                if (PluginManager.LoadPlugin(file.FullName))
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Error.WriteLine("{0} load failed.", file.Name);
        }
    }
}
