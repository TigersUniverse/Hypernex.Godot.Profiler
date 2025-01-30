dotnet build -c Debug

Copy-Item -Path .godot/mono/temp/bin/Debug/Hypernex.Godot.Profiler.dll -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
Copy-Item -Path .godot/mono/temp/bin/Debug/Hypernex.Godot.Profiler.pdb -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
Copy-Item -Path .godot/mono/temp/bin/Debug/0Harmony.dll -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
Copy-Item -Path .godot/mono/temp/bin/Debug/Tracy.dll -Destination $env:APPDATA/Godot/app_userdata/Hypernex.Godot/Plugins -Force
