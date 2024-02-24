using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.IO;

namespace DataBaseCourseWork
{
    public partial class Form1 : Form
    {
        NpgsqlConnection connection;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionInfo = "Host=localhost;Username=postgres;password='';Database=Course_work";
            connection = new NpgsqlConnection(connectionInfo);
            connection.Open();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            connection.Close();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void contractsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageContractsForm form = new ManageContractsForm(connection);
            form.MdiParent = this;
            form.Show();
        }

        private void clientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageClientsForm form = new ManageClientsForm(connection);
            form.MdiParent = this;
            form.Show();
        }

        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageEmployeesForm form = new ManageEmployeesForm(connection);
            form.MdiParent = this;
            form.Show();
        }

        private void objectTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageObjectTypesForm form = new ManageObjectTypesForm(connection);
            form.MdiParent = this;
            form.Show();
        }

    }
}
