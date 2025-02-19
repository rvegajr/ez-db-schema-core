#!/bin/zsh

# Setup .NET environment
export PATH="$PATH:/usr/local/share/dotnet:/usr/local/share/dotnet/x64"

# Exit on any error
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored messages
print_message() {
    echo -e "${2}${1}${NC}"
}

# Function to check if version exists on NuGet
check_version_exists() {
    local package_name=$1
    local version=$2
    
    print_message "Checking if ${package_name} version ${version} exists on NuGet..." "${YELLOW}"
    
    # Query NuGet API
    local response=$(curl -s "https://api.nuget.org/v3-flatcontainer/${package_name}/index.json")
    
    # Check if version exists in the versions array
    if echo "$response" | grep -q "\"$version\""; then
        return 0 # Version exists
    else
        return 1 # Version doesn't exist
    fi
}

# Function to publish package if version doesn't exist
publish_if_new() {
    local package_path=$1
    local package_name=$2
    local version=$3
    
    if check_version_exists "$package_name" "$version"; then
        print_message "Version ${version} of ${package_name} already exists on NuGet. Skipping..." "${YELLOW}"
    else
        print_message "Publishing ${package_name} version ${version}..." "${GREEN}"
        dotnet nuget push "$package_path" --source https://api.nuget.org/v3/index.json --api-key "$NUGET_API_KEY"
        print_message "Successfully published ${package_name} version ${version}" "${GREEN}"
    fi
}

# Check if NUGET_API_KEY is set
if [ -z "$NUGET_API_KEY" ]; then
    print_message "Error: NUGET_API_KEY environment variable is not set" "${RED}"
    exit 1
fi

# Directory setup
SCRIPT_DIR=$(dirname "$0")
cd "$SCRIPT_DIR"

# Clean all bin and obj directories
print_message "Cleaning all bin and obj directories..." "${YELLOW}"
find . -type d -name bin -o -name obj | xargs rm -rf

# 1. Run tests
print_message "Running tests..." "${YELLOW}"
dotnet test ./Tests/EzDbSchema.Core.Tests/EzDbSchema.Core.Tests.csproj -c Release

if [ $? -ne 0 ]; then
    print_message "Tests failed! Aborting publish process." "${RED}"
    exit 1
fi

print_message "Tests passed successfully!" "${GREEN}"

# 2. Build and pack Core library
print_message "Building and packing EzDbSchema.Core..." "${YELLOW}"
cd ./Src/EzDbSchema.Core
dotnet clean -c Release
dotnet build -c Release
dotnet pack -c Release

# Get Core version from csproj
CORE_VERSION=$(grep -m 1 '<Version>' EzDbSchema.Core.csproj | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')
CORE_PACKAGE="bin/Release/EzDbSchema.${CORE_VERSION}.nupkg"

# 3. Build and pack CLI tool
print_message "Building and packing EzDbSchema.CLI..." "${YELLOW}"
cd ../EzDbSchema.Cli
dotnet clean -c Release
dotnet build -c Release
dotnet pack -c Release

# Get CLI version from csproj
CLI_VERSION=$(grep -m 1 '<Version>' EzDbSchema.Cli.csproj | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')
CLI_PACKAGE="bin/Release/EzDbSchema.Cli.${CLI_VERSION}.nupkg"

# 4. Publish packages if they don't exist
cd ../..

# Publish Core package
publish_if_new "Src/EzDbSchema.Core/$CORE_PACKAGE" "EzDbSchema" "$CORE_VERSION"

# Publish CLI package
publish_if_new "Src/EzDbSchema.Cli/$CLI_PACKAGE" "EzDbSchema.Cli" "$CLI_VERSION"

print_message "Publish process completed successfully!" "${GREEN}"
