using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management.Automation;
using System.ServiceProcess;
using System.Data.Odbc;

/*
 * Title: SD Automation
 * Author: Peter S (qwmq)
 * Purpose: to automate gathering/storing user data for more efficient calls
 * Created: 5-28-16
 */

namespace ADStatus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User user = new User();
        PowerShell ps = PowerShell.Create();
        string data = "";

        public MainWindow()
        {

            ps.Commands.AddCommand("Import-Module").AddArgument("ActiveDirectory");
            ps.Invoke();
            ps.Commands.Clear();
            InitializeComponent();
            
        }

        /*
         * Check to see if data that has been entered in is a LAN ID or if it's a employee number, 
         * and then gather the first/last name.
         * */
        public void getData(string userInput)
        {
            data = userInput;
            int error = 0;
            ps.Commands.Clear();

            ps.AddCommand("get-aduser")
                .AddParameter("-filter", "SamAccountName -eq " + "'" + data + "'")
                .AddParameter("properties", "*"); //Creates cmdlet for getting user info

            try
            {
                //ps invoke does not currently filter out bad results.
                ps.Invoke();
                error = ps.Streams.Error.Count;
            }
            catch (CmdletInvocationException e)
            {
                MessageBox.Show("User not found, please check input. \nError:" + e.Message);
                return;
            }

            if (error == 0)
            {
                foreach (PSObject result in ps.Invoke())
                {
                    user.setFirstName(result.Members["GivenName"].Value.ToString());
                    user.setLastName(result.Members["Surname"].Value.ToString());
                }

                if (isLan(data))
                {
                    user.setLan(data);
                    findEmpNum();
                    getUser();
                }
                else
                {
                    findLan();
                    user.setEmpNum(data);
                    getUser();
                }
            }
            else
            {
                MessageBox.Show("User not found. Please check input and try again. \nError: " + error);
            }
        }

        /*
         * Check if input is lan, and return a boolean.
         */
        public bool isLan(string input) //Check to see if input is LAN ID.
        {
            foreach (char c in input)
            {
                if (Char.IsLetter(c))
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Get Bulk of user data and assign it to object.
         */
        public void getUser()
        {

            if(input != null)
            {
                if (user.getLan().Length != 0)
                {
                    ps.AddCommand("get-aduser")
                        .AddParameter("-filter", "SamAccountName -eq " + "'" + user.getLan() + "'")
                        .AddParameter("properties", "*"); //Creates cmdlet for getting user info
                }
                else
                {
                    ps.AddCommand("get-aduser")
                        .AddParameter("-filter", "SamAccountName -eq " + "'" + user.getEmpNum() + "'")
                        .AddParameter("properties", "*"); //Creates cmdlet for getting user info
                }

                try
                {
                    ps.Invoke();
                }
                catch (CmdletInvocationException e)
                {
                    MessageBox.Show("User not found, please check input. \nError:" + e.Message);
                    return;
                }

                try
                {
                    foreach (PSObject result in ps.Invoke())
                    {

                        if (result.Members["Enabled"].Value.Equals(true))
                        {
                            Console.WriteLine("User enabled.");
                            string samAccountName = result.Members["samaccountname"].Value.ToString();
                            string canName = result.Members["CanonicalName"].Value.ToString();
                            string isLocked = result.Members["LockedOut"].Value.ToString();
                            DateTime expired = new DateTime();
                            DateTime passSet = new DateTime();
                            TimeSpan ts = new TimeSpan();

                            Console.WriteLine(samAccountName);

                            if(isLan(samAccountName))
                            {
                                Console.WriteLine("LanID Entered in by user.");

                                if (isLocked.CompareTo("TRUE") == 0)
                                {
                                    Console.WriteLine("Account Locked.");
                                    user.setLocked(true);
                                }
                                else
                                {
                                    Console.WriteLine("Account unlocked.");
                                    user.setLocked(false);
                                }

                                user.setStore(result.Members["Office"].Value.ToString());
                                user.setDept(result.Members["Department"].Value.ToString());
                                    try
                                    {
                                        user.setPhone(result.Members["telephoneNumber"].Value.ToString());
                                    }
                                    catch (NullReferenceException e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                user.setContainer(canName);

                                if (result.Members["enabled"].Value.ToString().Contains("True"))
                                {
                                    user.setStatus("Enabled");
                                }
                                else
                                {
                                    user.setStatus("Disabled");
                                }

                                Console.WriteLine(result.Members["PasswordLastSet"].Value.ToString());
                                passSet = Convert.ToDateTime(result.Members["PasswordLastSet"].Value.ToString());
                                expired = passSet.AddDays(60);
                                user.setExpDate(expired.ToString());
                                ts = expired - passSet;
                                user.setDaysLeft(ts.ToString());

                            }
                            fillForm();
                            MessageBox.Show("Done.");

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
        }

        /*
         * Fill form with populated information from User object.
         */
        private void fillForm() //Fill windows form.
        {
            txtName.Text = user.getFirstName() + " " + user.getLastName();
            txtLan.Text = user.getLan();
            txtEmpNum.Text = user.getEmpNum();
            txtStore.Text = user.getStore();
            txtDept.Text = user.getDept();
            txtPhone.Text = user.getPhone();

            lblContainer.Content = user.getContainer();
            lblStatus.Content = user.getStatus();
            lblExpireDate.Content = user.getExpDate();
            lblExpired.Content = user.getLocked().ToString();
        }
        /*
         * Find lan ID information based off of employee number input.
         */
        private void findLan()
        {
            ps.AddCommand("get-aduser")
                .AddParameter("-filter", "givenname -eq " + "'" + user.getFirstName() + "' -and surname -eq " + "'" + user.getLastName() + "'")
                .AddParameter("properties", "*"); //Creates cmdlet for getting user info

            try
            {
                ps.Invoke();
            }
            catch (CmdletInvocationException e)
            {
                MessageBox.Show("User not found, please check input. \nError:" + e.Message);
                return;
            }

            foreach (PSObject result in ps.Invoke())
            {
                if (isLan(result.Members["samaccountname"].Value.ToString()) && result.Members["Enabled"].Value.Equals(true))
                {
                    if (result.Members["canonicalName"].Value.ToString().Contains("Users"))
                    {
                        user.setLan(result.Members["samaccountname"].Value.ToString());
                    }
                }
            }

        }

        /*
         * Find Employee number from lan ID input.
         */
        private void findEmpNum()
        {
            ps.AddCommand("get-aduser")
                .AddParameter("-filter", "givenname -eq " + "'" + user.getFirstName() + "' -and surname -eq " + "'" + user.getLastName() + "'")
                .AddParameter("properties", "*"); //Creates cmdlet for getting user info

            try
            {
                ps.Invoke();
            }
            catch (CmdletInvocationException e)
            {
                MessageBox.Show("User not found, please check input. \nError:" + e.Message);
                return;
            }

            foreach (PSObject result in ps.Invoke())
            {
                if (!isLan(result.Members["samaccountname"].Value.ToString()) && result.Members["Enabled"].Value.Equals(true))
                {
                    user.setEmpNum(result.Members["samaccountname"].Value.ToString());
                } 
            }
        }

        /*
         * Button click event for finding other half of complete user data.
         */
        private void btnFindUser_OnClick(object sender, RoutedEventArgs e)
        {
            if (input.Text == "")
            {
                MessageBox.Show("Please check input.");
                return;
            }

            getData(input.Text);
            
        }

        private void findOracleuser(string input)
        {

        }

        private void oracleConnect()
        {

        }

        private void btnStartCMRC_Click(object sender, RoutedEventArgs e)
        {
            string wsid = txtWSID.Text;

            if(wsid.Length == 0)
            {
                MessageBox.Show("Please enter WSID.");
                return;
            }

            ServiceController sc = new ServiceController("cmrcservice", wsid);
            sc.Start(); //add priviledges for that to work.

        }

        private void btnCopyInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetDataObject(
                "Name: " + user.getFirstName() + " " + user.getLastName() + "\r\n" +
                "Lan: " + user.getLan() + "\r\n" +
                "Ph: " + user.getPhone() + "\r\n" +
                "WSID: " +
                "Employee Number: " + user.getEmpNum() + "\r\n" +
                "Store: " + user.getStore() + "\r\n" +
                "Floor: " + "\r\n" +
                "Cube: " + "\r\n" +
                "Best time to Contact: " + "\r\n" +
                "=====================================" + "\r\n"
                );
        }
    }
}
