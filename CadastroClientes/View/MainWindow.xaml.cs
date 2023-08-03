using CadastroClientes.Services;
using CadastroClientes.ViewModel;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CadastroClientes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowServices
    {
        public MainWindow()
        {
            InitializeComponent();
            VMClient.windowServices = this;
        }

        public FileInfo SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (!saveFileDialog.FileName.EndsWith(".txt"))
                {
                    saveFileDialog.FileName += ".txt";
                }
                return new FileInfo(saveFileDialog.FileName);
            }
            return null;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            int errorCount = 0;

            string name = InputName.Text.Trim();
            string email = InputEmail.Text.Trim();
            string street = InputStreet.Text.Trim();
            string cep = InputCEP.Text.Trim();
            string city = InputCity.Text.Trim();

            if (!VMClient.RequiredFieldsFilled(name, email, street, InputNumber.Text.Trim(), cep, city))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                errorCount++;
            }
            if (!IsValidPhoneNumber(InputPhone.Text))
            {
                if (errorCount == 0)
                {
                    MessageBox.Show("Número de telefone inválido!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    errorCount++;
                }
            }
            if (!IsNumber(InputNumber.Text))
            {
                if (errorCount == 0)
                {
                    MessageBox.Show("Número da casa inválido!\nO campo deve ser composto apenas por números", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    errorCount++;
                }
            }
            if (!IsNumber(InputCEP.Text))
            {
                if (errorCount == 0)
                {
                    MessageBox.Show("CEP inválido!\nO campo deve ser composto apenas por números", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    errorCount++;
                }
            }

            if (errorCount == 0)
            {
                BindingExpression nameBinding = InputName.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression emailBinding = InputEmail.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression phoneBinding = InputPhone.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression streetBinding = InputStreet.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression numberBinding = InputNumber.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression additionalBinding = InputAdditional.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression neighborhoodBinding = InputNeighborhood.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression cepBinding = InputCEP.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                BindingExpression cityBinding = InputCity.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);

                nameBinding.UpdateSource();
                emailBinding.UpdateSource();
                phoneBinding.UpdateSource();
                streetBinding.UpdateSource();
                numberBinding.UpdateSource();
                additionalBinding.UpdateSource();
                neighborhoodBinding.UpdateSource();
                cepBinding.UpdateSource();
                cityBinding.UpdateSource();

                if (VMClient.SelectedIndex == -1)
                {
                    VMClient.NewClient();
                }
                else
                {
                    VMClient.SaveClient();
                }
            }
        }

        private void SearchTextGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Foreground = System.Windows.Media.Brushes.Black;
            VMClient.SearchString = "";
        }

        private void SearchTextLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                textBox.Text = "Pesquisar...";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Trim();
            string pattern1 = @"^\(\d{2}\) \d{5}-\d{4}$";   // (xx) xxxxx-xxxx
            string pattern2 = @"^\(\d{2}\) \d{9}$";         // (xx) xxxxxxxxx
            string pattern3 = @"^\(\d{2}\)\d{5}-\d{4}$";    // (xx)xxxxx-xxxx
            string pattern4 = @"^\(\d{2}\)\d{9}$";          // (xx)xxxxxxxxx
            string pattern5 = @"^\d{2} \d{5}-\d{4}$";       // xx xxxxx-xxxx
            string pattern6 = @"^\d{7}-\d{4}$";             // xxxxxxx-xxxx
            string pattern7 = @"^\d{2} \d{9}$";             // xx xxxxxxxx
            string pattern8 = @"^\d{11}$";                  // xxxxxxxxxxx
            string pattern9 = @"^$";

            return Regex.IsMatch(phoneNumber, pattern1)
                || Regex.IsMatch(phoneNumber, pattern2)
                || Regex.IsMatch(phoneNumber, pattern3)
                || Regex.IsMatch(phoneNumber, pattern4)
                || Regex.IsMatch(phoneNumber, pattern5)
                || Regex.IsMatch(phoneNumber, pattern6)
                || Regex.IsMatch(phoneNumber, pattern7)
                || Regex.IsMatch(phoneNumber, pattern8)
                || Regex.IsMatch(phoneNumber, pattern9);
        }

        private bool IsNumber(string number)
        {
            number = number.Trim();
            string pattern = @"^\d+$";

            return Regex.IsMatch(number, pattern);
        }
    }
}
