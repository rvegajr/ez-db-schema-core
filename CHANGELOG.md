# Changelog

All notable changes to EzDbSchema will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [9.0.1] - 2025-02-19

### Added
- Comprehensive test coverage for DependencyGraph and RequiredFeatures
- Smart feature detection system:
  - Auditable entities (CreatedDate, ModifiedDate)
  - Versioned entities (Version, RowVersion)
  - Soft-deletable entities (IsDeleted, DeletedDate)
- Enhanced dependency graph generation with support for complex relationships
- Support for .NET 9.0

### Changed
- Improved null handling and error resilience throughout the codebase
- Enhanced code generation templates
- Updated documentation with comprehensive examples

### Fixed
- Fixed null reference exceptions in relationship handling
- Improved circular reference detection

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
