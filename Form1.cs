using BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator
{
    public partial class Form1 : Form
    {
        clsSettings Settings;
        public Form1()
        {
            InitializeComponent();
            Settings = new clsSettings();
            
        }

        private void LoadDataBases()
        {
            var Items = Settings.GetAllLocalDatabases();
            cbDataBase.Items.Clear();

            foreach (var item in Items)
            {
                cbDataBase.Items.Add(item);
            }

            cbDataBase.SelectedIndex = 0;
        }

        private void LoadTables()
        {
            var Items = Settings.GetAllTables(cbDataBase.SelectedItem.ToString());

            cbTables.Items.Clear();
            foreach(var item in Items)
            {
                cbTables.Items.Add(item);
            }

            cbTables.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDataBases();
        }

        private void cbDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTables();
        }
    }
}
