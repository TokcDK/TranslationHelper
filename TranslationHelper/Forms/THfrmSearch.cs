using AIHelper.Manage;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class THfrmSearch : Form
    {
        internal ListBox THFilesListBox;
        readonly THDataWork thDataWork;
        internal DataGridView THFileElementsDataGridView;
        readonly RichTextBox THTargetRichTextBox;
        readonly INIFile Config;

        internal THfrmSearch(THDataWork thDataWork, ListBox listBox, DataGridView DGV, RichTextBox TTB)
        {
            InitializeComponent();
            //Main = MainForm;
            THFilesListBox = listBox;
            this.thDataWork = thDataWork;
            THFileElementsDataGridView = DGV;
            THTargetRichTextBox = TTB;

            Config = thDataWork.Main.Settings.THConfigINI;

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
            if (thDataWork != null)
            {
                if (thDataWork.Main.THFileElementsDataGridView.CurrentCell.IsInEditMode)
                {
                    //https://stackoverflow.com/questions/41380883/retrieve-partly-selected-text-when-the-cell-is-in-edit-mode-in-datagridview
                    if (thDataWork.Main.THFileElementsDataGridView.EditingControl is TextBox textBox)
                    {
                        SearchFormFindWhatTextBox.Text = textBox.SelectedText;
                    }
                }
                else if (thDataWork.Main.THSourceRichTextBox.SelectedText.Length > 0)
                {
                    SearchFormFindWhatTextBox.Text = thDataWork.Main.THSourceRichTextBox.SelectedText;
                }
                else if (thDataWork.Main.THTargetRichTextBox.SelectedText.Length > 0)
                {
                    SearchFormFindWhatTextBox.Text = thDataWork.Main.THTargetRichTextBox.SelectedText;
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

        int startrowsearchindex;        //Индекс стартовой ячейки для поиска
        int tableindex;
        int rowindex;
        private void SearchFormFindNextButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || thDataWork.THFilesElementsDataset == null)
            {
            }
            else
            {
                bool inputqualwithlatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, lastfoundvalue, true, CultureInfo.InvariantCulture) == 0;
                if (inputqualwithlatest)
                {
                    string searchcolumn = GetSearchColumn();
                    tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                    rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);

                    if (tableindex == THFilesListBox.SelectedIndex)
                    {
                    }
                    else
                    {
                        THFilesListBox.SelectedIndex = tableindex;
                        THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[tableindex];
                    }
                    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                    //подсвечивание найденного
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                    selectstring.Start();

                    startrowsearchindex++;
                    if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                    {
                        startrowsearchindex = 0;
                    }

                }
                else
                {
                    startrowsearchindex = 0;
                    lblSearchMsg.Visible = false;
                    oDsResults = thDataWork.THFilesElementsDataset.Clone();

                    DataTable drFoundRowsTable = SearchNew(oDsResults);

                    if (drFoundRowsTable == null)
                    {
                    }
                    else
                    {
                        if (drFoundRowsTable.Rows.Count > 0)
                        {
                            StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text);

                            oDsResults.AcceptChanges();

                            string searchcolumn = GetSearchColumn();
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);

                            if (tableindex == THFilesListBox.SelectedIndex)
                            {
                            }
                            else
                            {
                                THFilesListBox.SelectedIndex = tableindex;
                                THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[tableindex];
                            }
                            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                            selectstring.Start();

                            startrowsearchindex++;
                            if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                            {
                                startrowsearchindex = 0;
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
                SearchQueries = Config.ReadSectionValuesToArray("Search Queries");
                if (SearchQueries != null && SearchQueries.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref SearchQueries);
                    SearchFormFindWhatComboBox.Items.Clear();
                    SearchFormFindWhatComboBox.Items.AddRange(SearchQueries);
                }
                SearchReplacers = Config.ReadSectionValuesToArray("Search Replacers");
                if (SearchReplacers != null && SearchReplacers.Length > 0)
                {
                    RemoveQuotesFromLoadedSearchValues(ref SearchReplacers);
                    SearchFormReplaceWithComboBox.Items.Clear();
                    SearchFormReplaceWithComboBox.Items.AddRange(SearchReplacers);
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
                if (SearchFormFindWhatComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(SearchQueries, SearchFormFindWhatComboBox.Items))
                {
                    SearchQueries = new string[SearchFormFindWhatComboBox.Items.Count];
                    SearchFormFindWhatComboBox.Items.CopyTo(SearchQueries, 0);
                    AddQuotesToWritingSearchValues(ref SearchQueries);
                    Config.WriteArrayToSectionValues("Search Queries", SearchQueries);
                }
                if (SearchFormReplaceWithComboBox.Items.Count > 0 && IsSearchQueriesReplacersListChanged(SearchReplacers, SearchFormReplaceWithComboBox.Items))
                {
                    SearchReplacers = new string[SearchFormReplaceWithComboBox.Items.Count];
                    SearchFormReplaceWithComboBox.Items.CopyTo(SearchReplacers, 0);
                    AddQuotesToWritingSearchValues(ref SearchReplacers);
                    Config.WriteArrayToSectionValues("Search Replacers", SearchReplacers);
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
                    if (ItemsCount == Properties.Settings.Default.THSavedSearchQueriesReplacersCount)
                    {
                        ComboBox.Items.RemoveAt(Properties.Settings.Default.THSavedSearchQueriesReplacersCount - 1);
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
        private void FindAllButton_Click(object sender, EventArgs e)
        {
            if (thDataWork.THFilesElementsDataset != null && (SearchFindLinesWithPossibleIssuesCheckBox.Checked || SearchFormFindWhatTextBox.Text.Length > 0))
            {
                lblSearchMsg.Visible = false;
                oDsResults = thDataWork.THFilesElementsDataset.Clone();
                //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
                DataTable drFoundRowsTable = SearchNew(oDsResults);

                if (drFoundRowsTable != null)
                {
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
            }
        }

        private readonly DataTable oDsResultsCoordinates = new DataTable();

        private DataTable SearchNew(DataSet DS)
        {
            lblSearchMsg.Visible = false;
            if (thDataWork.THFilesElementsDataset.Tables.Count > 0)
            {
                string searchcolumn = GetSearchColumn();
                bool info = SearchInInfoCheckBox.Checked;
                string strQuery = SearchFormFindWhatTextBox.Text;
                bool found = false;
                var ForSelected = SearchRangeSelectedRadioButton.Checked || SearchRangeVisibleRadioButton.Checked;
                int DatatablesCount = SearchRangeTableRadioButton.Checked || ForSelected ? THFilesListBox.SelectedIndex + 1 : thDataWork.THFilesElementsDataset.Tables.Count;
                int StartTableIndex = SearchRangeTableRadioButton.Checked || ForSelected ? THFilesListBox.SelectedIndex : 0;

                for (int t = StartTableIndex; t < DatatablesCount; t++)
                {
                    var table = thDataWork.THFilesElementsDataset.Tables[t];

                    System.Collections.Generic.HashSet<int> selectedrowsHashes = null;
                    if (ForSelected)
                    {
                        selectedrowsHashes = FunctionsTable.GetDGVRowsIndexesHashesInDT(thDataWork, t, SearchRangeVisibleRadioButton.Checked);
                    }

                    var rowsCount = table.Rows.Count;
                    for (int r = 0; r < rowsCount; r++)
                    {
                        if (ForSelected && !selectedrowsHashes.Contains(r))
                        {
                            continue;
                        }

                        var Row = thDataWork.THFilesElementsDataset.Tables[t].Rows[r];

                        //skip equal lines if need, skip empty search cells && not skip when row issue search
                        if ((chkbxDoNotTouchEqualOT.Checked && Equals(Row[0], Row[1])) || (!chkbxDoNotTouchEqualOT.Checked && (Row[searchcolumn] + string.Empty).Length == 0 && !SearchFindLinesWithPossibleIssuesCheckBox.Checked))
                        {
                            continue;
                        }

                        string SelectedCellValue = thDataWork.THFilesElementsDataset.Tables[t].Rows[r][searchcolumn] + string.Empty;

                        if (info)//search in info box
                        {
                            if ((thDataWork.THFilesElementsDatasetInfo.Tables[t].Rows[r][0] + string.Empty).Contains(strQuery))
                            {
                                if (!found)
                                {
                                    found = true;
                                    oDsResultsCoordinates.Rows.Clear();
                                    this.Height = 368;
                                }
                                DS.Tables[0].ImportRow(Row);
                                oDsResultsCoordinates.Rows.Add(t, r);
                            }

                        }
                        else if (SearchFindLinesWithPossibleIssuesCheckBox.Checked)//search rows with possible issues
                        {
                            if (IsTheRowHasPossibleIssues(Row))
                            {
                                if (!found)
                                {
                                    found = true;
                                    oDsResultsCoordinates.Rows.Clear();
                                    this.Height = 368;
                                }
                                DS.Tables[0].ImportRow(Row);
                                oDsResultsCoordinates.Rows.Add(t, r);
                            }
                        }
                        else
                        {
                            //regex search
                            if (SearchModeRegexRadioButton.Checked)//regex
                            {
                                try
                                {
                                    if (THSearchMatchCaseCheckBox.Checked)
                                    {
                                        if (Regex.IsMatch(SelectedCellValue, strQuery))
                                        {
                                            if (!found)
                                            {
                                                found = true;
                                                oDsResultsCoordinates.Rows.Clear();
                                                this.Height = 368;
                                            }
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                    else
                                    {
                                        if (Regex.IsMatch(SelectedCellValue, strQuery, RegexOptions.IgnoreCase))
                                        {
                                            if (!found)
                                            {
                                                found = true;
                                                oDsResultsCoordinates.Rows.Clear();
                                                this.Height = 368;
                                            }
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
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
                                    if (THSearchMatchCaseCheckBox.Checked)
                                    {
                                        if (SelectedCellValue.Contains(strQuery))
                                        {
                                            if (!found)
                                            {
                                                found = true;
                                                oDsResultsCoordinates.Rows.Clear();
                                                this.Height = 368;
                                            }
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                    else
                                    {
                                        if (SelectedCellValue.ToUpperInvariant().Contains(strQuery.ToUpperInvariant()))
                                        {
                                            if (!found)
                                            {
                                                found = true;
                                                oDsResultsCoordinates.Rows.Clear();
                                                this.Height = 368;
                                            }
                                            DS.Tables[0].ImportRow(Row);
                                            oDsResultsCoordinates.Rows.Add(t, r);
                                        }
                                    }
                                }
                                catch
                                {

                                }

                            }

                        }
                    }
                }
            }
            return DS.Tables[0];
        }

        DataTable Actors;
        private bool IsTheRowHasPossibleIssues(DataRow row)
        {
            var rowTranslation = (row[1] + string.Empty);
            if (rowTranslation.Length == 0 || Equals(row[0], row[1]))
            {
                return false;
            }

            //translation contains non romaji symbols
            if (Regex.IsMatch(row[1] + "", @"[^\x00-\x7F]+\ *(?:[^\x00-\x7F]| )*"))
            {
                return true;
            }

            //------------------------------
            //если длина любой из строк длиннее лимита
            //if ((FunctionsString.GetLongestLineLength(rowTranslation) > Properties.Settings.Default.THOptionLineCharLimit))
            //{
            //    return true;
            //}

            //------------------------------
            //project specific checks
            if (thDataWork.CurrentProject.CheckForRowIssue(row))
            {
                return true;
            }

            //------------------------------
            //Проверка актеров
            if (Actors == null)
            {
                GetActorsTable();
            }
            if (Actors == null || Actors.Rows.Count == 0)
            {
                return false;
            }

            foreach (DataRow ActorsLine in Actors.Rows)
            {
                string original = ActorsLine[0] as string;
                string translation = ActorsLine[1] + string.Empty;

                //если оригинал содержит оригинал(Анна) из Actors, а перевод не содержит определение(Anna) из Actors
                if (translation.Length > 0 && (original.Length < 80 && (row[0] as string).Contains(original) && !rowTranslation.Contains(translation)))
                {
                    return true;
                }
            }
            //--------------------------------

            return false;
        }

        private void GetActorsTable()
        {
            foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                if (table.TableName.StartsWith("Actors"))
                {
                    Actors = table;
                    break;
                }
            }
        }

        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = 368;
            tableindex = THFilesListBox.SelectedIndex;
            oDsResultsCoordinates.Columns.Add("t");
            oDsResultsCoordinates.Columns.Add("r");

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
                    if (THTargetRichTextBox.Text.Length == 0)
                    {
                        //MessageBox.Show("THTargetRichTextBox.Text="+ THTargetRichTextBox.Text);
                    }
                    else
                    {
                        if (SearchModeRegexRadioButton.Checked)
                        {
                            //https://www.c-sharpcorner.com/article/search-and-highlight-text-in-rich-textbox/
                            //распознает лучше, чем код ниже, но не выделяет слово TEST
                            bool MatchCase = THSearchMatchCaseCheckBox.Checked;
                            MatchCollection mc = Regex.Matches(THTargetRichTextBox.Text, SearchFormFindWhatTextBox.Text, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
                            if (mc.Count > 0)
                            {
                                string m = mc[0].Value;
                                foreach (Match word in mc)
                                {
                                    //string word = SearchFormFindWhatTextBox.Text;
                                    int startindex = 0;
                                    while (startindex < THTargetRichTextBox.TextLength)
                                    {
                                        int wordstartIndex;
                                        if (MatchCase)
                                        {
                                            wordstartIndex = THTargetRichTextBox.Find(word.Value, startindex, RichTextBoxFinds.MatchCase);
                                        }
                                        else
                                        {
                                            wordstartIndex = THTargetRichTextBox.Find(word.Value, startindex, RichTextBoxFinds.None);
                                        }
                                        if (wordstartIndex == -1)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            THTargetRichTextBox.SelectionStart = wordstartIndex;
                                            THTargetRichTextBox.SelectionLength = word.Length;
                                            THTargetRichTextBox.SelectionBackColor = Color.Yellow;
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
                            while (startindex < THTargetRichTextBox.TextLength)
                            {
                                int wordstartIndex;
                                if (THSearchMatchCaseCheckBox.Checked)
                                {
                                    wordstartIndex = THTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.MatchCase);
                                }
                                else
                                {
                                    wordstartIndex = THTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.None);
                                }
                                if (wordstartIndex == -1)
                                {
                                    break;
                                }
                                else
                                {
                                    THTargetRichTextBox.SelectionStart = wordstartIndex;
                                    THTargetRichTextBox.SelectionLength = word.Length;
                                    THTargetRichTextBox.SelectionBackColor = Color.Yellow;
                                }

                                startindex += wordstartIndex + word.Length;
                            }
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
                tableindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][0].ToString(), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\r\n" + ex + "e.RowIndex=" + e.RowIndex + "\r\noDsResultsCoordinates.Rows count=" + oDsResultsCoordinates.Rows.Count);
            }

            thDataWork.THFilesElementsDataset.Tables[tableindex].DefaultView.RowFilter = string.Empty;
            thDataWork.THFilesElementsDataset.Tables[tableindex].DefaultView.Sort = string.Empty;
            THFileElementsDataGridView.Refresh();

            rowindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][1].ToString(), CultureInfo.InvariantCulture);
            FunctionsTable.ShowSelectedRow(thDataWork, tableindex, searchcolumn, rowindex);

            if (THFileElementsDataGridView.DataSource != null)
            {
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(THFileElementsDataGridView.CurrentCell.Value.ToString())));
                selectstring.Start();
            }
        }

        private void SearchFormFindWhatComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            SearchFormFindWhatTextBox.Text = SearchFormFindWhatComboBox.SelectedItem.ToString();
        }

        private void SearchFormReplaceButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || SearchFormReplaceWithTextBox.Text.Length == 0 || thDataWork.THFilesElementsDataset == null)
            {
            }
            else
            {
                bool inputqualwithlatest = THSearchMatchCaseCheckBox.Checked ? SearchFormFindWhatTextBox.Text == lastfoundvalue : string.Compare(SearchFormFindWhatTextBox.Text, lastfoundvalue, true, CultureInfo.InvariantCulture) == 0;
                if (inputqualwithlatest)
                {
                    string searchcolumn = GetSearchColumn();

                    if (startrowsearchindex == 0)
                    {
                    }
                    else
                    {
                        if (tableindex >= 0 && tableindex < THFilesListBox.Items.Count && rowindex >= 0 && rowindex < THFileElementsDataGridView.Rows.Count)
                        {
                            string value = THFileElementsDataGridView[searchcolumn, rowindex].Value + string.Empty;
                            if (value.Length > 0)
                            {
                                if (SearchModeRegexRadioButton.Checked)
                                {
                                    if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                                    {
                                        StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                                        THFileElementsDataGridView["Translation", rowindex].Value = Regex.Replace(GetFirstIfNotEmpty(THFileElementsDataGridView["Translation", rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, RegexOptions.IgnoreCase);
                                    }
                                }
                                else
                                {
                                    if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                                    {
                                        StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                                        THFileElementsDataGridView["Translation", rowindex].Value = ReplaceEx.Replace(GetFirstIfNotEmpty(THFileElementsDataGridView["Translation", rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }

                    if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                    {
                        startrowsearchindex = 0;
                    }

                    tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                    rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);


                    if (tableindex == THFilesListBox.SelectedIndex)
                    {
                    }
                    else
                    {
                        THFilesListBox.SelectedIndex = tableindex;
                        THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[tableindex];
                    }
                    THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                    //подсвечивание найденного
                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                    selectstring.Start();

                    startrowsearchindex++;
                }
                else
                {
                    startrowsearchindex = 0;
                    lblSearchMsg.Visible = false;
                    oDsResults = thDataWork.THFilesElementsDataset.Clone();

                    DataTable drFoundRowsTable = SearchNew(oDsResults);

                    if (drFoundRowsTable == null)
                    {
                    }
                    else
                    {
                        if (drFoundRowsTable.Rows.Count > 0)
                        {
                            StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);

                            oDsResults.AcceptChanges();

                            string searchcolumn = GetSearchColumn();
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.InvariantCulture);
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.InvariantCulture);

                            if (tableindex == THFilesListBox.SelectedIndex)
                            {
                            }
                            else
                            {
                                THFilesListBox.SelectedIndex = tableindex;
                                THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[tableindex];
                            }
                            THFileElementsDataGridView.CurrentCell = THFileElementsDataGridView[searchcolumn, rowindex];

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(SearchFormFindWhatTextBox.Text)));
                            selectstring.Start();

                            startrowsearchindex++;
                            if (startrowsearchindex == oDsResults.Tables[0].Rows.Count)
                            {
                                startrowsearchindex = 0;
                            }
                        }
                        else
                        {
                            //PopulateGrid(null);
                            lblSearchMsg.Visible = true;
                            lblSearchMsg.Text = T._("Nothing Found.");
                            this.Height = 368;
                        }
                    }
                }

            }
        }

        private static string GetFirstIfNotEmpty(string first, string defaultvalue)
        {
            return first.Length == 0 ? defaultvalue : first;
        }

        private void SearchFormReplaceAllButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || thDataWork.THFilesElementsDataset == null)
            {
                return;
            }

            if (ConfirmReplaceAllCheckBox.Checked && !FunctionsMessage.ShowConfirmationDialog(T._("Replace All") + "?", T._("Confirmation")))
            {
                return;
            }


            lblSearchMsg.Visible = false;
            oDsResults = thDataWork.THFilesElementsDataset.Clone();
            DataTable drFoundRowsTable = SearchNew(oDsResults);

            if (drFoundRowsTable == null)
            {
            }
            else
            {
                if (drFoundRowsTable.Rows.Count > 0)
                {
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
                        tableindex = int.Parse(oDsResultsCoordinates.Rows[r][0] + string.Empty, CultureInfo.CurrentCulture);
                        rowindex = int.Parse(oDsResultsCoordinates.Rows[r][1] + string.Empty, CultureInfo.CurrentCulture);
                        var row = thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[rowindex];
                        string value = row[searchcolumn] + string.Empty;
                        if (value.Length > 0)
                        {
                            if (SearchInInfoCheckBox.Checked)
                            {
                                if (SearchFormReplaceWithTextBox.Text == "=")
                                {
                                    row[THSettingsData.TranslationColumnName()] = row[THSettingsData.OriginalColumnName()];
                                }
                                else if (string.IsNullOrEmpty(SearchFormReplaceWithTextBox.Text))
                                {
                                    row[THSettingsData.TranslationColumnName()] = string.Empty;
                                }
                            }
                            else
                            {
                                if (SearchModeRegexRadioButton.Checked)
                                {
                                    if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                                    {
                                        StoreQueryAndReplacer = true;
                                        row[THSettingsData.TranslationColumnName()] = Regex.Replace(GetFirstIfNotEmpty(row[THSettingsData.TranslationColumnName()] + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, RegexOptions.IgnoreCase);
                                    }
                                }
                                else
                                {
                                    if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                                    {
                                        StoreQueryAndReplacer = true;
                                        row[THSettingsData.TranslationColumnName()] = ReplaceEx.Replace(GetFirstIfNotEmpty(row[THSettingsData.TranslationColumnName()] + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }

                    if (StoreQueryAndReplacer)
                    {
                        StoryFoundValueToComboBox(SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text);
                    }
                }
                else
                {
                    //PopulateGrid(null);
                    lblSearchMsg.Visible = true;
                    lblSearchMsg.Text = T._("Nothing Found");
                    this.Height = 368;
                }
            }
        }

        private void THSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
            oDsResultsCoordinates.Dispose();
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
