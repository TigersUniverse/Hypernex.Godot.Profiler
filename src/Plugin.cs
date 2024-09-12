using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Godot;
using Hypernex.CCK;
using Hypernex.CCK.GodotVersion;
using Hypernex.CCK.GodotVersion.Classes;

namespace Hypernex.GodotVersion.ProfilerPlugin
{
    public class Plugin : HypernexPlugin
    {
        public override string PluginName => "TracyProfiler";

        public override string PluginCreator => "TigersUniverse";

        public override string PluginVersion => "0.0.0.0";

        public override void OnPluginLoaded()
        {
            NativeLibrary.SetDllImportResolver(typeof(Tracy.PInvoke).Assembly, TracyResolver);
            Profiler.ProfileFrame(nameof(OnPluginLoaded));
        }

        public override void Start()
        {
            Patcher.Patch();
        }

        public override void LateUpdate()
        {
            Profiler.ProfileFrame(nameof(LateUpdate));
        }

        private IntPtr TracyResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName.Equals("TracyClient"))
            {
                return NativeLibrary.Load(Path.GetFullPath(Path.Combine(assembly.Location, "..", "runtimes", libraryName)));
            }
            return IntPtr.Zero;
        }
    }
}