---
mode: 'agent'
tools: ['codebase', 'file', 'fetch', 'context7']
description: 'Analyze Domain layer with full project context'
---

Conduct a comprehensive analysis of the domain layer within the codebase, focusing on:

1. Domain model clarity and correctness:
   - Entity relationships and aggregates
   - Business rules and invariants
   - Value objects vs entities separation

2. Design patterns implementation:
   - Domain-Driven Design principles adherence
   - SOLID principles application 
   - Tactical patterns usage (factories, repositories, services)

3. Code quality metrics:
   - Coupling between domain objects
   - Cohesion within aggregates
   - Business logic encapsulation
   - Domain event handling

4. Technical debt assessment:
   - Code duplication in domain logic
   - Business rule violations
   - Outdated modeling patterns

Refer to [TeamOrganization.md](../../Docs/Team/TeamOrganization.md) for team structure and responsibilities.

Deliverables:
1. Full analysys report as markdown on the file [DomainLayerAnalysis.md](../../Docs/Tasks/DomainLayerAnalysis.md)
   - Prioritized list of improvement opportunities
   - Impact assessment for each finding
   - Refactoring recommendations with concrete examples
   - Risk analysis for suggested changes
2. Full board with prioritized actionable tasks in [DomainLayerBoard](../../Docs/Tasks/DomainLayerBoard.md) based on [TeamOrganization.md](../../Docs/Team/TeamOrganization.md)
   - Tasks categorized by team responsibilities
   - Clear descriptions and acceptance criteria for each task
   - Each actionable task should include:
     - Task title
     - Description
     - Priority level
     - Assigned team (Domain Model Team, Infrastructure Team, etc.)
     - Estimated effort
     - Acceptance criteria
     - Especialiced prompt for the team to implement the task with enough details and context

Ensure the analysis is actionable, providing clear next steps for the Domain Model Team to enhance the domain layer's quality and maintainability.

Consider both immediate optimizations and long-term architectural improvements while maintaining backward compatibility.
