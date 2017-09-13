function set_netcore_version ($project, $packageVersion, $fileVersion, $assemblyVersion) {
    $content = [IO.File]::ReadAllText($project)
    $replaced = $content -replace '<Version>1.0.0</Version>',"<Version>$packageVersion</Version>"
    $replaced = $replaced -replace '<AssemblyVersion>1.0.0.0</AssemblyVersion>',"<AssemblyVersion>$fileVersion</AssemblyVersion>"
    $replaced = $replaced -replace '<FileVersion>1.0.0.0</FileVersion>',"<FileVersion>$assemblyVersion</FileVersion>"
    [IO.File]::WriteAllText($project, $replaced)
    Write-Host set build version $buildNum in $project 
}

 #set_netcore_version "P:\GridDomain\GridDomain.CQRS\GridDomain.CQRS.csproj" "1.1.1" "1.1.1.0" "1.1.1.0"