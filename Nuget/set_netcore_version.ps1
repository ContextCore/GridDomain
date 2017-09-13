function set_netcore_version ($buildNum){
$configs = Get-ChildItem .\ -Recurse "*.csproj"
foreach ($projectFile in $configs) {
Write-Host Processing $projectFile
$content = [IO.File]::ReadAllText($projectFile)

$regex = new-object System.Text.RegularExpressions.Regex ('()([\d]+.[\d]+.[\d]+)(.[\d]+)(<\/Version>)', 
         [System.Text.RegularExpressions.RegexOptions]::MultiLine)

$version = $null
$match = $regex.Match($content)
if($match.Success) {
    # from "1.0.0.0" this will extract "1.0.0"
    $version = $match.groups[2].value
}

# suffix build number onto $version. eg "1.0.0.15"
$version = "$version.$buildNum"

# update "1.0.0.0" to "$version"
$content = $regex.Replace($content, '${1}' + $version + '${4}')

# update csproj file
[IO.File]::WriteAllText($projectFile, $content)
}
# update AppVeyor build
#Update-AppveyorBuild -Version $version
}