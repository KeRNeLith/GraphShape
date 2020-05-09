if ($env:PackageSamples -eq $true)
{
    $initialFolder = $pwd;

    cd samples\GraphShape.Sample\bin\*\;

    7z.exe a GraphShape.Sample.zip "*.*" -r -xr!"*.pdb" -x!"*.xml";

    cd $initialFolder;
}