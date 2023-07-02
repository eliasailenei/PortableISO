using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EXMLE
{
    public partial class Specialize : Form
    {
        string defaultplan = "381b4222-f694-41f0-9685-ff5bb260df2e";
        bool skipactivate;
        DataTable favoriteBarItems;
        string[] ieconfig ;
        string ico, mkey, mname, nuri;
        bool empty = true;
        Dictionary<string, DataTable> tables;




        string[] tableNames = { "acceleratorsTable", "FavoriteBarItemsTable", "FavoritesListTable", "FeedListTable", "InstalledBHOListTable", "InstalledBrowserExtensionsTable", "InstalledToolbarsListTable", "PreApprovedAddonsTable", "QuickLinkListTable", "SearchScopesTable", "StartPagesTable" };
        public Specialize()
        {
           
            InitializeComponent();
        }

        private void Specialize_Load(object sender, EventArgs e)
        {
            textBox1.Text = defaultplan;
            skipactivate = false;
            label6.Text = skipactivate.ToString();
            label6.ForeColor = System.Drawing.Color.Red;
            string[] database = { "MicrosoftEdgeBrowser", "InternetExplorer" };
            comboBox1.Items.AddRange(database);
            comboBox2.Hide();

            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //textBox1.Text = defaultplan;
            MessageBox.Show(defaultplan);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            defaultplan = textBox1.Text;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                skipactivate = true;
                label6.Text = skipactivate.ToString();
                label6.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                skipactivate = false;
                label6.Text = skipactivate.ToString();
                label6.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            makeiedata();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                comboBox2.Hide();
                favoriteBarItems = new DataTable("FavoriteBarItems");
                favoriteBarItems.Columns.Add("ItemFavIconFile", typeof(string));
                favoriteBarItems.Columns.Add("ItemKey", typeof(string));
                favoriteBarItems.Columns.Add("ItemName", typeof(string));
                favoriteBarItems.Columns.Add("ItemUrl", typeof(string));
                dataGridView1.DataSource = favoriteBarItems;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                comboBox2.Show();
                comboBox2.Items.AddRange(tableNames);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                ico = Microsoft.VisualBasic.Interaction.InputBox("Enter a value for ItemFavIconFile.", "Initiate");
                mkey = Microsoft.VisualBasic.Interaction.InputBox("Enter a value for ItemKey.", "Initiate");
                mname = Microsoft.VisualBasic.Interaction.InputBox("Enter a value for ItemName.", "Initiate");
                nuri = Microsoft.VisualBasic.Interaction.InputBox("Enter a value for ItemUrl.", "Initiate");
                if (empty)
                {

                    favoriteBarItems.Rows.Add(ico, mkey, mname, nuri);
                    empty = false;
                }
                else
                {

                    favoriteBarItems.Rows.Clear();
                    favoriteBarItems.Rows.Add(ico, mkey, mname, nuri);
                }

                dataGridView1.DataSource = favoriteBarItems;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                MessageBox.Show("entered");
                string selectedTableName = comboBox2.SelectedItem.ToString();
                MessageBox.Show(selectedTableName);
                if (tables.ContainsKey(selectedTableName))
                {
                    MessageBox.Show("entered");
                    DataTable selectedTable = tables[selectedTableName]; // Access the DataTable from the dictionary

                    foreach (DataRow row in selectedTable.Rows)
                    {
                        foreach (DataColumn column in selectedTable.Columns)
                        {
                            MessageBox.Show(row[column.ColumnName].ToString());
                            row[column.ColumnName] = Microsoft.VisualBasic.Interaction.InputBox("Enter a value for " + column.ColumnName, "Initiate");
                        }
                    }
                }
            }
        }
        private void makeiedata()
        {
            DataTable acceleratorsTable = new DataTable("Accelerators");
            acceleratorsTable.Columns.Add("Action", typeof(string));
            acceleratorsTable.Columns.Add("AcceleratorXML", typeof(string));
            acceleratorsTable.Columns.Add("IsDefault", typeof(bool));
            acceleratorsTable.Columns.Add("ItemKey", typeof(string));

            DataTable favoriteBarItemTable = new DataTable("FavoriteBarItems");
            favoriteBarItemTable.Columns.Add("Action", typeof(string));
            favoriteBarItemTable.Columns.Add("ItemKey", typeof(string));
            favoriteBarItemTable.Columns.Add("ItemName", typeof(string));
            favoriteBarItemTable.Columns.Add("ItemType", typeof(string));
            favoriteBarItemTable.Columns.Add("ItemUrl", typeof(string));

            DataTable favoritesListTable = new DataTable("FavoritesList");
            favoritesListTable.Columns.Add("Action", typeof(string));
            favoritesListTable.Columns.Add("FavIconFile", typeof(string));
            favoritesListTable.Columns.Add("FavID", typeof(string));
            favoritesListTable.Columns.Add("FavTitle", typeof(string));
            favoritesListTable.Columns.Add("FavURL", typeof(string));

            DataTable feedListTable = new DataTable("FeedList");
            feedListTable.Columns.Add("Action", typeof(string));
            feedListTable.Columns.Add("FeedKey", typeof(string));
            feedListTable.Columns.Add("FeedTitle", typeof(string));
            feedListTable.Columns.Add("FeedURL", typeof(string));

            DataTable installedBHOListTable = new DataTable("InstalledBHOList");
            installedBHOListTable.Columns.Add("Action", typeof(string));
            installedBHOListTable.Columns.Add("AddonGuid", typeof(string));

            DataTable installedBrowserExtensionsTable = new DataTable("InstalledBrowserExtensions");
            installedBrowserExtensionsTable.Columns.Add("Action", typeof(string));
            installedBrowserExtensionsTable.Columns.Add("AddonGuid", typeof(string));

            DataTable installedToolbarsListTable = new DataTable("InstalledToolbarsList");
            installedToolbarsListTable.Columns.Add("Action", typeof(string));
            installedToolbarsListTable.Columns.Add("AddonGuid", typeof(string));

            DataTable preApprovedAddonsTable = new DataTable("PreApprovedAddons");
            preApprovedAddonsTable.Columns.Add("Action", typeof(string));
            preApprovedAddonsTable.Columns.Add("AddonGuid", typeof(string));

            DataTable quickLinkListTable = new DataTable("QuickLinkList");
            quickLinkListTable.Columns.Add("Action", typeof(string));
            quickLinkListTable.Columns.Add("QLID", typeof(string));
            quickLinkListTable.Columns.Add("QuickLinkName", typeof(string));
            quickLinkListTable.Columns.Add("QuickLinkUrl", typeof(string));

            DataTable searchScopesTable = new DataTable("SearchScopes");
            searchScopesTable.Columns.Add("Action", typeof(string));
            searchScopesTable.Columns.Add("FaviconURL", typeof(string));
            searchScopesTable.Columns.Add("PreviewURL", typeof(string));
            searchScopesTable.Columns.Add("ScopeDisplayName", typeof(string));
            searchScopesTable.Columns.Add("ScopeKey", typeof(string));
            searchScopesTable.Columns.Add("ScopeUrl", typeof(string));
            searchScopesTable.Columns.Add("SuggestionsURL", typeof(string));
            searchScopesTable.Columns.Add("SuggestionsURL_JSON", typeof(string));

            DataTable startPagesTable = new DataTable("StartPages");
            startPagesTable.Columns.Add("Action", typeof(string));
            startPagesTable.Columns.Add("StartPageKey", typeof(string));
            startPagesTable.Columns.Add("StartPageUrl", typeof(string));

            tables = new Dictionary<string, DataTable> { { "acceleratorsTable", acceleratorsTable }, { "FavoriteBarItemsTable", favoriteBarItemTable }, { "FavoritesListTable", favoritesListTable }, { "FeedListTable", feedListTable }, { "InstalledBHOListTable", installedBHOListTable }, { "InstalledBrowserExtensionsTable", installedBrowserExtensionsTable }, { "InstalledToolbarsListTable", installedToolbarsListTable }, { "PreApprovedAddonsTable", preApprovedAddonsTable }, { "QuickLinkListTable", quickLinkListTable }, { "SearchScopesTable", searchScopesTable }, { "StartPagesTable", startPagesTable } };

            string selectedTableName = comboBox2.SelectedItem.ToString();

            if (tables.ContainsKey(selectedTableName))
            {
                dataGridView1.DataSource = tables[selectedTableName];
                dataGridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Invalid Table Name");
            }


          
        }
    }
}
        
    

