using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers;
using TranslationHelper.Main.Functions;
using static RubyMarshal.RubyMarshal;

namespace TranslationHelper
{
    public partial class THfrmSearch : Form
    {
        readonly ListBox _filesList;
        readonly DataGridView _workFileDgv;
        readonly DataTableCollection _tables;
        readonly RichTextBox _translationTextBox;
        readonly INIFileMan.INIFile _config;

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

        private string GetSearchColumn()
        {
            return SearchMethodTranslationRadioButton.Checked ? THSettings.TranslationColumnName : THSettings.OriginalColumnName;
        }

        int startRowSearchIndex;        //Индекс стартовой ячейки для поиска
        int tableIndex;
        int rowIndex;
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || AppData.CurrentProject.FilesContent == null)
            {
                return;
            }

            bool inputEqualwithLatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, lastfoundvalue, true, CultureInfo.InvariantCulture) == 0;
            if (inputEqualwithLatest)
            {
                string searchColumn = GetSearchColumn();
                (tableIndex, rowIndex) = _foundRowsTableRowIndexes[startRowSearchIndex];

                if (tableIndex != _filesList.SelectedIndex)
                {
                    _filesList.SelectedIndex = tableIndex;
                    _workFileDgv.DataSource = _tables[tableIndex];
                }
                _workFileDgv.CurrentCell = _workFileDgv[searchColumn, rowIndex];

                //подсвечивание найденного
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectString = new Thread(new ParameterizedThreadStart((obj) => SelectTextInTextBox(SearchFormFindWhatTextBox.Text)));
                selectString.Start();

                startRowSearchIndex++;
                if (startRowSearchIndex == oDsResults.Tables[0].Rows.Count) startRowSearchIndex = 0;
            }
            else
            {
                startRowSearchIndex = 0;
                lblSearchMsg.Visible = false;
                oDsResults = AppData.CurrentProject.FilesContent.Clone();

                DataTable drFoundRowsTable = GetSearchResults(oDsResults);

                if (drFoundRowsTable == null) return;

                if (drFoundRowsTable.Rows.Count == 0)
                {
                    //PopulateGrid(null);
                    lblSearchMsg.Visible = true;
                    lblSearchMsg.Text = "Nothing Found.";
                    this.Height = 368;
                    return;
                }

                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                oDsResults.AcceptChanges();

                string searchcolumn = GetSearchColumn();
                (tableIndex, rowIndex) = _foundRowsTableRowIndexes[startRowSearchIndex];

                if (tableIndex != _filesList.SelectedIndex)
                {
                    _filesList.SelectedIndex = tableIndex;
                    _workFileDgv.DataSource = _tables[tableIndex];
                }
                _workFileDgv.CurrentCell = _workFileDgv[searchcolumn, rowIndex];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextInTextBox(SearchFormFindWhatTextBox.Text)));
                selectstring.Start();

                startRowSearchIndex++;
                if (startRowSearchIndex == oDsResults.Tables[0].Rows.Count) startRowSearchIndex = 0;
            }
        }

        private void LoadSearchQueriesReplacers()
        {
            try
            {
                SearchQueries = _config.GetSectionValues("Search Queries").ToArray();
                if (SearchQueries != null && SearchQueries.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref SearchQueries);
                    UnEscapeSearchValues(ref SearchQueries);
                    SearchFormFindWhatComboBox.Items.Clear();
                    SearchFormFindWhatComboBox.Items.AddRange(SearchQueries);
                }
                SearchReplacers = _config.GetSectionValues("Search Replacers").ToArray();
                if (SearchReplacers != null && SearchReplacers.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref SearchReplacers);
                    UnEscapeSearchValues(ref SearchReplacers);
                    SearchFormReplaceWithComboBox.Items.Clear();
                    SearchFormReplaceWithComboBox.Items.AddRange(SearchReplacers);
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
                        Regex.Unescape(arr[i]);
                    }
                    else
                    {
                        Regex.Escape(arr[i]);
                    }
                }
                catch { }
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
                if (SearchFormFindWhatComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(SearchQueries, SearchFormFindWhatComboBox.Items))
                {
                    SearchQueries = new string[SearchFormFindWhatComboBox.Items.Count];
                    SearchFormFindWhatComboBox.Items.CopyTo(SearchQueries, 0);
                    AddQuotesToWritingSearchValues(ref SearchQueries);
                    UnEscapeSearchValues(ref SearchQueries, false);
                    _config.SetArrayToSectionValues("Search Queries", SearchQueries);
                }
                if (SearchFormReplaceWithComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(SearchReplacers, SearchFormReplaceWithComboBox.Items))
                {
                    SearchReplacers = new string[SearchFormReplaceWithComboBox.Items.Count];
                    SearchFormReplaceWithComboBox.Items.CopyTo(SearchReplacers, 0);
                    AddQuotesToWritingSearchValues(ref SearchReplacers);
                    UnEscapeSearchValues(ref SearchReplacers, false);
                    _config.SetArrayToSectionValues("Search Replacers", SearchReplacers);
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

        string lastfoundvalue = string.Empty;
        string lastfoundreplacedvalue = string.Empty;
        string[] SearchQueries;
        string[] SearchReplacers;
        private void StoryFoundValueToComboBox(string foundvalue, string replaceWithValue = "")
        {
            //store found value
            lastfoundvalue = foundvalue;
            StoreFoundReplaceValues(foundvalue, SearchFormFindWhatComboBox);

            //store replace value
            lastfoundreplacedvalue = replaceWithValue;
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

        private void PopulateGrid(DataSet oDsResults)
        {
            if (oDsResults == null)
            {
            }
            else
            {
                SearchResultsDatagridview.DataSource = oDsResults.Tables[0];
                for (int c = 0; c < SearchResultsDatagridview.ColumnCount; c++)
                {
                    if (c > 1)
                    {
                        SearchResultsDatagridview.Columns[c].Visible = false;
                    }
                }
                SearchResultsDatagridview.Columns[0].ReadOnly = true;

                SearchResultsDatagridview.Visible = true;
                SearchResultsPanel.Visible = true;
            }
        }

        //http://mrbool.com/dataset-advance-operations-search-sort-filter-net/24769
        //https://stackoverflow.com/questions/3608388/c-sharp-access-dataset-data-from-another-class
        //http://qaru.site/questions/236566/how-to-know-the-row-index-from-datatable-object
        DataSet oDsResults;
        private void SearchAllButton_Click(object sender, EventArgs e)
        {
            if (AppData.CurrentProject.FilesContent == null
                || !SearchFindLinesWithPossibleIssuesCheckBox.Checked && SearchFormFindWhatTextBox.Text.Length == 0)
            {
                return;
            }

            lblSearchMsg.Visible = false;
            oDsResults = AppData.CurrentProject.FilesContent.Clone();
            //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
            DataTable drFoundRowsTable = GetSearchResults(oDsResults);

            if (drFoundRowsTable == null) return;

            if (drFoundRowsTable.Rows.Count > 0)
            {
                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                oDsResults.AcceptChanges();
                PopulateGrid(oDsResults);

                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
                this.Height = 589;
            }
            else
            {
                //PopulateGrid(null);
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Nothing Found");
                SearchResultsDatagridview.DataSource = null;
                SearchResultsDatagridview.Refresh();
                this.Height = 368;
            }
        }

        private readonly List<(int tableIndex, int rowIndex)> _foundRowsTableRowIndexes = new List<(int tableIndex, int rowIndex)>();

        private DataTable GetSearchResults(DataSet dataSet)
        {
            lblSearchMsg.Visible = false;
            if (_tables.Count == 0) return dataSet.Tables[0];

            string searchColumnIndex = GetSearchColumn();
            bool isSearchInInfo = SearchInInfoCheckBox.Checked;
            bool isIssuesSearch = SearchFindLinesWithPossibleIssuesCheckBox.Checked;
            string searchQueryText = SearchFormFindWhatTextBox.Text;
            bool isFound = false;
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

                int originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
                int translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;

                var rowsCount = tableWhereSearching.Rows.Count;
                for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    if (searchInSelected && !selectedrowsHashes.Contains(rowIndex)) continue;

                    var row2check = _tables[tableIndex].Rows[rowIndex];

                    if (IsValid2Search(row2check, originalColumnIndex, translationColumnIndex, searchColumnIndex))
                    {
                        continue;
                    }

                    if (isSearchInInfo)//search in info box
                    {
                        var infoValue = (AppData.CurrentProject.FilesContentInfo.Tables[tableIndex].Rows[rowIndex][0] + string.Empty);
                        if (GetCheckResult(ref isFound, dataSet, row2check, tableIndex, rowIndex, infoValue, searchQueryText) == SearchResult.Error) return null;
                    }
                    else if (isIssuesSearch && IsTheRowHasPossibleIssues(row2check)) //search rows with possible issues
                    {
                        ImportRowToFound(ref isFound, dataSet, row2check, tableIndex, rowIndex); 
                    }
                    else
                    {
                        string SelectedCellValue = _tables[tableIndex].Rows[rowIndex][searchColumnIndex] + string.Empty;
                        if (GetCheckResult(ref isFound, dataSet, row2check, tableIndex, rowIndex, SelectedCellValue, searchQueryText) == SearchResult.Error) return null; // general search
                    }
                }
            }
            return dataSet.Tables[0];
        }

        private bool IsValid2Search(DataRow row2check, int origColumnIndex, int transColumnIndex, string searchcolumn)
        {
            //skip equal lines if need, skip empty search cells && not skip when row issue search
            return (chkbxDoNotTouchEqualOT.Checked && row2check[origColumnIndex].Equals(row2check[transColumnIndex]))
                        || (!chkbxDoNotTouchEqualOT.Checked
                        && !SearchFindLinesWithPossibleIssuesCheckBox.Checked
                        && (row2check[searchcolumn] + string.Empty).Length == 0);
        }

        private SearchResult GetCheckResult(ref bool found, DataSet ds, DataRow row, int t, int r, string infoValue, string strQuery)
        {
            if (SearchModeRegexRadioButton.Checked) // regex check
            {
                return CheckRegex(ref found, ds, row, t, r, infoValue, strQuery);
            }
            else return CheckIsFound(ref found, ds, row, t, r, infoValue, strQuery); // common check
        }

        private SearchResult CheckIsFound(ref bool found, DataSet ds, DataRow row, int t, int r, string infoValue, string strQuery)
        {
            try
            {
                if ((THSearchMatchCaseCheckBox.Checked && infoValue.Contains(strQuery))
                    || (!THSearchMatchCaseCheckBox.Checked && infoValue.IndexOf(strQuery, StringComparison.CurrentCultureIgnoreCase) != -1))
                {
                    ImportRowToFound(ref found, ds, row, t, r);
                    return SearchResult.Found;
                }
            }
            catch { return SearchResult.Error; }

            return SearchResult.NotFound;
        }

        private SearchResult CheckRegex(ref bool found, DataSet ds, DataRow row, int t, int r, string infoValue, string strQuery)
        {
            try
            {
                if ((THSearchMatchCaseCheckBox.Checked && Regex.IsMatch(infoValue, strQuery))
                    || (!THSearchMatchCaseCheckBox.Checked && Regex.IsMatch(infoValue, strQuery, RegexOptions.IgnoreCase)))
                {
                    ImportRowToFound(ref found, ds, row, t, r);
                    return SearchResult.Found;
                }
            }
            catch (ArgumentException ex)
            {
                //при ошибках регекса выходить
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Invalid regex") + ">" + ex.Message;
                return SearchResult.Error;
            }

            return SearchResult.NotFound;
        }

        public enum SearchResult
        {
            Found,
            NotFound,
            Error
        }

        /// <summary>
        /// Add found row to results
        /// </summary>
        /// <param name="found"></param>
        /// <param name="ds"></param>
        /// <param name="row"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        private void ImportRowToFound(ref bool found, DataSet ds, DataRow row, int t, int r)
        {
            if (!found)
            {
                found = true;
                _foundRowsTableRowIndexes.Clear();
                this.Height = 368;
            }
            ds.Tables[0].ImportRow(row);
            _foundRowsTableRowIndexes.Add((t, r));
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
            var o = row[AppData.CurrentProject.OriginalColumnIndex] + string.Empty;
            var t = row[AppData.CurrentProject.TranslationColumnIndex] + string.Empty;
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
            this.Height = 368;
            tableIndex = _filesList.SelectedIndex;

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
                var start = match.Index;
                var length = match.Length;

                while (start < _translationTextBox.TextLength)
                {
                    var startIndex = _translationTextBox.Find(match.Value, start, findOptions);

                    if (startIndex == -1) break;

                    _translationTextBox.SelectionStart = startIndex;
                    _translationTextBox.SelectionLength = length;
                    _translationTextBox.SelectionBackColor = Color.Yellow;

                    start = startIndex + length;
                }
            }
        }

        private void SearchResultsDatagridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //ShowSelectedCellInMainTable(sender, e);
        }

        private void ShowSelectedCellInMainTable(object sender, DataGridViewCellEventArgs e)
        {
            string searchcolumn = GetSearchColumn();
            try
            {
                //было исключение, отсутствует позиция, хотя позицияприсутствовала
                (tableIndex, rowIndex) = _foundRowsTableRowIndexes[e.RowIndex];

                _tables[tableIndex].DefaultView.RowFilter = string.Empty;
                _tables[tableIndex].DefaultView.Sort = string.Empty;
                _workFileDgv.Refresh();

                FunctionsTable.ShowSelectedRow(tableIndex, searchcolumn, rowIndex);

                if (_workFileDgv != null && _workFileDgv.DataSource != null && _workFileDgv.CurrentCell != null)
                {
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextInTextBox(_workFileDgv.CurrentCell.Value.ToString())));
                    selectstring.Start();
                }
            }
            catch { } // ignore errors

        }

        private void SearchFormFindWhatComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Text = SearchFormFindWhatComboBox.SelectedItem.ToString();
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || AppData.CurrentProject.FilesContent == null)
            {
                return;
            }

            bool inputEqualWithLatest = THSearchMatchCaseCheckBox.Checked ?
                SearchFormFindWhatTextBox.Text == lastfoundvalue
                : string.Compare(SearchFormFindWhatTextBox.Text, lastfoundvalue, true, CultureInfo.InvariantCulture) == 0;
            if (inputEqualWithLatest)
            {
                string searchcolumn = GetSearchColumn();

                if (startRowSearchIndex == 0) return;

                if (tableIndex >= 0 && tableIndex < _filesList.Items.Count && rowIndex >= 0 && rowIndex < _workFileDgv.Rows.Count)
                {
                    string value = _workFileDgv[searchcolumn, rowIndex].Value + string.Empty;
                    if (value.Length > 0)
                    {
                        if (SearchModeRegexRadioButton.Checked)
                        {
                            if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                            {
                                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                                _workFileDgv[THSettings.TranslationColumnName, rowIndex].Value = Regex.Replace(GetFirstIfNotEmpty(_workFileDgv[THSettings.TranslationColumnName, rowIndex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, FixRegexReplacementFromTextbox(SearchFormReplaceWithTextBox.Text), RegexOptions.IgnoreCase);
                            }
                        }
                        else
                        {
                            if (value.IndexOf(SearchFormFindWhatTextBox.Text, StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                                _workFileDgv[THSettings.TranslationColumnName, rowIndex].Value = ReplaceEx.Replace(GetFirstIfNotEmpty(_workFileDgv[THSettings.TranslationColumnName, rowIndex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                            }
                        }
                    }
                }

                if (startRowSearchIndex == oDsResults.Tables[0].Rows.Count) startRowSearchIndex = 0;

                (tableIndex, rowIndex) = _foundRowsTableRowIndexes[startRowSearchIndex];

                if (tableIndex != _filesList.SelectedIndex)
                {
                    _filesList.SelectedIndex = tableIndex;
                    _workFileDgv.DataSource = _tables[tableIndex];
                }
                _workFileDgv.CurrentCell = _workFileDgv[searchcolumn, rowIndex];

                //подсвечивание найденного
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextInTextBox(SearchFormFindWhatTextBox.Text)));
                selectstring.Start();

                startRowSearchIndex++;
            }
            else
            {
                startRowSearchIndex = 0;
                lblSearchMsg.Visible = false;
                oDsResults = AppData.CurrentProject.FilesContent.Clone();

                DataTable drFoundRowsTable = GetSearchResults(oDsResults);

                if (drFoundRowsTable == null) return;

                if (drFoundRowsTable.Rows.Count == 0)
                {
                    //PopulateGrid(null);
                    lblSearchMsg.Visible = true;
                    lblSearchMsg.Text = T._("Nothing Found.");
                    this.Height = 368;
                    return;
                }

                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);

                oDsResults.AcceptChanges();

                (tableIndex, rowIndex) = _foundRowsTableRowIndexes[startRowSearchIndex];
                if (tableIndex != _filesList.SelectedIndex)
                {
                    _filesList.SelectedIndex = tableIndex;
                    _workFileDgv.DataSource = _tables[tableIndex];
                }

                _workFileDgv.CurrentCell = _workFileDgv[GetSearchColumn(), rowIndex];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextInTextBox(SearchFormFindWhatTextBox.Text)));
                selectstring.Start();

                startRowSearchIndex++;
                if (startRowSearchIndex == oDsResults.Tables[0].Rows.Count) startRowSearchIndex = 0;
            }
        }

        private static string GetFirstIfNotEmpty(string first, string defaultvalue)
        {
            return first.Length == 0 ? defaultvalue : first;
        }

        private void SearchFormReplaceAllButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || AppData.CurrentProject.FilesContent == null)
            {
                return;
            }

            if (ConfirmReplaceAllCheckBox.Checked && !FunctionsMessage.ShowConfirmationDialog(T._("Replace All") + "?", T._("Confirmation")))
            {
                return;
            }

            var replacementUnescaped = FixRegexReplacementFromTextbox(SearchFormReplaceWithTextBox.Text);

            lblSearchMsg.Visible = false;
            oDsResults = AppData.CurrentProject.FilesContent.Clone();
            DataTable drFoundRowsTable = GetSearchResults(oDsResults);

            if (drFoundRowsTable == null) return;

            if (drFoundRowsTable.Rows.Count == 0)
            {
                //PopulateGrid(null);
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Nothing Found");
                this.Height = 368;
                return;
            }

            bool StoreQueryAndReplacer = false;

            oDsResults.AcceptChanges();
            PopulateGrid(oDsResults);

            lblSearchMsg.Visible = true;
            lblSearchMsg.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
            this.Height = 589;

            string searchcolumn = GetSearchColumn();
            int oDsResultsCount = oDsResults.Tables[0].Rows.Count;
            for (int r = 0; r < oDsResultsCount; r++)
            {
                (tableIndex, rowIndex) = _foundRowsTableRowIndexes[r];
                var row = _tables[tableIndex].Rows[rowIndex];
                string value = row[searchcolumn] + string.Empty;
                if (value.Length == 0) continue;

                if (SearchInInfoCheckBox.Checked)
                {
                    if (replacementUnescaped == "=")
                    {
                        row[THSettings.TranslationColumnName] = row[THSettings.OriginalColumnName];
                    }
                    else if (string.IsNullOrEmpty(replacementUnescaped))
                    {
                        row[THSettings.TranslationColumnName] = string.Empty;
                    }
                }
                else
                {
                    if (SearchModeRegexRadioButton.Checked)
                    {
                        if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                        {
                            StoreQueryAndReplacer = true;
                            row[THSettings.TranslationColumnName] = Regex.Replace(GetFirstIfNotEmpty(row[THSettings.TranslationColumnName] + string.Empty, value), SearchFormFindWhatTextBox.Text, replacementUnescaped, RegexOptions.IgnoreCase);
                        }
                    }
                    else
                    {
                        if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                        {
                            StoreQueryAndReplacer = true;
                            row[THSettings.TranslationColumnName] = ReplaceEx.Replace(GetFirstIfNotEmpty(row[THSettings.TranslationColumnName] + string.Empty, value), SearchFormFindWhatTextBox.Text, replacementUnescaped, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }

            if (StoreQueryAndReplacer)
            {
                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
            }
        }

        private string FixRegexReplacementFromTextbox(string text)
        {
            if (SearchModeRegexRadioButton.Checked)
            {
                try
                {
                    return Regex.Unescape(text);
                }
                catch { }

            }

            return text;
        }

        private void THSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void SearchAlwaysOnTopCheckBox_Click(object sender, EventArgs e)
        {
        }

        //https://www.c-sharpcorner.com/uploadfile/kirtan007/make-form-stay-always-on-top-of-every-window/
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        //private static readonly IntPtr HWND_TOP = new IntPtr(0);
        //private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        private void SearchAlwaysOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                ExternalAdditions.NativeMethods.SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                ExternalAdditions.NativeMethods.SetWindowPos(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

        private void THSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSearchQueriesReplacers();
        }

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
            ShowSelectedCellInMainTable(sender, e);
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
    }
}
