using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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


namespace CurrecyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public class Root
        {
            public Rate rates { get; set; }
        }

        // We have to make sure that the API return value that name where you want to store that name are the same
        // Like in API, I need to get the respone INR, then set it with INR name 

        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double NZD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }

        }

        Root val =  new Root();


        public MainWindow()
        {
            InitializeComponent();
            GetValue();
            ClearControls();    
        }

        private async void GetValue()
        {
            val = await GetDataGetMethod<Root> ("<https://openexchangerates.org/api/latest.json?app_id=d28c71e8e26b4282859f140a4d11a4dd>");
            BindCurrency();
        }

        public static async Task<Root> GetDataGetMethod<T>(string url)
        {
            var ss = new Root();
            try
            {
                using(var client =  new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);

                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ResponceString = await response.Content.ReadAsStringAsync();
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);
                        return ResponceObject;

                    }

                    return ss;
                }
            }
            catch
            {
                return ss;
            }
        }

        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();

            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Rate");
            dtCurrency.Rows.Add("--Select--", 0);
            dtCurrency.Rows.Add("INR", val.rates.INR);
            dtCurrency.Rows.Add("NZD", val.rates.NZD);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("CAD", val.rates.CAD);
            dtCurrency.Rows.Add("ISK", val.rates.ISK);
            dtCurrency.Rows.Add("PHP", val.rates.PHP);
            dtCurrency.Rows.Add("DKK", val.rates.DKK);
            dtCurrency.Rows.Add("CZK", val.rates.CZK);

            // The data to currecny combox is assigned from the datatable
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath property is used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used to set the value in the combox
            cmbFromCurrency.SelectedValuePath = "Rate";

            //SelectedIndex property is used to bind hint in the Combobox. The default value is Select
            cmbFromCurrency.SelectedIndex = 0;

            // All the properties are set for 'To Currency' Combox as 'From Currency' Combobox
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath= "Text";
            cmbToCurrency.SelectedValuePath="Rate";
            cmbToCurrency.SelectedIndex = 0;

        }

        public void ClearControls()
        {
            txtCurrency.Text =string.Empty;
            if(cmbFromCurrency.Items.Count > 0 )
            {
                cmbFromCurrency.SelectedIndex = 0;

            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }

            lblCurrency.Content = "";
            txtCurrency.Focus();
            
        }

        // Allow only the integer value in the textbox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to add regex add library using the System.Text.RegularExpressions
            Regex regex = new Regex("^[0-9]+");
            e.Handled = ! regex.IsMatch(e.Text);

        }

        // Convert the button click event 
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            // Create a variable to store the converted value.
            double ConvertedValue;

            // Check if the amount textbox is null or blank.
            if (string.IsNullOrWhiteSpace(txtCurrency.Text))
            {
                MessageBox.Show("Please Enter the currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }

            // Optional: Validate that a valid currency is selected in the From ComboBox.
            if (cmbFromCurrency.SelectedItem == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a valid 'From' currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }

            // Optional: Validate that a valid currency is selected in the To ComboBox.
            if (cmbToCurrency.SelectedItem == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a valid 'To' currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // The amount textbox value is set in converted value
                //double.Parse is used to convert the datatype String to Double 
                //TextBox text have string and ConvertedValue is double datatype

                ConvertedValue = double.Parse(txtCurrency.Text);

                //show in label the converted currenccy and converted currency name 
                // add ToString ("N3") is used to place 000 after the (.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currecny converter is From Currency value multiply(*)
                //with amount textbox value and then the total is divided(/) with To Currency value

                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * (double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString()));

                //Show in label converted currecny and converted name 
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

            // Clear button click event 
            
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();

        }



    }
}
