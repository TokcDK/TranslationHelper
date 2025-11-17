using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Forms.Search;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers;
using TranslationHelper.Main.Functions;
using Zuby.ADGV;

namespace TranslationHelper
{

    public partial class THfrmSearch : Form
    {
        private const string DoubleSearchMarker = "|<OT>|";
        private const int SearchResultsWindowExpandedHeight = 589;
        private const int SearchResultsWindowNormalHeight = 368;
        private const string ReplaceToEqualMarker = "=";
        private const string NothingFoundMessage = "Nothing Found.";

        private readonly ListBox _filesList;
        private readonly AdvancedDataGridView _workFileDgv;
        private readonly DataTableCollection _tables;
        private readonly RichTextBox _translationTextBox;
        private readonly INIFileMan.INIFile _config;

        private bool _isAnyRowFound;
        private int _startRowSearchIndex; // Tracks the current position in search results
        private int _selectedTableIndex;
        private int _selectedRowIndex;
        private string _lastFoundValue = string.Empty;
        private string _lastFoundReplacedValue = string.Empty;
        private readonly List<string> _searchQueries = new List<string>();
        private readonly List<string> _searchReplacers = new List<string>();
        private List<FoundRowData> _foundRowsList;
        private IEnumerator<FoundRowData> _foundRowsEnum;
        private string _lastSearchString;

        private static readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
        private static readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
        private int SearchColumnIndex { get => SearchMethodTranslationRadioButton.Checked ? _translationColumnIndex : _originalColumnIndex; }
        public int MaxSavedQueries { get; private set; } = 20;

        #region Nested Classes
        internal interface ISearchComparer
        {
            bool IsMatch(string searchString, string searchPattern, bool isCaseInsensitive = false);
            void Replace(string searchString, string searchPattern, string replaceString, bool isCaseInsensitive = true);
        }

        internal class NormalComparer : ISearchComparer
        {
            public bool IsMatch(string searchString, string searchPattern, bool isCaseInsensitive = false)
                => searchString.IndexOf(searchPattern, isCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) != -1;

            public void Replace(string searchString, string searchPattern, string replaceString, bool isCaseInsensitive = false)
                => searchString.Replace(searchPattern, replaceString, isCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        internal class RegexComparer : ISearchComparer
        {
            public bool IsMatch(string searchString, string searchPattern, bool isCaseInsensitive = false)
                => Regex.IsMatch(searchString, searchPattern, isCaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);

            public void Replace(string searchString, string searchPattern, string replaceString, bool isCaseInsensitive = false)
                => Regex.Replace(searchString, searchPattern, replaceString, isCaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public class FoundRowData
        {
            public FoundRowData(DataRow row)
            {
                Row = row;
                TableIndex = AppData.CurrentProject.FilesContent.Tables.IndexOf(row.Table);
                RowIndex = row.Table.Rows.IndexOf(row);
            }

            [Browsable(false)]
            public DataRow Row { get; }
            public string Original => Row.Field<string>(_originalColumnIndex);
            public string Translation
            {
                get => Row.Field<string>(_translationColumnIndex);
                set => Row.SetField(_translationColumnIndex, value);
            }

            [Browsable(false)]
            public int TableIndex { get; }
            [Browsable(false)]
            public int RowIndex { get; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the search form.
        /// </summary>
        internal THfrmSearch(object[] args)
        {
            InitializeComponent();
            _filesList = args[0] as ListBox;
            _workFileDgv = args[1] as AdvancedDataGridView;
            _tables = AppData.CurrentProject.FilesContent.Tables;
            _translationTextBox = args[2] as RichTextBox;
            _config = AppData.Settings.THConfigINI;

            InitializeTranslations();
            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                ExternalAdditions.NativeMethods.SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }

            GetSelectedText();
        }

        private void InitializeTranslations()
        {
            THSearch1st.Text = T._("Find and Replace");
            SearchModeGroupBox.Text = T._("Search Mode");
            SearchModeNormalRadioButton.Text = T._("Normal");
            label5.Text = T._("Range");
            SearchRangeTableRadioButton.Text = T._("Table");
            SearchRangeAllRadioButton.Text = T._("Anywhere");
            label2.Text = T._("Method");
            SearchMethodOriginalToTranslationRadioButton.Text = T._("Find in Original and Paste to Translation");
            SearchMethodTranslationRadioButton.Text = T._("Find and Replace in Translation");
            THSearchMatchCaseCheckBox.Text = T._("Match Case");
            FindAllButton.Text = T._("Find All");
            SearchFormFindNextButton.Text = T._("Find Next");
            SearchFormReplaceButton.Text = T._("Replace");
            SearchFormReplaceAllButton.Text = T._("Replace All");
            THSearchFindWhatLabel.Text = T._("Find what:");
            label1.Text = T._("Replace with:");
            Text = T._("Find and Replace");
        }

        /// <summary>
        /// Retrieves the selected text from the current control and sets it to the find textbox.
        /// </summary>
        internal void GetSelectedText()
        {
            if (AppData.Main.THFileElementsDataGridView.CurrentCell?.IsInEditMode == true &&
                AppData.Main.THFileElementsDataGridView.EditingControl is TextBox textBox &&
                !string.IsNullOrEmpty(textBox.SelectedText))
            {
                SearchFormFindWhatTextBox.Text = textBox.SelectedText;
            }
            else if (!string.IsNullOrEmpty(AppData.Main.THSourceRichTextBox.SelectedText))
            {
                SearchFormFindWhatTextBox.Text = AppData.Main.THSourceRichTextBox.SelectedText;
            }
            else if (!string.IsNullOrEmpty(AppData.Main.THTargetRichTextBox.SelectedText))
            {
                SearchFormFindWhatTextBox.Text = AppData.Main.THTargetRichTextBox.SelectedText;
            }
        }

        #region Search Query Management
        private void WriteSearchQueriesReplacers() => SaveLoadSearchQueryData(false);

        private void LoadSearchQueriesReplacers() => SaveLoadSearchQueryData();

        private void SaveLoadSearchQueryData(bool load = true)
        {
            var items = new[]
            {
                (ComboBox: SearchFormFindWhatComboBox, List: _searchQueries, IniName: THSettings.SearchQueriesSectionName),
                (ComboBox: SearchFormReplaceWithComboBox, List: _searchReplacers, IniName: THSettings.SearchReplacersSectionName)
            };

            foreach (var (comboBox, list, iniName) in items)
            {
                if (load)
                    LoadSearchQueryData(comboBox, list, iniName);
                else
                    SaveSearchQueryData(comboBox, list, iniName);
            }
        }

        private void SaveSearchQueryData(ComboBox comboBox, List<string> list, string iniName)
        {
            try
            {
                if (comboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(list, comboBox.Items))
                {
                    list.Clear();
                    list.AddRange(comboBox.Items.Cast<string>());
                    SearchSharedHelpers.AddQuotesToWritingSearchValues(list);
                    SearchSharedHelpers.UnEscapeSearchValues(list, false);
                    _config.SetArrayToSectionValues(iniName, list.ToArray());
                }
            }
            catch (IOException ex)
            {
                // Log the error instead of silently ignoring it
                //AppData.LogError("Failed to save search queries", ex);
            }
        }

        private void LoadSearchQueryData(ComboBox comboBox, List<string> list, string iniName)
        {
            try
            {
                var savedQueries = _config.GetSectionValues(iniName)?.ToArray();
                if (savedQueries?.Length > 0)
                {
                    list.Clear();
                    list.AddRange(savedQueries.Take(MaxSavedQueries));
                    SearchSharedHelpers.RemoveQuotesFromLoadedSearchValues(list);
                    SearchSharedHelpers.UnEscapeSearchValues(list);
                    comboBox.Items.Clear();
                    comboBox.Items.AddRange(list.ToArray());
                }
            }
            catch (IOException ex)
            {
                //AppData.LogError("Failed to load search queries", ex);
            }
        }        

        private static bool IsSearchQueriesReplacersListChanged(List<string> oldList, ComboBox.ObjectCollection items)
            => oldList.Count != items.Count || !oldList.SequenceEqual(items.Cast<string>());
        #endregion

        #region ComboBox Management
        private void StoreFoundValueToComboBox(string foundValue, string replaceWithValue = "")
        {
            _lastFoundValue = foundValue;
            StoreFoundReplaceValues(foundValue, SearchFormFindWhatComboBox);
            _lastFoundReplacedValue = replaceWithValue;
            StoreFoundReplaceValues(replaceWithValue, SearchFormReplaceWithComboBox);
            WriteSearchQueriesReplacers();
        }

        private static void StoreFoundReplaceValues(string value, ComboBox comboBox)
        {
            if (string.IsNullOrEmpty(value)) return;

            int index = comboBox.Items.IndexOf(value);
            if (index == 0) return;

            if (index > 0) comboBox.Items.RemoveAt(index);
            else if (comboBox.Items.Count >= AppSettings.THSavedSearchQueriesReplacersCount)
                comboBox.Items.RemoveAt(comboBox.Items.Count - 1);

            comboBox.Items.Insert(0, value);
        }
        #endregion

        #region Grid Population
        private void PopulateGrid(List<FoundRowData> foundRowsList)
        {
            if (foundRowsList == null) return;

            SearchResultsDatagridview.DataSource = foundRowsList;
            for (int i = 0; i < SearchResultsDatagridview.ColumnCount; i++)
                SearchResultsDatagridview.Columns[i].Visible = i == _originalColumnIndex || i == _translationColumnIndex;

            SearchResultsDatagridview.Columns[_originalColumnIndex].ReadOnly = true;
            SearchResultsDatagridview.Visible = true;
            SearchResultsPanel.Visible = true;
        }
        #endregion

        #region Search Logic
        private void SearchAllButton_Click(object sender, EventArgs e)
        {
            if (AppData.CurrentProject.FilesContent == null ||
                (!SearchFindLinesWithPossibleIssuesCheckBox.Checked && !SearchEmptyCheckBox.Checked && string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text)))
                return;

            lblSearchMsg.Visible = false;
            GetSearchResults();

            if (_foundRowsList?.Count > 0)
            {
                StoreFoundValueToComboBox(SearchFormFindWhatTextBox.Text);
                PopulateGrid(_foundRowsList);
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Found ") + _foundRowsList.Count + T._(" records");
                Height = SearchResultsWindowExpandedHeight;
            }
            else
            {
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = NothingFoundMessage;
                SearchResultsDatagridview.DataSource = null;
                SearchResultsDatagridview.Refresh();
                Height = SearchResultsWindowNormalHeight;
            }
        }

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
            if(_foundRowsList == null) 
                _foundRowsList = new List<FoundRowData>();

            foreach (var foundRowData in EnumerateFoundRows())
            {
                _foundRowsList.Add(foundRowData);
                yield return foundRowData;
            }
        }

        /// <summary>
        /// Enumerates rows that match the current search criteria.
        /// </summary>
        private IEnumerable<FoundRowData> EnumerateFoundRows()
        {
            if (_tables.Count == 0) yield break;

            string[] searchQueries = SearchFormFindWhatTextBox.Text.Split(new[] { DoubleSearchMarker }, StringSplitOptions.None);
            if (searchQueries.Length == 2 && string.IsNullOrEmpty(searchQueries[1]))
                searchQueries = new[] { searchQueries[0] };

            bool isRegex = SearchModeRegexRadioButton.Checked;
            bool isCaseSensitive = THSearchMatchCaseCheckBox.Checked;
            bool isSearchInInfo = SearchInInfoCheckBox.Checked;
            bool isIssuesSearch = SearchFindLinesWithPossibleIssuesCheckBox.Checked;
            bool isEmptySearch = SearchEmptyCheckBox.Checked;
            bool isDoubleSearchEnabled = !isSearchInInfo && !isIssuesSearch && searchQueries.Length == 2 && !string.IsNullOrEmpty(searchQueries[1]);

            if (!isIssuesSearch && !isEmptySearch && string.IsNullOrEmpty(searchQueries[0])) yield break;

            if (isRegex && searchQueries.Any(q => !q.IsValidRegexPattern()))
            {
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Invalid regex!");
                yield break;
            }

            Func<DataRow, bool> searchPredicate = GetSearchPredicate(searchQueries, isRegex, isCaseSensitive, isDoubleSearchEnabled);
            int searchColumnIndex = SearchColumnIndex;

            foreach (var (table, row) in GetRowsToSearch())
            {
                if (IsValidRowForSearch(row, searchColumnIndex) && searchPredicate(row))
                    yield return new FoundRowData(row);
            }
        }

        private Func<DataRow, bool> GetSearchPredicate(string[] searchQueries, bool isRegex, bool isCaseSensitive, bool isDoubleSearchEnabled)
        {
            if (SearchInInfoCheckBox.Checked && searchQueries.Length >= 1)
                return row => IsMatchingInfoSearch(row, searchQueries[0], isRegex, isCaseSensitive);
            if (SearchFindLinesWithPossibleIssuesCheckBox.Checked)
                return IsTheRowHasPossibleIssues;
            if (SearchEmptyCheckBox.Checked)
                return IsEmptyTranslation;
            if (isDoubleSearchEnabled)
                return row => IsMatchingDoubleSearch(row, searchQueries[0], searchQueries[1], isRegex, isCaseSensitive);
            if (searchQueries.Length >= 1)
                return row => IsMatchingGeneralSearch(row, SearchColumnIndex, searchQueries[0], isRegex, isCaseSensitive);

            return _ => false;
        }

        private IEnumerable<(DataTable Table, DataRow Row)> GetRowsToSearch()
        {
            bool searchInSelected = SearchRangeSelectedRadioButton.Checked || SearchRangeVisibleRadioButton.Checked;
            int startIndex = SearchRangeTableRadioButton.Checked || searchInSelected ? _filesList.SelectedIndex : 0;
            int endIndex = SearchRangeTableRadioButton.Checked || searchInSelected ? _filesList.SelectedIndex + 1 : _tables.Count;

            for (int i = startIndex; i < endIndex; i++)
            {
                var table = _tables[i];
                HashSet<int> selectedRowIndices = searchInSelected ? FunctionsTable.GetDGVRowsIndexesHashesInDT(i, SearchRangeVisibleRadioButton.Checked) : null;

                foreach (DataRow row in table.Rows)
                {
                    int rowIndex = table.Rows.IndexOf(row);
                    if (!searchInSelected || selectedRowIndices.Contains(rowIndex))
                        yield return (table, row);
                }
            }
        }

        private bool IsValidRowForSearch(DataRow row, int searchColumnIndex)
        {
            return !(chkbxDoNotTouchEqualOT.Checked && OriginalEqualTranslation(row) ||
                     !SearchModeRegexRadioButton.Checked && !SearchFindLinesWithPossibleIssuesCheckBox.Checked &&
                     !SearchEmptyCheckBox.Checked && string.IsNullOrEmpty(row.Field<string>(searchColumnIndex)) ||
                     SearchEmptyCheckBox.Checked && !IsEmptyTranslation(row));
        }

        private static bool IsMatchingGeneralSearch(DataRow row, int columnIndex, string searchPattern, bool isRegex, bool isCaseSensitive)
        {
            string text = row.Field<string>(columnIndex);
            if (string.IsNullOrEmpty(text)) return false;

            return isRegex
                ? Regex.IsMatch(text, searchPattern, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)
                : text.IndexOf(searchPattern, isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsMatchingDoubleSearch(DataRow row, string originalPattern, string translationPattern, bool isRegex, bool isCaseSensitive)
        {
            string originalText = row.Field<string>(_originalColumnIndex);
            string translationText = row.Field<string>(_translationColumnIndex);
            if (string.IsNullOrEmpty(originalText) || string.IsNullOrEmpty(translationText)) return false;

            return isRegex
                ? Regex.IsMatch(originalText, originalPattern, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase) &&
                  Regex.IsMatch(translationText, translationPattern, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)
                : originalText.IndexOf(originalPattern, isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0 &&
                  translationText.IndexOf(translationPattern, isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsMatchingInfoSearch(DataRow row, string searchPattern, bool isRegex, bool isCaseSensitive)
        {
            int tableIndex = _tables.IndexOf(row.Table);
            string infoText = AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows[row.Table.Rows.IndexOf(row)].Field<string>(0);
            if (string.IsNullOrEmpty(infoText)) return false;

            return isRegex
                ? Regex.IsMatch(infoText, searchPattern, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)
                : infoText.IndexOf(searchPattern, isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsEmptyTranslation(DataRow row) => string.IsNullOrEmpty(row.Field<string>(_translationColumnIndex));

        private static bool OriginalEqualTranslation(DataRow row) => row.Field<string>(_originalColumnIndex).Equals(row.Field<string>(_translationColumnIndex));

        private readonly List<ISearchIssueChecker> _issueCheckers = new List<ISearchIssueChecker>
        {
            new ContainsNonRomaji(),
            new CheckActorsNameTranslationConsistent(),
            new CheckAnyLineTranslatable(),
            new ProjectSpecificIssues(),
            new CheckInternalQuoteUnescaped()
        };

        private bool IsTheRowHasPossibleIssues(DataRow row)
        {
            string original = row.Field<string>(_originalColumnIndex);
            string translation = row.Field<string>(_translationColumnIndex);
            if (string.IsNullOrEmpty(translation) || (AppSettings.IgnoreOrigEqualTransLines && original.Equals(translation)))
                return false;

            var data = new SearchIssueCheckerData(row, original, translation);
            return _issueCheckers.Any(checker => checker.IsHaveTheIssue(data));
        }
        #endregion

        #region UI Event Handlers
        private void THSearch_Load(object sender, EventArgs e)
        {
            Height = SearchResultsWindowNormalHeight;
            _selectedTableIndex = _filesList.SelectedIndex;
            LoadSearchQueriesReplacers();
            chkbxDoNotTouchEqualOT.Checked = AppSettings.IgnoreOrigEqualTransLines;
        }

        private void SelectTextInTextBox(string input)
        {
            Invoke((Action)(() =>
            {
                if (string.IsNullOrEmpty(_translationTextBox.Text)) return;

                bool isRegex = SearchModeRegexRadioButton.Checked;
                bool matchCase = THSearchMatchCaseCheckBox.Checked;
                string searchWord = isRegex ? SearchFormFindWhatTextBox.Text : Regex.Escape(SearchFormFindWhatTextBox.Text);
                var options = matchCase ? RichTextBoxFinds.MatchCase : RichTextBoxFinds.None;

                if (isRegex)
                {
                    foreach (Match match in Regex.Matches(_translationTextBox.Text, searchWord, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase))
                    {
                        _translationTextBox.Select(match.Index, match.Length);
                        _translationTextBox.SelectionBackColor = Color.Yellow;
                    }
                }
                else
                {
                    int start = 0;
                    while ((start = _translationTextBox.Find(searchWord, start, options)) != -1)
                    {
                        _translationTextBox.SelectionBackColor = Color.Yellow;
                        start += SearchFormFindWhatTextBox.TextLength;
                    }
                }
            }));
        }

        private void SearchResultsDatagridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _foundRowsList.Count)
                ShowSelectedCellInMainTable(e.RowIndex);
        }

        private async void ShowSelectedCellInMainTable(int rowIndex)
        {
            try
            {
                var foundRowData = _foundRowsList[rowIndex];
                (_selectedTableIndex, _selectedRowIndex) = (foundRowData.TableIndex, foundRowData.RowIndex);

                _workFileDgv.CleanFilter();

                var tableDefaultView = _tables[_selectedTableIndex].DefaultView;
                tableDefaultView.RowFilter = string.Empty;
                tableDefaultView.Sort = string.Empty;
                _workFileDgv.Refresh();

                FunctionsTable.ShowSelectedRow(_selectedTableIndex, SearchColumnIndex, _selectedRowIndex, AppData.Main.THFileElementsDataGridView);
                if (_workFileDgv.CurrentCell != null)
                {
                    await Task.Run(() => SelectTextInTextBox(_workFileDgv.CurrentCell.Value.ToString())).ConfigureAwait(false);
                }
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
        }

        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text) || AppData.CurrentProject.FilesContent == null)
                return;

            lblSearchMsg.Visible = false;
            Height = SearchResultsWindowNormalHeight;

            if (_foundRowsEnum == null || _lastSearchString != SearchFormFindWhatTextBox.Text)
            {
                _foundRowsEnum = EnumerateAndFillSearchResults().GetEnumerator();
                _lastSearchString = SearchFormFindWhatTextBox.Text;
            }

            if (!_foundRowsEnum.MoveNext())
            {
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = NothingFoundMessage;
                _foundRowsEnum = null;
                return;
            }

            var foundRow = _foundRowsEnum.Current;
            _filesList.SelectedIndex = foundRow.TableIndex;
            _workFileDgv.DataSource = _tables[foundRow.TableIndex];
            _workFileDgv.CurrentCell = _workFileDgv[SearchColumnIndex, foundRow.RowIndex];
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text) || AppData.CurrentProject.FilesContent == null)
                return;

            bool inputEqualWithLatest = THSearchMatchCaseCheckBox.Checked
                ? SearchFormFindWhatTextBox.Text == _lastFoundValue
                : string.Equals(SearchFormFindWhatTextBox.Text, _lastFoundValue, StringComparison.OrdinalIgnoreCase);

            if (inputEqualWithLatest && _startRowSearchIndex > 0)
            {
                ReplaceCurrentRow();
                MoveToNextRow();
            }
            else
            {
                _startRowSearchIndex = 0;
                lblSearchMsg.Visible = false;
                GetSearchResults();

                if (_foundRowsList?.Count > 0)
                {
                    StoreFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                    MoveToNextRow();
                }
                else
                {
                    lblSearchMsg.Visible = true;
                    lblSearchMsg.Text = NothingFoundMessage;
                    Height = SearchResultsWindowNormalHeight;
                }
            }
        }

        private void ReplaceCurrentRow()
        {
            if (_selectedTableIndex >= 0 && _selectedTableIndex < _filesList.Items.Count &&
                _selectedRowIndex >= 0 && _selectedRowIndex < _workFileDgv.Rows.Count)
            {
                string value = _workFileDgv[SearchColumnIndex, _selectedRowIndex].Value?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(value)) return;

                bool isRegex = SearchModeRegexRadioButton.Checked;
                string findText = SearchFormFindWhatTextBox.Text;
                string replaceText = FixRegexReplacementFromTextbox(SearchFormReplaceWithTextBox.Text);

                if (isRegex && Regex.IsMatch(value, findText, RegexOptions.IgnoreCase) ||
                    !isRegex && value.IndexOf(findText, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    StoreFoundValueToComboBox(findText, replaceText);
                    string translation = _workFileDgv[THSettings.TranslationColumnName, _selectedRowIndex].Value?.ToString() ?? value;
                    _workFileDgv[THSettings.TranslationColumnName, _selectedRowIndex].Value = isRegex
                        ? Regex.Replace(translation, findText, replaceText, RegexOptions.IgnoreCase)
                        : ReplaceEx.Replace(translation, findText, replaceText, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private void MoveToNextRow()
        {
            if (_startRowSearchIndex >= _foundRowsList.Count) _startRowSearchIndex = 0;

            var foundRowData = _foundRowsList[_startRowSearchIndex];
            (_selectedTableIndex, _selectedRowIndex) = (foundRowData.TableIndex, foundRowData.RowIndex);

            if (_selectedTableIndex != _filesList.SelectedIndex)
            {
                _filesList.SelectedIndex = _selectedTableIndex;
                _workFileDgv.DataSource = _tables[_selectedTableIndex];
            }

            _workFileDgv.CurrentCell = _workFileDgv[SearchColumnIndex, _selectedRowIndex];
            new Thread(() => SelectTextInTextBox(SearchFormFindWhatTextBox.Text)).Start();
            _startRowSearchIndex++;
        }

        private void SearchFormReplaceAllButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SearchFormFindWhatTextBox.Text) || AppData.CurrentProject.FilesContent == null ||
                (ConfirmReplaceAllCheckBox.Checked && !FunctionsMessage.ShowConfirmationDialog(T._("Replace All") + "?", T._("Confirmation"))))
                return;

            lblSearchMsg.Visible = false;
            bool isAnyChanged = false;
            string searchPattern = SearchFormFindWhatTextBox.Text;
            string searchReplacementUnescaped = FixRegexReplacementFromTextbox(SearchFormReplaceWithTextBox.Text);

            Action<DataRow> replaceAction = SearchInInfoCheckBox.Checked
                ? GetInfoSearchReplaceAction(searchReplacementUnescaped)
                : GetGeneralReplaceAction(searchPattern, searchReplacementUnescaped);

            foreach (var foundRowData in EnumerateAndFillSearchResults())
            {
                replaceAction(foundRowData.Row);
                isAnyChanged = true;
                (_selectedTableIndex, _selectedRowIndex) = (foundRowData.TableIndex, foundRowData.RowIndex);
            }

            lblSearchMsg.Visible = true;
            if (isAnyChanged)
            {
                StoreFoundValueToComboBox(searchPattern, SearchFormReplaceWithTextBox.Text);
                lblSearchMsg.Text = T._("Found ") + _foundRowsList.Count + T._(" records");
                Height = SearchResultsWindowExpandedHeight;
                PopulateGrid(_foundRowsList);
            }
            else
            {
                lblSearchMsg.Text = T._("Nothing Found");
                Height = SearchResultsWindowNormalHeight;
            }
        }

        private static Action<DataRow> GetInfoSearchReplaceAction(string replacement)
        {
            if (replacement == ReplaceToEqualMarker)
                return row => row.SetField(_translationColumnIndex, row[_originalColumnIndex]);
            if (string.IsNullOrEmpty(replacement))
                return row => row.SetField(_translationColumnIndex, DBNull.Value);
            return _ => { }; // No replacement if invalid for info search
        }

        private Action<DataRow> GetGeneralReplaceAction(string searchPattern, string replacement)
        {
            return row =>
            {
                string valueToReplace = row.Field<string>(SearchColumnIndex);
                if (string.IsNullOrEmpty(valueToReplace)) return;

                bool isRegex = SearchModeRegexRadioButton.Checked;
                var options = THSearchMatchCaseCheckBox.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
                var comparison = THSearchMatchCaseCheckBox.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                row.SetField(_translationColumnIndex, isRegex
                    ? Regex.Replace(valueToReplace, searchPattern, replacement, options)
                    : ReplaceEx.Replace(valueToReplace, searchPattern, replacement, comparison));
            };
        }

        private string FixRegexReplacementFromTextbox(string text)
            => SearchModeRegexRadioButton.Checked ? Regex.Unescape(text) : text;
        #endregion

        #region Form Behavior
        private readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        private void SearchAlwaysOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ExternalAdditions.NativeMethods.SetWindowPos(Handle, SearchAlwaysOnTopCheckBox.Checked ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private void THSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSearchQueriesReplacers();
            _foundRowsList = null;
        }
        #endregion


        #region Events
        private void SearchFindLinesWithPossibleIssuesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Text = string.Empty;
            SearchFormFindWhatTextBox.Enabled = !SearchFormFindWhatTextBox.Enabled;
            SearchFormFindWhatComboBox.Enabled = !SearchFormFindWhatComboBox.Enabled;


            SearchFormReplaceWithTextBox.Enabled = !SearchFormReplaceWithTextBox.Enabled;
            SearchFormReplaceWithComboBox.Enabled = !SearchFormReplaceWithComboBox.Enabled;
            SearchFormReplaceAllButton.Enabled = !SearchFormReplaceAllButton.Enabled;
            SearchFormReplaceButton.Enabled = !SearchFormReplaceButton.Enabled;
            SearchFormFindNextButton.Enabled = !SearchFormFindNextButton.Enabled;
        }

        private void ClearFindWhatTextBoxLabel_Click(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Clear();
        }

        private void ClearReplaceWithTextBoxLabel_Click(object sender, EventArgs e)
        {
            SearchFormReplaceWithTextBox.Clear();
        }

        private void SearchFormReplaceWithComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchFormReplaceWithTextBox.Text = (sender as ComboBox).SelectedItem.ToString();
        }

        private void SearchResultsDatagridview_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //ShowSelectedCellInMainTable(sender, e);//если здесь, то после последних изменений после нахождения результатов сразу отображает первый
        }

        private void SearchResultsDatagridview_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            ShowSelectedCellInMainTable(e.RowIndex);
        }

        private void SearchRangeSelectedRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeSelectedRadioButton.Checked = true;
            SearchRangeVisibleRadioButton.Checked = false;
            SearchRangeTableRadioButton.Checked = false;
            SearchRangeAllRadioButton.Checked = false;
        }

        private void SearchRangeVisibleRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeSelectedRadioButton.Checked = false;
            SearchRangeVisibleRadioButton.Checked = true;
            SearchRangeTableRadioButton.Checked = false;
            SearchRangeAllRadioButton.Checked = false;
        }

        private void DoubleSearchOptionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DoubleSearchOptionCheckBox.Checked && !SearchFormFindWhatTextBox.Text.Contains(DoubleSearchMarker))
            {
                SearchFormFindWhatTextBox.Text += DoubleSearchMarker;
            }
            else if (!DoubleSearchOptionCheckBox.Checked && SearchFormFindWhatTextBox.Text.Contains(DoubleSearchMarker))
            {
                SearchFormFindWhatTextBox.Text = SearchFormFindWhatTextBox.Text.Replace(DoubleSearchMarker, "");
            }
        }

        private void SearchFormFindWhatTextBox_TextChanged(object sender, EventArgs e)
        {
            DoubleSearchOptionCheckBox.Checked = SearchFormFindWhatTextBox.Text.Contains(DoubleSearchMarker);
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

        private void SearchFormFindWhatComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Text = SearchFormFindWhatComboBox.SelectedItem.ToString();
        }

        private void THSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void SearchAlwaysOnTopCheckBox_Click(object sender, EventArgs e)
        {
        }
        #endregion
    }
}
