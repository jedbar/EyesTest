using EyesTest.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Schema;

namespace EyesTest
{
    /// <summary>
    /// Interaction logic for PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        /// <summary>
        /// A patient object
        /// </summary>
        private ClientModel client;
        /// <summary>
        /// A main window object
        /// </summary>
        private MainWindow mainWindow;
        /// <summary>
        /// An appointment object
        /// </summary>
        private AppointmentModel appointment = new AppointmentModel();

        /// <summary>
        /// List that holds examination objects
        /// </summary>
        public List<ExaminationModel> ExaminationList = new List<ExaminationModel>();

        /// <summary>
        /// List that holds appointment objects
        /// </summary>
        public List<AppointmentModel> AppointmentList = new List<AppointmentModel>();



        /// <summary>
        /// Default constructor for a new patient window
        /// </summary>
        /// <param name="c">ClientModel object</param>
        /// <param name="p">MainWindow object</param>
        public PatientWindow(ClientModel c, MainWindow p)
        {
            InitializeComponent();

            client = c;
            mainWindow = p;

            DataContext = client;

            Title += " - " + client.FirstName + " " + client.LastName;

            readOnlyPatientForm(true);

            ExaminationModel examination = new ExaminationModel();
            ExaminationList.AddRange(examination.GetHistory(client.ID));
            MyDataGridHistory.ItemsSource = ExaminationList;

            DateTime today = DateTime.Today;
            AppointmentList.AddRange(appointment.FindAppointments(today.ToString("yyyy-MM-dd")));
            MyDataGridPlan.ItemsSource = AppointmentList;

        }

        /// <summary>
        /// Method closes patient's window    
        /// </summary>
        /// <param name="sender">control that fired the event</param>
        /// <param name="e">events arguments</param>
        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary> 
        /// Method deletes selected examination and updates examination's list 
        /// </summary>
        /// <param name="sender">control that fired the event</param>
        /// <param name="e">events arguments</param>
        private void MenuExaminationDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy napewno chcesz usunąć wybrane badanie?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                
                var exam = (ExaminationModel)MyDataGridHistory.SelectedItem;
                exam.Delete();
                ExaminationList.Remove(exam);
                MyDataGridHistory.Items.Refresh();
                MyDataGridHistoryDetailsD.Items.Clear();
                MyDataGridHistoryDetailsN.Items.Clear();
    
            }
        }

        /// <summary>
        /// Method deletes selected patient and update patient's list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuPatientDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy napewno chcesz usunąć wszystkie dane pacjenta, " + client.FirstName + " " + client.LastName + " ?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    client.Delete();
                    mainWindow.ClientList.Remove(client);
                    mainWindow.MyDataGridFind.Items.Refresh();

                    List<AppointmentModel> aList = mainWindow.AppointmentList.FindAll(a => a.ClientID == client.ID);
                    foreach (AppointmentModel a in aList)
                    {
                        mainWindow.AppointmentList.Remove(a);
                    }
                    mainWindow.MyDataGridApp.Items.Refresh();

                    if (mainWindow.ClientList.Count == 0)
                    {
                        mainWindow.clearFindForm();
                        mainWindow.enableFindForm(true);
                    }
                    client = null;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        /// <summary>
        /// Method sets readonly attribute to a group of text controls based on param.
        /// </summary>
        /// <param name="flag">true or false</param>
        private void readOnlyPatientForm(bool flag)
        {
            txtFirstName.IsReadOnly = flag;
            txtLastName.IsReadOnly = flag;
            txtPesel.IsReadOnly = flag;
            txtPhoneNumber.IsReadOnly = flag;
            txtStreet.IsReadOnly = flag;
            txtCity.IsReadOnly = flag;
            txtZipCode.IsReadOnly = flag;
        }

        /// <summary>
        /// Method calls readOnlyPatientForm() with false param
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPatientEdit_Checked(object sender, RoutedEventArgs e)
        {
            readOnlyPatientForm(false);
        }

        /// <summary>
        ///  Method calls readOnlyPatientForm() with true param
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkPatientEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            readOnlyPatientForm(true);
        }


        /// <summary>
        /// Method validates and updates the patient and calls sets the form back to readonly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <see cref="readOnlyPatientForm()"/>
        private void btnPatientSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Validate();
                client.Update();
                readOnlyPatientForm(true);
                chkPatientEdit.IsChecked = false;
                MessageBox.Show("Zmiany zostały zapisane.", "Potwierdzenie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        /// <summary>
        /// Method displays examinations details when examination is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyDataGridHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuExaminationDelete.IsEnabled = true;
            try
            {
                DataGrid dataGrid = (DataGrid)e.Source;
                var exam = (ExaminationModel)MyDataGridHistory.SelectedItem;

                MyDataGridHistoryDetailsD.Items.Clear();
                MyDataGridHistoryDetailsN.Items.Clear();

                if (exam != null)
                {
                    if (exam.Tests.Count == 0)
                    {
                        exam.Tests.AddRange(exam.GetTests());
                    }

                    foreach (EyeTestModel test in exam.Tests)
                    {
                        if (test.Vision.StartsWith("D"))
                        {
                            MyDataGridHistoryDetailsD.Items.Add(test);
                        }
                        else
                        {
                            MyDataGridHistoryDetailsN.Items.Add(test);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method validates and adds a new examination record with up to 4 different eye tests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSubmit_Click(object sender, RoutedEventArgs e)
        {

            ExaminationModel examination = new ExaminationModel();
            DateTime today = DateTime.Now;
            examination.Timestamp = today.ToString("yyyy-MM-dd hh:mm:ss");
            examination.ClientID = client.ID;

            bool vFlag = false;
            if (txtSphRD.Text.Trim().Length + 
                txtCylRD.Text.Trim().Length +
                txtAxRD.Text.Trim().Length + 
                txtPdRD.Text.Trim().Length > 0)
            {
                EyeTestModel eyeTestRD = new EyeTestModel();
                eyeTestRD.ClientID = client.ID;
                eyeTestRD.Eye = "R";
                eyeTestRD.Vision = "D";

                float parsedFloat;
                int parsedInt;
                eyeTestRD.Sph = float.TryParse(txtSphRD.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestRD.Cyl = float.TryParse(txtCylRD.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestRD.Ax = int.TryParse(txtAxRD.Text, out parsedInt) ? parsedInt : -1;
                eyeTestRD.Pd = int.TryParse(txtPdRD.Text, out parsedInt) ? parsedInt : -1;

                try
                {
                    eyeTestRD.Validate();
                    examination.Tests.Add(eyeTestRD);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    vFlag = true;
                }
            }




            if (txtSphLD.Text.Trim().Length + 
                txtCylLD.Text.Trim().Length + 
                txtAxLD.Text.Trim().Length + 
                txtPdLD.Text.Trim().Length > 0  && !vFlag)
            {
                EyeTestModel eyeTestLD = new EyeTestModel();
                eyeTestLD.ClientID = client.ID;
                eyeTestLD.Eye = "L";
                eyeTestLD.Vision = "D";

                float parsedFloat;
                int parsedInt;
                eyeTestLD.Sph = float.TryParse(txtSphLD.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestLD.Cyl = float.TryParse(txtCylLD.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestLD.Ax = int.TryParse(txtAxLD.Text, out parsedInt) ? parsedInt : -1;
                eyeTestLD.Pd = int.TryParse(txtPdLD.Text, out parsedInt) ? parsedInt : -1;

                try
                {
                    eyeTestLD.Validate();
                    examination.Tests.Add(eyeTestLD);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    vFlag = true;
                }
            }





            if (txtSphRN.Text.Trim().Length + 
                txtCylRN.Text.Trim().Length + 
                txtAxRN.Text.Trim().Length + 
                txtPdRN.Text.Trim().Length > 0 && !vFlag)
            {
                EyeTestModel eyeTestRN = new EyeTestModel();
                eyeTestRN.ClientID = client.ID;
                eyeTestRN.Eye = "R";
                eyeTestRN.Vision = "N";

                float parsedFloat;
                int parsedInt;
                eyeTestRN.Sph = float.TryParse(txtSphRN.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestRN.Cyl = float.TryParse(txtCylRN.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestRN.Ax = int.TryParse(txtAxRN.Text, out parsedInt) ? parsedInt : -1;
                eyeTestRN.Pd = int.TryParse(txtPdRN.Text, out parsedInt) ? parsedInt : -1;

                try
                {
                    eyeTestRN.Validate();
                    examination.Tests.Add(eyeTestRN);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    vFlag = true;
                }
            }




            if (txtSphLN.Text.Trim().Length + 
                txtCylLN.Text.Trim().Length + 
                txtAxLN.Text.Trim().Length + 
                txtPdLN.Text.Trim().Length > 0 && !vFlag)
            {
                EyeTestModel eyeTestLN = new EyeTestModel();
                eyeTestLN.ClientID = client.ID;
                eyeTestLN.Eye = "L";
                eyeTestLN.Vision = "N";

                float parsedFloat;
                int parsedInt;
                eyeTestLN.Sph = float.TryParse(txtSphLN.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestLN.Cyl = float.TryParse(txtCylLN.Text, out parsedFloat) ? parsedFloat : 100;
                eyeTestLN.Ax = int.TryParse(txtAxLN.Text, out parsedInt) ? parsedInt : -1;
                eyeTestLN.Pd = int.TryParse(txtPdLN.Text, out parsedInt) ? parsedInt : -1;

                try
                {
                    eyeTestLN.Validate();
                    examination.Tests.Add(eyeTestLN);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                    vFlag = true;
                }
            }






            if (examination.Tests.Count > 0 & !vFlag)
            {
                try
                {
                    int examinationID = examination.Add(client.ID);
                    examination.ID = examinationID;
                    foreach (EyeTestModel test in examination.Tests)
                    {
                        test.ExaminationID = examinationID;
                        test.Save();
                    }
                    ExaminationList.Add(examination);
                    ExaminationList.Reverse();
                    clearAddExaminationForm();
                    lblAddStatus.Content = "Dodano nowe badanie o ID " + examinationID;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
                MyDataGridHistory.Items.Refresh();

            }

        }

        /// <summary>
        /// Method enables/disables menu item used to delete records
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyTabItem3.IsSelected && MyDataGridHistory.SelectedItem != null)
            {
                MenuExaminationDelete.IsEnabled = true;
            }
            else
            {
                MenuExaminationDelete.IsEnabled = false;
            }

            if (MyTabItem4.IsSelected && MyDataGridPlan.SelectedItem != null)
            {
                MenuAppointmentDelete.IsEnabled = true;
            }
            else
            {
                MenuAppointmentDelete.IsEnabled = false;
            }
        }

        /// <summary>
        /// Method clears the value of the examination form and it's status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <see cref="clearAddExaminationForm()"/>
        private void btnAddReset_Click(object sender, RoutedEventArgs e)
        {
            clearAddExaminationForm();
            lblAddStatus.Content = "";
        }

        /// <summary>
        /// Method clears examination text controls.
        /// </summary>
        private void clearAddExaminationForm()
        {
            txtSphRD.Clear();
            txtCylRD.Clear();
            txtAxRD.Clear();
            txtPdRD.Clear();

            txtSphLD.Clear();
            txtCylLD.Clear();
            txtAxLD.Clear();
            txtPdLD.Clear();

            txtSphRN.Clear();
            txtCylRN.Clear();
            txtAxRN.Clear();
            txtPdRN.Clear();

            txtSphLN.Clear();
            txtCylLN.Clear();
            txtAxLN.Clear();
            txtPdLN.Clear();
        }

        /// <summary>
        /// Method adds a new appointment with selected time and date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (!MyCalendar.SelectedDate.HasValue)
            {
                MyCalendar.SelectedDate = DateTime.Now;
            }
            DateTime calendarDate = MyCalendar.SelectedDate.Value;
                
            string tmp = calendarDate.ToString("yyyy-MM-dd") + " " + cbHour.Text + ":" + cbMinute.Text + ":00";

            DateTime timestamp = DateTime.Parse(tmp);
            AppointmentModel app = new AppointmentModel();
            app.ClientID = client.ID;
            app.Timestamp = timestamp;
            app.Client = client;
            app.ID = app.Add();
            AppointmentList.Add(app);
            MyDataGridPlan.Items.Refresh();
            cbHour.SelectedIndex = -1;
            cbMinute.SelectedIndex = -1;
            btnAddAppointment.IsEnabled = false;

            if (mainWindow.MyCalendarPV.SelectedDate != null && mainWindow.MyCalendarPV.SelectedDate.Value == calendarDate)
            {
                mainWindow.AppointmentList.Add(app);
                mainWindow.MyDataGridApp.Items.Refresh();
            }

        }

        /// <summary>
        /// Method updates the appointment's list based on selected date and calls enableAddAppointment()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyCalendar.SelectedDate.HasValue)
            {
                DateTime date = MyCalendar.SelectedDate.Value;
                AppointmentList.Clear();
                AppointmentList.AddRange(appointment.FindAppointments(date.ToString("yyyy-MM-dd")));
                MyDataGridPlan.Items.Refresh();
            }
            enableAddAppointment();
        }

        /// <summary>
        /// Method calls enableAddAppointment() when hours are set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMinute != null)
            {
                enableAddAppointment();
            }
        }

        /// <summary>
        /// Method calls enableAddAppointment() when minutes are set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbHour != null)
            {
                enableAddAppointment();
            }
        }

        /// <summary>
        /// Method enables the add appointmet button when requirements are met
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enableAddAppointment()
        {
            if (cbHour.SelectedIndex >= 0 && cbMinute.SelectedIndex >= 0 && MyCalendar.SelectedDate.HasValue)
            {
                btnAddAppointment.IsEnabled = true;
            }
            else
            {
                btnAddAppointment.IsEnabled = false;
            }
        }

        /// <summary>
        /// Method deletes an appointment and updates appointment's list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuAppointmentDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy napewno chcesz usunąć wybrany termin?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {

                AppointmentModel app = (AppointmentModel)MyDataGridPlan.SelectedItem;
                
                AppointmentList.Remove(app);
                MyDataGridPlan.Items.Refresh();
                app.Delete();
                if (mainWindow.MyCalendarPV.SelectedDate != null && mainWindow.MyCalendarPV.SelectedDate.Value == MyCalendar.SelectedDate.Value)
                {
                    AppointmentModel appMW = mainWindow.AppointmentList.Find(a => a.ID == app.ID);
                    mainWindow.AppointmentList.Remove(appMW);
                    mainWindow.MyDataGridApp.Items.Refresh();
                }
            }
        }
    }
}
