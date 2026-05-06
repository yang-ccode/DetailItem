# .NET 8 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 8 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8 upgrade.
3. Upgrade src\DetailItem.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name | Description |
|:-------------|:-----------:|

### Project upgrade details

#### src\DetailItem.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net8.0-windows`

Other changes:
  - Update project assets and code as needed for compatibility with Revit 2025 and .NET 8.
  - Rebuild the add-in and validate that the generated assembly and manifest remain aligned with the Revit 2025 add-in deployment path.
