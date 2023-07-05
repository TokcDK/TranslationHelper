using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class THfrmSearch : Form
    {
        readonly string _doubleSearchMarker = "|<OT>|";
        const int SEARCH_RESULTS_WINDOW_EXPANDED_HEIGHT = 589;
        const int SEARCH_RESULTS_WINDOW_NORMAL_HEIGHT = 368;

        readonly ListBox _filesList;
        readonly DataGridView _workFileDgv;
        readonly DataTableCollection _tables;
        readonly RichTextBox _translationTextBox;
        readonly INIFileMan.INIFile _config;
        readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
        readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;

        bool _isAnyRowFound;
        int _startRowSearchIndex;        //Индекс стартовой ячейки для поиска
        int _selectedTableIndex;
        int _selectedRowIndex;
        string _lastfoundvalue = string.Empty;
        string _lastfoundreplacedvalue = string.Empty;
        string[] _searchQueries;
        string[] _searchReplacers;
        const string _replaceToEqualMarker = "=";

        public enum SearchResult
        {
            Found,
            NotFound,
            Error
        }

        interface ISearchMethod
        {
            void Search();
        }

        interface ISearchComparer
        {
            bool IsMatch(string searchString, string searchPattern, bool isCaseInsensitive = false);
            void Replace(string searchString, string searchPattern, string replaceString, bool isCaseInsensitive = true);
        }

        class NormalComparer : ISearchComparer
        {
            public bool IsMatch(string searchString, string searchPattern, bool isCaseInsensitive = false)
            {
                return searchString.IndexOf(searchPattern, isCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) != -1;
            }

            public void Replace(string searchString, string searchPattern, string replaceString, bool isCaseInsensitive = false)
            {
                searchString.Replace(searchPattern, replaceString, isCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            }
        }

        class RegexComparer : ISearchComparer
        {
            public bool IsMatch(string searchString, string searchPattern, bool isCaseInsensitive = false)
            {
                return Regex.IsMatch(searchString, searchPattern, isCaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
            }

            public void Replace(string searchString, string searchPattern, string replaceString, bool isCaseInsensitive = false)
            {
                Regex.Replace(searchString, searchPattern, replaceString, isCaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
            }
        }

        public class FoundRowData
        {
            public FoundRowData(DataRow row)
            {
                //_row = row;
                Original = row.Field<string>(AppData.CurrentProject.OriginalColumnIndex);
                Translation = row.Field<string>(AppData.CurrentProject.TranslationColumnIndex);
                TableIndex = AppData.CurrentProject.FilesContent.Tables.IndexOf(row.Table);
                RowIndex = row.Table.Rows.IndexOf(row);
            }

            public string Original { get; }
            public string Translation { get; }

            [Browsable(false)]
            public int TableIndex { get; }
            [Browsable(false)]
            public int RowIndex { get; }
            //DataRow _row { get; set; }
        }

        internal THfrmSearch(ListBox filesList, DataGridView workFileDgv, RichTextBox translationTextBox)
        {
            InitializeComponent();
            //Main = MainForm;
            _filesList = filesList;
            _workFileDgv = workFileDgv;
            _tables = AppData.CurrentProject.FilesContent.Tables;
            _translationTextBox = translationTextBox;
            _config = AppData.Settings.THConfigINI;

            //translation
            this.THSearch1st.Text = T._("Find and Replace");
            //this.label3.Text = T._("Translation  Helper support:");
            this.SearchModeGroupBox.Text = T._("Search Mode");
            this.SearchModeNormalRadioButton.Text = T._("Normal");
            this.label5.Text = T._("Range");
            this.SearchRangeTableRadioButton.Text = T._("Table");
            this.SearchRangeAllRadioButton.Text = T._("Anywhere");
            this.label2.Text = T._("Method");
            this.SearchMethodOriginalToTranslationRadioButton.Text = T._("Find in Original and Paste to Translation");
            this.SearchMethodTranslationRadioButton.Text = T._("Find and Replace in Translation");
            this.THSearchMatchCaseCheckBox.Text = T._("Match Case");
            this.FindAllButton.Text = T._("Find All");
            this.SearchFormFindNextButton.Text = T._("Find Next");
            this.SearchFormReplaceButton.Text = T._("Replace");
            this.SearchFormReplaceAllButton.Text = T._("Replace All");
            this.THSearchFindWhatLabel.Text = T._("Find what:");
            this.label1.Text = T._("Replace with:");
            this.Text = T._("Find and Replace");
            this.Text = T._("Find and Replace");

            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                ExternalAdditions.NativeMethods.SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }

            GetSelectedText();
        }

        public void GetSelectedText()
        {
            //if (projectData != null)
            {
                if (AppData.Main.THFileElementsDataGridView.CurrentCell.IsInEditMode)
                {
                    //https://stackoverflow.com/questions/41380883/retrieve-partly-selected-text-when-the-cell-is-in-edit-mode-in-datagridview
                    if (AppData.Main.THFileElementsDataGridView.EditingControl is TextBox textBox)
                    {
                        SearchFormFindWhatTextBox.Text = textBox.SelectedText;
                    }
                }
                else if (AppData.Main.THSourceRichTextBox.SelectedText.Length > 0)
                {
                    SearchFormFindWhatTextBox.Text = AppData.Main.THSourceRichTextBox.SelectedText;
                }
                else if (AppData.Main.THTargetRichTextBox.SelectedText.Length > 0)
                {
                    SearchFormFindWhatTextBox.Text = AppData.Main.THTargetRichTextBox.SelectedText;
                }
            }
        }

        private void SearchModeNormalRadioButton_Click(object sender, EventArgs e)
        {
            SearchModeRegexRadioButton.Checked = false;
            SearchModeNormalRadioButton.Checked = true;
        }

        private void SearchModeRegexRadioButton_Click(object sender, EventArgs e)
        {
            SearchModeNormalRadioButton.Checked = false;
            SearchModeRegexRadioButton.Checked = true;
        }

        private void SearchMethodOriginalTranslationRadioButton_Click(object sender, EventArgs e)
        {
            SearchMethodOriginalToTranslationRadioButton.Checked = false;
            SearchMethodTranslationRadioButton.Checked = true;
        }

        private void SearchMethodTranslationRadioButton_Click(object sender, EventArgs e)
        {
            SearchMethodTranslationRadioButton.Checked = false;
            SearchMethodOriginalToTranslationRadioButton.Checked = true;
        }

        private void SearchRangeTableRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeSelectedRadioButton.Checked = false;
            SearchRangeVisibleRadioButton.Checked = false;
            SearchRangeTableRadioButton.Checked = true;
            SearchRangeAllRadioButton.Checked = false;
        }

        private void SearchRangeAllRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeSelectedRadioButton.Checked = false;
            SearchRangeVisibleRadioButton.Checked = false;
            SearchRangeTableRadioButton.Checked = false;
            SearchRangeAllRadioButton.Checked = true;
        }

        private int SearchColumnIndex => SearchMethodTranslationRadioButton.Checked ? _translationColumnIndex : _originalColumnIndex;

        private void LoadSearchQueriesReplacers()
        {
            try
            {
                _searchQueries = _config.GetSectionValues("Search Queries").ToArray();
                if (_searchQueries != null && _searchQueries.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref _searchQueries);
                    UnEscapeSearchValues(ref _searchQueries);
                    SearchFormFindWhatComboBox.Items.Clear();
                    SearchFormFindWhatComboBox.Items.AddRange(_searchQueries);
                }
                _searchReplacers = _config.GetSectionValues("Search Replacers").ToArray();
                if (_searchReplacers != null && _searchReplacers.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref _searchReplacers);
                    UnEscapeSearchValues(ref _searchReplacers);
                    SearchFormReplaceWithComboBox.Items.Clear();
                    SearchFormReplaceWithComboBox.Items.AddRange(_searchReplacers);
                }
            }
            catch
            {
            }
        }

        private static void UnEscapeSearchValues(ref string[] arr, bool unescape = true)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    if (unescape)
                    {
                        arr[i] = Regex.Unescape(arr[i]);
                    }
                    else
                    {
                        arr[i] = Regex.Escape(arr[i]);
                    }
                }
                catch (ArgumentException) { }
            }
        }

        private static void RemoveQuotesFromLoadedSearchValues(ref string[] searchQueriesReplacers)
        {
            for (int i = 0; i < searchQueriesReplacers.Length; i++)
            {
                if (searchQueriesReplacers[i].StartsWith("\"") && searchQueriesReplacers[i].EndsWith("\""))
                {
                    searchQueriesReplacers[i] = searchQueriesReplacers[i].Remove(searchQueriesReplacers[i].Length - 1, 1).Remove(0, 1);
                }
            }
        }

        private static void AddQuotesToWritingSearchValues(ref string[] searchQueriesReplacers)
        {
            for (int i = 0; i < searchQueriesReplacers.Length; i++)
            {
                searchQueriesReplacers[i] = "\"" + searchQueriesReplacers[i] + "\"";
            }
        }

        private void WriteSearchQueriesReplacers()
        {
            try
            {
                if (SearchFormFindWhatComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(_searchQueries, SearchFormFindWhatComboBox.Items))
                {
                    _searchQueries = new string[SearchFormFindWhatComboBox.Items.Count];
                    SearchFormFindWhatComboBox.Items.CopyTo(_searchQueries, 0);
                    AddQuotesToWritingSearchValues(ref _searchQueries);
                    UnEscapeSearchValues(ref _searchQueries, false);
                    _config.SetArrayToSectionValues("Search Queries", _searchQueries);
                }
                if (SearchFormReplaceWithComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(_searchReplacers, SearchFormReplaceWithComboBox.Items))
                {
                    _searchReplacers = new string[SearchFormReplaceWithComboBox.Items.Count];
                    SearchFormReplaceWithComboBox.Items.CopyTo(_searchReplacers, 0);
                    AddQuotesToWritingSearchValues(ref _searchReplacers);
                    UnEscapeSearchValues(ref _searchReplacers, false);
                    _config.SetArrayToSectionValues("Search Replacers", _searchReplacers);
                }
            }
            catch
            {
            }
        }

        private static bool IsSearchQueriesReplacersListChanged(string[] OldList, ComboBox.ObjectCollection items)
        {
            if (OldList.Length != items.Count)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < OldList.Length; i++)
                {
                    if (OldList[i] != items[i].ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void StoryFoundValueToComboBox(string foundvalue, string replaceWithValue = "")
        {
            //store found value
            _lastfoundvalue = foundvalue;
            StoreFoundReplaceValues(foundvalue, SearchFormFindWhatComboBox);

            //store replace value
            _lastfoundreplacedvalue = replaceWithValue;
            StoreFoundReplaceValues(replaceWithValue, SearchFormReplaceWithComboBox);

            //write found values
            WriteSearchQueriesReplacers();
        }

        private static void StoreFoundReplaceValues(string value, ComboBox ComboBox)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            //lastvalue = value;
            int ItemsCount = ComboBox.Items.Count;
            if (ItemsCount > 0)
            {
                if (!ComboBox.Items.Contains(value))
                {
                    if (ItemsCount == AppSettings.THSavedSearchQueriesReplacersCount)
                    {
                        ComboBox.Items.RemoveAt(AppSettings.THSavedSearchQueriesReplacersCount - 1);
                    }

                    AddRestComboBoxValuesWithNew(ComboBox.Items, value);
                }
                else
                {
                    if (ComboBox.Items.IndexOf(value) != 0)
                    {
                        ComboBox.Items.Remove(value);
                        AddRestComboBoxValuesWithNew(ComboBox.Items, value);
                    }
                }
            }
            else
            {
                ComboBox.Items.Add(value);
            }
        }

        /// <summary>
        /// set old values to item array, clear combobox, adda new and after add old values
        /// </summary>
        /// <param name="items"></param>
        /// <param name="tempArray"></param>
        /// <param name="value"></param>
        private static void AddRestComboBoxValuesWithNew(ComboBox.ObjectCollection items, string value)
        {
            object[] oldItems = new object[items.Count];
            items.CopyTo(oldItems, 0);
            items.Clear();
            items.Add(value);
            foreach (var oldvalue in oldItems)
            {
                items.Add(oldvalue);
            }
        }

        private void PopulateGrid(List<FoundRowData> foundRowsList)
        {
            if (foundRowsList == null)
            {
                return;
            }
            SearchResultsDatagridview.DataSource = foundRowsList;
            int originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
            int translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
            for (int columnIndex = 0; columnIndex < SearchResultsDatagridview.ColumnCount; columnIndex++)
            {
                if (columnIndex == originalColumnIndex || columnIndex == translationColumnIndex)
                {
                    continue;
                }
                SearchResultsDatagridview.Columns[columnIndex].Visible = false;
            }
            SearchResultsDatagridview.Columns[originalColumnIndex].ReadOnly = true;

            SearchResultsDatagridview.Visible = true;
            SearchResultsPanel.Visible = true;
        }

        //http://mrbool.com/dataset-advance-operations-search-sort-filter-net/24769
        //https://stackoverflow.com/questions/3608388/c-sharp-access-dataset-data-from-another-class
        //http://qaru.site/questions/236566/how-to-know-the-row-index-from-datatable-object
        private List<FoundRowData> _foundRowsList;
        private void SearchAllButton_Click(object sender, EventArgs e)
        {
            if (AppData.CurrentProject.FilesContent == null
                || !SearchFindLinesWithPossibleIssuesCheckBox.Checked && SearchFormFindWhatTextBox.Text.Length == 0)
            {
                return;
            }

            lblSearchMsg.Visible = false;
            GetSearchResults();

            if (_foundRowsList == null) return;

            if (_foundRowsList.Count > 0)
            {
                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                PopulateGrid(_foundRowsList);

                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Found ") + _foundRowsList.Count + T._(" records");
                this.Height = SEARCH_RESULTS_WINDOW_EXPANDED_HEIGHT;
            }
            else
            {
                //PopulateGrid(null);
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Nothing Found");
                SearchResultsDatagridview.DataSource = null;
                SearchResultsDatagridview.Refresh();
                this.Height = SEARCH_RESULTS_WINDOW_NORMAL_HEIGHT;
            }
        }

        bool _isDoubleSearch = false;
        private void GetSearchResults()
        {
            lblSearchMsg.Visible = false;
            if (_tables.Count == 0) return;

            _isAnyRowFound = false;
            _foundRowsList = EnumerateFoundRows().ToList();
        }
        private IEnumerable<FoundRowData> EnumerateAndFillSearchResults()
        {
            lblSearchMsg.Visible = false;
            if (_tables.Count == 0) yield break;

            _isAnyRowFound = false;

            if (_foundRowsList == null) _foundRowsList = new List<FoundRowData>();
            foreach (var foundRowData in EnumerateFoundRows())
            {
                _foundRowsList.Add(foundRowData);
                yield return foundRowData;
            }
        }

        private IEnumerable<FoundRowData> EnumerateFoundRows(bool isSearchReplaceMode = false)
        {
            int searchColumnIndex = SearchColumnIndex;
            bool isSearchInInfo = SearchInInfoCheckBox.Checked;
            bool isIssuesSearch = SearchFindLinesWithPossibleIssuesCheckBox.Checked;

            var searchQueryText = SearchFormFindWhatTextBox.Text.Split(new[] { _doubleSearchMarker }, StringSplitOptions.None);
            if (searchQueryText.Length == 2 && string.IsNullOrEmpty(searchQueryText[1]))
            {
                // when second array element is empty reset array to 1st element
                searchQueryText = new string[1] { searchQueryText[0] };
            }
            if (!SearchFormFindWhatTextBox.Text.Contains(_doubleSearchMarker))
            {
                // uncheck double search checkbox if marker is missing in search query
                DoubleSearchOptionCheckBox.Checked = false;
            }
            if (string.IsNullOrEmpty(searchQueryText[0])) yield break; // return if 1st query is empty
            _isDoubleSearch = !isSearchInInfo && !isIssuesSearch && searchQueryText.Length == 2;

            // check if regex pattern is valid
            if (SearchModeRegexRadioButton.Checked && searchQueryText.Any(v => !v.IsValidRegexPattern()))
            {
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Invalid regex!");

                yield break;
            }

            var searchInSelected = SearchRangeSelectedRadioButton.Checked || SearchRangeVisibleRadioButton.Checked;
            int tableIndexMax = SearchRangeTableRadioButton.Checked || searchInSelected ? _filesList.SelectedIndex + 1 : _tables.Count;
            int initTableIndex = SearchRangeTableRadioButton.Checked || searchInSelected ? _filesList.SelectedIndex : 0;

            for (int tableIndex = initTableIndex; tableIndex < tableIndexMax; tableIndex++)
            {
                var tableWhereSearching = _tables[tableIndex];

                HashSet<int> selectedrowsHashes = null;
                if (searchInSelected)
                {
                    selectedrowsHashes = FunctionsTable.GetDGVRowsIndexesHashesInDT(tableIndex, SearchRangeVisibleRadioButton.Checked);
                }

                var rowsCount = tableWhereSearching.Rows.Count;
                for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    if (searchInSelected && !selectedrowsHashes.Contains(rowIndex)) continue;

                    var row2check = _tables[tableIndex].Rows[rowIndex];

                    if (IsValid2Search(row2check, searchColumnIndex))
                    {
                        continue;
                    }

                    if (isSearchInInfo) //search in info box
                    {
                        // in info always one query
                        var infoValue = (AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows[rowIndex].Field<string>(0));

                        if (GetCheckResult(new string[1] { infoValue }, searchQueryText))
                        {
                            yield return GetFoundRowData(row2check);
                        }
                    }
                    else if (isIssuesSearch && IsTheRowHasPossibleIssues(row2check)) //search rows with possible issues
                    {
                        yield return GetFoundRowData(row2check);
                    }
                    else
                    {
                        var row = _tables[tableIndex].Rows[rowIndex];

                        string[] text2Search;
                        if (_isDoubleSearch)
                        {
                            text2Search = new string[2] { row.Field<string>(_originalColumnIndex), row.Field<string>(_translationColumnIndex) };
                        }
                        else
                        {
                            text2Search = new string[1] { row.Field<string>(searchColumnIndex) };
                        }

                        // general search
                        if (GetCheckResult(text2Search, searchQueryText))
                        {
                            yield return GetFoundRowData(row2check);
                        }
                    }
                }
            }
        }

        private bool IsValid2Search(DataRow row2check, int searchcolumn)
        {
            //skip equal lines if need, skip empty search cells && not skip when row issue search
            return (chkbxDoNotTouchEqualOT.Checked && row2check[_originalColumnIndex].Equals(row2check[_translationColumnIndex]))
                        || (!chkbxDoNotTouchEqualOT.Checked
                        && !SearchFindLinesWithPossibleIssuesCheckBox.Checked
                        && (row2check[searchcolumn] + string.Empty).Length == 0);
        }

        private bool GetCheckResult(string[] textWhereToSearch, string[] strQuery)
        {
            if (string.IsNullOrEmpty(textWhereToSearch[_isDoubleSearch ? 1 : 0]))
                return false; // skip when value to search is empty, check translation in case of double search

            return SearchModeRegexRadioButton.Checked
                ? CheckTextByRegex(textWhereToSearch, strQuery) // regex check
                : CheckText(textWhereToSearch, strQuery); // common check
        }

        private bool CheckText(string[] textWhereToSearch, string[] strQuery)
        {
            return IsMatchedByContains(textWhereToSearch, strQuery);
        }

        private bool IsMatchedByContains(string[] textWhereToSearch, string[] strQuery)
        {
            if (THSearchMatchCaseCheckBox.Checked)
            {
                return textWhereToSearch[0].Contains(strQuery[0])
                    && (!_isDoubleSearch || textWhereToSearch[1].Contains(strQuery[1]));
            }
            else
            {
                return textWhereToSearch[0].IndexOf(strQuery[0], StringComparison.CurrentCultureIgnoreCase) != -1
                    && (!_isDoubleSearch || textWhereToSearch[1].IndexOf(strQuery[1], StringComparison.CurrentCultureIgnoreCase) != -1);
            }
        }

        private bool CheckTextByRegex(string[] textWhereSearch, string[] searchPattern)
        {
            return IsMatchedByRegex(textWhereSearch, searchPattern);
        }

        private bool IsMatchedByRegex(string[] textWhereSearch, string[] searchPattern)
        {
            if (THSearchMatchCaseCheckBox.Checked)
            {
                return Regex.IsMatch(textWhereSearch[0], searchPattern[0])
                    && (!_isDoubleSearch || Regex.IsMatch(textWhereSearch[1], searchPattern[1]));
            }
            else
            {
                return Regex.IsMatch(textWhereSearch[0], searchPattern[0], RegexOptions.IgnoreCase)
                    && (!_isDoubleSearch || Regex.IsMatch(textWhereSearch[1], searchPattern[1], RegexOptions.IgnoreCase));
            }
        }

        /// <summary>
        /// Add found row to results
        /// </summary>
        /// <param name="isFound"></param>
        /// <param name="ds"></param>
        /// <param name="row"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        private FoundRowData GetFoundRowData(DataRow row)
        {
            if (!_isAnyRowFound)
            {
                _isAnyRowFound = true;
                this.Height = SEARCH_RESULTS_WINDOW_NORMAL_HEIGHT;
            }
            return new FoundRowData(row);
        }

        readonly List<ISearchIssueChecker> _issueChecers = new List<ISearchIssueChecker>(4)
        {
            new ContainsNonRomaji(),
            //new CheckAnyLineLngerOfMaxLength(),
            new CheckActorsNameTranslationConsistent(),
            new CheckAnyLineTranslatable(),
            new ProjectSpecificIssues(),
            new CheckInternalQuoteUnescaped(),
        };
        private bool IsTheRowHasPossibleIssues(DataRow row)
        {
            var o = row.Field<string>(_originalColumnIndex);
            var t = row.Field<string>(_translationColumnIndex);
            if (string.IsNullOrEmpty(t) || (AppSettings.IgnoreOrigEqualTransLines && o.Equals(t)))
            {
                return false;
            }

            var data = new SearchIssueCheckerData(row, o, t);
            return _issueChecers.Any(p => p.IsHaveTheIssue(data));
        }

        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = SEARCH_RESULTS_WINDOW_NORMAL_HEIGHT;
            _selectedTableIndex = _filesList.SelectedIndex;

            LoadSearchQueriesReplacers();

            //set default values for search settings
            chkbxDoNotTouchEqualOT.Checked = AppSettings.IgnoreOrigEqualTransLines;
        }

        private void SelectTextInTextBox(string input)
        {
            Invoke((Action)(() =>
            {
                if (string.IsNullOrEmpty(_translationTextBox.Text)) return;

                var searchWord = Regex.Escape(SearchFormFindWhatTextBox.Text);
                var matchCase = THSearchMatchCaseCheckBox.Checked;
                var searchOptions = matchCase ? RichTextBoxFinds.MatchCase : RichTextBoxFinds.None;

                if (SearchModeRegexRadioButton.Checked)
                {
                    SelectTextInTextBoxRegex(searchWord, matchCase, searchOptions);
                }
                else
                {
                    SelectTextInTextBoxNormal(searchWord, searchOptions);
                }
            }));
        }

        private void SelectTextInTextBoxNormal(string searchWord, RichTextBoxFinds searchOptions)
        {
            var start = 0;
            var length = SearchFormFindWhatTextBox.TextLength;

            while (start < _translationTextBox.TextLength)
            {
                var startIndex = _translationTextBox.Find(searchWord, start, searchOptions);

                if (startIndex == -1) break;

                _translationTextBox.SelectionStart = startIndex;
                _translationTextBox.SelectionLength = length;
                _translationTextBox.SelectionBackColor = Color.Yellow;

                start = startIndex + length;
            }
        }

        private void SelectTextInTextBoxRegex(string searchWord, bool matchCase, RichTextBoxFinds findOptions)
        {
            var matches = Regex.Matches(_translationTextBox.Text, searchWord, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {

