# Changelog

All notable changes to EzDbSchema will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [8.4.0] - 2025-03-17

### Changed
- Improved project structure with centralized package management
- Enhanced string extension methods for better .NET 8.0 compatibility
- Updated Microsoft.Extensions.* packages to 9.0.3
- Updated Microsoft.Data.SqlClient to 6.0.1
- Improved handling of null inputs in string operations
- Standardized StringExtensions usage across codebase
- Removed duplicate package references
- Improved build output organization

### Security
- Updated all dependencies to latest stable versions
- Removed legacy .NET Framework dependencies

## [8.3.1] - 2025-02-26

### Changed
- Updated to target .NET 8.0
- Fixed RelationshipMultiplicityType enum ordering
- Improved nullable reference type handling

## [8.1.0] - Previous Release

### Added
- Support for .NET 8.0
- Server Trust Certificate setting

### Changed
- Updated to Microsoft SqlClient
- Updated NuGet packages

## [7.0.0] - Previous Release

### Added
- Support for .NET 7.0

### Changed
- Updated NuGet packages

## [6.0.1] - Previous Release

### Added
- Option to disable automatic primary key creation

## [6.0.0] - Previous Release

### Added
- Initial support for .NET 6.0

### Changed
- Migrated codebase to .NET 6.0
