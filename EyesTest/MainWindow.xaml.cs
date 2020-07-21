using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EyesTest.Models;

namespace EyesTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// A patient object
        /// </summary>
        private ClientModel client = new ClientModel();
        /// <summary>
        /// An appointment object
        /// </summary>
        private AppointmentModel appointment = new AppointmentModel();

        /// <summary>
        /// List that holds appointment objects
        /// </summary>
        public List<AppointmentModel> AppointmentList = new List<AppointmentModel>();
        /// <summary>
        /// List that holds patient objects
        /// </summary>
        public List<ClientModel> ClientList = new List<ClientModel>();
        
        /// <summary>
        /// Default controller for main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();


            //ClientList.AddRange(client.FindByLastName("j"));
            MyDataGridFind.ItemsSource = ClientList;

            DateTime today = DateTime.Today;
            AppointmentList.AddRange(appointment.FindAppointments(today.ToString("yyyy-MM-dd")));
            MyDataGridApp.ItemsSource = AppointmentList;


            //client = ClientList[0];
            //PatientWindow patientWindow = new PatientWindow(client, this);
            //patientWindow.Show();

        }

        /// <summary>
        /// Method opens a new patient's window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyGridFind_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                client = (ClientModel)MyDataGridFind.SelectedItem;

                PatientWindow patientWindow = new PatientWindow(client, this);
                patientWindow.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Method opens a new patient's window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyGridApp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                appointment = (AppointmentModel)MyDataGridApp.SelectedItem;

                PatientWindow patientWindow = new PatientWindow(appointment.Client, this);
                patientWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>   
        private void AppExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Method searches for and updates the list of patients based on the entered ID number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtId.Text.Length > 0)
            {
                txtPesel.IsEnabled = false;
                txtLastName.IsEnabled = false;

                if (Regex.IsMatch(txtId.Text, "^[0-9]+$"))
                {
                    ClientList.Clear();
                    ClientList.AddRange(client.FindById(txtId.Text));
                    MyDataGridFind.Items.Refresh();
                }
                else
                {
                    txtId.Clear();
                }
            }
            else
            {
                txtPesel.IsEnabled = true;
                txtLastName.IsEnabled = true;
                ClientList.Clear();
                MyDataGridFind.Items.Refresh();
            }
        }

        /// <summary>
        /// Method searches for and updates the list of patients based on the entered pesel number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPesel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPesel.Text.Length > 0)
            {
                txtId.IsEnabled = false;
                txtLastName.IsEnabled = false;

                if (Regex.IsMatch(txtPesel.Text, "^[0-9]+$"))
                {
                    ClientList.Clear();
                    ClientList.AddRange(client.FindByPesel(txtPesel.Text));
                    MyDataGridFind.Items.Refresh();
                }
                else
                {
                    txtPesel.Clear();
                }
            }
            else
            {
                txtId.IsEnabled = true;
                txtLastName.IsEnabled = true;
                ClientList.Clear();
                MyDataGridFind.Items.Refresh();
            }
        }

        /// <summary>
        /// Method searches for and updates the list of patients based on the entered last name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtLastName.Text.Length > 0)
            {
                txtId.IsEnabled = false;
                txtPesel.IsEnabled = false;
                ClientList.Clear();
                ClientList.AddRange(client.FindByLastName(txtLastName.Text));
                MyDataGridFind.Items.Refresh();
            }
            else
            {
                txtId.IsEnabled = true;
                txtPesel.IsEnabled = true;
                ClientList.Clear();
                MyDataGridFind.Items.Refresh();
            }
        }

        /// <summary>
        /// Method clears the text controls and the patient's list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <see cref="clearFindForm()"/>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            clearFindForm();
            enableFindForm(true);
            ClientList.Clear();
            MyDataGridFind.Items.Refresh();
        }

        /// <summary>
        /// Method updates the appointment's list based on selected date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCalendarPV_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyCalendarPV.SelectedDate.HasValue)
            {
                DateTime date = MyCalendarPV.SelectedDate.Value;
                AppointmentList.Clear();
                AppointmentList.AddRange(appointment.FindAppointments(date.ToString("yyyy-MM-dd")));
                MyDataGridApp.Items.Refresh();
            }
        }

        /// <summary>
        /// Method clears the value of the patient's form and it's status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddReset_Click(object sender, RoutedEventArgs e)
        {
            clearAddForm();
            lblAddStatus.Content = "";
        }

        /// <summary>
        /// Method validates and adds a new patient, clears the patient's list and adds the newst patient to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSubmit_Click(object sender, RoutedEventArgs e)
        {

            ClientModel client = new ClientModel();
            client.FirstName = txtAddFirstName.Text;
            client.LastName = txtAddLastName.Text;
            client.Pesel = txtAddPesel.Text;
            client.PhoneNumber = txtAddPhoneNumber.Text;
            client.Street = txtAddStreet.Text;
            client.City = txtAddCity.Text;
            client.ZipCode = txtAddZipCode.Text;

            try
            {
                client.Validate();
                client.ID = client.Add(client);
                lblAddStatus.Content = "Dodano nowego pacjenta: " + client.FirstName + " " + client.LastName;
                clearAddForm();
                clearFindForm();
                enableFindForm(false);
                //ClientList.Add(client);
                //MyDataGridFind.Items.Refresh();
                txtId.Text = client.ID.ToString();
                txtId.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Błąd!",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method clears text controls used for searching patients
        /// </summary>
        public void clearFindForm()
        {
            txtId.Clear();
            txtLastName.Clear();
            txtPesel.Clear();
        }

        /// <summary>
        /// Method enables text controls used for searching patients
        /// </summary>
        /// <param name="enabled"></param>
        public void enableFindForm(bool enabled)
        {
            txtId.IsEnabled = enabled;
            txtLastName.IsEnabled = enabled;
            txtPesel.IsEnabled = enabled;
        }

        /// <summary>
        /// Method clears the text controls used on add new patient tab
        /// </summary>
        private void clearAddForm()
        {
            txtAddCity.Clear();
            txtAddFirstName.Clear();
            txtAddLastName.Clear();
            txtAddPesel.Clear();
            txtAddPhoneNumber.Clear();
            txtAddStreet.Clear();
            txtAddZipCode.Clear();
        }

        /// <summary>
        /// Method shows the about window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}
