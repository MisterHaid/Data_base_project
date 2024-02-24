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
    public partial class ManageObjectTypesForm : Form
    {
        NpgsqlConnection connection;
        int selectedObjectId = -1;

        public ManageObjectTypesForm()
        {
            InitializeComponent();
        }

        public ManageObjectTypesForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
        }

        private void ManageObjectTypesForm_Load(object sender, EventArgs e)
        {
            var sql = "select * from objecttype ORDER BY objectid";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                ObjectsView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetValue(2).ToString());

            rdr.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sql = $"UPDATE objecttype SET objectname='{UpdateName.Text}', objectcost={Single.Parse(UpdateCost.Text)} WHERE objectid={selectedObjectId}";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            ObjectsView.SelectedRows[0].Cells[1].Value = UpdateName.Text;
            ObjectsView.SelectedRows[0].Cells[2].Value = UpdateCost.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            string formattedDate = $"{now.Year}.{now.Month}.{now.Day}";
            var sql = $"start transaction;" +
                $"INSERT INTO ObjectType(ObjectId, ObjectName, ObjectCost) VALUES({ObjectsView.Rows.Count + 1}, '{AddName.Text}', {Single.Parse(AddCost.Text)});" +
                $"INSERT INTO Brigade(ObjectId, BrigadeName, DateOfForm) VALUES({ObjectsView.Rows.Count + 1}, 'B-{string.Join("", AddName.Text.Split().Select(x => x[0]))}', '{formattedDate}');" +
                $"commit;";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            ObjectsView.Rows.Add(ObjectsView.Rows.Count + 1, AddName.Text, AddCost.Text);
        }

        private void ObjectsView_SelectionChanged(object sender, EventArgs e)
        {
            if (ObjectsView.SelectedRows.Count == 0)
                return;

            UpdateName.Text = ObjectsView.SelectedRows[0].Cells[1].Value.ToString();
            UpdateCost.Text = ObjectsView.SelectedRows[0].Cells[2].Value.ToString();
            selectedObjectId = Int32.Parse(ObjectsView.SelectedRows[0].Cells[0].Value.ToString());
        }
    }
}
