---
applyTo: '**'
---
# AGENTS Guidelines for .NET Application Development

This repository contains a .NET application. When working on the project interactively with an agent (e.g. GitHub Copilot, Codex CLI, etc.), please follow these guidelines to ensure a smooth and efficient development experience, especially regarding hot reload and dependency management.

## 1. Use the Development Server and Hot Reload, **not** Production Build

* **Always use `dotnet watch run`** (or the equivalent hot reload command in your IDE) while iterating on the application. This starts the application in development mode with hot-reload enabled.
* **Do _not_ run `dotnet publish` or build production artifacts inside the agent session.** Running these commands can overwrite development builds, disable hot reload, or leave the environment in an inconsistent state. If a production build is required, perform it outside the interactive agent workflow.

## 2. Keep Dependencies in Sync

If you add or update dependencies, remember to:

1. Update the relevant project file (`.csproj`, `.fsproj`, etc.) to include the new or updated package references.
2. Run `dotnet restore` to update the dependency cache.
3. Re-start the development server so the application picks up the changes.

## 3. Coding Conventions

* Prefer C# (`.cs`) or F# (`.fs`) for new components, following the language conventions used in the project.
* Organize related files (such as models, controllers, and views) into appropriate folders.
* Co-locate component-specific styles or resources in the same folder as the component where practical.

## 4. Useful Commands Recap

| Command                 | Purpose                                                      |
| ----------------------- | ------------------------------------------------------------ |
| `dotnet watch run`      | Start the dev server with hot reload enabled.                |
| `dotnet build`          | Build the application (development mode).                    |
| `dotnet test`           | Run the test suite.                                          |
| `dotnet restore`        | Restore NuGet packages and dependencies.                     |
| `dotnet publish`        | **Production build – _do not run during agent sessions_**    |

---

Following these practices ensures that agent-assisted development stays fast and dependable. When in doubt, restart the development server rather than running a production build. 