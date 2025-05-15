Create a comprehensive copilot.instructions.md file to optimize AI assistance for this project following these guidelines:

1. Create a file named `copilot.instructions.md` at the root of the workspace.

2. Define the project's:
   - Primary domain and tech stack
   - Key architectural patterns
   - Coding conventions and standards
   - Common terminology and abbreviations
   - Project structure and organization

3. Include workspace-specific instructions:
   - Preferred code patterns and practices
   - Error handling requirements
   - Testing and documentation standards
   - Security considerations
   - Performance requirements

4. Specify file-level instructions:
   - Place `.copilot` folders in relevant directories
   - Create task-specific instruction files using the pattern:
     `{component-name}.instructions.md` or `.copilot/instructions.md`

5. Format instructions with:
   - Clear, concise language
   - Code examples for common patterns
   - Links to relevant documentation
   - Task-specific context and constraints

6. Organize the file structure:
```
project-root/
├── copilot.instructions.md
├── src/
│   ├── .copilot/
│   │   └── instructions.md
│   └── components/
│       └── {component-name}/
│           └── .copilot/
│               └── instructions.md
```

Reference: https://code.visualstudio.com/docs/copilot/copilot-customization

use context 7