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
using System.Xml;

namespace Gordon_PCHR
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Register frmRegister = new Register();
            frmRegister.ShowDialog();
        }
        /// <summary>
        /// Checks the textboxes on screen to make sure the login is valid
        /// </summary>

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string password = "";
            int ID = 0;
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\pchr42563.mdf;Integrated Security=True;Connect Timeout=30";
            string typed = "Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\pchr42563.mdf; Integrated Security=True; Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
            string commandString = String.Format("SELECT PATIENT_ID, PASS FROM LOGIN WHERE USERNAME = '{0}'", txtUsername.Text);

                using (SqlCommand command = new SqlCommand(commandString, connection))
                {

            
                connection.Open();
            
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    {
                        reader.Read();
                        ID = reader.GetInt32(0); 
                        password = reader.GetString(1);


                    }

                connection.Close();
                }

            }


            if (ID == 0 || password != textBox1.Text)
            {
                MessageBox.Show("Incorrect username or password");
                return;
            }


            //Open a form with the patient id


        }
    }
}
