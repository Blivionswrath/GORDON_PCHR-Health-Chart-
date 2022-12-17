using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gordon_PCHR
{
    class Validator
    {


        /// <param name="textBox">The TextBox to be checked</param>
        /// <returns>Returns true if the textbox has content</returns>
        public static bool hasContent(TextBox textBox)
        {
            bool ret;

            ret = textBox.Text != string.Empty ? true : false;

            return ret;
        }


        /// <param name="textBoxes">A collection of TextBoxes</param>
        /// <returns>Returns true if all textboxes have content</returns>
        public static bool hasContent(TextBox[] textBoxes)
        {

            foreach (TextBox box in textBoxes)
            {
                if (!hasContent(box))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool isNum(string text)
        {

            bool ret = true;
            //Check for the ASCII numbers
            for (int i = 0; i < text.Length; i++)
            {
                if (!(text[i] >= '0' && text[i] <= '9'))
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        static bool isAlpha(TextBox textBox)
        {

            string txt = textBox.Text;
            bool ret = true;

            for (int i = 0; i < txt.Length; i++)
            {
                //Check for capital letters in ASCII...
                if ((txt[i] >= 'A' && txt[i] <= 'Z')) { continue; }
                //and lowercase letters in ASCII
                if (!(txt[i] >= 'a' && txt[i] <= 'z')) {
                    ret = false;
                    break;
                }

            }

            return ret;
        }

    
        /// <summary>
        /// This checks the database to make sure the ID numbers stay unique
        /// </summary>
        /// <returns>True if this ID is valid</returns>
        public static int getFreshId()
        {
            string conString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\pchr42563.mdf;Integrated Security=True;Connect Timeout=30";
            string commandString = "Select * From PATIENT_TBL";
            //This is used to track the last patient ID
            string lastId = "";

            using (SqlConnection connection = new SqlConnection(conString))
            {
                using (SqlCommand command = new SqlCommand(commandString, connection))
                {
                    
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lastId = reader.GetString(0);
                        }

                    }
                //Query the database for this id, if a user is returned than the id is not unique


                }
            }

            int id;

            int.TryParse(lastId, out id);

            id++;

            return id;
        }
        /// <summary>
        /// Increments int datatype
        /// </summary>
        /// <param name="field">The field to increment</param>
        /// <param name="table">The table the field is on</param>
        /// <param name="con">The SqlConnection to use</param>
        /// <returns></returns>
        public static int incrementField(string field, string table, SqlConnection con)
        {
            int ret;
            string getId = String.Format("SELECT {0} FROM {1}", field, table);

            SqlCommand command = new SqlCommand(getId, con);

            SqlDataReader reader = command.ExecuteReader();

            reader.Read();

            ret = reader.GetInt32(0);
            ret++;
            return ret;
        }
        /// <summary>
        /// Increments a nchar datatype
        /// </summary>
        /// <param name="field">The field to increment</param>
        /// <param name="table">The table the field is on</param>
        /// <param name="con">The SqlConnection to use</param>
        /// <returns></returns>
        public static int incrementFieldString(string field, string table, SqlConnection con)
        {
            string dataIn;
            int ret = -2;
            string getId = String.Format("SELECT {0} FROM {1} ORDER BY {0} DESC;", field, table);

            SqlCommand command = new SqlCommand(getId, con);

            SqlDataReader reader = command.ExecuteReader();

            reader.Read();


            if (!reader.HasRows)
            {
                reader.Close();
                return 0;
            }
            dataIn = reader.GetString(0);
            Int32.TryParse(dataIn, out ret);
            ret++;
            reader.Close();
            return ret;
        }

        /// <summary>
        /// A validator to ensure your users aren't born in the future
        /// </summary>
        /// <returns>Returns true if this date has passed</returns>
        public static bool isPastDate(DateTime date)
        {
            DateTime now = DateTime.Now;

            int result = DateTime.Compare(now, date);

            bool ret = result >= 0 ? true : false;

            return ret;
        }

    }
}
