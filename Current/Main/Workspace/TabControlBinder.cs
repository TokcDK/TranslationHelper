using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// Keeps a <see cref="TabControl"/> in step with a <see cref="BindingList{T}"/>: one tab per item,
    /// in list order.
    /// <para>
    /// It exists because <see cref="TabControl"/> has no data source of its own. Without it every
    /// screen that shows a list of tabs has to add, remove and select tabs by hand, and the two
    /// directions — "the list changed, so change the tabs" and "the user changed the tab, so change the
    /// selection" — have to be written twice and kept from recursing into each other.
    /// </para>
    /// <para>
    /// The tab a page belongs to is found by its index, so the item at index <c>i</c> of the list is
    /// always the item of <c>TabPages[i]</c>. That holds because this class is the only thing that
    /// adds, removes or reorders pages.
    /// </para>
    /// </summary>
    /// <typeparam name="T">What a tab presents.</typeparam>
    internal sealed class TabControlBinder<T> : IDisposable where T : class
    {
        private readonly TabControl _tabs;
        private readonly BindingList<T> _source;
        private readonly Func<T, TabPage> _createTabPage;

        /// <summary>
        /// True while this class is the one changing the selected tab. Without it, selecting a tab
        /// would raise the event that selects it again.
        /// </summary>
        private bool _syncingSelection;

        private bool _disposed;

        internal TabControlBinder(TabControl tabs, BindingList<T> source, Func<T, TabPage> createTabPage)
        {
            _tabs = tabs ?? throw new ArgumentNullException(nameof(tabs));
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _createTabPage = createTabPage ?? throw new ArgumentNullException(nameof(createTabPage));

            _source.ListChanged += OnListChanged;
            _tabs.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        /// <summary>
        /// Raised when the user selects a tab, with the item that tab presents. Not raised when the
        /// selection is changed through <see cref="SelectItem"/>, which is what keeps a caller from
        /// being told about the change it just made.
        /// </summary>
        internal event Action<T> SelectionChanged;

        /// <summary>
        /// The item the selected tab presents, or null when no tab is selected.
        /// </summary>
        internal T SelectedItem
        {
            get
            {
                int index = _tabs.SelectedIndex;
                return index >= 0 && index < _source.Count ? _source[index] : null;
            }
        }

        /// <summary>
        /// Build the tabs from the list as it is now. Called once after the items that already exist
        /// have been added, and again whenever the list is reset wholesale.
        /// </summary>
        internal void Rebuild()
        {
            _syncingSelection = true;
            try
            {
                _tabs.TabPages.Clear();

                int count = _source.Count;
                for (int i = 0; i < count; i++)
                {
                    _tabs.TabPages.Add(_createTabPage(_source[i]));
                }
            }
            finally
            {
                _syncingSelection = false;
            }

            SelectItem(SelectedItem ?? (Count > 0 ? _source[0] : null));
        }

        /// <summary>
        /// Select the tab that presents <paramref name="item"/>. Does nothing when the item is not in
        /// the list or is already the selected one.
        /// </summary>
        internal void SelectItem(T item)
        {
            int index = item == null ? -1 : _source.IndexOf(item);
            if (index < 0 || index == _tabs.SelectedIndex) return;

            _syncingSelection = true;
            try
            {
                _tabs.SelectedIndex = index;
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        private int Count => _source.Count;

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (e.NewIndex < 0 || e.NewIndex > _tabs.TabPages.Count) return;

                    _syncingSelection = true;
                    try
                    {
                        _tabs.TabPages.Insert(e.NewIndex, _createTabPage(_source[e.NewIndex]));
                    }
                    finally
                    {
                        _syncingSelection = false;
                    }

                    // A project that has just been opened is the one the user wants to see, which is what
                    // selecting it here means. A caller that wants another tab can select it after.
                    SelectItem(_source[e.NewIndex]);
                    break;

                case ListChangedType.ItemDeleted:
                    if (e.NewIndex < 0 || e.NewIndex >= _tabs.TabPages.Count) return;

                    _syncingSelection = true;
                    try
                    {
                        var page = _tabs.TabPages[e.NewIndex];
                        _tabs.TabPages.Remove(page);
                        page.Dispose();
                    }
                    finally
                    {
                        _syncingSelection = false;
                    }
                    break;

                case ListChangedType.Reset:
                    Rebuild();
                    break;
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingSelection) return;

            SelectionChanged?.Invoke(SelectedItem);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _source.ListChanged -= OnListChanged;
            _tabs.SelectedIndexChanged -= OnSelectedIndexChanged;
        }
    }
}
