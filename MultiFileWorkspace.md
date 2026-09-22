# Refactoring Plan – Multi‑Project / Multi‑File Workspace App (.NET Framework 4.6‑4.8)

Below is a complete re‑architecture that turns the current "one global set of controls (`THFilesList`, `THFileElementsDataGridView`, …) that everything talks to" design into a properly layered, testable, per‑project scoped application.

---

## 1. Goals

| Problem today | Fix |
|---|---|
| Global/static controls (`THFilesList`, `THSourceRichTextBox`, …) used everywhere | Controls become **instance members** owned by a per‑project view; business logic never touches controls at all |
| Business logic (`RowBase`, format loading) mixed with UI code | Move to a **Services** layer that only knows about `DataTable`/Models |
| One project assumed | `Project` becomes a first‑class model; everything (files, grid, textboxes) is scoped **per `Project` instance** |
| `[ALL]` pseudo file is a special case hacked into the UI | Modeled as just another `OpenedFileData`, produced/merged by a service |
| Hard to unit test | Models/Services have zero `System.Windows.Forms` dependency |

---

## 2. Solution / Project layout

```
MyApp.sln
 ├─ MyApp.Common        (Class Library)  – base classes, interfaces, no UI, no IO
 ├─ MyApp.Models        (Class Library)  – POCO/observable data model
 ├─ MyApp.FileFormats   (Class Library)  – IFileFormat implementations (csv/po/xliff/...)
 ├─ MyApp.Services      (Class Library)  – business logic (project/file/row operations)
 ├─ MyApp.UI            (Windows Forms Class Library) – Forms, UserControls, Presenters
 └─ MyApp.App           (Windows Forms Application, exe) – composition root (Program.cs)
 
*MyApp means the current refactoring target app name
```

**Dependency direction (no cycles):**

```
MyApp.App  ──►  MyApp.UI  ──►  MyApp.Services  ──►  MyApp.Models
                     │                │                  │
                     ▼                ▼                  ▼
                MyApp.Common ◄── MyApp.FileFormats ──► MyApp.Common
```

`MyApp.Models` never references `MyApp.FileFormats` or `System.Windows.Forms` — it only depends on `MyApp.Common` (for `ObservableObject` and the `IFileFormat` interface). This is what lets you unit‑test parsing/row logic without spinning up any WinForms control.

---

## 3. `MyApp.Common` – shared building blocks

```csharp
// MyApp.Common/ObservableObject.cs
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
```

```csharp
// MyApp.Common/IFileFormat.cs
public interface IFileFormat
{
    string Id { get; }                       // "PO", "CSV", "XLIFF"...
    bool CanHandle(string filePath);
    DataTable Load(string filePath);
    void Save(string filePath, DataTable table);
}
```

---

## 4. `MyApp.Models` – pure data, per‑project scoped

```csharp
// MyApp.Models/ProjectsData.cs
public class ProjectsData : ObservableObject
{
    public BindingList<Project> ProjectsList { get; } = new BindingList<Project>();

    private Project _selectedProject;
    public Project SelectedProject
    {
        get => _selectedProject;
        set => SetProperty(ref _selectedProject, value);
    }
}
```

```csharp
// MyApp.Models/Project.cs
public class Project : ObservableObject
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public string RootPath { get; set; }

    /// Each project owns its own opened files state - no more globals.
    public OpenedFilesData OpenedFilesData { get; } = new OpenedFilesData();

    public override string ToString() => Name; // convenient for tab header binding
}
```

```csharp
// MyApp.Models/OpenedFilesData.cs
public class OpenedFilesData : ObservableObject
{
    public BindingList<OpenedFileData> OpenedFilesList { get; } = new BindingList<OpenedFileData>();

    private OpenedFileData _selectedOpenedFileData;
    public OpenedFileData SelectedOpenedFileData
    {
        get => _selectedOpenedFileData;
        set => SetProperty(ref _selectedOpenedFileData, value);
    }

    /// Cached [ALL] aggregate, created lazily by the service layer.
    public OpenedFileData AllFilesView { get; internal set; }
}
```

```csharp
// MyApp.Models/OpenedFileData.cs
public class OpenedFileData : ObservableObject
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public bool IsAllFilesAggregate { get; set; }   // true only for the "[ALL]" item

    public IFileFormat Format { get; set; }

    private DataTable _table;
    public DataTable Table
    {
        get => _table;
        set => SetProperty(ref _table, value);
    }

    public bool IsDirty { get; set; }
}
```

> Notice: `OpenedFileData` references `IFileFormat` (an interface from `MyApp.Common`), **not** a concrete parser. Concrete parsers live in `MyApp.FileFormats` and are resolved by a service — Models stay dumb and dependency‑free.

---

## 5. `MyApp.FileFormats`

```csharp
public sealed class CsvFileFormat : IFileFormat
{
    public string Id => "CSV";
    public bool CanHandle(string filePath) => Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase);
    public DataTable Load(string filePath) { /* ... */ return new DataTable(); }
    public void Save(string filePath, DataTable table) { /* ... */ }
}

public interface IFileFormatResolver
{
    IFileFormat Resolve(string filePath);
}

public class FileFormatResolver : IFileFormatResolver
{
    private readonly IEnumerable<IFileFormat> _formats;
    public FileFormatResolver(IEnumerable<IFileFormat> formats) => _formats = formats;

    public IFileFormat Resolve(string filePath)
        => _formats.FirstOrDefault(f => f.CanHandle(filePath))
           ?? throw new NotSupportedException($"No format handler for '{filePath}'.");
}
```

---

## 6. `MyApp.Services` – all logic that used to live in `RowBase` etc.

### 6.1 Project service (the "Open" workflow)

```csharp
public interface IProjectService
{
    Project OpenProject(string path);
    void SaveProject(Project project);
    void CloseProject(Project project);
}

public class ProjectService : IProjectService
{
    public Project OpenProject(string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException(path);

        return new Project
        {
            Name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar)),
            RootPath = path
        };
    }

    public void SaveProject(Project project) { /* persist project metadata */ }
    public void CloseProject(Project project) { /* cleanup, prompt for unsaved changes handled by presenter */ }
}
```

### 6.2 File workspace service (opening files + `[ALL]` aggregation)

```csharp
public interface IFileWorkspaceService
{
    OpenedFileData OpenFile(Project project, string filePath);
    void CloseFile(Project project, OpenedFileData file);
    OpenedFileData GetOrCreateAllFilesView(Project project);
    void CommitAllFilesChanges(Project project);
}

public class FileWorkspaceService : IFileWorkspaceService
{
    private readonly IFileFormatResolver _resolver;
    public FileWorkspaceService(IFileFormatResolver resolver) => _resolver = resolver;

    public OpenedFileData OpenFile(Project project, string filePath)
    {
        var format = _resolver.Resolve(filePath);
        var data = new OpenedFileData
        {
            FileName = Path.GetFileName(filePath),
            FilePath = filePath,
            Format = format,
            Table = format.Load(filePath)
        };
        project.OpenedFilesData.OpenedFilesList.Add(data);
        project.OpenedFilesData.AllFilesView = null; // invalidate cache
        return data;
    }

    public void CloseFile(Project project, OpenedFileData file)
        => project.OpenedFilesData.OpenedFilesList.Remove(file);

    public OpenedFileData GetOrCreateAllFilesView(Project project)
    {
        if (project.OpenedFilesData.AllFilesView != null)
            return project.OpenedFilesData.AllFilesView;

        var merged = new DataTable();
        merged.Columns.Add("__SourceFile", typeof(string));
        merged.Columns.Add("__SourceRowIndex", typeof(int));

        foreach (var file in project.OpenedFilesData.OpenedFilesList)
            foreach (DataColumn col in file.Table.Columns)
                if (!merged.Columns.Contains(col.ColumnName))
                    merged.Columns.Add(col.ColumnName, col.DataType);

        foreach (var file in project.OpenedFilesData.OpenedFilesList)
        {
            for (int i = 0; i < file.Table.Rows.Count; i++)
            {
                var src = file.Table.Rows[i];
                var row = merged.NewRow();
                row["__SourceFile"] = file.FileName;
                row["__SourceRowIndex"] = i;
                foreach (DataColumn col in file.Table.Columns)
                    row[col.ColumnName] = src[col.ColumnName];
                merged.Rows.Add(row);
            }
        }

        var allData = new OpenedFileData
        {
            FileName = "[ALL]",
            IsAllFilesAggregate = true,
            Table = merged
        };
        project.OpenedFilesData.AllFilesView = allData;
        return allData;
    }

    public void CommitAllFilesChanges(Project project)
    {
        var all = project.OpenedFilesData.AllFilesView;
        if (all == null) return;

        foreach (DataRow row in all.Table.Rows)
        {
            var fileName = (string)row["__SourceFile"];
            var index = (int)row["__SourceRowIndex"];
            var target = project.OpenedFilesData.OpenedFilesList.First(f => f.FileName == fileName);
            var targetRow = target.Table.Rows[index];

            foreach (DataColumn col in target.Table.Columns)
                targetRow[col.ColumnName] = row[col.ColumnName];

            target.IsDirty = true;
        }
    }
}
```

### 6.3 Row operations (this replaces the old `RowBase` static class)

**Before** (implicit dependency on global controls):
```csharp
public static class RowBase
{
    public static void ApproveRow()
    {
        var row = THFileElementsDataGridView.SelectedRows[0];
        row.Cells["Status"].Value = "Approved";
        THTargetRichTextBox.Text = row.Cells["Target"].Value.ToString();
    }
}
```

**After** (works with data only — reusable for *any* project's grid):
```csharp
public interface IRowOperationsService
{
    void ApproveRow(OpenedFileData file, DataRow row);
    void RejectRow(OpenedFileData file, DataRow row);
    void SetTargetText(OpenedFileData file, DataRow row, string newText);
}

public class RowOperationsService : IRowOperationsService
{
    public void ApproveRow(OpenedFileData file, DataRow row)
    {
        row["Status"] = "Approved";
        file.IsDirty = true;
    }

    public void RejectRow(OpenedFileData file, DataRow row)
    {
        row["Status"] = "Rejected";
        file.IsDirty = true;
    }

    public void SetTargetText(OpenedFileData file, DataRow row, string newText)
    {
        row["Target"] = newText;
        file.IsDirty = true;
    }
}
```

No control types appear in the signature anywhere — **the service doesn't know a `DataGridView` exists**. This is the real fix behind "must work with their project's controls as input parameters": instead of passing controls around, we pass the **project-scoped data** (`OpenedFileData`/`DataRow`) and let the UI layer be the only place that touches controls.

---

## 7. `MyApp.UI` – Views + Presenters (thin controls, no logic)

### 7.1 Problem: `TabControl` isn't bindable out of the box

WinForms `TabControl.TabPages` has no `DataSource`. We add one small reusable helper instead of hand-rolling per-control sync code:

```csharp
// MyApp.UI/Helpers/TabControlBinder.cs
public static class TabControlBinder<T> where T : class
{
    public static void Bind(TabControl tabControl, BindingList<T> source, Func<T, TabPage> tabPageFactory)
    {
        foreach (var item in source)
            tabControl.TabPages.Add(tabPageFactory(item));

        source.ListChanged += (s, e) =>
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    var page = tabPageFactory(source[e.NewIndex]);
                    tabControl.TabPages.Insert(e.NewIndex, page);
                    tabControl.SelectedTab = page;
                    break;

                case ListChangedType.ItemDeleted:
                    if (e.NewIndex < tabControl.TabPages.Count)
                        tabControl.TabPages.RemoveAt(e.NewIndex);
                    break;
            }
        };
    }
}
```

### 7.2 `ProjectsTabControl` wiring (top level)

```csharp
// In MainForm
public MainForm(ProjectsData projectsData, IProjectService projectService,
                 IFileWorkspaceService fileService, IRowOperationsService rowService)
{
    InitializeComponent();

    TabControlBinder<Project>.Bind(projectsTabControl, projectsData.ProjectsList, project =>
    {
        var panel = new ProjectFilesWorkspacePanel { Dock = DockStyle.Fill };
        var presenter = new ProjectWorkspacePresenter(project, panel, fileService, rowService);
        presenter.Initialize();

        return new TabPage(project.Name) { Tag = project, Controls = { panel } };
    });

    projectsTabControl.SelectedIndexChanged += (s, e) =>
        projectsData.SelectedProject = projectsTabControl.SelectedTab?.Tag as Project;
}
```

### 7.3 `ProjectFilesWorkspacePanel` + `OpenedFilesTabControl` presenter

```csharp
public class ProjectWorkspacePresenter
{
    private readonly Project _project;
    private readonly ProjectFilesWorkspacePanel _view;
    private readonly IFileWorkspaceService _fileService;
    private readonly IRowOperationsService _rowService;

    public ProjectWorkspacePresenter(Project project, ProjectFilesWorkspacePanel view,
        IFileWorkspaceService fileService, IRowOperationsService rowService)
    {
        _project = project;
        _view = view;
        _fileService = fileService;
        _rowService = rowService;
    }

    public void Initialize()
    {
        RefreshFilesList();

        _view.FilesList.SelectedIndexChanged += OnFileSelected;

        TabControlBinder<OpenedFileData>.Bind(
            _view.OpenedFilesTabControl,
            _project.OpenedFilesData.OpenedFilesList,
            CreateFileTab);
    }

    private void RefreshFilesList()
    {
        _view.FilesList.Items.Clear();
        _view.FilesList.Items.Add("[ALL]");
        foreach (var f in _project.OpenedFilesData.OpenedFilesList)
            _view.FilesList.Items.Add(f.FileName);
    }

    private void OnFileSelected(object sender, EventArgs e)
    {
        var name = _view.FilesList.SelectedItem as string;
        OpenedFileData data = name == "[ALL]"
            ? _fileService.GetOrCreateAllFilesView(_project)
            : _project.OpenedFilesData.OpenedFilesList.FirstOrDefault(f => f.FileName == name);

        if (data != null)
            _project.OpenedFilesData.SelectedOpenedFileData = data; // triggers tab select via binding
    }

    private TabPage CreateFileTab(OpenedFileData file)
    {
        var workspace = new OpenedFileWorkspace { Dock = DockStyle.Fill };
        new OpenedFileWorkspacePresenter(file, workspace, _rowService, _fileService, _project).Initialize();

        return new TabPage(file.FileName) { Tag = file, Controls = { workspace } };
    }
}
```

### 7.4 `OpenedFileWorkspace` presenter (grid + source/target textboxes)

```csharp
public class OpenedFileWorkspacePresenter
{
    private readonly OpenedFileData _file;
    private readonly OpenedFileWorkspace _view;
    private readonly IRowOperationsService _rowService;
    private readonly IFileWorkspaceService _fileService;
    private readonly Project _project;
    private BindingSource _bindingSource;

    public OpenedFileWorkspacePresenter(OpenedFileData file, OpenedFileWorkspace view,
        IRowOperationsService rowService, IFileWorkspaceService fileService, Project project)
    {
        _file = file; _view = view; _rowService = rowService; _fileService = fileService; _project = project;
    }

    public void Initialize()
    {
        _bindingSource = new BindingSource { DataSource = _file.Table };
        _view.ElementsDataGridView.DataSource = _bindingSource;

        _view.ElementsDataGridView.SelectionChanged += (s, e) => LoadSelectedRowIntoTextBoxes();
        _view.TargetRichTextBox.Leave += (s, e) => CommitTargetText();
        _view.ApproveButton.Click += (s, e) => ApproveSelectedRow();
    }

    private DataRow CurrentRow =>
        (_bindingSource.Current as DataRowView)?.Row;

    private void LoadSelectedRowIntoTextBoxes()
    {
        var row = CurrentRow;
        if (row == null) return;
        _view.SourceRichTextBox.Text = row["Source"]?.ToString();
        _view.TargetRichTextBox.Text = row["Target"]?.ToString();
    }

    private void CommitTargetText()
    {
        var row = CurrentRow;
        if (row == null) return;
        _rowService.SetTargetText(_file, row, _view.TargetRichTextBox.Text);

        if (_file.IsAllFilesAggregate)
            _fileService.CommitAllFilesChanges(_project);
    }

    private void ApproveSelectedRow()
    {
        var row = CurrentRow;
        if (row == null) return;
        _rowService.ApproveRow(_file, row);

        if (_file.IsAllFilesAggregate)
            _fileService.CommitAllFilesChanges(_project);
    }
}
```

Notice the controls (`DataGridView`, `RichTextBox`) never leave the `MyApp.UI` project, and the presenter is the **only** place that reads/writes them — the service layer stays control‑free.

### 7.5 The "Open" workflow

```csharp
public class OpenProjectPresenter
{
    private readonly IProjectService _projectService;
    private readonly ProjectsData _projectsData;

    public OpenProjectPresenter(IProjectService projectService, ProjectsData projectsData)
    {
        _projectService = projectService;
        _projectsData = projectsData;
    }

    public bool TryOpen(string path, out string error)
    {
        error = null;
        try
        {
            var project = _projectService.OpenProject(path);
            _projectsData.ProjectsList.Add(project);   // TabControlBinder creates the tab automatically
            _projectsData.SelectedProject = project;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
```

---

## 8. Composition root (`MyApp.App/Program.cs`)

```csharp
[STAThread]
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    IFileFormatResolver resolver = new FileFormatResolver(new IFileFormat[] { new CsvFileFormat(), new PoFileFormat() });
    IProjectService projectService = new ProjectService();
    IFileWorkspaceService fileService = new FileWorkspaceService(resolver);
    IRowOperationsService rowService = new RowOperationsService();

    var projectsData = new ProjectsData();
    var openPresenter = new OpenProjectPresenter(projectService, projectsData);

    Application.Run(new MainForm(projectsData, fileService, rowService, openPresenter));
}
```

No statics, no service locator — every dependency flows through constructors from a single place.

---

## 9. Summary of principles applied

1. **Separation of Concerns / layered architecture**
   `Models` (data) → `FileFormats` (I/O strategy) → `Services` (business rules) → `UI/Presenters` (glue) → `Views` (dumb controls).
2. **No more global/static state.** `THFilesList`, `THFileElementsDataGridView`, etc. are gone; every `Project` owns its own `OpenedFilesData`, and every open tab owns its own controls, created by `TabControlBinder`.
3. **Controls are inputs to the *UI* layer only**, never to business logic. Services/`RowOperations` operate on `OpenedFileData`/`DataRow`, so the same method works regardless of which project's grid triggered it.
4. **`[ALL]` is not a UI special case** — it's just another `OpenedFileData` produced by `FileWorkspaceService`, with row‑level provenance (`__SourceFile`, `__SourceRowIndex`) so edits can be written back deterministically.
5. **MVP-style Presenters** keep code‑behind of Forms/UserControls minimal — they only wire events to presenter calls.
6. **Interfaces everywhere in the Services layer** (`IProjectService`, `IFileWorkspaceService`, `IRowOperationsService`) make unit testing possible without WinForms.
7. **Custom `TabControlBinder<T>`** solves the real WinForms limitation (no native list binding for `TabPages`) instead of hand-writing ad-hoc sync code per screen.
8. **Composition root pattern** (`Program.cs`) is the single place that `new`s up concrete implementations — everything else depends on interfaces.

This structure scales cleanly to N projects open simultaneously, each with its own files/grid/textbox state, while keeping all "row logic" and file I/O fully decoupled from any specific `DataGridView`/`RichTextBox` instance.