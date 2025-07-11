param (
    [string]$jsonFilePath,
    [string]$targetFramework
)

# Extract the version number using a regular expression
$version = if ($targetFramework -match 'net(\d+\.\d+)') { $matches[1] } else { $null }

if (-not $version) {
    Write-Error "Unable to extract version number from target framework: $targetFramework"
    exit 1
}

# Construct the property path dynamically
$propertyPath = ".NETCoreApp,Version=v$version"

# Define an array of property names to remove
$propertiesToRemove = @(
    "CastleLibrary/1.0.0",
    "CompressionLibrary/1.0.0",
    "CustomLogger/1.0.0",
    "MultiServerLibrary/1.0.0",
    "EndianTools/1.0.0",
	"TechnitiumLibrary.Net/5.0.0",
	"TechnitiumLibrary.Net.Firewall/5.0.0"
)

try {
    # Read the JSON file
    $jsonContent = Get-Content -LiteralPath $jsonFilePath -Raw | ConvertFrom-Json
} catch {
    Write-Error "Failed to read or parse JSON file: $jsonFilePath"
    exit 1
}

# Debug: Check the structure of the JSON content
Write-Output "JSON Content before modification: $($jsonContent | ConvertTo-Json -Depth 32)"

if (-not $jsonContent.targets.$propertyPath) {
    Write-Error "The property path '$propertyPath' does not exist in the JSON file."
    exit 1
}

# Remove the specified properties
foreach ($property in $propertiesToRemove) {
    if ($jsonContent.targets.$propertyPath.PSObject.Properties[$property]) {
        $jsonContent.targets.$propertyPath.PSObject.Properties.Remove($property)
    } else {
        Write-Output "Property '$property' does not exist under '$propertyPath'. Skipping..."
    }
}

try {
    # Save the modified JSON content back to the file
    $jsonContent | ConvertTo-Json -Depth 32 | Set-Content -LiteralPath $jsonFilePath
} catch {
    Write-Error "Failed to write modified JSON content back to file: $jsonFilePath"
    exit 1
}

Write-Output "Successfully updated the JSON file: $jsonFilePath"
exit 0