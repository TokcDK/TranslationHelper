using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace TranslationHelper.Workspace
{
    /// <summary>
    /// Shows one page per item of a <see cref="BindingList{T}"/> in a <see cref="TabControl"/>, and
    /// builds a page only for the item that is being shown.
    /// <para>
    /// It exists because <see cref="TabControl"/> has no data source of its own. Without it every
    /// screen that shows a list of tabs has to add, remove and select tabs by hand, and the two
    /// directions — "the list changed, so change the tabs" and "the user changed the tab, so change the
    /// selection" — have to be written twice and kept from recursing into each other.
    /// </para>
    /// <para>
    /// Nothing is shown until something is selected, and a page is built at that moment rather than
    /// when its item enters the list. A page is not free — the page of a file holds a grid bound to the
    /// file's whole table and two text boxes — and a project can hold thousands of files, so building
    /// one page per entry while the list is filled is work nobody asked for. It is also what keeps
    /// opening a project cheap: the files list is filled, and the tab control stays empty until the
    /// user picks an entry from it.
    /// </para>
    /// <para>
    /// Because of that, a page's position is not its item's position in the list: the pages are the
    /// items that have been shown, in the order they were shown. What a page presents is carried by its
    /// <see cref="Control.Tag"/>, which this class sets, so an item is always reachable from its page
    /// whatever the pages' order.
    /// </para>
    /// </summary>
    /// <typeparam name="T">What a page presents.</typeparam>
    internal sealed class TabControlBinder<T> : IDisposable where T : class
    {
        private readonly TabControl _tabs;
        private readonly BindingList<T> _source;
        private readonly Func<T, TabPage> _createTabPage;

        /// <summary>
        /// The page built for each item that has been shown, so showing an item a second time returns
        /// the page it already has instead of building another one.
        /// </summary>
        private readonly Dictionary<T, TabPage> _pages = new Dictionary<T, TabPage>();

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
        /// Drop every page, so nothing is shown. Used when the list is reset wholesale.
        /// <para>
        /// The item that was being shown is shown again when it is still in the list, so a reset that
        /// keeps it does not move the user away from it. Nothing is shown when there was nothing to
        /// show, which is the state a project starts in.
        /// </para>
        /// </summary>
        internal void Rebuild()
        {
            var shown = SelectedItem;

            _syncingSelection = true;
            try
            {
                _tabs.TabPages.Clear();
                _pages.Clear();

                // Clearing the pages is not enough to be sure of the state: a page added to an empty
                // control is made visible by the control itself, so the selection is settled here.
                _tabs.SelectedIndex = -1;
            }
            finally
            {
                _syncingSelection = false;
            }

            if (shown != null && _source.Contains(shown))
            {
                SelectItem(shown);
            }
        }

        /// <summary>
        /// Show the page of <paramref name="item"/>, building it if this is the first time. Shows
        /// nothing when <paramref name="item"/> is null or is not in the list.
        /// </summary>
        internal void SelectItem(T item)
        {
            var page = PageOf(item);
            if (ReferenceEquals(_tabs.SelectedTab, page)) return;

            _syncingSelection = true;
            try
            {
                // A page that does not exist is the same to this control as one that is not shown:
                // assigning null selects no tab, which is how "nothing is shown" is expressed.
                _tabs.SelectedTab = page;
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        /// <summary>
        /// The page of <paramref name="item"/>, built and added to the control if it has none yet, or
        /// null when the item is null or is not in the list.
        /// </summary>
        private TabPage PageOf(T item)
        {
            if (item == null || !_source.Contains(item)) return null;

            if (_pages.TryGetValue(item, out var page)) return page;

            page = _createTabPage(item);

            // The page carries what it presents, so the item is found from the tab the user selected
            // without this class having to keep the pages in the list's order.
            page.Tag = item;

            _pages[item] = page;
            _tabs.TabPages.Add(page);

            return page;
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    // A new item is not shown: showing one is the caller's decision, made by selecting
                    // it. A list is filled entry by entry, and building a page for each entry on the
                    // way in is exactly the work this class exists to avoid.
                    break;

                case ListChangedType.ItemDeleted:
                    DropPagesNotInList();
                    break;

                case ListChangedType.Reset:
                    Rebuild();
                    break;
            }
        }

        /// <summary>
        /// Drop the pages of the items the list no longer holds, so a tab does not outlive what it
        /// presents. The page that was shown leaves nothing shown, because what is shown instead is the
        /// decision of whoever removed the item.
        /// </summary>
        private void DropPagesNotInList()
        {
            var shown = SelectedItem;

            List<T> gone = null;
            foreach (var item in _pages.Keys)
            {
                if (_source.Contains(item)) continue;

                if (gone == null) gone = new List<T>();
                gone.Add(item);
            }

            if (gone == null) return;

            _syncingSelection = true;
            try
            {
                foreach (var item in gone)
                {
                    var page = _pages[item];
                    _pages.Remove(item);

                    _tabs.TabPages.Remove(page);
                    page.Dispose();
                }

                if (gone.Contains(shown))
                {
                    _tabs.SelectedTab = null;
                }
            }
            finally
            {
                _syncingSelection = false;
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
