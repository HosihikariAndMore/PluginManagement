using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Loader;

internal static class PluginManager
{
    public static Dictionary<AssemblyLoadContext, List<IModule>> PluginContexts { get; }

    static PluginManager()
    {
        PluginContexts = new();
    }

    public static IEnumerable<string> EnumerableAllDll()
    {
        DirectoryInfo directoryInfo = new("plugins");
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        foreach (var file in directoryInfo.EnumerateFiles("*.dll"))
        {
            yield return file.FullName;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void ReloadAllPlugin()
    {
        //uninstall loaded modules
        foreach (var (ctx, unload) in PluginContexts)
        {
            try
            {
                foreach (var module in unload)
                {
                    try
                    {
                        module.Unload();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(
                            "plugin {0} unload failed. file {1}. {2}",
                            module.Name,
                            ctx.Name,
                            e
                        );
                    }
                }
                ctx.Unload();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("assembly unload failed {0}. {1}", ctx.Name, e);
            }
        }
        //clear PluginContexts
        PluginContexts.Clear();
        //dict for sort
        SortedDictionary<
            ModuleEntryPointAttributeBase,
            (AssemblyLoadContext ctx, List<IModule> unload)
        > allEntryPoint = new();
        //enumerate all dll and load all EntryPointAttribute
        //then sort by priority
        foreach (var file in EnumerableAllDll())
        {
            try
            {
                AssemblyLoadContext loadContext = new(file);
                Assembly assembly = loadContext.LoadFromAssemblyPath(file);
                foreach (
                    var entryPoint in assembly.GetCustomAttributes<ModuleEntryPointAttributeBase>()
                )
                {
                    allEntryPoint.Add(entryPoint, new());
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0} load failed. {1}", file, e);
            }
        }
        //load all modules
        foreach (var (entryPoint, (_, modules)) in allEntryPoint)
        {
            try
            {
                var module = entryPoint.CreateInstance();
                module.Initialize(); //initialize module
                modules.Add(module);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        //add all modules to PluginContexts
        foreach (var (_, (ctx, modules)) in allEntryPoint)
        {
            PluginContexts.Add(ctx, modules);
        }
    }
}
