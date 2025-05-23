# Changelog

All notable changes to the SoftwareEngineerSkills project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased] - yyyy-mm-dd

### Added
- **Enhanced Domain-Driven Design (DDD) Implementation**:
  - Comprehensive domain base classes (`BaseEntity`, `AggregateRoot`, `ValueObject`) with invariant validation
  - Domain exception hierarchy for better error handling
  - Thread-safe domain event handling system
  - Soft delete capabilities with `SoftDeleteEntity` and `SoftDeleteAggregateRoot`
- **Entity Auditing System**:
  - Automatic tracking of creation/modification timestamps and users
  - Interfaces for auditable entities (`IAuditableEntity`, `ISoftDelete`)
  - Current user service integration for proper audit trails
- **Repository Pattern Enhancements**:
  - Specialized soft-delete repositories with automatic filtering
  - `GetByIdOrThrowAsync` methods for entity existence validation
  - EF Core extensions for soft-deleted entities
- **Domain Value Objects**:
  - `Address` value object for customer shipping information
- **Documentation Improvements**:
  - Comprehensive Domain Layer Reference Guide aligned with actual implementation
  - Updated project structure documentation to reflect current codebase organization
  - Corrected audit property naming conventions in documentation

### Changed
- **Project Structure**:
  - Reorganized domain layer: "Entities" → "Aggregates" for better DDD alignment
  - Simplified inheritance hierarchy by removing redundant `Entity` class
  - Moved interfaces to consistent locations (`Common/Interfaces`)
  - Standardized domain event handling across all aggregates
- **Domain Events**:
  - Improved event naming and documentation consistency
  - Added specific events for state changes (e.g., `SkillDifficultyChangedEvent`)
- **Entity Framework Integration**:
  - Global query filters for soft-deleted entities
  - Automatic audit property handling in `ApplicationDbContext`

### Fixed
- Replaced hardcoded "system" user with proper user context resolution in auditing
- **Documentation Consistency**: Fixed discrepancies between documented and actual:
  - Audit property naming conventions
  - Project folder structure (Entities vs Aggregates)
  - Domain event handling location (BaseEntity vs AggregateRoot only)

## [0.1.0] - 2025-05-01

### Added
- Initial Clean Architecture setup with Domain, Application, Infrastructure, and API layers
- Basic CQRS implementation with MediatR
- Entity Framework Core integration
- Unit test structure for all layers

[Unreleased]: https://github.com/yourusername/SoftwareEngineerSkills/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/yourusername/SoftwareEngineerSkills/releases/tag/v0.1.0
