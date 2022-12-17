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
    public partial class MainForm : Form
    {
        //These arrays are all caches to avoid constantly querying the database evey time a list box is changed
        private string[] allergenNotes, immunisationNotes, medNotes, testNotes, testResults, condNotes, procedureNotes, procedurePerform;
        private DateTime[] allergenOnsets, immunisationDates, prescribedDates, testDates, condOnset, procedureDates;
        private bool[] chronicMedication, chronicCondition;
        int patientId;
        string[] allergyId, conditionId, immunizationId, procedureId, medId, testId;
        //These variables help avoid some bugs when adding to the list boxes. 
        bool newEntry = false;
        int newIndex = -1;

        public MainForm(int PATIENT_ID)
        {
            patientId = PATIENT_ID;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            fillPersonal();
        }


        /// <summary>
        /// Event handler for all the edit buttons on page 1
        /// </summary>
        private void editClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
            //Health Insurance group
            if (sender.Equals(lblEditHealth))
            {
                toggleGroup(grpHealthInsurance);
            }
            //Personal Contact group
            else if (sender.Equals(lblPersonalContactEdit))
            {
                toggleGroup(grpContactDetails);
            }
            //Personal Details group
            else if (sender.Equals(lblPersonalDetailsEdit))
            {
                toggleGroup(grpPersonal);
            }
            //Change Password Group
            else if (sender.Equals(lblChangePassword))
            {
                toggleGroup(grpChangePass);
            }
            //Emergency Contact Group
            else if (sender.Equals(lblEditEmergency))
            {
                toggleGroup(grpEmergency);
            }
            //Primary care group
            else if (sender.Equals(lblPrimaryEdit))
            {
                toggleGroup(grpPrimary);
            }
            //Personal medical details group
            else if (sender.Equals(lblEditPersonalMed))
            {
                toggleGroup(grpPersonalMedicalDetails);
            }
            //Allergy Details group
            else if (sender.Equals(lblEditAllergies))
            {
                toggleGroup(grpAllergies);
            }
            //Immunisation Details group
            else if (sender.Equals(lblEditImmunisation))
            {
                toggleGroup(grpImmunization);
            }
            //Prescribed medicine group
            else if (sender.Equals(lblEditMedication))
            {
                toggleGroup(grpMedication);
            }
            //Test Results group
            else if (sender.Equals(lblEditTest))
            {
                toggleGroup(grpTestResults);
            }
            //Medical Conditions group
            else if (sender.Equals(lblEditConditions))
            {
                toggleGroup(grpMedicalCondition);
            }
            //Medical Procedure group
            else if (sender.Equals(lblEditProcedure))
            {
                toggleGroup(grpMedicalProcedures);
            }

        }

        /// <summary>
        /// Event handler for all the cancel buttons on page 1
        /// </summary>
        private void cancelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool page1 = true;
            //Health Insurance group
            if (sender.Equals(lblCancelHealthEdit))
            {
                toggleGroup(grpHealthInsurance);
            }
            //Personal contact group
            else if (sender.Equals(lblCancelContactEdit))
            {
                toggleGroup(grpContactDetails);
            }
            //Personal details group
            else if (sender.Equals(lblCancelPersonalDetailEdit))
            {
                toggleGroup(grpPersonal);
            }
            //Change Password Group
            else if (sender.Equals(lblCancelChangePassword))
            {
                toggleGroup(grpChangePass);
            }
            //Emergency Contact Group
            else if (sender.Equals(lblCancelEmergencyEdit))
            {
                toggleGroup(grpEmergency);
            }
            //Primary care group
            else if (sender.Equals(lblCancelPrimary))
            {
                toggleGroup(grpPrimary);
            }else
            {
                page1 = false;
            }

            //Personal medical details group
            if (sender.Equals(lblCancelPersonalMed))
            {
                toggleGroup(grpPersonalMedicalDetails);
            }
            //Allergy Details group
            else if (sender.Equals(lblCancelAllergies))
            {
                if (newEntry)
                {
                    lstAllergies.Items.RemoveAt(lstAllergies.SelectedIndex);   
                }
                toggleGroup(grpAllergies);
            }
            //Immunisation Details group
            else if (sender.Equals(lblCancelImmunisation))
            {
                if (newEntry)
                {
                    
                    lstImmunizations.Items.RemoveAt(lstImmunizations.SelectedIndex);
                }
                toggleGroup(grpImmunization);
            }
            //Prescribed medicine group
            else if (sender.Equals(lblCancelMedication))
            {
                if (newEntry)
                {
                    lstMedication.Items.RemoveAt(lstMedication.SelectedIndex);
                }
                toggleGroup(grpMedication);
            }
            //Test Results group
            else if (sender.Equals(lblCancelTest))
            {
                if (newEntry)
                {
                    lstTestResults.Items.RemoveAt(lstTestResults.SelectedIndex);
                }
                toggleGroup(grpTestResults);
            }
            //Medical Conditions group
            else if (sender.Equals(lblCancelConditions))
            {
                if (newEntry)
                {
                    lstConditions.Items.RemoveAt(lstConditions.SelectedIndex);
                }
                toggleGroup(grpMedicalCondition);
            }
            //Medical Procedure group
            else if (sender.Equals(lblCancelProcedure))
            {
                if (newEntry)
                {
                    lstProcedures.Items.RemoveAt(lstProcedures.SelectedIndex);
                }
                toggleGroup(grpMedicalProcedures);
            }


            if (page1)
            {
                fillPersonal();
            }else
            {
                fillMedical();
            }

        }
        /// <summary>
        /// Event handler for all the save buttons on page 1
        /// </summary>
        //TODO: Need to validate the boxes before I save.
        private void saveClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\Tanner\\source\\repos\\Gordon_PCHR\\Gordon_PCHR\\pchr42563.mdf; Integrated Security = True; Connect Timeout = 30";
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();



            TextBox[] boxes;
            //Health Insurance group   
            if (sender.Equals(lblSaveHealthInsurance))
            {
                boxes = new TextBox[] { txtInsurer, txtInsuranceNumber, txtInsurancePlan };

                if (!Validator.hasContent(boxes))
                {
                    con.Close();
                    con.Dispose();
                    MessageBox.Show("Please Fill in all fields");
                    return;
                }

                if (!Validator.isNum(txtInsuranceNumber.Text))
                {
                    con.Close();
                    con.Dispose();
                    MessageBox.Show("Please enter a number into the policy number field.");
                }
                string[] columns = { "INSURER", "INSURANCE_PLAN", "POLICY_NUMBER" };
                string[] values = { txtInsurer.Text, txtInsurancePlan.Text, txtInsuranceNumber.Text };
                sqlUpdate(columns, values, "PATIENT_TBL", con);


                toggleGroup(grpHealthInsurance);

            }
            //Personal Contact group
            else if (sender.Equals(lblSaveContactDetails))
            {

                string[] columns = { "ADDRESS_STREET", "ADDRESS_CITY", "ADDRESS_STATE", "ADDRESS_ZIP", "PHONE_HOME", "PHONE_MOBILE", "ADDRESS_SUBURB", "EMAIL" };
                string[] values = { txtAddress.Text, txtCity.Text, txtState.Text, txtPostalCode.Text, txtHomePhone.Text, txtMobilePhone.Text,
                    txtSuburb.Text, txtEmail.Text };

                sqlUpdate(columns, values, "PATIENT_TBL", con);

                toggleGroup(grpContactDetails);
            }
            //Personal Details group
            else if (sender.Equals(lblSavePersonalDetails))
            {
                string gender = rdoMale.Checked ? "Male" : "Female";

                string date = dtpDOB.Value.ToString("yyyy-MM-dd");

                string initials = txtInitials.Text;
                string lastName = txtLastName.Text;
                string firstName = txtFirstName.Text;

                string[] columns = { "LAST_NAME", "FIRST_NAME", "DATE_OF_BIRTH", "GENDER" };
                string[] values = { txtLastName.Text, txtFirstName.Text, date, gender };

                sqlUpdate(columns, values, "PATIENT_TBL", con);

                toggleGroup(grpPersonal);
            }
            //Change Password Group
            else if (sender.Equals(lblSavePassword))
            {

                string passCommandString = String.Format("SELECT PASS FROM LOGIN WHERE PATIENT_ID = {0}", patientId);
                SqlCommand passCommand = new SqlCommand(passCommandString, con);

                SqlDataReader reader = passCommand.ExecuteReader();
                reader.Read();
                string sqlPassword = reader.GetString(0);
                reader.Close();
                if (!String.Equals(txtOldPassword.Text, sqlPassword))
                {
                    MessageBox.Show("Incorrect password.");
                    return;
                }

                if (!String.Equals(txtNewPassword.Text, txtConfirmPassword.Text))
                {
                    MessageBox.Show("New passwords do not match.");
                    return;
                }

                string[] columns = { };
                string[] values = { };

                sqlUpdate(columns, values, "LOGIN", con);

                string updateCommandString = String.Format("UPDATE LOGIN SET PASS = '{0}' WHERE PATIENT_ID = {1}", txtNewPassword.Text, patientId);
                SqlCommand updateCommand = new SqlCommand(updateCommandString, con);

                updateCommand.ExecuteNonQuery();

                toggleGroup(grpChangePass);
            }
            //Emergency Contact Group
            else if (sender.Equals(lblSaveEmergencyContact))
            {

                string checkString = String.Format("SELECT * FROM EMERGENCY_CONTACTS WHERE PATIENT_ID = {0}", patientId);
                string updateString = "";
                SqlCommand checkCommand = new SqlCommand(checkString, con);

                SqlDataReader reader = checkCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();

                    string[] columns = { "FULL_NAME", "RELATIONSHIP", "ADDRESS_STREET", "ADDRESS_STATE", "ADDRESS_CITY", "HOME_PHONE", "WORK_PHONE",
                                         "FAX_NUMBER", "EMAIL" };
                    string[] values = { txtEmergencyFullName.Text, txtRelationship.Text, txtEmergencyAddress.Text, txtEmergencyState.Text,
                        txtEmergencyCity.Text, txtEmergencyHomePhone.Text, txtEmergencyWorkPhone.Text, txtEmergencyFax.Text, txtEmergencyEmail.Text};

                    sqlUpdate(columns, values, "EMERGENCY_CONTACTS", con);

                }
                else
                {
                    reader.Close();

                    updateString = String.Format("INSERT INTO EMERGENCY_CONTACTS " +
                        "VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')", patientId, txtEmergencyFullName.Text, txtRelationship.Text, txtEmergencyAddress.Text,
                        txtEmergencyState.Text, txtEmergencyCity.Text, txtEmergencyHomePhone.Text, txtEmergencyWorkPhone.Text, txtEmergencyFax.Text, txtEmergencyEmail.Text);

                    SqlCommand updateCommand = new SqlCommand(updateString, con);

                    updateCommand.ExecuteNonQuery();

                }

                toggleGroup(grpEmergency);

            }
            //Primary care group
            else if (sender.Equals(lblSavePrimaryCare))
            {
                string getProvider = String.Format("Select PRIMARY_ID FROM PATIENT_TBL WHERE PATIENT_ID = {0}", patientId);
                SqlCommand providerCommand = new SqlCommand(getProvider, con);

                SqlDataReader reader = providerCommand.ExecuteReader();
                reader.Read();
                int providerId = reader.GetInt32(0);

                string checkQuery = String.Format("SELECT * FROM PRIMARY_CARE_TBL WHERE PRIMARY_ID = {0}", patientId);
                SqlCommand command = new SqlCommand(checkQuery, con);
                reader.Close();
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    string[] columns = { "NAME_LAST", "NAME_FISRT", "SPECIALTY", "PHONE_OFFICE", "PHONE_MOBILE", "FAX_NUMBER", "EMAIL" };
                    string[] values = { txtPrimaryName.Text, txtPrimaryName.Text, txtPrimarySpecialty.Text, txtPrimaryWork.Text,
                                        txtPrimaryMobile.Text, txtPrimaryFax.Text, txtPrimaryEmail.Text};

                    sqlUpdate(columns, values, "PRIMARY_CARE_TBL", con);

                }
                else
                {
                    int newValue = Validator.incrementField("PRIMARY_ID", "PRIMARY_CARE_TBL", con);
                    string[] name = txtPrimaryName.Text.Split(' ');
                    string insertStatement = String.Format("INSERT INTO PRIMARY_CARE_TBL VALUES({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", newValue, name[1], name[2], txtPrimarySpecialty.Text,
                                                            txtPrimaryWork.Text, txtPrimaryMobile.Text, txtPrimaryFax.Text, txtPrimaryEmail.Text);

                    SqlCommand insertCommand = new SqlCommand(insertStatement, con);

                    insertCommand.ExecuteNonQuery();

                }

                toggleGroup(grpPrimary);

            }
            //Personal medical details group
            else if (sender.Equals(lblSavePersonalMed))
            {
                string bloodType = cboBloodType.Text;
                string[] columns = { "BLOOD_TYPE", "ORGAN_DONOR", "HIV_STATUS", "HEIGHT_INCHES", "WEIGHT_LBS" };
                string[] values = { bloodType, chkOrganDonor.Checked.ToString(), txtHeight.Text, txtWeight.Text };

                toggleGroup(grpPersonalMedicalDetails);
            }
            //Allergy Details group
            else if (sender.Equals(lblSaveAllergies))
            {
                if (newEntry)
                {
                    DateTime onset = dtpAllergyOnset.Value;
                    string onsetDate = onset.ToString("yyyy-MM-dd");
                    string allergyId = Validator.incrementFieldString("ALLERGY_ID", "ALLERGY_TBL", con).ToString();
                    string[] toUpload = {
                                        allergyId,
                                        txtAllergicTo.Text, onsetDate, txtAllergyNote.Text };
                    sqlInsert(toUpload, "ALLERGY_TBL", patientId, con);
                }
                else
                {

                    string date = dtpAllergyOnset.Value.ToString("yyyy-MM-dd");
                    string[] columns = { "ALLERGEN", "ONSET_DATE", "NOTE" };
                    string[] values = { txtAllergicTo.Text, date, txtAllergyNote.Text };
                    sqlUpdate(columns, values, "ALLERGY_TBL", "ALLERGY_ID", allergyId[lstAllergies.SelectedIndex], con);
                    fillListBox(lstAllergies, "ALLERGY_TBL", "ALLERGEN", con);
                    cacheData(con);
                }
                toggleGroup(grpAllergies);
            }
            //Immunisation Details group
            else if (sender.Equals(lblSaveImmunisation))
            {
                if (newEntry)
                {
                    DateTime date = dtpTestDate.Value;
                    string immunizationDate = date.ToString("yyyy-MM-dd");
                    string immunizationId = Validator.incrementFieldString("IMMUNIZATION_ID", "IMMUNIZATION_TBL", con).ToString();
                    string[] toUpload = { immunizationId, txtImmunisation.Text, immunizationDate, txtImmunisationNote.Text };
                    sqlInsert(toUpload, "IMMUNIZATION_TBL", patientId, con);
                }
                else
                {

                    string date = dtpImmunizationDate.Value.ToString("yyyy-MM-dd");
                    string[] columns = { "IMMUNIZATION", "DATE", "NOTE" };
                    string[] values = { txtImmunisation.Text, date, txtImmunisationNote.Text };
                    sqlUpdate(columns, values, "IMMUNIZATION_TBL", "IMMUNIZATION_ID", immunizationId[lstImmunizations.SelectedIndex], con);
                    fillListBox(lstImmunizations, "IMMUNIZATION_TBL", "IMMUNIZATION", con);
                    cacheData(con);
                }
                toggleGroup(grpImmunization);
            }
            //Prescribed medicine group
            else if (sender.Equals(lblSaveMedication))
            {
                if (newEntry)
                {
                    DateTime date = dtpPrescribeDate.Value;
                    string dateString = date.ToString("yyyy-MM-dd");
                    string medId = Validator.incrementFieldString("MED_ID", "MEDICATION_TBL", con).ToString();
                    string[] toUpload = {
                                        medId,
                                        txtMedication.Text, dateString, chkCronicMedication.Checked.ToString(), txtMedicationNote.Text };
                    sqlInsert(toUpload, "MEDICATION_TBL", patientId, con);
                }
                else
                {

                    string date = dtpPrescribeDate.Value.ToString("yyyy-MM-dd");
                    string chronic = chkCronicMedication.Checked.ToString();
                    string[] columns = { "MEDICATION", "DATE", "CHRONIC", "NOTE" };
                    string[] values = { txtMedication.Text, date, chronic, txtMedicationNote.Text };
                    sqlUpdate(columns, values, "MEDICATION_TBL", "MED_ID", medId[lstMedication.SelectedIndex], con);

                }
                toggleGroup(grpMedication);
            }
            //Test Results group
            else if (sender.Equals(lblSaveTest))
            {
                if (newEntry)
                {

                    DateTime date = dtpTestDate.Value;
                    string dateString = date.ToString("yyyy-MM-dd");
                    string testId = Validator.incrementFieldString("TEST_ID", "TEST_TBL", con).ToString();
                    string[] toUpload = {
                                        testId,
                                        txtTest.Text, dateString, txtResult.Text,txtMedicationNote.Text };
                    sqlInsert(toUpload, "TEST_TBL", patientId, con);
                }
                else
                {
                    string date = dtpTestDate.Value.ToString("yyyy-MM-dd");
                    string[] columns = { "TEST", "DATE", "RESULT", "NOTE" };
                    string[] values = { txtTest.Text, date, txtResult.Text, txtTestNote.Text };
                    sqlUpdate(columns, values, "TEST_TBL", "TEST_ID", testId[lstTestResults.SelectedIndex], con);
                }
                    toggleGroup(grpTestResults);
            }
            //Medical Conditions group
            else if (sender.Equals(lblMedicalConditions))
            {
                if (newEntry)
                {
                    DateTime date = dtpConditionOnset.Value;
                    string dateString = date.ToString("yyyy-MM-dd");
                    string newId= Validator.incrementFieldString("CONDITION_ID", "CONDITION", con).ToString();
                    string[] toUpload = {
                                        newId,
                                        txtCondition.Text, dateString, rdoChronic.Checked.ToString(), txtConditionNote.Text       };
                    sqlInsert(toUpload, "CONDITION_ID", patientId, con);
                }
                else
                {

                    string date = dtpConditionOnset.Value.ToString("yyyy-MM-dd");
                    bool chronic = rdoChronic.Checked ? true : false;
                    string[] columns = { "CONDITION", "ONSET_DATE", "CHRONIC", "NOTE" };
                    string[] values = { txtCondition.Text, date, chronic.ToString(), txtConditionNote.Text };
                    sqlUpdate(columns, values, "CONDITION", con);
                }
                toggleGroup(grpMedicalCondition);
            }
            //Medical Procedure group
            else if (sender.Equals(lblSaveProcedure))
            {
                if (newEntry)
                {
                    DateTime date = dtpProcedureDate.Value;
                    string dateString = date.ToString("yyyy-MM-dd");
                    string newId = Validator.incrementFieldString("PROCEDURE_ID", "MED_PROC_TBL", con).ToString();
                    string[] toUpload = {
                                        newId,
                                        txtProcedure.Text, dateString, txtPerformedBy.Text, txtProcedureNote.Text };
                    sqlInsert(toUpload, "PROCEDURE_ID", patientId, con);
                }
                else
                {
                    string date = dtpProcedureDate.Value.ToString("yyyy-MM-dd");
                    bool chronic = chkCronicMedication.Checked;
                    string[] columns = { "MED_PROCEDURE", "DATE", "CHRONIC", "NOTE" };
                    string[] values = { txtProcedure.Text, date, chronic.ToString(), txtProcedureNote.Text};
                    
                        sqlUpdate(columns, values, "MED_PROC_TBL", "PROCEDURE_ID", procedureId[lstProcedures.SelectedIndex], con);
                    

                }

                toggleGroup(grpMedicalProcedures);
            }

            cacheData(con);
            newEntry = false;
            
            con.Close();

            con.Dispose();

        }
        
        private void addClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Object.Equals(sender, lblAddAllergy))
            {
                newIndex  = lstAllergies.Items.Add("New Allergy");
                txtAllergicTo.Text = "New Allergy";
                txtAllergyNote.Text = "New Allergen note";
                dtpAllergyOnset.Value = DateTime.Now;
                newEntry = true;
                toggleGroup(grpAllergies);
            }
            else if (Object.Equals(sender, lblAddImmunisation))
            {
                lstImmunizations.Items.Add("New Immunization");
                txtAllergicTo.Text = "New Immunization";
                txtImmunisationNote.Text = "New Immunization note";
                dtpImmunizationDate.Value = DateTime.Now;
                newEntry = true;
                toggleGroup(grpImmunization);
            }
            else if (Object.Equals(sender, lblAddMed))
            {
                lstImmunizations.Items.Add("New Medication");
                txtAllergicTo.Text = "New Medication";
                txtMedicationNote.Text = "New Medication note";
                dtpPrescribeDate.Value = DateTime.Now;
                chkCronicMedication.Checked = false;
                newEntry = true;
                toggleGroup(grpMedication);
            }
            else if (Object.Equals(sender, lblAddMedCondition))
            {
                lstConditions.Items.Add("New Condition");
                txtCondition.Text = "New Condition";
                txtConditionNote.Text = "New Condition Note";
                dtpConditionOnset.Value = DateTime.Now;
                rdoAcute.Checked = false;
                rdoChronic.Checked = false;
                newEntry = true;
                toggleGroup(grpMedicalCondition);
            }
            else if (Object.Equals(sender, lblAddTest))
            {
                lstTestResults.Items.Add("New Test Result");
                txtTest.Text = "New Test";
                txtTestNote.Text = "New Test Note";
                txtResult.Text = "New Test Result";
                newEntry = true;
                dtpTestDate.Value = DateTime.Now;
                toggleGroup(grpTestResults);
            }
            else if (Object.Equals(sender, lblAddProcedure))
            {
                lstProcedures.Items.Add("New Procedure");
                txtProcedure.Text = "New Procedure";
                txtPerformedBy.Text = "New Doctor";
                txtProcedureNote.Text = "New Procedure Note";
                dtpProcedureDate.Value = DateTime.Now;
                newEntry = true;
                toggleGroup(grpMedicalProcedures);
            }
        }

        private void removeClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\Tanner\\source\\repos\\Gordon_PCHR\\Gordon_PCHR\\pchr42563.mdf; Integrated Security = True; Connect Timeout = 30";
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            if (Object.Equals(sender, lblRemoveAllergy))
            {
                if (lstAllergies.SelectedIndex < 0) { return; }

                sqlDelete("ALLERGY_TBL", "ALLERGY_ID", lstAllergies.SelectedIndex + 1, con);
                
            }

            cacheData(con);
            con.Close();
            con.Dispose();
        }


        public void listSelectionChanged(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            if (box.SelectedIndex < 0 ) { return; }
            if (Object.Equals(sender, lstAllergies))
            {
                txtAllergyNote.Text = allergenNotes[lstAllergies.SelectedIndex];
                dtpAllergyOnset.Value = allergenOnsets[lstAllergies.SelectedIndex];
                txtAllergicTo.Text = lstAllergies.SelectedItem.ToString();
            }
            else if(Object.Equals(sender, lstMedication))
            {
                txtMedication.Text = lstMedication.SelectedItem.ToString();
                dtpPrescribeDate.Value = prescribedDates[lstMedication.SelectedIndex];
                chkCronicMedication.Checked = chronicMedication[lstMedication.SelectedIndex];
                txtMedicationNote.Text = medNotes[lstMedication.SelectedIndex];
            }
            else if(Object.Equals(sender, lstImmunizations))
            {
                txtImmunisation.Text = lstImmunizations.SelectedItem.ToString();
                dtpImmunizationDate.Value = immunisationDates[lstImmunizations.SelectedIndex];
                txtImmunisationNote.Text = immunisationNotes[lstImmunizations.SelectedIndex];
            }
            else if(Object.Equals(sender, lstTestResults))
            {
                dtpTestDate.Value = testDates[lstTestResults.SelectedIndex];
                txtTestNote.Text = testNotes[lstTestResults.SelectedIndex];
                txtTest.Text = lstTestResults.SelectedItem.ToString();
                txtResult.Text = testResults[lstTestResults.SelectedIndex];
            }
            else if(Object.Equals(sender, lstConditions))
            {
                txtCondition.Text = lstConditions.SelectedIndex.ToString();
                dtpConditionOnset.Value = condOnset[lstConditions.SelectedIndex];
                txtConditionNote.Text = condNotes[lstConditions.SelectedIndex];

            }
            else if(Object.Equals(sender, lstProcedures))
            {
                txtProcedure.Text = lstProcedures.SelectedIndex.ToString();
                txtPerformedBy.Text = procedurePerform[lstProcedures.SelectedIndex];
                dtpProcedureDate.Value = procedureDates[lstProcedures.SelectedIndex];
                txtProcedureNote.Text = procedureNotes[lstProcedures.SelectedIndex];
            }
        }


        /// <summary>
        /// Builds a sql command from the supplied parameters to update the given
        /// columns with the given values
        /// </summary>
        /// <param name="columns">The columns you are uploading to</param>
        /// <param name="values">The values to updload</param>
        /// <param name="tableName">The table to update</param>
        /// <param name="con">The SqlConnection to use</param>
        /// <returns></returns>
        public int sqlUpdate(string[] columns, string[] values, string tableName, SqlConnection con)
        {
            string updateCommand = "UPDATE " + tableName + " SET ";

            for (int i = 0; i < columns.Length; i++)
            {
                string temp;
                if (i + 1 < columns.Length)
                {
                    temp = String.Format("{0} = '{1}', ", columns[i], values[i]);
                }else
                {
                    temp = String.Format("{0} = '{1}' where PATIENT_ID = {2}", columns[i], values[i], patientId);
                }
                updateCommand += temp;

            }

            SqlCommand command = new SqlCommand(updateCommand, con);
            int rowsAffected = command.ExecuteNonQuery();


            return rowsAffected;
        }

        public int sqlUpdate(string[] columns, string[] values, string tableName, string key, string keyId, SqlConnection con)
        {
            string updateCommand = "UPDATE " + tableName + " SET ";

            for (int i = 0; i < columns.Length; i++)
            {
                string temp;
                if (i + 1 < columns.Length)
                {
                    temp = String.Format("{0} = '{1}', ", columns[i], values[i]);
                }
                else
                {
                    temp = String.Format("{0} = '{1}' where {2} = {3}", columns[i], values[i], key, keyId);
                }
                updateCommand += temp;

            }

            SqlCommand command = new SqlCommand(updateCommand, con);
            int rowsAffected = command.ExecuteNonQuery();


            return rowsAffected;
        }

        public int sqlInsert(string[] values, string tableName, int key, SqlConnection con)
        {
            string keyString = toNchar(key);
            string insertString = String.Format("INSERT INTO {0} VALUES('{1}', ", tableName, keyString );

            for (int i = 0; i < values.Length; i++)
            {
                if (i + 1 == values.Length)
                {
                    insertString += String.Format("'{0}');", values[i]);
                }
                else
                {
                    insertString += String.Format("'{0}', ", values[i]);
                }
            }
            
            SqlCommand command = new SqlCommand(insertString, con);
            int affectedRows = command.ExecuteNonQuery();
            
            return affectedRows;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName">The table that you are deleting from</param>
        /// <param name="key">The key that identifies the row to delete</param>
        /// <param name="keyId">The name of the column that contains key</param>
        /// <param name="con">Sql connection to use</param>
        /// <returns></returns>
        public int sqlDelete(string tableName, string keyId, int key, SqlConnection con)
        {
            string commandString = String.Format("DELETE FROM {0} WHERE {1} = {2}", tableName, keyId, key);
            SqlCommand deleteCommand = new SqlCommand(commandString, con);
            return deleteCommand.ExecuteNonQuery();

        }
        /// <summary>
        /// Adds leading zeros to an int and returns it as a string to mock a sql datatype (nChar)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string toNchar(int number)
        {
            string baseString = number.ToString();
            int digits = baseString.Length;

            int leadingZeros = 7 - digits;

            if ( digits < 1)
            {
                return baseString;

            }
                return baseString.PadLeft(leadingZeros, '0');
            
        }

        private void tabChange(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                fillPersonal();
            } else if (tabControl.SelectedIndex == 1) {
                fillMedical();
            }
        }



        private void fillPersonal()
        {
            string title, gender;
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Tanner\\source\\repos\\Gordon_PCHR\\Gordon_PCHR\\pchr42563.mdf;Integrated Security=True;Connect Timeout=30";
            string commandString = String.Format("Select * from PATIENT_TBL WHERE PATIENT_ID = {0}", patientId);

            txtIdNumber.Text = patientId.ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandString, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    txtLastName.Text = reader.GetString(1);
                    txtFirstName.Text = reader.GetString(2);

                    dtpDOB.Value = reader.GetDateTime(3);
                    txtAddress.Text = reader.GetString(4);
                    txtCity.Text = reader.GetString(5);
                    txtState.Text = reader.GetString(6);
                    txtPostalCode.Text = reader.GetString(7);
                    txtHomePhone.Text = reader.GetString(8);
                    txtMobilePhone.Text = reader.GetString(9);
                    int id = reader.GetInt32(10);
                    gender = reader.GetString(11);
                    title = reader.GetString(12);
                    txtInsurer.Text = reader.GetString(13);
                    txtInsurancePlan.Text = reader.GetString(14);
                    txtInsuranceNumber.Text = reader.GetInt32(15).ToString();
                    txtSuburb.Text = reader.GetString(16);
                    reader.Close();

                    string primaryString = String.Format("Select * from PRIMARY_CARE_TBL where PRIMARY_ID = {0}", id);

                    SqlCommand primaryCommand = new SqlCommand(primaryString, con);

                    reader = primaryCommand.ExecuteReader();
                    reader.Read();
                    txtPrimaryName.Text = reader.GetString(2) + reader.GetString(1);
                    txtPrimarySpecialty.Text = reader.GetString(4);
                    txtPrimaryWork.Text = reader.GetString(5);
                    txtPrimaryMobile.Text = reader.GetInt32(6).ToString();

                    reader.Close();


                    string emergencyString = String.Format("Select * from EMERGENCY_CONTACTS where PATIENT_ID = {0}", patientId);
                    SqlCommand emergencyContactCmd = new SqlCommand(emergencyString, con);
                    SqlDataReader emergencyReader = emergencyContactCmd.ExecuteReader();

                    emergencyReader.Read();

                    txtEmergencyFullName.Text = emergencyReader.GetString(1);
                    txtRelationship.Text = emergencyReader.GetString(2);
                    txtEmergencyAddress.Text = emergencyReader.GetString(3);
                    txtEmergencyState.Text = emergencyReader.GetString(4);
                    txtEmergencyCity.Text = emergencyReader.GetString(5);
                    txtEmergencyHomePhone.Text = emergencyReader.GetInt32(6).ToString();
                    txtEmergencyWorkPhone.Text = emergencyReader.GetInt32(7).ToString();
                    txtEmergencyFax.Text = emergencyReader.GetInt32(8).ToString();
                    txtEmergencyEmail.Text = emergencyReader.GetString(9);
                    txtEmergencyMobile.Text = emergencyReader.GetInt32(10).ToString();
                    txtEmergencyPostalCode.Text = emergencyReader.GetInt32(11).ToString();


                    con.Close();

                }

            }

            if (String.Equals(title, "Mr."))
            {
                cboTitle.SelectedIndex = 1;
            } else if (String.Equals(title, "Mrs."))
            {
                cboTitle.SelectedIndex = 2;
            }
            else
            {
                cboTitle.SelectedIndex = 0;
            }

            if (String.Equals(gender, "Male"))
            {
                rdoMale.Checked = true;
            }
            else if (String.Equals(gender, "Female"))
            {
                rdoFemale.Checked = true;
            }

        }

        private void fillMedical()
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Tanner\\source\\repos\\Gordon_PCHR\\Gordon_PCHR\\pchr42563.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            string selectString = String.Format("SELECT * FROM PER_DETAILS_TBL WHERE PATIENT_ID = {0}", patientId);
            SqlCommand com = new SqlCommand(selectString, con);
            SqlDataReader reader = com.ExecuteReader();


            //NOTE: I wasnt happy with the way this looked. I had planned to find a way to automate the 
            //null checking and spent a lot of time coding for it.
            if (reader.HasRows)
            {
                reader.Read();
                if (!reader.IsDBNull(1)) cboBloodType.Text = reader.GetString(1);
                if (!reader.IsDBNull(2)) chkOrganDonor.Checked = reader.GetBoolean(2);

                if (!reader.IsDBNull(4)) txtHeight.Text = reader.GetInt16(4).ToString();
                if (!reader.IsDBNull(5)) txtWeight.Text = reader.GetInt16(5).ToString();
                bool hivStatus = false;

                if (!reader.IsDBNull(3))
                    hivStatus = reader.GetBoolean(3);
                else rdoUnknown.Checked = true;

                if (!rdoUnknown.Checked)
                {


                    if (!hivStatus)
                    {
                        rdoNegative.Checked = true;
                    }
                    else
                    {
                        rdoPositive.Checked = true;
                    }
                }
                reader.Close();


            }




            cacheData(con);

            fillListBox(lstAllergies, "ALLERGY_TBL", "ALLERGEN", con);
            fillListBox(lstImmunizations, "IMMUNIZATION_TBL", "IMMUNIZATION", con);
            fillListBox(lstMedication, "MEDICATION_TBL", "MEDICATION", con);
            fillListBox(lstTestResults, "TEST_TBL", "TEST", con);
            fillListBox(lstConditions, "CONDITION", "CONDITION", con);
            fillListBox(lstProcedures, "MED_PROC_TBL", "MED_PROCEDURE", con);




            con.Close();
            con.Dispose();
        }

        private string[] cacheStrings(string field, string table, SqlConnection con)
        {
            List<string> cache = new List<string>();

            string cacheString = String.Format("SELECT {0} FROM {1} WHERE PATIENT_ID = {2}", field, table, patientId);
            SqlCommand command = new SqlCommand(cacheString, con);
            SqlDataReader reader = command.ExecuteReader();

   

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    cache.Add(reader.GetString(0));
                }
            }
            reader.Close();
            return cache.ToArray();
        }

        private DateTime[] cacheDates(string field, string table, SqlConnection con)
        {
            string cacheString = String.Format("SELECT {0} FROM {1} WHERE PATIENT_ID = {2}", field, table, patientId);
            List<DateTime> cache = new List<DateTime>();

            SqlCommand cacheCommand = new SqlCommand(cacheString, con);
            SqlDataReader reader = cacheCommand.ExecuteReader();

            if (reader.HasRows)
            {
                while(reader.Read())
                {
                    cache.Add(reader.GetDateTime(0));
                }
            }
            reader.Close();
            cacheCommand.Dispose();
            reader.Dispose();
            return cache.ToArray();
        }

        private bool[] cacheBool(string field, string table, SqlConnection con) 
        {
            string cacheString = String.Format("SELECT {0} FROM {1} WHERE PATIENT_ID = {2}", field, table, patientId);
            List<bool> cache = new List<bool>();
            SqlCommand cacheCommand = new SqlCommand(cacheString, con);
            SqlDataReader reader = cacheCommand.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    cache.Add(reader.GetBoolean(0));
                }
            }
            reader.Close();
            cacheCommand.Dispose();
            reader.Dispose();
            return cache.ToArray();
        }

        private void cacheData(SqlConnection con)
        {
            allergenNotes = cacheStrings("NOTE", "ALLERGY_TBL", con);
            allergenOnsets = cacheDates("ONSET_DATE", "ALLERGY_TBL", con);
            immunisationNotes = cacheStrings("NOTE", "IMMUNIZATION_TBL", con);
            immunisationDates = cacheDates("DATE", "IMMUNIZATION_TBL", con);
            medNotes = cacheStrings("NOTE", "MEDICATION_TBL", con);
            prescribedDates = cacheDates("DATE", "MEDICATION_TBL", con);
            chronicMedication = cacheBool("CHRONIC", "MEDICATION_TBL", con);
            testNotes = cacheStrings("NOTE", "TEST_TBL", con);
            testDates = cacheDates("DATE", "TEST_TBL", con);
            testResults = cacheStrings("RESULT", "TEST_TBL", con);
            condNotes = cacheStrings("NOTE", "CONDITION", con);
            condOnset = cacheDates("ONSET_DATE", "CONDITION", con);
            chronicCondition = cacheBool("CHRONIC", "CONDITION", con);
            procedureNotes = cacheStrings("NOTE", "MED_PROC_TBL", con);
            procedureDates = cacheDates("DATE", "MED_PROC_TBL", con);
            
            allergyId = cacheStrings("ALLERGY_ID", "ALLERGY_TBL", con);
            immunizationId = cacheStrings("IMMUNIZATION_ID", "IMMUNIZATION_TBL", con);
            procedureId = cacheStrings("PROCEDURE_ID", "MED_PROC_TBL", con);
            medId = cacheStrings("MED_ID", "MEDICATION_TBL", con);
            testId = cacheStrings("TEST_ID", "TEST_TBL", con);
            conditionId = cacheStrings("CONDITION_ID", "CONDITION", con);
            
        }

        private void toggleGroup(GroupBox group)
        {

            for (int i = 0; i < group.Controls.Count; i++)
            {
                List<LinkLabel> linkLabels = new List<LinkLabel>();
                bool isLabel = Object.ReferenceEquals(group.Controls[i].GetType(), typeof(Label));
                bool isListBox = Object.ReferenceEquals(group.Controls[i].GetType(), typeof(ListBox));
                if (isLabel)
                {
                    continue;
                }

                group.Controls[i].Enabled = !group.Controls[i].Enabled;

            }

        }
        /// <summary>
        /// I coded this function in less time than it would have taken me to figure out
        /// the visual basic list box query, so...
        /// </summary>
        /// <param name="box">The box to fill</param>
        /// <param name="tableName">The table to query</param>
        /// <param name="fillProperty">The column to insert into the listbox</param>
        private void fillListBox(ListBox box, string tableName,  string fillProperty, SqlConnection con)
        {
            box.Items.Clear();
            string query = String.Format("SELECT {0} FROM {1} WHERE PATIENT_ID = {2}", fillProperty, tableName, patientId);
            SqlCommand command = new SqlCommand(query, con);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string item = reader.GetString(0);
                    box.Items.Add(item);
                }
            }
            if (box.Items.Count > 0) box.SelectedIndex = 0;


            reader.Close();
            return;
        }

    }
}
