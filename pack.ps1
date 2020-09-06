$currentVersion = [Version](Select-String -Path WindowDebugger/WindowDebugger.csproj "(?<=<Version>).*(?=</Version>)").Matches.Value
if (!$currentVersion) {
    Write-Output "Cannot find current version"
    exit -1
}

$maxOldVersion = ([Version[]](git tag) | Measure-Object -Maximum).Maximum

if ($maxOldVersion -and $currentVersion -le $maxOldVersion) {
    Write-Output "No need To release"
    exit 0
}

Write-Output "Do release"

$releaseObj =
@{
    "name"     = [string]$currentVersion;
    "tag_name" = [string]$currentVersion;
    "ref"      = $env:CI_BUILD_REF;
    "assets"   = @{"links" = [System.Collections.ArrayList]@() }
}

foreach ($folderName in Get-ChildItem -Path "WindowDebugger\bin\Release" -Name) {
    $fileName = "WindowDebugger.$currentVersion.$folderName.7z"
    7z a $fileName .\WindowDebugger\bin\Release\$folderName\*
    Invoke-WebRequest -Method Post -Uri https://upload-z2.qiniup.com -Form @{"file" = (Get-item -Path $fileName); "key" = $fileName; "token" = $env:QiniuUploadToken }
    $releaseObj.assets.links.Add(@{"name" = [string]$folderName; "url" = "$env:QiniuReleaseUrl$fileName" })
}

Invoke-WebRequest -Method Post -Uri "$env:CI_API_V4_URL/projects/$env:CI_PROJECT_ID/releases" -ContentType "application/json" -Headers @{"JOB-TOKEN" = $env:CI_JOB_TOKEN } -Body (ConvertTo-Json -Depth 3 $releaseObj)
