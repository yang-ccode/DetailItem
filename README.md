# DetailItem – Revit Add-in

A Revit add-in that loads all **Detail Item** category elements from the active view into a live, interactive form.

---

## Features

| Feature | Description |
|---------|-------------|
| **Parameter ComboBox** | Pick any parameter available on Detail Item elements. The *Parameter Name* and *Parameter Value* columns update instantly. |
| **Checkbox selection** | Each row has a checkbox. Use **Check All / Uncheck All** to bulk-select without hiding the form or switching views. |
| **Highlight Checked** | Pushes the checked elements into the Revit selection so they appear in the Properties Panel — the form stays visible. |
| **Row selection → Revit selection** | Clicking one or more rows in the grid also selects the corresponding elements in Revit in real time. |
| **Column sorting** | Click any column header to sort ascending/descending. |
| **Refresh** | Reloads parameter names and element data from the current active view. |

---

## Project Layout

```
DetailItem/
├── DetailItem.sln              ← Visual Studio solution
├── src/
│   ├── DetailItem.csproj       ← .NET 4.8 WinForms project
│   ├── Command.cs              ← IExternalCommand entry point
│   ├── Models/
│   │   └── DetailItemRow.cs    ← Row data model
│   ├── Services/
│   │   └── DetailItemCollector.cs  ← Revit API data collector
│   └── UI/
│       ├── RevitWindowHandle.cs     ← IWin32Window wrapper
│       ├── SelectionHandler.cs      ← IExternalEventHandler for element selection
│       ├── DetailItemForm.cs        ← Form logic
│       └── DetailItemForm.Designer.cs ← Generated form layout
└── manifest/
    └── DetailItem.addin        ← Revit add-in manifest
```

---

## Requirements

- **Revit** 2022 / 2023 / 2024 / 2025 (64-bit)
- **.NET Framework 4.8** (bundled with Windows 10/11 and Revit 2022+)
- **Visual Studio 2022** (or any IDE that supports SDK-style `.csproj`)

---

## Build

### 1. Set the Revit API path

The project resolves `RevitAPI.dll` / `RevitAPIUI.dll` from the environment variable **`REVIT_API_PATH`** (falls back to `C:\Program Files\Autodesk\Revit 2024`).

```powershell
# PowerShell – set for the current session
$env:REVIT_API_PATH = "C:\Program Files\Autodesk\Revit 2024"
```

Or create a `Directory.Build.props` file next to the solution:

```xml
<Project>
  <PropertyGroup>
    <RevitAPIPath>C:\Program Files\Autodesk\Revit 2024</RevitAPIPath>
  </PropertyGroup>
</Project>
```

### 2. Build

```bash
dotnet build DetailItem.sln -c Release
```

Output: `src/bin/Release/net48/DetailItem.dll`

---

## Installation

1. Build the project (see above).
2. Create the add-in folder:
   ```
   %APPDATA%\Autodesk\Revit\Addins\2024\DetailItem\
   ```
3. Copy `DetailItem.dll` (and any satellite assemblies) into that folder.
4. Edit `manifest/DetailItem.addin` → update the `<Assembly>` path to match step 2.
5. Copy `DetailItem.addin` to `%APPDATA%\Autodesk\Revit\Addins\2024\`.
6. Start Revit.  The command **Detail Item Manager** is available under *Add-Ins → External Tools*.

---

## Usage

1. Open a Revit project and navigate to a view that contains Detail Items.
2. Run **Detail Item Manager** from *Add-Ins → External Tools*.
3. The form opens and lists all Detail Item instances visible in the active view.
4. Use the **Parameter** drop-down to choose which parameter to display.
5. Click row(s) in the grid → those elements are selected/highlighted in Revit immediately.
6. Use the checkboxes (or **Check All**) to mark rows, then click **Highlight Checked** to push the selection to the Properties Panel.
7. Click any column header to sort the list.
8. Click **Refresh** to reload after adding/deleting elements.

---

## Architecture Notes

| Layer | Class | Responsibility |
|-------|-------|----------------|
| Entry point | `Command` | Implements `IExternalCommand`; creates `ExternalEvent` + form |
| Data | `DetailItemCollector` | Pure read-only queries via `FilteredElementCollector` |
| Model | `DetailItemRow` | Plain data transfer object for one grid row |
| UI | `DetailItemForm` | WinForms form; owns `DataTable`/`DataView` for sorting |
| Revit bridge | `SelectionHandler` | `IExternalEventHandler` that calls `Selection.SetElementIds` on the Revit main thread |

The form is shown **modeless** (`form.Show(owner)`) so the user can continue working in Revit while it is open. All calls back into the Revit API go through `ExternalEvent.Raise()` to respect Revit's single-threaded API contract.
