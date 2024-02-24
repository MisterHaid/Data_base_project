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
    public partial class ManageEmployeesForm : Form
    {
        NpgsqlConnection connection;

        public ManageEmployeesForm()
        {
            InitializeComponent();
        }

        public ManageEmployeesForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
        }

        private void ManageEmployeesForm_Load(object sender, EventArgs e)
        {
            requestEmployees();
            requestBrigades();
        }

        private void requestEmployees()
        {
            var sql = "SELECT employeeid, fullname, salary, brigadename FROM employee, brigade WHERE employee.objectid=brigade.objectid ORDER BY employeeid";
            var cmd = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                EmployeeView.Rows.Add(rdr.GetInt32(0), rdr.GetString(1), rdr.GetValue(2), rdr.GetString(3));

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
                UpdateBrigadeCB.Items.Add(pair);
                AddBrigadeCB.Items.Add(pair);
            }

            rdr.Close();
        }

        private void EmployeeView_SelectionChanged(object sender, EventArgs e)
        {
            if (EmployeeView.SelectedRows.Count == 0)
                return;

            UpdateName.Text = EmployeeView.SelectedRows[0].Cells[1].Value.ToString();
            UpdateSalary.Text = EmployeeView.SelectedRows[0].Cells[2].Value.ToString();
            UpdateBrigadeCB.SelectedIndex = getIdByName(EmployeeView.SelectedRows[0].Cells[3].Value.ToString());
        }

        private int getIdByName(string brigadeName)
        {
            for (int i = 0; i < UpdateBrigadeCB.Items.Count; i++)
                if (((IdNamePair)UpdateBrigadeCB.Items[i]).name == brigadeName)
                    return i;

            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UpdateBrigadeCB.SelectedItem == null)
            {
                MessageBox.Show("Brigade wasn't selected!");
                return;
            }

            var sql = $"UPDATE Employee " +
                $"SET fullname='{UpdateName.Text}', salary={Single.Parse(UpdateSalary.Text)}, objectid={((IdNamePair)UpdateBrigadeCB.SelectedItem).id} " +
                $"WHERE employeeid={Int32.Parse(EmployeeView.SelectedRows[0].Cells[0].Value.ToString())};";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            EmployeeView.SelectedRows[0].Cells[1].Value = UpdateName.Text;
            EmployeeView.SelectedRows[0].Cells[2].Value = UpdateSalary.Text;
            EmployeeView.SelectedRows[0].Cells[3].Value = ((IdNamePair)UpdateBrigadeCB.SelectedItem).name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (AddBrigadeCB.SelectedItem == null)
            {
                MessageBox.Show("Brigade wasn't selected!");
                return;
            }

            var sql = $"INSERT INTO Employee(EmployeeId, ObjectId, FullName, Salary) " +
                $"VALUES({EmployeeView.Rows.Count + 1}, {((IdNamePair)AddBrigadeCB.SelectedItem).id}, '{AddName.Text}', {Single.Parse(AddSalary.Text)});";
            var cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
            EmployeeView.Rows.Add(EmployeeView.Rows.Count + 1, AddName.Text, AddSalary.Text, ((IdNamePair)AddBrigadeCB.SelectedItem).name);
        }
    }
}
