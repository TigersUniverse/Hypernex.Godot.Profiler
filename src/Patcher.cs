using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Godot;
using HarmonyLib;
using System.Diagnostics;
using Hypernex.CCK.GodotVersion;
using Hypernex.GodotVersion.ProfilerPlugin;

public static class Patcher
{
    public static Harmony harmony { get; private set; } = new Harmony("net.virtualwebsite.csharp_profiler");
    // public static readonly ConcurrentBag<ICoreDetour> prefixes = new ConcurrentBag<ICoreDetour>();

    [ThreadStatic]
    private static Dictionary<MethodBase, ProfilerZone> lookup;

    public static void Patch()
    {
        foreach (var asm in PluginLoader.LoadedPlugins)
        {
            foreach (var type in asm.GetType().Assembly.GetTypes())
            {
                if (type == typeof(Profiler) || type == typeof(ProfilerZone) || type == typeof(Patcher) || type == typeof(Plugin))
                    continue;
                // if (type.IsAssignableTo(typeof(GodotObject)))
                    PatchType(type);
            }
        }
        foreach (var type in typeof(Patcher).Assembly.GetTypes())
        {
            if (type == typeof(Profiler) || type == typeof(ProfilerZone) || type == typeof(Patcher) || type == typeof(Plugin))
                continue;
            // if (type.IsAssignableTo(typeof(GodotObject)))
                PatchType(type);
        }
    }

    public static void PatchType(Type type)
    {
        if (type.IsAssignableTo(typeof(Delegate)))
            return;
        if (type.IsAbstract)
            return;
        GD.Print($"Patching type {type.FullName}");
        var prefix = typeof(Patcher).GetMethod(nameof(_Prefix));
        var postfix = typeof(Patcher).GetMethod(nameof(_Postfix));
        foreach (var method in type.GetMethods())
        {
            if (method.MethodImplementationFlags.HasFlag(MethodImplAttributes.AggressiveInlining))
                continue;
            try
            {
                GD.Print($"Patching method {method.Name}");
                if (!method.IsConstructor && method.DeclaringType == type && !method.IsAbstract && !method.ContainsGenericParameters && !method.IsGenericMethodDefinition && method.GetCustomAttribute<DllImportAttribute>() == null && method.GetCustomAttribute<UnmanagedFunctionPointerAttribute>() == null)
                {
                    harmony.Patch(method, prefix, finalizer: postfix);
                }
            }
            catch (Exception)
            {
                GD.PushError($"Failed to patch method {method.Name} in type {type.FullName}");
            }
        }
    }

    public static void _Prefix(MethodBase __originalMethod, ref ProfilerZone __state)
    {
        __state = Profiler.BeginZone(__originalMethod.DeclaringType?.FullName + "::" + __originalMethod.Name, lineNumber: 0, filePath: null, memberName: __originalMethod.Name);
    }

    public static void _Postfix(MethodBase __originalMethod, ProfilerZone __state)
    {
        __state.Dispose();
    }
}