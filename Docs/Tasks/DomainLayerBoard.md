# Domain Layer Task Board

This board outlines prioritized actionable tasks based on the Domain Layer Analysis Report. Tasks are assigned to relevant teams as per the TeamOrganization.md document.

## High Priority Tasks

---

### Task ID: DL-001
**Task Title:** Refactor `DeveloperSkill` to Establish Clear Relationship with Canonical `Skill` Entity
**Description:** The current `DeveloperSkill` entity uses a string `Name` for skills, potentially leading to data inconsistency and making it difficult to manage a canonical list of skills. This task involves refactoring `DeveloperSkill` to link to the `Skill` entity via `SkillId`, and updating related logic in the `Developer` aggregate.
**Priority Level:** High
**Assigned Team:** Domain Model Team
**Estimated Effort:** L
**Expected Outcome:**
*   `DeveloperSkill` entity will reference a canonical `Skill` entity via `SkillId`.
*   Reduced data redundancy and improved data integrity for skills.
*   Simplified querying for developers based on canonical skills.
*   Clear distinction between canonical skill properties and developer-specific skill attributes.
**Acceptance Criteria:**
1.  `DeveloperSkill.cs` is modified to include `SkillId` (Guid) property.
2.  The `DeveloperSkill` constructor is updated to accept `SkillId` instead of `skillName` and `category`.
3.  `Developer.AddSkill()` method signature is changed to accept a `Skill` object or `SkillId`.
4.  Logic within `Developer.AddSkill()` is updated to create `DeveloperSkill` using `SkillId` and to check for duplicates based on `SkillId`.
5.  Domain events like `SkillAddedToDeveloperEvent` are updated to reflect the changes (e.g., include `canonicalSkillName`, `canonicalSkillCategory` if needed).
6.  Redundant validation for skill name/description length in `DeveloperSkill.CheckInvariants` is removed if these properties are now primarily derived from the canonical `Skill` entity.
7.  Unit tests for `Developer` and `DeveloperSkill` are updated or created to verify the new relationship, skill addition, and duplicate checking.
8.  Consideration for data migration of existing `DeveloperSkill` entries is documented (migration script implementation can be a separate task).
**Specialized Prompt for Domain Model Team:**
"Implement changes to the `DeveloperSkill` entity and `Developer` aggregate to establish a foreign key relationship from `DeveloperSkill` to the `Skill` entity using `SkillId`.
Tasks:
1.  Modify `DeveloperSkill.cs`: Add `public Guid SkillId { get; private set; }`. Remove `Name` and `Category` string properties if they are to be sourced from the `Skill` entity via `SkillId`. Adjust constructor to take `skillId`. Update `CheckInvariants` to remove validations for `Name`/`Category` if they are removed.
2.  Modify `Developer.cs`: Update `AddSkill` method to accept `Skill canonicalSkill` (or `Guid skillId`), and instantiate `DeveloperSkill` using `canonicalSkill.Id`. Ensure duplicate skill check uses `SkillId`.
3.  Update `SkillAddedToDeveloperEvent`: Ensure it carries necessary identifiers (DeveloperId, SkillId) and potentially denormalized name/category from the canonical skill for consumers who need it.
4.  Review and update unit tests in `SoftwareEngineerSkills.Domain.UnitTests/` for `DeveloperTests` and any tests related to `DeveloperSkill`.
5.  Document any assumptions made about how `DeveloperSkill.Description` should be used (e.g., developer's personal notes vs. canonical skill description)."

---

## Medium Priority Tasks

---

### Task ID: DL-002
**Task Title:** Define `IDeveloperRepository` Interface and Integrate with `IUnitOfWork`
**Description:** The `Developer` aggregate root currently lacks a repository interface for persistence operations, and it's not exposed via `IUnitOfWork`. This task involves defining `IDeveloperRepository` and adding it to the `IUnitOfWork` interface.
**Priority Level:** Medium
**Assigned Team:** Domain Model Team (for interface definition), Infrastructure Team (for future implementation)
**Estimated Effort:** S
**Expected Outcome:**
*   An `IDeveloperRepository` interface is defined in the Domain layer, inheriting from `IRepository<Developer>`.
*   The `IUnitOfWork` interface exposes an `IDeveloperRepository` property.
*   The domain layer is prepared for `Developer` aggregate persistence.
**Acceptance Criteria:**
1.  A new file `IDeveloperRepository.cs` is created in `SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/Abstractions/Persistence/`.
2.  The `IDeveloperRepository` interface inherits from `IRepository<Developer>`.
3.  The interface includes any necessary developer-specific query methods (e.g., `GetByEmailAsync` - to be decided by the team).
4.  `IUnitOfWork.cs` is updated to include `IDeveloperRepository Developers { get; }`.
5.  The solution compiles successfully.
**Specialized Prompt for Domain Model Team:**
"Define the `IDeveloperRepository` interface and integrate it into `IUnitOfWork`.
Tasks:
1.  Create `IDeveloperRepository.cs` in `SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/Abstractions/Persistence/`.
2.  Define `public interface IDeveloperRepository : IRepository<Developer>`.
3.  Discuss and add at least one example of a developer-specific query method signature if obvious ones come to mind (e.g., `Task<Developer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);`), otherwise leave it minimal.
4.  Modify `IUnitOfWork.cs` to add the property: `IDeveloperRepository Developers { get; }`.
5.  Ensure the project builds."

---

## Low Priority Tasks

---

### Task ID: DL-003
**Task Title:** Remove Unused Validation Methods in `Skill` Entity
**Description:** The `Skill` entity (`Skill.cs`) contains unused private static validation methods (`ValidateName`, `ValidateDescription`) as the logic is handled in `CheckInvariants`. These should be removed to reduce code clutter.
**Priority Level:** Low
**Assigned Team:** Domain Model Team
**Estimated Effort:** S
**Expected Outcome:**
*   Unused methods are removed from `Skill.cs`.
*   Improved code clarity and reduced clutter in the `Skill` entity.
**Acceptance Criteria:**
1.  The `private static void ValidateName(string name)` method is removed from `Skill.cs`.
2.  The `private static void ValidateDescription(string description)` method is removed from `Skill.cs`.
3.  The `Skill` entity and overall solution compile and all existing tests pass.
**Specialized Prompt for Domain Model Team:**
"Remove the dead code (unused `ValidateName` and `ValidateDescription` static methods) from `SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/Entities/Skills/Skill.cs`. Verify that the removal does not affect any existing logic or tests."

---

### Task ID: DL-004
**Task Title:** Review and Potentially Enhance Email Validation in `Email` Value Object
**Description:** The `Email` value object uses custom logic for email validation. This task is to review this logic, compare with standard practices (e.g., using `System.Net.Mail.MailAddress` or third-party libraries), and enhance if deemed necessary for robustness.
**Priority Level:** Low
**Assigned Team:** Domain Model Team
**Estimated Effort:** M (if significant changes and new library evaluation is needed) / S (if only minor tweaks)
**Expected Outcome:**
*   Email validation in `Email.cs` is reviewed and confirmed to meet project requirements or is enhanced.
*   Reduced risk of accepting invalid email formats or rejecting valid ones.
**Acceptance Criteria:**
1.  The `Email.IsValidEmail` method in `Email.cs` is reviewed.
2.  A decision is made whether to keep, modify, or replace the current validation logic.
3.  If changes are made, they are implemented and unit tested thoroughly with various valid and invalid email examples.
4.  If a third-party library is considered, its suitability and licensing are evaluated.
**Specialized Prompt for Domain Model Team:**
"Review the email validation logic in `SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/ValueObjects/Email.cs`.
Tasks:
1.  Analyze the current `IsValidEmail` method for correctness and edge case handling.
2.  Compare its behavior with `System.Net.Mail.MailAddress` and/or research dedicated email validation libraries.
3.  Decide on an approach: keep current, modify, or replace.
4.  If modifying or replacing, implement the new logic.
5.  Write comprehensive unit tests for the email validation, covering common formats, international characters (if applicable), and various invalid inputs."

---
