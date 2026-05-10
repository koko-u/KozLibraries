# AGENTS.md

## Project Overview

KozLibraries is a collection of small .NET libraries written in C#.

Current packages include:

- `AutoRegisterAnnotation`: attributes and extension methods for service registration.
- `KozLibraries.TagHelpers`: ASP.NET Core Razor Tag Helpers.

The repository uses modern .NET with nullable reference types enabled.

## Repository Layout

- `KozLibraries.slnx`: solution file.
- `Directory.Build.props`: shared package metadata such as version, author, and repository URL.
- `src/AutoRegisterAnnotation/`: dependency injection auto-registration helpers.
- `src/TagHelpers/`: ASP.NET Core Razor Tag Helpers.
- `dotnet-tools.json`: local .NET tools, including CSharpier.

## Development Commands

Restore tools:

```sh
dotnet tool restore
```

Restore packages:

```sh
dotnet restore KozLibraries.slnx
```

Build:

```sh
dotnet build KozLibraries.slnx
```

Format:

```sh
dotnet csharpier format .
```

Pack:

```sh
dotnet pack KozLibraries.slnx
```

## Agent Behavior

- Do not edit code unless the user explicitly asks for code changes.
- When asked to investigate, review, explain, or propose changes, provide findings or a plan without modifying files.
- Before making file edits, identify the intended scope and keep changes limited to that scope.
- Do not revert user changes unless the user explicitly requests it.

## Coding Guidelines

- Use C# idioms consistent with the existing codebase.
- Keep nullable reference types enabled and handle nullability explicitly.
- Prefer simple public APIs; these libraries are intended to be small and reusable.
- Avoid adding unnecessary abstractions.
- Keep XML documentation on public types and public members when the API is intended for package consumers.
- Use file and namespace organization consistent with the existing projects.

## Formatting

Use CSharpier for formatting.

Do not hand-format code in a style that conflicts with CSharpier output.

## Public API Changes

These projects are libraries, so public API changes should be treated carefully.

When changing public types, methods, attributes, package IDs, namespaces, or behavior:

- Consider backward compatibility.
- Update XML documentation where appropriate.
- Prefer additive changes over breaking changes.
- If a breaking change is necessary, make it explicit in the change description.

## Project-Specific Notes

### AutoRegisterAnnotation

- Keep dependency injection behavior explicit and predictable.
- Avoid scanning more assemblies than requested by the caller.
- Preserve compatibility with `Microsoft.Extensions.DependencyInjection.Abstractions`.
- Attribute-based registration should remain easy to understand from the consuming application.

### KozLibraries.TagHelpers

- Tag Helpers should avoid surprising output changes.
- Remove custom marker attributes from rendered output when they are only used for server-side behavior.
- Preserve existing HTML attributes unless the Tag Helper intentionally modifies them.
- Use ASP.NET Core Tag Helper APIs rather than string-only HTML manipulation where possible.

## Testing

There are currently no test projects in the repository.

For behavior changes, consider adding tests before or alongside implementation. If tests are not added, manually verify with:

```sh
dotnet build KozLibraries.slnx
dotnet csharpier --check .
```

## Dependency Policy

- Keep dependencies minimal.
- Prefer framework references for ASP.NET Core functionality where appropriate.
- Avoid adding runtime dependencies unless they clearly benefit library consumers.
- Update package versions deliberately, especially for public package projects.

## Versioning and Packaging

Shared package metadata is defined in `Directory.Build.props`.

When preparing package releases:

- Update the shared `Version` value intentionally.
- Verify package IDs and descriptions in each `.csproj`.
- Run build and pack commands before publishing.
