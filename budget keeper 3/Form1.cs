using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace budget_keeper_3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeDataGrid1();
            InitializeDataGrid2();
            addmonthsandyeartodropdown();
            calculatebalance("Cash", dataGridView1);
            calculatebalance("Credit", dataGridView2);
            calculateCombinedBalance();
        }
        private DataTable table1 = new DataTable("Table1");
        private DataTable table2 = new DataTable("Table2");
        private void InitializeDataGrid1()
        {
            table1.Columns.AddRange(new[]
            {
                new DataColumn("Paid", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Amount", typeof(string)),
            });
            dataGridView1.DataSource = table1;
            dataGridView1.Columns["Paid"].Visible = false;
        }
        private void populatecomboboxontabpageentery()
        {

            if (tabControl1.SelectedIndex == 2)
            {

                addmonthsandyeartodropdown();

            }
        }

        private void InitializeDataGrid2()
        {
            table2.Columns.AddRange(new[]
            {
                new DataColumn("Paid", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Amount", typeof(string)),
            });

            dataGridView2.DataSource = table2;
            dataGridView2.Columns["Paid"].Visible = false;
        }

        private void SwitchToTabControl1(int tabIndex)
        {
            // Check if the tab index is within the valid range
            if (tabIndex >= 0 && tabIndex < tabControl1.TabCount)
            {
                // Switch to the specified tab
                tabControl1.SelectedIndex = tabIndex;
            }
            else
            {
                // Handle the case when the provided index is out of range
                MessageBox.Show("Invalid tab index!");
            }
        }

        private void cashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchToTabControl1(0);
        }

        private void creditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchToTabControl1(1);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }
        private void entercashinformation()
        {

            string name = textBox1.Text;
            string amount = textBox2.Text;
            string paid = checkBox1.Checked.ToString();
            table1.Rows.Add(paid, name, amount);
            dataGridView1.DataSource = table1;
        }
        private void calculatebalance(string fileType, DataGridView dataGridView)
        {
            try
            {
                // Assuming "Amount" is the column you want to calculate the balance for
                string columnName = "Amount";
                decimal totalBalance = 0;

                // Loop through the rows in the DataGridView and sum the values in the specified column
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    // Skip the last row (which is usually the new row for entering data)
                    if (!row.IsNewRow)
                    {
                        // Get the value in the specified column
                        string cellValue = row.Cells[columnName].Value?.ToString();

                        // Clean the value by removing non-numeric characters
                        string cleanedValue = new string(cellValue.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

                        // Check if the cleaned value is a valid decimal format before adding to the total balance
                        if (decimal.TryParse(cleanedValue, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out decimal numericValue))
                        {
                            totalBalance += (cellValue.StartsWith("-") ? -numericValue : numericValue);
                        }
                        else
                        {
                            // Handle the case where the value is not in a valid decimal format
                            MessageBox.Show($"Invalid numeric format in row {row.Index + 1}, column {columnName}");
                            return; // Stop calculation if an error occurs
                        }
                    }
                }

                // Now you have the total balance, you can use it as needed
                textBox5.AppendText($"Total {fileType} balance: {totalBalance:N2}\r\n");
                textBox6.AppendText($"Total {fileType} balance: {totalBalance:N2}\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void calculateCombinedBalance()
        {
            decimal cashBalance = calculateBalance("Cash", table1);
            decimal creditBalance = calculateBalance("Credit", table2);

            decimal combinedBalance = cashBalance + creditBalance;

            textBox5.AppendText($"Total balance: {combinedBalance:N2}\r\n");
            textBox6.AppendText($"Total balance: {combinedBalance:N2}\r\n");
        }

        private decimal calculateBalance(string fileType, DataTable dataTable)
        {
            try
            {
                // Assuming "Amount" is the column you want to calculate the balance for
                string columnName = "Amount";
                decimal totalBalance = 0;

                // Loop through the rows in the DataTable and sum the values in the specified column
                foreach (DataRow row in dataTable.Rows)
                {
                    // Get the value in the specified column
                    string cellValue = row[columnName].ToString();

                    // Clean the value by removing non-numeric characters
                    string cleanedValue = new string(cellValue.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

                    // Check if the cleaned value is a valid decimal format before adding to the total balance
                    if (decimal.TryParse(cleanedValue, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out decimal numericValue))
                    {
                        totalBalance += (cellValue.StartsWith("-") ? -numericValue : numericValue);
                    }
                    else
                    {
                        // Handle the case where the value is not in a valid decimal format
                        MessageBox.Show($"Invalid numeric format in DataTable '{fileType}', row {dataTable.Rows.IndexOf(row) + 1}, column {columnName}");
                        return 0; // Return 0 if an error occurs
                    }
                }

                return totalBalance;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return 0; // Return 0 if an error occurs
            }
        }

        private void clearcashtextboxes()
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void cleartotaltextboxes()
        {
            textBox5.Clear();
            textBox6.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            entercashinformation();
            clearcashtextboxes();
            cleartotaltextboxes();
            calculatebalance("Cash", dataGridView1);
            calculatebalance("Credit", dataGridView2);
            calculateCombinedBalance();
        }
        private void deleteSelectedRow(DataGridView dataGridView)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                // Remove the selected row from the DataTable
                DataRowView selectedRow = dataGridView.SelectedRows[0].DataBoundItem as DataRowView;
                if (selectedRow != null)
                {
                    selectedRow.Row.Delete();
                }
            }
        }

        private void refreshDataGridView(DataGridView dataGridView)
        {
            dataGridView.Refresh();
            dataGridView.ClearSelection();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            deleteSelectedRow(dataGridView1);
            refreshDataGridView(dataGridView1);
            cleartotaltextboxes();
            calculatebalance("Cash", dataGridView1);
            calculatebalance("Credit", dataGridView2);
            calculateCombinedBalance();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearcashtextboxes();
        }
        private void entercreditinformation()
        {
            string name = textBox3.Text;
            string amount = textBox4.Text;
            string paid = checkBox2.Checked.ToString();
            table2.Rows.Add(paid, name, amount);
            dataGridView2.DataSource = table2;
        }

        private void clearcredittextboxes()
        {
            textBox3.Clear();
            textBox4.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            entercreditinformation();
            clearcredittextboxes();
            cleartotaltextboxes();
            calculatebalance("Cash", dataGridView1);
            calculatebalance("Credit", dataGridView2);
            calculateCombinedBalance();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            deleteSelectedRow(dataGridView2);
            refreshDataGridView(dataGridView2);
            cleartotaltextboxes();
            calculatebalance("Cash", dataGridView1);
            calculatebalance("Credit", dataGridView2);
            calculateCombinedBalance();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clearcredittextboxes();
        }

        private void addmonthsandyeartodropdown()
        {
            for (int i = 0; i < 12; i++)
            {
                comboBox1.Items.Add(dateTimePicker1.Value.AddMonths(i).ToString("MMMM yyyy"));
            }
        }
        private void SaveData(string fileFormat, DataGridView dataGridView, DataTable dataTable, string defaultExtension)
        {
            string chosenMonth = comboBox1.Text;

            string folderPath = $@"C:\BudgetData\{chosenMonth}\";

            // Check if the folder exists, create it if not
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filename = Path.Combine(folderPath, $"{chosenMonth}_{fileFormat}.{defaultExtension}");

            try
            {
                // Save the states of checkboxes as headers
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    // Write column names as headers
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        writer.Write(column.ColumnName);

                        if (column.Ordinal < dataTable.Columns.Count - 1)
                            writer.Write(",");
                    }

                    writer.WriteLine();

                    // Write checkbox states and data rows
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if (dataGridView.Rows.Count == dataGridView.Rows.Count - 1)
                        {

                        }
                        else
                        {

                            writer.Write(row.Cells["Paid"].Value);
                            writer.Write(",");
                            writer.Write(row.Cells["Name"].Value);
                            writer.Write(",");
                            writer.Write(row.Cells["Amount"].Value);
                            writer.WriteLine();
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveData("Cash", dataGridView1, table1, "mbi");
            SaveData("Credit", dataGridView2, table2, "mbc");
            MessageBox.Show("Data saved!", "Success");
            SwitchToTabControl1(0);
        }
        private void OpenData(string fileType, DataGridView dataGridView, DataTable dataTable, string defaultExtension)
        {
            string chosenMonth = comboBox1.Text;
            string folderPath = $@"C:\BudgetData\{chosenMonth}\";

            string filename = Path.Combine(folderPath, $"{chosenMonth}_{fileType}.{defaultExtension}");

            if (!File.Exists(filename))
            {
                MessageBox.Show($"The file {filename} does not exist.", "File Not Found");
                return;
            }

            // Clear existing data in the DataTable
            dataTable.Rows.Clear();
            dataTable.Columns.Clear();

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // Read column headers
                string[] columns = parser.ReadFields();

                // Add columns to DataTable
                foreach (string column in columns)
                {
                    dataTable.Columns.Add(column, typeof(string));
                }

                // Read data and populate DataTable
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    // Skip adding rows with all empty values (blank rows)
                    if (fields.Any(field => !string.IsNullOrWhiteSpace(field)))
                    {
                        dataTable.Rows.Add(fields);
                    }
                }

                // Set the DataTable as the DataSource for the DataGridView
                dataGridView.DataSource = dataTable;

                if (dataTable.Columns.Contains("Paid"))
                {
                    dataGridView.Columns["Paid"].Visible = false;
                }
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            cleartotaltextboxes();
            OpenData("Cash", dataGridView1, table1, "mbi");
            calculatebalance("Cash", dataGridView1);
            OpenData("Credit", dataGridView2, table2, "mbc");
            calculatebalance("Credit", dataGridView2);
            MessageBox.Show("Data Opened!", "Success");
            calculateCombinedBalance();
            SwitchToTabControl1(0);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not a header and not the new row
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count - 1)
            {
                // Assuming you have columns named "Paid", "Name", and "Amount"
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Populate textboxes and checkbox with the values from the selected row
                textBox1.Text = selectedRow.Cells["Name"].Value.ToString();
                textBox2.Text = selectedRow.Cells["Amount"].Value.ToString();
                checkBox1.Checked = Convert.ToBoolean(selectedRow.Cells["Paid"].Value);
                button9.Visible = false;
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not a header and not the new row
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView2.Rows.Count - 1)
            {
                // Assuming you have columns named "Paid", "Name", and "Amount"
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];

                // Populate textboxes and checkbox with the values from the selected row
                textBox3.Text = selectedRow.Cells["Name"].Value.ToString();
                textBox4.Text = selectedRow.Cells["Amount"].Value.ToString();
                checkBox2.Checked = Convert.ToBoolean(selectedRow.Cells["Paid"].Value);
                button10.Visible = false;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            button9.Visible = true;
            if (checkBox1.Checked)
            {
                button9.Text = "Pay";
            }
            else
            {
                button9.Text = "UnPay";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            entercashinformation();
            clearcashtextboxes();
            dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            checkBox1.CheckState = CheckState.Unchecked;
            button9.Visible = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            button10.Visible = true;
            if (checkBox2.Checked)
            {
                button10.Text = "Pay";
            }
            else
            {
                button10.Text = "UnPay";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            entercreditinformation();
            clearcredittextboxes();
            dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
            checkBox2.CheckState = CheckState.Unchecked;
            button10.Visible = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            table1.Rows.Clear();
            table2.Rows.Clear();
            table1.Columns.Clear();
            table2.Columns.Clear();
            InitializeDataGrid1();
            InitializeDataGrid2();
            comboBox1.Items.Clear();
            addmonthsandyeartodropdown();
            cleartotaltextboxes();
            calculatebalance("Cash", dataGridView1);
            calculatebalance("Credit", dataGridView2);
            calculateCombinedBalance();
            SwitchToTabControl1(0);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchToTabControl1(2);
            button11.Visible = true;
            button8.Visible = false;
            button7.Visible = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchToTabControl1(2);
            button11.Visible = false;
            button8.Visible = true;
            button7.Visible = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchToTabControl1(2);
            button11.Visible = false;
            button8.Visible = false;
            button7.Visible = true;
        }
    }
}
