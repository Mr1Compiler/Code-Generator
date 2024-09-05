using BusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            cbDataBase.SelectedIndex = 6;
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

        private void button1_Click(object sender, EventArgs e)
        {
            tbBusinessLayer.Text = clsGenerateCode.FillBusinessLayer(cbDataBase.SelectedItem.ToString(), cbTables.SelectedItem.ToString());

            tbDataLayer.Text = clsGenerateCode.FillDataAccessLayer(cbDataBase.SelectedItem.ToString(), cbTables.SelectedItem.ToString());
        }

        private void btnBusinessCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbBusinessLayer.Text))
            {
                Clipboard.SetText(tbBusinessLayer.Text);
            }
            else
            {
                MessageBox.Show("The text box is empty. Nothing to copy.");
            }
        }

        private void btnDataCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbDataLayer.Text))
            { 
                Clipboard.SetText(tbDataLayer.Text);
            }
            else
            {
                MessageBox.Show("The text box is empty. Nothing to copy.");
            }
        }
    }
}
