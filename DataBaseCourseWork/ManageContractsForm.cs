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

namespace DataBaseCourseWork
{
    public struct IdNamePair {
        public int id;
        public string name;
        public IdNamePair(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public override string ToString() => this.name;
    }

    

    public partial class ManageContractsForm : Form
    {
        NpgsqlConnection connection;

        public ManageContractsForm()
        {
            InitializeComponent();
        }

        public ManageContractsForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
        }

        private void ManageContractsForm_Load(object sender, EventArgs e)
        {
            requestContracts();
            requestObjectTypes();
            requestClients();
            requestBrigades();
        }

        private void requestContracts()
        {
            var sql = "SELECT contractid, objectname, clientname, dateofsign, contractstate FROM contract, objecttype, client WHERE contract.objectid=objecttype.objectid AND contract.clientid=client.clientid ORDER BY contractid;";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ContractsView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDateTime(3).ToString().Split()[0], rdr.GetString(4));

            rdr.Close();
        }

        private void requestObjectTypes() 
        {
            var sql = "SELECT objectid, objectname FROM objecttype ORDER BY objectid";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var pair = new IdNamePair(rdr.GetInt32(0), rdr.GetString(1));
                ObjectTypeCB.Items.Add(pair);
            }

            rdr.Close();
        }

        private void requestClients()
        {
            var sql = "SELECT clientid, clientname FROM client";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var pair = new IdNamePair(rdr.GetInt32(0), rdr.GetString(1));
                ClientCB.Items.Add(pair);
            }

            rdr.Close();
        }

        private void requestBrigades()
        {
            var sql = "SELECT objectid, brigadename FROM brigade";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var pair = new IdNamePair(rdr.GetInt32(0), rdr.GetString(1));
                BrigadeCB.Items.Add(pair);
            }

            rdr.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ClientCB.SelectedItem == null || ObjectTypeCB.SelectedItem == null)
                return;

            var now = DateTime.Now;
            string formattedDate = $"{now.Year}-{now.Month}-{now.Day}";
            var selectedObjectType = (IdNamePair)ObjectTypeCB.SelectedItem;
            var selectedClient = (IdNamePair)ClientCB.SelectedItem;

            var sql = $"INSERT INTO Contract(ContractId, ObjectId, ClientId, DateOfSign, ContractState) " +
                $"VALUES({ContractsView.Rows.Count+1}, {selectedObjectType.id}, {selectedClient.id}, '{formattedDate}', 'Pending');";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            ContractsView.Rows.Add(ContractsView.Rows.Count + 1, selectedObjectType.name, selectedClient.name, formattedDate, "Pending");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (StateCB.SelectedItem == null)
            {
                MessageBox.Show("State wasn't selected!");
                return;
            }

            var sql = $"UPDATE Contract SET contractstate='{StateCB.Text}' WHERE contractid={Int32.Parse(ContractsView.SelectedRows[0].Cells[0].Value.ToString())}";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            ContractsView.SelectedRows[0].Cells[4].Value = StateCB.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (BrigadeCB.SelectedItem == null)
            {
                MessageBox.Show("Brigade wasn't selected!");
                return;
            }

            ContractsView.Rows.Clear();

            var sql = $"SELECT contractid, objectname, clientname, dateofsign, contractstate FROM contract, objecttype, client " +
                $"WHERE contract.objectid=objecttype.objectid AND contract.clientid=client.clientid AND contract.objectid={((IdNamePair)BrigadeCB.SelectedItem).id};";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ContractsView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDateTime(3).ToString().Split()[0], rdr.GetString(4));

            rdr.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ContractsView.Rows.Clear();
            requestContracts();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ContractsView.Rows.Clear();

            var sql = $"SELECT contractid, objectname, clientname, dateofsign, contractstate FROM contract, objecttype, client " +
                            $"WHERE contract.objectid=objecttype.objectid AND contract.clientid=client.clientid AND contractstate='Pending';";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ContractsView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDateTime(3).ToString().Split()[0], rdr.GetString(4));

            rdr.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ContractsView.Rows.Clear();

            var sql = $"SELECT contractid, objectname, clientname, dateofsign, contractstate FROM contract, objecttype, client " +
                            $"WHERE contract.objectid=objecttype.objectid AND contract.clientid=client.clientid AND contractstate='Rented';";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ContractsView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDateTime(3).ToString().Split()[0], rdr.GetString(4));

            rdr.Close();

        }

        private void ContractsView_SelectionChanged(object sender, EventArgs e)
        {
            if (ContractsView.SelectedRows.Count == 0)
                return;

            StateCB.Text = ContractsView.SelectedRows[0].Cells[4].Value.ToString();
        }
    }
}
