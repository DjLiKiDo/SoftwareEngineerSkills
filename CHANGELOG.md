# Changelog

All notable changes to the SoftwareEngineerSkills project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Enhanced entity auditing system with interfaces for separation of concerns
- `IAuditableEntity` interface for tracking entity changes
- `ISoftDelete` interface for entities that support soft deletion
- `SoftDeleteEntity` base class for soft-deletable entities
- `ICurrentUserService` and implementation to capture the current user for auditing
- Specialized repositories for soft-delete entities (`ISoftDeleteRepository<T>`)
- EF Core extensions for working with soft-deleted entities
- Global query filters to automatically exclude soft-deleted entities
- Comprehensive documentation on the entity auditing approach

### Changed
- Refactored user service code to its own folder under Infrastructure/Services/User for better organization

- Updated `BaseEntity` to implement `IAuditableEntity` interface
- Modified `ApplicationDbContext` to handle soft deletes and fetch current user information
- Refactored audit property settings to use the current user service

### Fixed
- Fixed hardcoded "system" user in auditing data

## [0.1.0] - 2025-05-01

### Added
- Initial project setup with Clean Architecture structure
- Domain layer with base entities and common interfaces
- Infrastructure layer with repositories and DbContext
- Application layer with basic CQRS setup
- API layer with basic configuration
- Unit tests structure for each layer
