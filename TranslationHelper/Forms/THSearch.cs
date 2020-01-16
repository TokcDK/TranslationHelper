using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.ExternalAdditions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper
{
    public partial class THSearch : Form
    {
        //private readonly THMain Main;
        internal ListBox THFilesListBox;
        THDataWork thDataWork;
        internal DataGridView THFileElementsDataGridView;
        readonly RichTextBox THTargetRichTextBox;

        public THSearch(THDataWork thDataWork, ListBox listBox, DataGridView DGV, RichTextBox TTB)
        {
            InitializeComponent();
            //Main = MainForm;
            THFilesListBox = listBox;
            this.thDataWork = thDataWork;
            THFileElementsDataGridView = DGV;
            THTargetRichTextBox = TTB;

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

            if (SearchAlwaysOnTopCheckBox.Checked)
            {
                NativeMethods.SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
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
                    if (thDataWork.Main.THFileElementsDataGridView.EditingControl is TextBox)
                    {
                        var textBox = (TextBox)thDataWork.Main.THFileElementsDataGridView.EditingControl;
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
            SearchRangeAllRadioButton.Checked = false;
            SearchRangeTableRadioButton.Checked = true;
        }

        private void SearchRangeAllRadioButton_Click(object sender, EventArgs e)
        {
            SearchRangeTableRadioButton.Checked = false;
            SearchRangeAllRadioButton.Checked = true;
        }

        private string GetSearchColumn()
        {
            return SearchMethodTranslationRadioButton.Checked ? "Translation" : "Original";
        }

        int startrowsearchindex = 0;        //Индекс стартовой ячейки для поиска
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
                    tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.GetCultureInfo("en-US"));
                    rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.GetCultureInfo("en-US"));

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
                    lblError.Visible = false;
                    oDsResults = thDataWork.THFilesElementsDataset.Clone();
                    //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
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
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.GetCultureInfo("en-US"));
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.GetCultureInfo("en-US"));

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
                            //PopulateGrid(oDsResults);

                            //lblError.Visible = true;
                            //lblError.Text = "Found " + drFoundRowsTable.Rows.Count + " records";
                            //this.Height = 589;
                        }
                        else
                        {
                            //PopulateGrid(null);
                            lblError.Visible = true;
                            lblError.Text = "Nothing Found.";
                            this.Height = 368;
                        }
                    }
                }

            }
        }

        string lastfoundvalue = string.Empty;
        private void StoryFoundValueToComboBox(string foundvalue)
        {
            lastfoundvalue = foundvalue;
            for (int i = 0; i < SearchFormFindWhatComboBox.Items.Count; i++)
            {
                if (SearchFormFindWhatComboBox.Items[i].ToString() == foundvalue)
                {
                    return;
                }
            }
            SearchFormFindWhatComboBox.Items.Add(foundvalue);
        }

        private bool IsContainsText(string searchobject)
        {
            if (THSearchMatchCaseCheckBox.Checked)
            {
                return searchobject.Contains(SearchFormFindWhatTextBox.Text);
            }
            else
            {
                return searchobject.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant());
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
                lblError.Visible = false;
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

                        lblError.Visible = true;
                        lblError.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
                        this.Height = 589;


                    }
                    else
                    {
                        //PopulateGrid(null);
                        lblError.Visible = true;
                        lblError.Text = T._("Nothing Found");
                        this.Height = 368;
                    }
                }
            }
        }

        private readonly DataTable oDsResultsCoordinates = new DataTable();

        private DataTable SearchNew(DataSet DS)
        {
            if (thDataWork.THFilesElementsDataset.Tables.Count > 0)
            {
                string searchcolumn = GetSearchColumn();
                string strQuery = SearchFormFindWhatTextBox.Text;
                oDsResultsCoordinates.Rows.Clear();
                int DatatablesCount = SearchRangeTableRadioButton.Checked ? THFilesListBox.SelectedIndex + 1 : thDataWork.THFilesElementsDataset.Tables.Count;
                int StartTableIndex = SearchRangeTableRadioButton.Checked ? THFilesListBox.SelectedIndex : 0;
                for (int t = StartTableIndex; t < DatatablesCount; t++)
                {
                    for (int r = 0; r < thDataWork.THFilesElementsDataset.Tables[t].Rows.Count; r++)
                    {
                        if (((thDataWork.THFilesElementsDataset.Tables[t].Rows[r][searchcolumn] + string.Empty).Length > 0) || SearchFindLinesWithPossibleIssuesCheckBox.Checked)
                        {
                            DataRow Row = thDataWork.THFilesElementsDataset.Tables[t].Rows[r];
                            string SelectedCellValue = thDataWork.THFilesElementsDataset.Tables[t].Rows[r][searchcolumn] + string.Empty;

                            if (SearchFindLinesWithPossibleIssuesCheckBox.Checked)
                            {
                                if (IsTheRowHasPossibleIssues(Row))
                                {
                                    DS.Tables[0].ImportRow(Row);
                                    oDsResultsCoordinates.Rows.Add(t, r);
                                }
                            }
                            else
                            {
                                if (SearchModeRegexRadioButton.Checked)
                                {
                                    try
                                    {
                                        if (THSearchMatchCaseCheckBox.Checked)
                                        {
                                            if (Regex.IsMatch(SelectedCellValue, strQuery))
                                            {
                                                DS.Tables[0].ImportRow(Row);
                                                oDsResultsCoordinates.Rows.Add(t, r);
                                            }
                                        }
                                        else
                                        {
                                            if (Regex.IsMatch(SelectedCellValue, strQuery, RegexOptions.IgnoreCase))
                                            {
                                                DS.Tables[0].ImportRow(Row);
                                                oDsResultsCoordinates.Rows.Add(t, r);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        return null;//при ошибках регекса выходить
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        if (THSearchMatchCaseCheckBox.Checked)
                                        {
                                            if (SelectedCellValue.Contains(strQuery))
                                            {
                                                DS.Tables[0].ImportRow(Row);
                                                oDsResultsCoordinates.Rows.Add(t, r);
                                            }
                                        }
                                        else
                                        {
                                            if (SelectedCellValue.ToUpperInvariant().Contains(strQuery.ToUpperInvariant()))
                                            {
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
            }
            return DS.Tables[0];
        }

        DataTable Actors;
        private bool IsTheRowHasPossibleIssues(DataRow row)
        {
            string rowTranslation = (row[1] + string.Empty);
            if (rowTranslation.Length == 0)
            {
                return false;
            }

            //если длина любой из строк длиннее лимита
            if ((FunctionsString.GetLongestLineLength(rowTranslation) > Properties.Settings.Default.THOptionLineCharLimit))
            {
                return true;
            }

            if (Actors == null)
            {
                GetActorsTable();
            }

            foreach (DataRow ActorsLine in Actors.Rows)
            {
                string original = ActorsLine[0] as string;
                string translation = ActorsLine[1] + string.Empty;
                //если оригинал содержит оригинал(Анна) из Actors, а перевод не содержит определение(Anna) из Actors
                if (translation.Length > 0 && (original.Length<80 && (row[0] as string).Contains(original) && !rowTranslation.Contains(translation)))
                {
                    return true;
                }
            }

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

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://patreon.com/TranslationHelper");
        }

        //private readonly DataTable SearchFormFindWhatComboBoxCustomeSource = new DataTable();
        private void THSearch_Load(object sender, EventArgs e)
        {
            //some other info: https://stackoverflow.com/questions/20893725/how-to-hide-and-show-panels-on-a-form-and-have-it-resize-to-take-up-slack
            this.Height = 368;
            tableindex = THFilesListBox.SelectedIndex;
            oDsResultsCoordinates.Columns.Add("t");
            oDsResultsCoordinates.Columns.Add("r");
            //SearchFormFindWhatComboBox.AutoCompleteCustomSource = SearchFormFindWhatComboBoxCustomeSource.Rows.
        }

        private void SelectTextinTextBox(string input)
        {

            _ = Invoke(new MethodInvoker(delegate ()
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
                        //string[] words = SearchFormFindWhatTextBox.Text.Split(' ');
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
                                    //int wordstartIndex = THTargetRichTextBox.Find(word, startindex, RichTextBoxFinds.None);
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

        private void SearchResultsDatagridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string searchcolumn = GetSearchColumn();
            tableindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][0].ToString(), CultureInfo.GetCultureInfo("en-US"));

            thDataWork.THFilesElementsDataset.Tables[tableindex].DefaultView.RowFilter = string.Empty;
            thDataWork.THFilesElementsDataset.Tables[tableindex].DefaultView.Sort = string.Empty;
            THFileElementsDataGridView.Refresh();

            rowindex = int.Parse(oDsResultsCoordinates.Rows[e.RowIndex][1].ToString(), CultureInfo.GetCultureInfo("en-US"));


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
            Thread selectstring = new Thread(new ParameterizedThreadStart((obj) => SelectTextinTextBox(THFileElementsDataGridView.CurrentCell.Value.ToString())));
            selectstring.Start();
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
                                        THFileElementsDataGridView["Translation", rowindex].Value = Regex.Replace(GetFirstIfNotEmpty(THFileElementsDataGridView["Translation", rowindex].Value + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, RegexOptions.IgnoreCase);
                                    }
                                }
                                else
                                {
                                    if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                                    {
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

                    tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.GetCultureInfo("en-US"));
                    rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.GetCultureInfo("en-US"));


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
                    lblError.Visible = false;
                    oDsResults = thDataWork.THFilesElementsDataset.Clone();
                    //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
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
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][0].ToString(), CultureInfo.GetCultureInfo("en-US"));
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[startrowsearchindex][1].ToString(), CultureInfo.GetCultureInfo("en-US"));

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
                            //PopulateGrid(oDsResults);

                            //lblError.Visible = true;
                            //lblError.Text = "Found " + drFoundRowsTable.Rows.Count + " records";
                            //this.Height = 589;
                        }
                        else
                        {
                            //PopulateGrid(null);
                            lblError.Visible = true;
                            lblError.Text = "Nothing Found.";
                            this.Height = 368;
                        }
                    }
                }

            }
        }

        private string GetFirstIfNotEmpty(string first, string defaultvalue)
        {
            return first.Length == 0 ? defaultvalue : first;
        }

        private void SearchFormReplaceAllButton_Click(object sender, EventArgs e)
        {
            if (SearchFormFindWhatTextBox.Text.Length == 0 || thDataWork.THFilesElementsDataset == null)
            {
            }
            else
            {
                lblError.Visible = false;
                oDsResults = thDataWork.THFilesElementsDataset.Clone();
                //DataTable drFoundRowsTable = SelectFromDatatables(oDsResults);
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
                        PopulateGrid(oDsResults);

                        lblError.Visible = true;
                        lblError.Text = T._("Found ") + drFoundRowsTable.Rows.Count + T._(" records");
                        this.Height = 589;

                        string searchcolumn = GetSearchColumn();
                        for (int r = 0; r < oDsResults.Tables[0].Rows.Count; r++)
                        {
                            tableindex = int.Parse(oDsResultsCoordinates.Rows[r][0] + string.Empty, CultureInfo.CurrentCulture);
                            rowindex = int.Parse(oDsResultsCoordinates.Rows[r][1] + string.Empty, CultureInfo.CurrentCulture);

                            string value = thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[rowindex][searchcolumn] + string.Empty;
                            if (value.Length > 0)
                            {
                                if (SearchModeRegexRadioButton.Checked)
                                {
                                    if (Regex.IsMatch(value, SearchFormFindWhatTextBox.Text, RegexOptions.IgnoreCase))
                                    {
                                        thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] = Regex.Replace(GetFirstIfNotEmpty(thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, RegexOptions.IgnoreCase);
                                    }
                                }
                                else
                                {
                                    if (value.ToUpperInvariant().Contains(SearchFormFindWhatTextBox.Text.ToUpperInvariant()))
                                    {
                                        thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] = ReplaceEx.Replace(GetFirstIfNotEmpty(thDataWork.THFilesElementsDataset.Tables[tableindex].Rows[rowindex]["Translation"] + string.Empty, value), SearchFormFindWhatTextBox.Text, SearchFormReplaceWithTextBox.Text, StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //PopulateGrid(null);
                        lblError.Visible = true;
                        lblError.Text = T._("Nothing Found");
                        this.Height = 368;
                    }
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
                NativeMethods.SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
            else
            {
                NativeMethods.SetWindowPos(this.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

        private void THSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            //oDsResults.Dispose();
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
    }
}
