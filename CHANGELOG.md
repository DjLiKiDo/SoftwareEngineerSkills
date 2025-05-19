# Changelog

All notable changes to the SoftwareEngineerSkills project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased] - yyyy-mm-dd

### Added
- **Domain-Driven Design Core Components**:
  - Enhanced `BaseEntity` with invariant validation system
  - Improved `AggregateRoot` class for domain event handling and invariant enforcement
  - Extended `IAggregateRoot` interface with versioning and boundary enforcement methods
  - Added `DomainValidationException` for domain invariant violations
  - Implemented thread-safe domain event handling
  - Created comprehensive documentation of DDD core components
- **Entity Auditing System**:
  - Enhanced architecture with interfaces for separation of concerns
  - `IAuditableEntity` interface for tracking entity changes (creation/modification timestamps and users)
  - `ISoftDelete` interface for entities that support soft deletion
  - `SoftDeleteEntity` base class implementing soft-delete functionality
  - `ICurrentUserService` and implementation to capture the current user for auditing
- **Repository Enhancements**:
  - Specialized repositories for soft-delete entities (`ISoftDeleteRepository<T>`)
  - EF Core extensions for working with soft-deleted entities
  - Global query filters to automatically exclude soft-deleted entities
- **Documentation**:
  - Comprehensive documentation on the entity auditing approach and implementation
  - Added new documentation for DDD core components and invariant validation system

### Changed
- **Code Organization**:
  - Refactored user service code to its own folder under Infrastructure/Services/User for better organization
- **Entity Framework Implementation**:
  - Updated `BaseEntity` to implement `IAuditableEntity` interface
  - Modified `ApplicationDbContext` to handle soft deletes and fetch current user information
  - Refactored audit property settings to use the current user service

### Fixed
- **Security & Data Integrity**:
  - Fixed hardcoded "system" user in auditing data by implementing proper user context resolution

## [0.1.0] - 2025-05-01

### Added
- Initial project setup with Clean Architecture structure
- Domain layer with base entities and common interfaces
- Infrastructure layer with repositories and DbContext
- Application layer with basic CQRS setup
- API layer with basic configuration
- Unit tests structure for each layer

[Unreleased]: https://github.com/yourusername/SoftwareEngineerSkills/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/yourusername/SoftwareEngineerSkills/releases/tag/v0.1.0
