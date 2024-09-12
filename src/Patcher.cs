using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using HarmonyLib;
using Hypernex.CCK.GodotVersion;

namespace Hypernex.GodotVersion.ProfilerPlugin
{
    public static class Patcher
    {
        public static Harmony harmony { get; private set; } = new Harmony("dev.hypernex.godot.profiler");

        [ThreadStatic]
        private static Dictionary<int, Profiler.ProfilerScope> lookup;

        public static void Patch()
        {
            // foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var asm in PluginLoader.LoadedPlugins)
            {
                foreach (var type in asm.GetType().Assembly.GetTypes())
                {
                    if (type == typeof(Profiler) || type == typeof(Patcher) || type == typeof(Plugin))
                        continue;
                    if (type.IsAssignableTo(typeof(GodotObject)))
                        PatchType(type);
                }
            }
            foreach (var type in typeof(Init).Assembly.GetTypes())
            {
                if (type == typeof(Profiler) || type == typeof(Patcher) || type == typeof(Plugin))
                    continue;
                if (type.IsAssignableTo(typeof(GodotObject)))
                    PatchType(type);
            }
        }

        public static void PatchType(Type type)
        {
            GD.Print($"Patching type {type.FullName}");
            var prefix = typeof(Patcher).GetMethod(nameof(_Prefix));
            var postfix = typeof(Patcher).GetMethod(nameof(_Postfix));
            foreach (var method in type.GetMethods())
            {
                if (method.DeclaringType == type && !method.IsAbstract && !method.ContainsGenericParameters)
                    harmony.Patch(method, prefix, postfix);
            }
        }

        public static void _Prefix(MethodBase __originalMethod/*, object __instance, object[] __args*/)
        {
            if (lookup == null)
                lookup = new Dictionary<int, Profiler.ProfilerScope>();
            int idx = __originalMethod.GetHashCode();
            if (!lookup.ContainsKey(idx))
                lookup.Add(__originalMethod.GetHashCode(), Profiler.BeginEvent(__originalMethod.Name));
        }

        public static void _Postfix(MethodBase __originalMethod/*, object __instance, object[] __args*/)
        {
            int idx = __originalMethod.GetHashCode();
            if (lookup.TryGetValue(idx, out Profiler.ProfilerScope scope))
            {
                scope.Dispose();
                lookup.Remove(idx);
            }
        }
    }
}