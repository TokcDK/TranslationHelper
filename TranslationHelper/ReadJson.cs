/*
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranslationHelper1
{
    public class ReadJson
    {
        //readonly THMain main = new THMain();

        public bool ReadTheJson(string Jsonname, string jsondata)
        {
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                MessageBox.Show(main.THSelectedDir);
                using (var reader = new StreamReader(main.THSelectedDir))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, Jsonname);
                    }
                }

                //main.THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                //main.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                main.THFilesElementsDatasetInfo.Tables.Add(Jsonname); // create table with json name
                main.THFilesElementsDatasetInfo.Tables[Jsonname].Columns.Add("Original"); //create Original column


                //treeView1.BeginUpdate();
                try
                {
                    // treeView1.Nodes.Clear();
                    //var tNode = treeView1.Nodes[treeView1.Nodes.Add(new TreeNode(rootName))];
                    //tNode.Tag = root;

                    ProceedJToken(root, Jsonname);

                    //treeView1.ExpandAll();
                }
                finally
                {
                    //LogToFile("", true);
                    //MessageBox.Show("sss");
                    //main.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                    //main.THFilesElementsDataset.Tables[Jsonname].Columns["Original"].ReadOnly = true;
                    //main.THFileElementsDataGridView.DataSource = main.THFilesElementsDataset.Tables[0];
                    //treeView1.EndUpdate();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        StringBuilder textsb = new StringBuilder();
        string curcode = "";
        string cType = "";
        string cCode = "";
        string cName = "";
        string cId = "";
        private void ProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
                return;
            if (token is JValue)
            {
                if (propertyname == "code")
                {
                    curcode = token.ToString();
                    //cCode = "Code" + curcode;
                    //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                }
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (token.Type == JTokenType.String)
                {
                    string tokenvalue = token.ToString();
                    if (curcode != "401" && curcode != "405")
                    {
                        if (string.IsNullOrEmpty(textsb.ToString()))
                        {
                        }
                        else
                        {
                            if (main.GetAlreadyAddedInTable(Jsonname, textsb.ToString()) || main.SelectedLocalePercentFromStringIsNotValid(textsb.ToString()))
                            {
                            }
                            else
                            {
                                main.THFilesElementsDataset.Tables[0].Rows.Add(textsb.ToString());
                                //dsinfo.Tables[0].Rows.Add(cType+"\\"+ cId + "\\" + cCode + "\\" + cName);
                                main.THFilesElementsDatasetInfo.Tables[0].Rows.Add("JArray:Object");
                            }
                            textsb.Clear();
                        }
                        if (string.IsNullOrEmpty(tokenvalue) || main.SelectedLocalePercentFromStringIsNotValid(tokenvalue) || main.GetAlreadyAddedInTable(Jsonname, tokenvalue))
                        {
                        }
                        else
                        {
                            if (curcode == "102")
                            {
                                cName = "Choice";
                            }
                            main.THFilesElementsDataset.Tables[0].Rows.Add(tokenvalue);
                            //dsinfo.Tables[0].Rows.Add(cType+"\\"+ cId + "\\" + cCode + "\\" + cName);
                            main.THFilesElementsDatasetInfo.Tables[0].Rows.Add(cType + ":" + cName);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(textsb.ToString()))
                        {
                        }
                        else
                        {
                            textsb.Append("\r\n");
                        }
                        textsb.Append(tokenvalue);
                    }
                }
                //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                //childNode.Tag = token;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    //LogToFile("JObject propery: " + property.Name + "=" + property.Value);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    //childNode.Tag = property;
                    cType = "JObject";
                    cName = property.Name;
                    ProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    //LogToFile("JArray=\r\n" + array[i]);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    //childNode.Tag = array[i];
                    cType = "JArray";
                    ProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }
    }
}

*/
