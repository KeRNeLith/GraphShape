<#
.Synopsis
    Gets the MSBuild property name from tag slug.
#>
function GetPropertyNameFromSlug
{
param(
[Parameter(Mandatory)]
[string] $tagSlug)
    switch ($tagSlug)
    {
        'core' { return "Generate_GraphShape_Core"; }
        'controls' { return "Generate_GraphShape_Controls"; }
        default { throw "Invalid tag slug." }
    }
}

<#
.Synopsis
    Update the PackagesGeneration.props based on given tag name.
#>
function UpdatePackagesGeneration
{
param(
[Parameter(Mandatory)]
[string] $propertyName)
    # Update the package generation props to enable package generation of the right package
    $genPackagesFilePath = "./build/PackagesGeneration.props";
    $genPackagesContent = Get-Content $genPackagesFilePath;
    $newGenPackagesContent = $genPackagesContent -replace "<$propertyName>\w+<\/$propertyName>","<$propertyName>true</$propertyName>";
    $newGenPackagesContent | Set-Content $genPackagesFilePath;

    # Check content changes (at least one property changed
    $genPackagesContentStr = $genPackagesContent | Out-String;
    $newGenPackagesContentStr = $newGenPackagesContent | Out-String;
    if ($genPackagesContentStr -eq $newGenPackagesContentStr)
    {
        throw "MSBuild property $propertyName does not exist in $genPackagesFilePath or content not updated.";
    }
}

<#
.Synopsis
    Update the PackagesGeneration.props to generate all packages.
#>
function UpdateAllPackagesGeneration()
{
    # Update the package generation props to enable package generation of the right package
    $genPackagesFilePath = "./build/PackagesGeneration.props";
    $genPackagesContent = Get-Content $genPackagesFilePath;
    $newGenPackagesContent = $genPackagesContent -replace "false","true";
    $newGenPackagesContent | Set-Content $genPackagesFilePath;
}

# Update .props based on git tag status & setup build version
$env:PackageSamples = $false;
if ($env:APPVEYOR_REPO_TAG -eq "true")
{
    $tagParts = $env:APPVEYOR_REPO_TAG_NAME.split("/", 2);

    # Full release
    if ($tagParts.Length -eq 1) # X.Y.Z
    {
        UpdateAllPackagesGeneration;
        $env:Build_Version = $env:APPVEYOR_REPO_TAG_NAME;
        $env:Release_Name = $env:Build_Version;
    }
    # Partial release
    else # Slug/X.Y.Z
    {
        # Retrieve MSBuild property name for which enabling package generation
        $tagSlug = $tagParts[0];
        $propertyName = GetPropertyNameFromSlug $tagSlug;
        $tagVersion = $tagParts[1];

        UpdatePackagesGeneration $propertyName;
        $env:Build_Version = $tagVersion;
        $projectName = $propertyName -replace "Generate_","";
        $projectName = $projectName -replace "_",".";
        $env:Release_Name = "$projectName $tagVersion";
    }

    $env:IsFullIntegrationBuild = $env:Configuration -eq "Release";
    $env:PackageSamples = $true;
}
else
{
    UpdateAllPackagesGeneration;
    $env:Build_Version = "$($env:APPVEYOR_BUILD_VERSION)";
    $env:Release_Name = $env:Build_Version;

    $env:IsFullIntegrationBuild = "$env:APPVEYOR_PULL_REQUEST_NUMBER" -eq "" -And $env:Configuration -eq "Release";
}

$env:Build_Assembly_Version = "$env:Build_Version" -replace "\-.*","";

"Building version: $env:Build_Version";
"Building assembly version: $env:Build_Assembly_Version";

if ($env:IsFullIntegrationBuild -eq $true)
{
    "With full integration";
}
else
{
    "Without full integration";
}