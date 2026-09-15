Set-Location $PSScriptRoot

dotnet build
if ($?) {
    $ErrorActionPreference = 'Continue'
    do {
        Remove-Item "C:\Program Files (x86)\Steam\steamapps\common\Human Fall Flat\BepInEx\plugins\AchievementHelper.dll" -Force
    } while (Test-Path "C:\Program Files (x86)\Steam\steamapps\common\Human Fall Flat\BepInEx\plugins\AchievementHelper.dll")
    do {
        Copy-Item "bin\Debug\net46\AchievementHelper.dll" "C:\Program Files (x86)\Steam\steamapps\common\Human Fall Flat\BepInEx\plugins\AchievementHelper.dll" -Force
    } until ((Test-Path "C:\Program Files (x86)\Steam\steamapps\common\Human Fall Flat\BepInEx\plugins\AchievementHelper.dll"))
    while ((Get-Process | Where-Object {$_.ProcessName -eq "Human"}).Count) {
        Get-Process | Where-Object {$_.ProcessName -eq "Human"} | Stop-Process
    }
    Start-Process "steam://rungameid/477160"
}