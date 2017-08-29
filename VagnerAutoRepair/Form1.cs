using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VagnerAutoRepair
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dgvPartsUsed_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 2)
            {
                double unitPrice = 0D;
                int quantity = 0;
                double subTotal = 0D;

                DataGridViewCell dgvcUnitPrice = dgvPartsUsed.Rows[e.RowIndex].Cells[1];
                DataGridViewCell dgvcQuantity = dgvPartsUsed.Rows[e.RowIndex].Cells[e.ColumnIndex];

                unitPrice = double.Parse(dgvcUnitPrice.EditedFormattedValue.ToString());
                quantity = int.Parse(dgvcQuantity.EditedFormattedValue.ToString());
                subTotal = unitPrice * quantity;
                dgvPartsUsed.Rows[e.RowIndex].Cells[3].Value = subTotal.ToString("F");

                CalculateTotal();
            }

        }

        void CalculateTotal()
        {
            double taxRate = 0d, taxAmount = 0d;
            double totalParts = 0d, totalLabor = 0d, totalPartsAndLabor, repairTotal;

            foreach (DataGridViewRow record in dgvPartsUsed.Rows)
            {
                try
                {
                    totalParts += double.Parse(record.Cells[3].EditedFormattedValue.ToString());
                    txtTotalParts.Text = totalParts.ToString("F");
                }
                catch (FormatException)
                {
                }
            }

            foreach (DataGridViewRow record in dgvJobsPerformed.Rows)
            {
                try
                {
                    totalLabor += double.Parse(record.Cells[1].EditedFormattedValue.ToString());
                    txtTotalLabor.Text = totalLabor.ToString("F");
                }
                catch (FormatException)
                {
                }
            }

            try
            {
                taxRate = double.Parse(txtTaxRate.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Tax Rate");
                txtTaxRate.Text = "5.75";
                txtTaxRate.Focus();
            }

            totalPartsAndLabor = totalParts + totalLabor;

            taxAmount = totalPartsAndLabor * taxRate / 100;
            repairTotal = totalPartsAndLabor + taxAmount;

            txtTotalParts.Text = totalParts.ToString("F");
            txtTotalLabor.Text = totalLabor.ToString("F");
            txtTaxAmount.Text = taxAmount.ToString("F");
            txtRepairTotal.Text = repairTotal.ToString("F");
        }

        private void dgvJobsPerformed_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            CalculateTotal();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZIPCode.Text = "";
            txtMake.Text = "";
            txtModel.Text = "";
            txtYear.Text = "";
            txtProblemDescription.Text = "";

            dgvPartsUsed.Rows.Clear();
            dgvJobsPerformed.Rows.Clear();

            txtTotalParts.Text = "0.00";
            txtTotalLabor.Text = "0.00";
            txtTaxRate.Text = "7.75";
            txtTaxAmount.Text = "0.00";
            txtRepairTotal.Text = "0.00";

            txtRecommendations.Text = "";
            txtCustomerName.Focus();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Just in case, calculate the order now
            CalculateTotal();

            // Check the folder that will contain the repair order.
            // If it exists, don't create it.
            // If it doesn't exist, then create it
            string strDirectory = @"C:\Vagner Auto Repair";
            DirectoryInfo dirInfo = Directory.CreateDirectory(strDirectory);

            // Prepare to create a repair order
            RepairOrder order = new RepairOrder();

            //Create String fileName 
            String FileName = txtCustomerName.Text;
            FileName = FileName.Replace(" ", String.Empty);
            


            // Display the Save As dialog box.
            // Select the above directory
            dlgSave.InitialDirectory = strDirectory;
            dlgSave.FileName = FileName;
            // Check if the user clicked OK
            if (dlgSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // If the user clicked OK, create a repair order
                order.CustomerName = txtCustomerName.Text;
                order.Address = txtAddress.Text;
                order.City = txtCity.Text;
                order.State = txtState.Text;
                order.ZIPCode = txtZIPCode.Text;
                order.Make = txtMake.Text;
                order.Model = txtModel.Text;
                order.Year = int.Parse(txtYear.Text);
                order.ProblemDescription = txtProblemDescription.Text;

                // Check whether the Parts Used data grid view contains some records.
                // If it does, save them
                if (dgvPartsUsed.Rows.Count > 1)
                {
                    List<Part> parts = new List<Part>();

                    for (int row = 0; row < dgvPartsUsed.Rows.Count - 1; row++)
                    {
                        Part prt = new Part();
                        prt.PartName = dgvPartsUsed.Rows[row].Cells[0].EditedFormattedValue.ToString();
                        prt.UnitPrice = double.Parse(dgvPartsUsed.Rows[row].Cells[1].EditedFormattedValue.ToString());
                        prt.Quantity = int.Parse(dgvPartsUsed.Rows[row].Cells[2].EditedFormattedValue.ToString());
                        prt.SubTotal = double.Parse(dgvPartsUsed.Rows[row].Cells[3].EditedFormattedValue.ToString());
                        parts.Add(prt);
                    }
                    order.Parts = parts;
                }
                else // If the data grid view is empty, flag its value as null
                    order.Parts = null;

                if (dgvJobsPerformed.Rows.Count > 1)
                {
                    List<JobPerformed> work = new List<JobPerformed>();

                    for (int row = 0; row < dgvJobsPerformed.Rows.Count - 1; row++)
                    {
                        JobPerformed done = new JobPerformed();
                        done.Job = dgvJobsPerformed.Rows[row].Cells[0].EditedFormattedValue.ToString();
                        done.Cost = double.Parse(dgvJobsPerformed.Rows[row].Cells[1].EditedFormattedValue.ToString());
                        work.Add(done);
                    }

                    order.Jobs = work;
                }

                order.TotalParts = double.Parse(txtTotalParts.Text);
                order.TotalLabor = double.Parse(txtTotalLabor.Text);
                order.TaxRate = double.Parse(txtTaxRate.Text);
                order.TaxAmount = double.Parse(txtTaxAmount.Text);
                order.RepairTotal = double.Parse(txtRepairTotal.Text);
                order.Recommendations = txtRecommendations.Text;

                FileStream stmRepair = new FileStream(dlgSave.FileName,
                                                           FileMode.Create);
                BinaryFormatter bfmRepair = new BinaryFormatter();
                bfmRepair.Serialize(stmRepair, order);

                stmRepair.Close();
                newToolStripMenuItem_Click(sender, e);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpen.InitialDirectory = @"C:\Vagner Auto Repair";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                FileStream stmRepair = new FileStream(dlgOpen.FileName,
                                                 FileMode.Open);
                BinaryFormatter bnrRepair = new BinaryFormatter();
                RepairOrder order = (RepairOrder)bnrRepair.Deserialize(stmRepair);

                txtCustomerName.Text = order.CustomerName;
                txtAddress.Text = order.Address;
                txtCity.Text = order.City;
                txtState.Text = order.State;
                txtZIPCode.Text = order.ZIPCode;
                txtMake.Text = order.Make;
                txtModel.Text = order.Model;
                txtYear.Text = order.Year.ToString();
                txtProblemDescription.Text = order.ProblemDescription;

                int i = 0;
                foreach (Part prt in order.Parts)
                {
                    dgvPartsUsed.Rows.Add();

                    dgvPartsUsed.Rows[i].Cells[0].Value = prt.PartName;
                    dgvPartsUsed.Rows[i].Cells[1].Value = prt.UnitPrice.ToString("F");
                    dgvPartsUsed.Rows[i].Cells[2].Value = prt.Quantity.ToString();
                    dgvPartsUsed.Rows[i].Cells[3].Value = prt.SubTotal.ToString("F");
                    i++;
                }

                i = 0;
                foreach (JobPerformed jp in order.Jobs)
                {
                    dgvJobsPerformed.Rows.Add();

                    dgvJobsPerformed.Rows[i].Cells[0].Value = jp.Job;
                    dgvJobsPerformed.Rows[i].Cells[1].Value = jp.Cost;
                    i++;
                }

                txtTotalParts.Text = order.TotalParts.ToString("F");
                txtTotalLabor.Text = order.TotalLabor.ToString("F");
                txtTaxRate.Text = order.TaxRate.ToString("F");
                txtTaxAmount.Text = order.TaxAmount.ToString("F");
                txtRepairTotal.Text = order.RepairTotal.ToString("F");
                txtRecommendations.Text = order.Recommendations;

                stmRepair.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsToolStripMenuItem_Click(sender, e);
        }
    }
}
