Set-Location $PSScriptRoot

dotnet build
if ($?) {
    Copy-Item "bin\Debug\net46\AchievementHelper.dll" "C:\Program Files (x86)\Steam\steamapps\common\Human Fall Flat\BepInEx\plugins\AchievementHelper.dll" -Force
    while ((Get-Process | Where-Object {$_.ProcessName -eq "Human"}).Count) {
        Get-Process | Where-Object {$_.ProcessName -eq "Human"} | Stop-Process
    }
    Start-Process "steam://rungameid/477160"
}