# GitHub Copilot - Structured Prompting Guide for SoftwareEngineerSkills Project

This document outlines a recommended strategy for crafting effective prompts for GitHub Copilot, particularly for complex tasks within the SoftwareEngineerSkills project. The goal is to ensure Copilot receives sufficient context to generate high-quality, efficient, and robust code that aligns with project standards and best practices.

## Core Strategy: Main Prompt + Detailed Instructions File

To provide comprehensive guidance to Copilot, we advocate for a two-part prompting structure:

1.  **Main Prompt File (e.g., `feature_x.prompt.md` or this `wip.prompt.md` when used for a specific task):**
    *   Serves as the primary, high-level request to Copilot.
    *   Clearly states the main objective or the specific task at hand.
    *   **Crucially, it must reference a separate, detailed instructions file** that contains project-specific guidelines, architectural patterns, coding conventions, and sub-task breakdowns.
    *   Can include context attachments like the active editor content, specific file paths, or selected code snippets.

2.  **Detailed Instructions File (e.g., `copilot_project_guidelines.instructions.md`):**
    *   This is a comprehensive document containing all project-specific details that Copilot needs to understand the development context. This includes:
        *   Project definition (domain, tech stack).
        *   Architectural patterns (Layered, REST, DI, Repository, CQRS).
        *   Coding conventions and standards (C# versions, async/await, LINQ, naming, XML comments).
        *   Project structure.
        *   Workspace-specific instructions (ASP.NET Core for .NET 9, EF Core for .NET 9, error handling with `Result` pattern, testing with xUnit & Moq, DTO usage, validation).
        *   Testing and documentation standards.
        *   Security considerations.
        *   Performance requirements.
    *   It should break down larger tasks or concepts into smaller, digestible pieces of information or sub-tasks.
    *   This file should be maintained and updated as the project evolves.

## How to Use This Strategy:

1.  **Maintain Your Detailed Instructions File:**
    Ensure you have a comprehensive `.instructions.md` file for the SoftwareEngineerSkills project. The detailed "Copilot Instructions for the SoftwareEngineerSkills .NET Project" you provided is an excellent candidate for this file. Let's assume you save it as `.github/prompts/project_copilot_guidelines.instructions.md`.

2.  **Craft Your Main Prompt (in `wip.prompt.md` or a new task-specific prompt file):**
    When you need Copilot's assistance for a task, structure your prompt like this:

    ```markdown
    # Task: [Briefly describe the main goal, e.g., Implement user authentication feature]

    I need assistance with [specific part of the goal, e.g., creating the JWT generation service].

    Please adhere to all guidelines, coding standards, and architectural patterns detailed in the project's Copilot instructions:
    `@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/.github/prompts/project_copilot_guidelines.instructions.md`

    Specifically, for this task, focus on:
    - [Sub-task 1, e.g., Define the IJwtService interface in the Application layer]
    - [Sub-task 2, e.g., Implement JwtService in the Infrastructure layer using C# 12 primary constructors]
    - [Sub-task 3, e.g., Ensure proper error handling using the Result pattern]

    The relevant files are:
    `@file /path/to/existing/related/file1.cs`
    `@file /path/to/existing/related/file2.cs` (if any)

    Refer to the official .NET 9 documentation via `context7` if specific new API details are needed.
    ```

3.  **Iterate with Copilot:**
    Provide this main prompt to Copilot. You can then iterate by referring to specific sections or sub-tasks within your `.instructions.md` file.

## Benefits of This Approach:

*   **Enhanced Context:** Provides Copilot with deep, structured knowledge about your project.
*   **Consistency:** Helps ensure generated code aligns with established project conventions.
*   **Efficiency:** Reduces the need to repeat common instructions in every prompt.
*   **Modularity:** Allows for easy updates to project guidelines in a central place.
*   **Improved Quality:** Enables Copilot to act as a more informed pair programmer, leading to more robust and maintainable code.

By adopting this structured prompting methodology, you can significantly improve the effectiveness of GitHub Copilot for the SoftwareEngineerSkills project.


#fetch 
   https://code.visualstudio.com/docs/copilot/copilot-customization
   https://code.visualstudio.com/docs/copilot/copilot-tips-and-tricks
   https://docs.github.com/en/copilot
   https://docs.github.com/en/copilot/using-github-copilot/best-practices-for-using-github-copilot
   https://docs.github.com/en/copilot/using-github-copilot/copilot-chat/prompt-engineering-for-copilot-chat

use context 7 to query the knowledge from the official documentation.
