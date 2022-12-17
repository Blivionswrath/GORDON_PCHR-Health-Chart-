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

namespace Gordon_PCHR
{

    public partial class Register : Form
    {

        public Register()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            //AttachDbFilename =| DataDirectory |\bin\Debug\pchr42563.mdf
            string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename =| DataDirectory |\\pchr42563.mdf; Integrated Security = True; Connect Timeout = 30;";
            TextBox[] boxes = { txtConfirm, txtFirstName, txtId, txtInitials, txtLastName, txtPassword, txtUsername };

            //Validate the data
            //If valid Save it, show a msg otherwise
            
            if (!Validator.hasContent(boxes))
            {
                MessageBox.Show("All fields are required.");
                return;
                
            }
            //Make sure the user typed the same password
            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("The two passwords you have provided do not match.");
                return;
            }

            //Make sure gender is selected
            if (!(rdoFemale.Checked || rdoMale.Checked))
            {
                MessageBox.Show("Please select your gender");
                return;
            }


            //Check the date (Currently only for future dates)
            if (!(Validator.isPastDate(dtpDateOfBirth.Value))) {
                MessageBox.Show("Please select a valid birthday");
                return;
            }


            using (SqlConnection con = new SqlConnection(connectionString))
            {

                string loginString = "Insert into LOGIN Values(@PATIENT_ID, @USERNAME, @PASSWORD)";
                string infoString = "Insert into PATIENT_TBL Values(@PATIENT_ID, @LAST_NAME, @FIRST_NAME, @DATE_OF_BIRTH, @ADDRESS_STREET, @ADDRESS_STATE, " +
                    "@ADDRESS_ZIP, @PHONE_HOME, @PHONE_MOBILE, @PRIMARY_ID)";

                using (SqlCommand loginCommand = new SqlCommand(loginString, con))
                {
                    loginCommand.Parameters.Add(new SqlParameter("PATIENT_ID", txtId.Text));
                    loginCommand.Parameters.Add(new SqlParameter("USERNAME", txtUsername.Text));
                    loginCommand.Parameters.Add(new SqlParameter("PASSWORD", txtPassword.Text));
                    con.Open();
                    loginCommand.ExecuteNonQuery();

                    SqlCommand detailsCommand = new SqlCommand(infoString, con);
                    detailsCommand.Parameters.Add(new SqlParameter("PATIENT_ID", txtId.Text));
                    detailsCommand.Parameters.Add(new SqlParameter("LAST_NAME", txtLastName.Text));
                    detailsCommand.Parameters.Add(new SqlParameter("FIRST_NAME", txtFirstName.Text));
                    detailsCommand.Parameters.Add(new SqlParameter("DATE_OF_BIRTH", dtpDateOfBirth.Value.ToString()));
                    detailsCommand.Parameters.Add(new SqlParameter("ADDRESS_STREET", null));
                    detailsCommand.Parameters.Add(new SqlParameter("ADDRESS_STATE", null));
                    detailsCommand.Parameters.Add(new SqlParameter("ADDRESS_ZIP", null));
                    detailsCommand.Parameters.Add(new SqlParameter("PHONE_HOME", null)); //ET\\
                    detailsCommand.Parameters.Add(new SqlParameter("PHONE_MOBILE", null));
                    detailsCommand.Parameters.Add(new SqlParameter("PRIMARY_ID", null));
                    detailsCommand.Parameters.Add(new SqlParameter("TITLE", cboTitle.SelectedItem));
                    detailsCommand.Parameters.Add(new SqlParameter("GENDER", dtpDateOfBirth.Value.ToString()));
                    int x =detailsCommand.ExecuteNonQuery();

                    if (x > 0)
                    {
                        MessageBox.Show("Something went wrong with your registration");
                        return;
                    }

                    //If we reach the end of this method, the udser information was pushed
                    //and we can go back to the login form
                    con.Close();
                    this.Close();
                }
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {
            txtId.Text = Validator.getFreshId().ToString();
            DateTime Now = DateTime.Now;
            //This is here to ensure the date is actually changed, since a future date is already checked for
            Now.AddYears(3);
            
        }
    }
}
