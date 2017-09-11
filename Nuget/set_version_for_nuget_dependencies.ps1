param(
$Version=$(throw "Version should be in script params")
)
filter grep($keyword) { if ( ($_ | Out-String) -match $keyword) {  
    $_ }
}

filter sed($before,$after) { %{$_ -replace $before,$after} } 

function replace ($source,$dest,$pattern) {
    Write-Host $dest
    $configs = Get-ChildItem ..\ -Recurse *.nuspec
    foreach ($config in $configs) {
        $match = cat $config.FullName | grep $pattern
        if ($match -match $source ) { Write-Host "`nFound"$config.FullName
            Write-Host "`nReplace"$match
            $result = cat $config.FullName | sed -before $source -after $dest
            $result | Out-File $config.FullName -Encoding utf8
        } 
    }
}