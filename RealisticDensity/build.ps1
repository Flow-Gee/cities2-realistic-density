param ($pluginName, $pluginVersion, $projectPath, $pluginPath, $bepInExVersion)

Write-Host "Project name: $pluginName"
Write-Host "Project version: $pluginVersion"
Write-Host "Project path: $projectPath"
Write-Host "Plugin path: $pluginPath"
Write-Host "BepInEx version: $bepInExVersion"

# Copy required thunderstore files
Copy-Item -Path $projectPath\..\manifest.json -Destination $pluginPath -Recurse -Force
Copy-Item -Path $projectPath\..\icon.png -Destination $pluginPath -Recurse -Force
Copy-Item -Path $projectPath\..\README.md -Destination $pluginPath -Recurse -Force
Copy-Item -Path $projectPath\..\CHANGELOG.md -Destination $pluginPath -Recurse -Force

try {
	$files = Get-ChildItem -Path $pluginPath
	Compress-Archive -Path $files.FullName -DestinationPath "$pluginPath\..\$pluginName-BepInEx$bepInExVersion-v$pluginVersion.zip" -Force
	Write-Host "$pluginName for BepInEx$bepInExVersion successfully packed to $pluginPath"
} catch {
	Write-Host "Error: $_"
	exit 1
}