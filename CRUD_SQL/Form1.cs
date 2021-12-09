using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD_SQL
{
    public partial class Form1 : Form
    {
        public string dbName { get; set; }
        public string tableName { get; set; }
        SqlConnection sqlConnection = new SqlConnection();
        public Form1()
        {
            InitializeComponent();
            Connection($@"Data Source=SQL5105.site4now.net;Initial Catalog=db_a7d3c0_opendi;UID=db_a7d3c0_opendi_admin;Password=qwerty20039");

            using (SqlCommand command = new SqlCommand(@"SELECT name FROM sys.databases;", sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object name = reader.GetValue(0);
                        this.treeView1.Nodes.Add(name.ToString());
                    }
                }
            }

        }
        private void Connection(string connectionstr)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(connectionstr);
            sqlConnection.Open();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dbName = string.Empty;
            tableName = string.Empty;
            if ((sender as TreeView).SelectedNode.Parent != null &&
            (sender as TreeView).SelectedNode.GetType() == typeof(TreeNode))
            {
                dbName = (sender as TreeView).SelectedNode.Parent.Text;
                tableName = (sender as TreeView).SelectedNode.Text;
            }
            else
            {
                dbName = (sender as TreeView).SelectedNode.Text;
            }
            if (isDatabaseExists(dbName))
            {

                (sender as TreeView).SelectedNode.Nodes.Clear();
                foreach (var item in Select(dbName, tableName))
                {
                    (sender as TreeView).SelectedNode.Nodes.Add(item);
                }
            }

        }
        private string Command(string name)
        {
            if (isDatabaseExists(name))
            {
                return $@"SELECT name FROM [{name}].sys.tables;";
            }
            else
            {
                return $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{name}';";
            }
        }
        private List<string> Select(string dbname, string tableName)
        {
            List<string> list = new List<string>();
            string commandstring = string.Empty;
            try
            {
                if (tableName != string.Empty)
                    commandstring = Command(tableName);
                else
                    commandstring = Command(dbname);

                if (commandstring != string.Empty)
                {
                    using (SqlCommand command = new SqlCommand(commandstring, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                object names = reader.GetValue(0);
                                list.Add(names.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return list;
        }
        private bool isDatabaseExists(string name)
        {
            using (SqlCommand command = new SqlCommand($@"if Exists(select 1 from master.dbo.sysdatabases where name='{name}') 
                       select 1 else select 0", sqlConnection))
            {
                int exists = Convert.ToInt32(command.ExecuteScalar());

                if (exists > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
