using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class HfrmSearch : Form
    {
        internal ListBox ThFilesListBox;

        internal DataGridView ThFileElementsDataGridView;
        readonly RichTextBox _thTargetRichTextBox;
        readonly INIFileMan.INIFile _config;

        internal HfrmSearch(ListBox listBox, DataGridView dgv, RichTextBox ttb)
        {
            InitializeComponent();
            //Main = MainForm;
            ThFilesListBox = listBox;

            ThFileElementsDataGridView = dgv;
            _thTargetRichTextBox = ttb;

            _config = ProjectData.Main.Settings.ThConfigIni;

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
                ExternalAdditions.NativeMethods.SetWindowPos(this.Handle, HwndTopmost, 0, 0, 0, 0, TopmostFlags);
            }

            GetSelectedText();
        }

        public void GetSelectedText()
        {
            //if (projectData != null)
            {
                if (ProjectData.Main.THFileElementsDataGridView.CurrentCell.IsInEditMode)
                {
                    //https://stackoverflow.com/questions/41380883/retrieve-partly-selected-text-when-the-cell-is-in-edit-mode-in-datagridview
                    if (ProjectData.Main.THFileElementsDataGridView.EditingControl is TextBox textBox)
                    {
                        SearchFormFindWhatTextBox.Text = textBox.SelectedText;
                    }
                }
                else if (ProjectData.Main.THSourceRichTextBox.SelectedText.Length > 0)
                {
                    SearchFormFindWhatTextBox.Text = ProjectData.Main.THSourceRichTextBox.SelectedText;
                }
                else if (ProjectData.Main.THTargetRichTextBox.SelectedText.Length > 0)
                {
                    SearchFormFindWhatTextBox.Text = ProjectData.Main.THTargetRichTextBox.SelectedText;
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
            return SearchMethodTranslationRadioButton.Checked ? "Translation" : "Original";
        }

        int _startrowsearchindex;        //Индекс стартовой ячейки для поиска
        int _tableindex;
        int _rowindex;
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || ProjectData.ThFilesElementsDataset == null)
            {
            }
            else
            {
                bool inputqualwithlatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == _lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, _lastfoundvalue, true, CultureInfo.InvariantCulture) == 0;
                if (inputqualwithlatest)
                {
                    string searchcolumn = GetSearchColumn();
                    _tableindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                    _rowindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);

                    if (_tableindex == ThFilesListBox.SelectedIndex)
                    {
                    }
                    else
                    {
                        ThFilesListBox.SelectedIndex = _tableindex;
                        ThFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[_tableindex];
                    }
                    ThFileElementsDataGridView.CurrentCell = ThFileElementsDataGridView[searchcolumn, _rowindex];

                    //подсвечивание найденного
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                    selectstring.Start();

                    _startrowsearchindex++;
                    if (_startrowsearchindex == _oDsResults.Tables[0].Rows.Count)
                    {
                        _startrowsearchindex = 0;
                    }

                }
                else
                {
                    _startrowsearchindex = 0;
                    lblSearchMsg.Visible = false;
                    _oDsResults = ProjectData.ThFilesElementsDataset.Clone();

                    DataTable drFoundRowsTable = SearchNew(_oDsResults);

                    if (drFoundRowsTable == null)
                    {
                    }
                    else
                    {
                        if (drFoundRowsTable.Rows.Count > 0)
                        {
                            StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                            _oDsResults.AcceptChanges();

                            string searchcolumn = GetSearchColumn();
                            _tableindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                            _rowindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);

                            if (_tableindex == ThFilesListBox.SelectedIndex)
                            {
                            }
                            else
                            {
                                ThFilesListBox.SelectedIndex = _tableindex;
                                ThFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[_tableindex];
                            }
                            ThFileElementsDataGridView.CurrentCell = ThFileElementsDataGridView[searchcolumn, _rowindex];

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                            selectstring.Start();

                            _startrowsearchindex++;
                            if (_startrowsearchindex == _oDsResults.Tables[0].Rows.Count)
                            {
                                _startrowsearchindex = 0;
                            }
                        }
                        else
                        {
                            //PopulateGrid(null);
                            lblSearchMsg.Visible = true;
                            lblSearchMsg.Text = "Nothing Found.";
                            this.Height = 368;
                        }
                    }
                }

            }
        }

        private void LoadSearchQueriesReplacers()
        {
            try
            {
                _searchQueries = _config.GetSectionValues("Search Queries").ToArray();
                if (_searchQueries != null && _searchQueries.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref _searchQueries);
                    SearchFormFindWhatComboBox.Items.Clear();
                    SearchFormFindWhatComboBox.Items.AddRange(_searchQueries);
                }
                _searchReplacers = _config.GetSectionValues("Search Replacers").ToArray();
                if (_searchReplacers != null && _searchReplacers.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref _searchReplacers);
                    SearchFormReplaceWithComboBox.Items.Clear();
                    SearchFormReplaceWithComboBox.Items.AddRange(_searchReplacers);
                }
            }
            catch
            {
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
                    _config.SetArrayToSectionValues("Search Queries", _searchQueries);
                }
                if (SearchFormReplaceWithComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(_searchReplacers, SearchFormReplaceWithComboBox.Items))
                {
                    _searchReplacers = new string[SearchFormReplaceWithComboBox.Items.Count];
                    SearchFormReplaceWithComboBox.Items.CopyTo(_searchReplacers, 0);
                    AddQuotesToWritingSearchValues(ref _searchReplacers);
                    _config.SetArrayToSectionValues("Search Replacers", _searchReplacers);
                }
            }
            catch
            {
            }
        }

        private static bool IsSearchQueriesReplacersListChanged(string[] oldList, ComboBox.ObjectCollection items)
        {
            if (oldList.Length != items.Count)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < oldList.Length; i++)
                {
                    if (oldList[i] != items[i].ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        string _lastfoundvalue = string.Empty;
        string _lastfoundreplacedvalue = string.Empty;
        string[] _searchQueries;
        string[] _searchReplacers;
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

        private static void StoreFoundReplaceValues(string value, ComboBox comboBox)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            //lastvalue = value;
            int itemsCount = comboBox.Items.Count;
            if (itemsCount > 0)
            {
                if (!comboBox.Items.Contains(value))
                {
                    if (itemsCount == Properties.Settings.Default.THSavedSearchQueriesReplacersCount)
                    {
                        comboBox.Items.RemoveAt(Properties.Settings.Default.THSavedSearchQueriesReplacersCount - 1);
                    }

                    AddRestComboBoxValuesWithNew(comboBox.Items, value);
                }
                else
                {
                    if (comboBox.Items.IndexOf(value) != 0)
                    {
                        comboBox.Items.Remove(value);
                        AddRestComboBoxValuesWithNew(comboBox.Items, value);
                    }
                }
            }
            else
            {
                comboBox.Items.Add(value);
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
        DataSet _oDsResults;
        private void FindAllButton_Click(object sender, EventArgs e)
        {
            if (ProjectData.ThFilesElementsDataset != null && (SearchFindLinesWithPossibleIssuesCheckBox.Checked || SearchFormFindWhatTextBox.Text.Length > 0))
            {
                lblSearchMsg.Visible = false;
                _oDsResults = ProjectData.ThFilesElementsDataset.Clone();
                //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
                DataTable drFoundRowsTable = SearchNew(_oDsResults);

                if (drFoundRowsTable != null)
                {
                    if (drFoundRowsTable.Rows.Count > 0)
                    {
                        StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                        _oDsResults.AcceptChanges();
                        PopulateGrid(_oDsResults);

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
            }
        }

        private readonly DataTable _oDsResultsCoordinates = new DataTable();

        private DataTable SearchNew(DataSet ds)
        {
            lblSearchMsg.Visible = false;
            if (ProjectData.ThFilesElementsDataset.Tables.Count > 0)
            {
                string searchcolumn = GetSearchColumn();
                bool info = SearchInInfoCheckBox.Checked;
                string strQuery = SearchFormFindWhatTextBox.Text;
                bool found = false;
                var forSelected = SearchRangeSelectedRadioButton.Checked || SearchRangeVisibleRadioButton.Checked;
                int datatablesCount = SearchRangeTableRadioButton.Checked || forSelected ? ThFilesListBox.SelectedIndex + 1 : ProjectData.ThFilesElementsDataset.Tables.Count;
                int startTableIndex = SearchRangeTableRadioButton.Checked || forSelected ? ThFilesListBox.SelectedIndex : 0;

                for (int t = startTableIndex; t < datatablesCount; t++)
                {
                    var table = ProjectData.ThFilesElementsDataset.Tables[t];

                    System.Collections.Generic.HashSet<int> selectedrowsHashes = null;
                    if (forSelected)
                    {
                        selectedrowsHashes = FunctionsTable.GetDgvRowsIndexesHashesInDt(t, SearchRangeVisibleRadioButton.Checked);
                    }

                    var rowsCount = table.Rows.Count;
                    for (int r = 0; r < rowsCount; r++)
                    {
                        if (forSelected && !selectedrowsHashes.Contains(r))
                        {
                            continue;
                        }

                        var row = ProjectData.ThFilesElementsDataset.Tables[t].Rows[r];

                        //skip equal lines if need, skip empty search cells && not skip when row issue search
                        if ((chkbxDoNotTouchEqualOT.Checked && Equals(row[0], row[1])) || (!chkbxDoNotTouchEqualOT.Checked && (row[searchcolumn] + string.Empty).Length == 0 && !SearchFindLinesWithPossibleIssuesCheckBox.Checked))
                        {
                            continue;
                        }

                        string selectedCellValue = ProjectData.ThFilesElementsDataset.Tables[t].Rows[r][searchcolumn] + string.Empty;

                        if (info)//search in info box
                        {
                            var infoValue = (ProjectData.ThFilesElementsDatasetInfo.Tables[t].Rows[r][0] + string.Empty);

                            //regex search
                            if (SearchModeRegexRadioButton.Checked)//regex
                            {
                                try
                                {
                                    if ((THSearchMatchCaseCheckBox.Checked && Regex.IsMatch(infoValue, strQuery)) 
                                        || (!THSearchMatchCaseCheckBox.Checked && Regex.IsMatch(infoValue, strQuery, RegexOptions.IgnoreCase)))
                                    {
                                        ImportRowToFound(ref found, ds, row, t, r);
                                    }
                                }
                                catch (ArgumentException ex)
                                {
                                    //при ошибках регекса выходить
                                    lblSearchMsg.Visible = true;
                                    lblSearchMsg.Text = T._("Invalid regex") + ">" + ex.Message;
                                    return null;
                                }
                                catch
                                {
                                    return null;
                                }
                            }
                            else//common text search
                            {
                                try
                                {
                                    if ((THSearchMatchCaseCheckBox.Checked && infoValue.Contains(strQuery)) 
                                        || (!THSearchMatchCaseCheckBox.Checked && infoValue.ToUpperInvariant().Contains(strQuery.ToUpperInvariant())))
                                    {
                                        ImportRowToFound(ref found, ds, row, t, r);
                                    }
                                }
                                catch { }
                            }

                        }
                        else if (SearchFindLinesWithPossibleIssuesCheckBox.Checked)//search rows with possible issues
                        {
                            if (IsTheRowHasPossibleIssues(row))
                            {
                                ImportRowToFound(ref found, ds, row, t, r);
                            }
                        }
                        else
                        {
                            //regex search
                            if (SearchModeRegexRadioButton.Checked)//regex
                            {
                                try
                                {
                                    if ((THSearchMatchCaseCheckBox.Checked && Regex.IsMatch(selectedCellValue, strQuery))
                                        || (!THSearchMatchCaseCheckBox.Checked && Regex.IsMatch(selectedCellValue, strQuery, RegexOptions.IgnoreCase))
                                        )
                                    {
                                        ImportRowToFound(ref found, ds, row, t, r);
                                    }
                                }
                                catch (ArgumentException ex)
                                {
                                    //при ошибках регекса выходить
                                    lblSearchMsg.Visible = true;
                                    lblSearchMsg.Text = T._("Invalid regex") + ">" + ex.Message;
                                    return null;
                                }
                                catch
                                {
                                    return null;
                                }
                            }
                            else//common text search
                            {
                                try
                                {
                                    if ((THSearchMatchCaseCheckBox.Checked && selectedCellValue.Contains(strQuery))
                                        || (!THSearchMatchCaseCheckBox.Checked && selectedCellValue.ToUpperInvariant().Contains(strQuery.ToUpperInvariant()))
                                        )
                                    {
                                        ImportRowToFound(ref found, ds, row, t, r);
                                    }
                                }
                                catch { }

                            }

                        }
                    }
                }
            }
            return ds.Tables[0];
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
                _oDsResultsCoordinates.Rows.Clear();
                this.Height = 368;
            }
            ds.Tables[0].ImportRow(row);
            _oDsResultsCoordinates.Rows.Add(t, r);
        }

        DataTable _actors;
        private bool IsTheRowHasPossibleIssues(DataRow row)
        {
            var rowTranslation = (row[1] + string.Empty);
            if (rowTranslation.Length == 0 || (Properties.Settings.Default.IgnoreOrigEqualTransLines && Equals(row[0], row[1])))
            {
                return false;
            }

            if (Properties.Settings.Default.SearchRowIssueOptionsCheckNonRomaji)
            {
                //translation contains non romaji symbols
                if (Regex.IsMatch(row[1] + "", @"[^\x00-\x7F]+\ *(?:[^\x00-\x7F]| )*"))
                {
                    return true;
                }
            }

            //------------------------------
            //если длина любой из строк длиннее лимита
            //if ((FunctionsString.GetLongestLineLength(rowTranslation) > Properties.Settings.Default.THOptionLineCharLimit))
            //{
            //    return true;
            //}

            //------------------------------

            if (Properties.Settings.Default.SearchRowIssueOptionsCheckProjectSpecific)
            {
                //project specific checks
                if (ProjectData.CurrentProject.CheckForRowIssue(row))
                {
                    return true;
                }
            }

            if (Properties.Settings.Default.SearchRowIssueOptionsCheckActors)
            {
                //------------------------------
                //Проверка актеров
                if (_actors == null)
                {
                    GetActorsTable();
                }
                if (_actors == null || _actors.Rows.Count == 0)
                {
                    return false;
                }

                foreach (DataRow actorsLine in _actors.Rows)
                {
                    var original = actorsLine[0] as string;
                    if (original.IsMultiline() || original.Length > 255)//skip multiline and long rows
                    {
                        continue;
                    }
                    var translation = actorsLine[1] + string.Empty;

                    //если оригинал содержит оригинал(Анна) из Actors, а перевод не содержит определение(Anna) из Actors
                    if (translation.Length > 0 && (original.Length < 80 && (row[0] as string).Contains(original) && !rowTranslation.Contains(translation)))
                    {
                        return true;
                    }
                }
                //--------------------------------
            }

            if (Properties.Settings.Default.SearchRowIssueOptionsCheckAnyLineTranslatable)
            {
                //check if multiline and one of line equal to original and valide for translation
                if (row.HasAnyTranslationLineValidAndEqualSameOrigLine())
                {
                    return true;
                }
            }

            return false;
        }

        private void GetActorsTable()
        {
            foreach (DataTable table in ProjectData.ThFilesElementsDataset.Tables)
            {
                if (table.TableName.StartsWith("Actors"))
                {
                    _actors = table;
                    break;
                }
            }
        }

        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = 368;
            _tableindex = ThFilesListBox.SelectedIndex;
            _oDsResultsCoordinates.Columns.Add("t");
            _oDsResultsCoordinates.Columns.Add("r");

            LoadSearchQueriesReplacers();

            //set default values for search settings
            chkbxDoNotTouchEqualOT.Checked = TranslationHelper.Properties.Settings.Default.IgnoreOrigEqualTransLines;
        }

        private void SelectTextinTextBox(string input)
        {
            try
            {
                _ = Invoke(new MethodInvoker(() =>
                {
                    Thread.Sleep(200);
                    if (_thTargetRichTextBox.Text.Length == 0)
                    {
                        //MessageBox.Show("THTargetRichTextBox.Text="+ THTargetRichTextBox.Text);
                        return;
                    }

                    if (SearchModeRegexRadioButton.Checked)
                    {
                        //https://www.c-sharpcorner.com/article/search-and-highlight-text-in-rich-textbox/
                        //распознает лучше, чем код ниже, но не выделяет слово TEST
                        bool matchCase = THSearchMatchCaseCheckBox.Checked;
                        MatchCollection mc = Regex.Matches(_thTargetRichTextBox.Text, SearchFormFindWhatTextBox.Text, matchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
                        if (mc.Count > 0)
                        {
                            string m = mc[0].Value;
                            foreach (Match word in mc)
                            {
                                //string word = SearchFormFindWhatTextBox.Text;
                                int startindex = 0;
                                while (startindex < _thTargetRichTextBox.TextLength)
                                {
                                    int wordstartIndex;
                                    if (matchCase)
                                    {
                                        wordstartIndex = _thTargetRichTextBox.Find(word.Value, startindex, RichTextBoxFinds.MatchCase);
                                    }
                                    else
                                    {
                                        wordstartIndex = _thTargetRichTextBox.Find(word.Value, startindex, RichTextBoxFinds.None);
                                    }
                                    if (wordstartIndex == -1)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        _thTargetRichTextBox.SelectionStart = wordstartIndex;
                                        _thTargetRichTextBox.SelectionLength = word.Length;
                                        _thTargetRichTextBox.SelectionBackColor = Color.Yellow;
                                    }

                                    startindex += wordstartIndex + word.Length;
                                }
                            }
                        }
                    }
                    else
                    {
                        string word = SearchFormFindWhatTextBox.Text;
                        int startindex = 0;
                        while (startindex < _thTargetRichTextBox.TextLength)
                        {
                            int wordstartIndex;
                            if (THSearchMatchCaseCheckBox.Checked)
                            {
                                wordstartIndex = _thTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.MatchCase);
                            }
                            else
                            {
                                wordstartIndex = _thTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.None);
                            }
                            if (wordstartIndex == -1)
                            {
                                break;
                            }
                            else
                            {
                                _thTargetRichTextBox.SelectionStart = wordstartIndex;
                                _thTargetRichTextBox.SelectionLength = word.Length;
                                _thTargetRichTextBox.SelectionBackColor = Color.Yellow;
                            }

                            startindex += wordstartIndex + word.Length;
                        }
                    }

                }));
            }
            catch (Exception ex)
            {
                new FunctionsLogs().LogToFile("SelectTextinTextBox. error occured:" + Environment.NewLine + ex);
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
                _tableindex = int.Parse(_oDsResultsCoordinates.Rows[e.RowIndex][0].ToString(), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\r\n" + ex + "e.RowIndex=" + e.RowIndex + "\r\noDsResultsCoordinates.Rows count=" + _oDsResultsCoordinates.Rows.Count);
            }

            ProjectData.ThFilesElementsDataset.Tables[_tableindex].DefaultView.RowFilter = string.Empty;
            ProjectData.ThFilesElementsDataset.Tables[_tableindex].DefaultView.Sort = string.Empty;
            ThFileElementsDataGridView.Refresh();

            _rowindex = int.Parse(_oDsResultsCoordinates.Rows[e.RowIndex][1].ToString(), CultureInfo.InvariantCulture);
            FunctionsTable.ShowSelectedRow(_tableindex, searchcolumn, _rowindex);

            if (ThFileElementsDataGridView != null && ThFileElementsDataGridView.DataSource != null && ThFileElementsDataGridView.CurrentCell != null)
            {
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(ThFileElementsDataGridView.CurrentCell.Value.ToString())));
                selectstring.Start();
            }
        }

        private void SearchFormFindWhatComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Text = SearchFormFindWhatComboBox.SelectedItem.ToString();
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || ProjectData.ThFilesElementsDataset == null)
            {
                return;
            }

            bool inputqualwithlatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == _lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, _lastfoundvalue, true, CultureInfo.InvariantCulture) == 0;
            if (inputqualwithlatest)
            {
                string searchcolumn = GetSearchColumn();

                if (_startrowsearchindex == 0)
                {
                    return;
                }

                if (_tableindex >= 0 && _tableindex < ThFilesListBox.Items.Count && _rowindex >= 0 && _rowindex < ThFileElementsDataGridView.Rows.Count)
                {
                    string value = ThFileElementsDataGridView[searchcolumn, _rowindex].Value + string.Empty;
                    if (value.Length > 0)
                    {
                        if (SearchModeRegexRadioButton.Checked)
                        {
                            if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                            {
                                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                                ThFileElementsDataGridView["Translation", _rowindex].Value = Regex.Replace(GetFirstIfNotEmpty(ThFileElementsDataGridView["Translation", _rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, FixRegexReplacementFromTextbox(SearchFormReplaceWithTextBox.Text), RegexOptions.IgnoreCase);
                            }
                        }
                        else
                        {
                            if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                            {
                                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                                ThFileElementsDataGridView["Translation", _rowindex].Value = ReplaceEx.Replace(GetFirstIfNotEmpty(ThFileElementsDataGridView["Translation", _rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                            }
                        }
                    }
                }

                if (_startrowsearchindex == _oDsResults.Tables[0].Rows.Count)
                {
                    _startrowsearchindex = 0;
                }

                _tableindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                _rowindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);


                if (_tableindex != ThFilesListBox.SelectedIndex)
                {
                    ThFilesListBox.SelectedIndex = _tableindex;
                    ThFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[_tableindex];
                }
                ThFileElementsDataGridView.CurrentCell = ThFileElementsDataGridView[searchcolumn, _rowindex];

                //подсвечивание найденного
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                selectstring.Start();

                _startrowsearchindex++;
            }
            else
            {
                _startrowsearchindex = 0;
                lblSearchMsg.Visible = false;
                _oDsResults = ProjectData.ThFilesElementsDataset.Clone();

                DataTable drFoundRowsTable = SearchNew(_oDsResults);

                if (drFoundRowsTable == null)
                {
                    return;
                }

                if (drFoundRowsTable.Rows.Count == 0)
                {
                    //PopulateGrid(null);
                    lblSearchMsg.Visible = true;
                    lblSearchMsg.Text = T._("Nothing Found.");
                    this.Height = 368;
                    return;
                }

                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);

                _oDsResults.AcceptChanges();

                string searchcolumn = GetSearchColumn();
                _tableindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                _rowindex = int.Parse(_oDsResultsCoordinates.Rows[_startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);

                if (_tableindex != ThFilesListBox.SelectedIndex)
                {
                    ThFilesListBox.SelectedIndex = _tableindex;
                    ThFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[_tableindex];
                }
                ThFileElementsDataGridView.CurrentCell = ThFileElementsDataGridView[searchcolumn, _rowindex];

                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                selectstring.Start();

                _startrowsearchindex++;
                if (_startrowsearchindex == _oDsResults.Tables[0].Rows.Count)
                {
                    _startrowsearchindex = 0;
                }
            }
        }

        private static string GetFirstIfNotEmpty(string first, string defaultvalue)
        {
            return first.Length == 0 ? defaultvalue : first;
        }

        private void SearchFormReplaceAllButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || ProjectData.ThFilesElementsDataset == null)
            {
                return;
            }

            if (ConfirmReplaceAllCheckBox.Checked && !FunctionsMessage.ShowConfirmationDialog(T._("Replace All") + "?", T._("Confirmation")))
            {
                return;
            }

            var replacementUnescaped = FixRegexReplacementFromTextbox(SearchFormReplaceWithTextBox.Text);

            lblSearchMsg.Visible = false;
            _oDsResults = ProjectData.ThFilesElementsDataset.Clone();
            DataTable drFoundRowsTable = SearchNew(_oDsResults);

            if (drFoundRowsTable == null)
            {
                return;
            }

            if (drFoundRowsTable.Rows.Count == 0)
            {
                //PopulateGrid(null);
                lblSearchMsg.Visible = true;
                lblSearchMsg.Text = T._("Nothing Found");
                this.Height = 368;
                return;
            }

            bool storeQueryAndReplacer = false;

            _oDsResults.AcceptChanges();
            PopulateGrid(_oDsResults);

            lblSearchMsg.Visible = true;
            lblSearchMsg.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
            this.Height = 589;

            string searchcolumn = GetSearchColumn();
            int oDsResultsCount = _oDsResults.Tables[0].Rows.Count;
            for (int r = 0; r < oDsResultsCount; r++)
            {
                _tableindex = int.Parse(_oDsResultsCoordinates.Rows[r][0] + string.Empty, CultureInfo.CurrentCulture);
                _rowindex = int.Parse(_oDsResultsCoordinates.Rows[r][1] + string.Empty, CultureInfo.CurrentCulture);
                var row = ProjectData.ThFilesElementsDataset.Tables[_tableindex].Rows[_rowindex];
                string value = row[searchcolumn] + string.Empty;
                if (value.Length == 0)
                {
                    continue;
                }

                if (SearchInInfoCheckBox.Checked)
                {
                    if (replacementUnescaped == "=")
                    {
                        row[ThSettings.TranslationColumnName()] = row[ThSettings.OriginalColumnName()];
                    }
                    else if (string.IsNullOrEmpty(replacementUnescaped))
                    {
                        row[ThSettings.TranslationColumnName()] = string.Empty;
                    }
                }
                else
                {
                    if (SearchModeRegexRadioButton.Checked)
                    {
                        if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                        {
                            storeQueryAndReplacer = true;
                            row[ThSettings.TranslationColumnName()] = Regex.Replace(GetFirstIfNotEmpty(row[ThSettings.TranslationColumnName()] + string.Empty, value), SearchFormFindWhatTextBox.Text, replacementUnescaped, RegexOptions.IgnoreCase);
                        }
                    }
                    else
                    {
                        if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                        {
                            storeQueryAndReplacer = true;
                            row[ThSettings.TranslationColumnName()] = ReplaceEx.Replace(GetFirstIfNotEmpty(row[ThSettings.TranslationColumnName()] + string.Empty, value), SearchFormFindWhatTextBox.Text, replacementUnescaped, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }

            if (storeQueryAndReplacer)
            {
                StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
            }
        }

        private string FixRegexReplacementFromTextbox(string text)
        {
            if (SearchModeRegexRadioButton.Checked)
            {
                return Regex.Unescape(text);
            }

            return text;
        }

        private void THSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
            _oDsResultsCoordinates.Dispose();
        }

        private void SearchAlwaysOnTopCheckBox_Click(object sender, EventArgs e)
        {
        }

        //https://www.c-sharpcorner.com/uploadfile/kirtan007/make-form-stay-always-on-top-of-every-window/
        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private static readonly IntPtr HwndNotopmost = new IntPtr(-2);
        //private static readonly IntPtr HWND_TOP = new IntPtr(0);
        //private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SwpNosize = 0x0001;
        private const uint SwpNomove = 0x0002;
        private const uint TopmostFlags = SwpNomove | SwpNosize;

        private void SearchAlwaysOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                ExternalAdditions.NativeMethods.SetWindowPos(this.Handle, HwndTopmost, 0, 0, 0, 0, TopmostFlags);
            }
            else
            {
                ExternalAdditions.NativeMethods.SetWindowPos(this.Handle, HwndNotopmost, 0, 0, 0, 0, TopmostFlags);
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
