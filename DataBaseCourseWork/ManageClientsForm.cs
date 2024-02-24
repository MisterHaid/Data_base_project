using System;
using System.Windows.Forms;
using Npgsql;

namespace DataBaseCourseWork
{
    public partial class ManageClientsForm : Form
    {
        NpgsqlConnection connection;
        int selectedClientId = -1;

        public ManageClientsForm()
        {
            InitializeComponent();
        }

        public ManageClientsForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
        }

        private void ManageClientsForm_Load(object sender, EventArgs e)
        {
            requestClients();
        }

        private void ClientView_SelectionChanged(object sender, EventArgs e)
        {
            if (ClientView.SelectedRows.Count == 0)
                return;

            UpdateName.Text = ClientView.SelectedRows[0].Cells[1].Value.ToString();
            selectedClientId = Int32.Parse(ClientView.SelectedRows[0].Cells[0].Value.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sql = $"UPDATE Client SET clientname='{UpdateName.Text}' WHERE clientid={selectedClientId}";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            ClientView.SelectedRows[0].Cells[1].Value = UpdateName.Text;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var formattedDate = $"{now.Day}.{now.Month}.{now.Year}";
            var sql = $"INSERT INTO Client(ClientId, ClientName, DateOfRegister) VALUES({ClientView.Rows.Count+1}, '{AddName.Text}', '{formattedDate}');";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            ClientView.Rows.Add(ClientView.Rows.Count + 1, AddName.Text, formattedDate);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClientView.Rows.Clear();

            string before = $"{BeforeDate.Value.Year}-{BeforeDate.Value.Month}-{BeforeDate.Value.Day}";
            string after = $"{AfterDate.Value.Year}-{AfterDate.Value.Month}-{AfterDate.Value.Day}";
            string subRequest = $"WHERE dateofregister between '{after}' and '{before}'";

            var sql = $"select * from Client {subRequest} ORDER BY dateofregister DESC";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ClientView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetDateTime(2).ToString().Split()[0]);

            rdr.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClientView.Rows.Clear();
            requestClients();
        }

        private void requestClients() 
        {
            var sql = "select * from Client ORDER BY clientid";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ClientView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetDateTime(2).ToString().Split()[0]);

            rdr.Close();
        }

        private void ClientView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
