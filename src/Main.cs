using System.Runtime.InteropServices;

namespace Loader;

public static class Main
{
    [UnmanagedCallersOnly(EntryPoint = "Initialize")]
    public static void Initialize()
    {
        Console.WriteLine("Hello Minecraft!");
    }
}
