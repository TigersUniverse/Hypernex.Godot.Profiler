dotnet build -c ExportRelease Hypernex.Godot.Profiler.csproj

Copy-Item -Path .godot/mono/temp/bin/ExportRelease/Hypernex.Godot.Profiler.dll -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
Copy-Item -Path .godot/mono/temp/bin/ExportRelease/Hypernex.Godot.Profiler.pdb -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
Copy-Item -Path .godot/mono/temp/bin/ExportRelease/0Harmony.dll -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
Copy-Item -Path .godot/mono/temp/bin/ExportRelease/Tracy.dll -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
