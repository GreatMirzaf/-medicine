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
using System.Data.SqlClient;
using System.Data;
using Path = System.IO.Path;

namespace Try1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Программы\Git\-medicine\Try1\Try1\Database1.mdf;Integrated Security=True";

            sqlConnection = new SqlConnection(connectionString);

            await sqlConnection.OpenAsync();


        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (lErr.Visibility == Visibility.Visible)
                lErr.Visibility = Visibility.Hidden;

            if (!string.IsNullOrEmpty(addBox1.Text) && !string.IsNullOrWhiteSpace(addBox1.Text) &&
                !string.IsNullOrEmpty(addBox2.Text) && !string.IsNullOrWhiteSpace(addBox2.Text) &&
                !string.IsNullOrEmpty(addBox3.Text) && !string.IsNullOrWhiteSpace(addBox3.Text) &&
                !string.IsNullOrEmpty(addBox4.Text) && !string.IsNullOrWhiteSpace(addBox4.Text) &&
                !string.IsNullOrEmpty(addBox5.Text) && !string.IsNullOrWhiteSpace(addBox5.Text) &&
                !string.IsNullOrEmpty(addBox6.Text) && !string.IsNullOrWhiteSpace(addBox6.Text) &&
                !string.IsNullOrEmpty(addBox7.Text) && !string.IsNullOrWhiteSpace(addBox7.Text))
            {
                SqlCommand command = new SqlCommand("INSERT INTO [Table] (Name, Price, Class, Subclass, Quantity, Info) VALUES (@Name, @Price, @Class, @Subclass, @Quantity, @Info)", sqlConnection);

                command.Parameters.AddWithValue("Name", addBox1.Text);
                command.Parameters.AddWithValue("Price", addBox2.Text);
                command.Parameters.AddWithValue("Class", addBox3.Text);
                command.Parameters.AddWithValue("Subclass", addBox4.Text);
                command.Parameters.AddWithValue("Quantity", addBox5.Text);
                command.Parameters.AddWithValue("Info", addBox6.Text);

                //command.Parameters.AddWithValue("Name", addBox7.Text);
                string lines = addBox7.Text;
                System.IO.File.WriteAllText(@addBox6.Text + ".txt", lines);


                await command.ExecuteNonQueryAsync();
                addBox1.Text = "";
                addBox2.Text = "";
                addBox3.Text = "";
                addBox4.Text = "";
                addBox5.Text = "";
                addBox6.Text = "";
                addBox7.Text = "";

            }
            else
            {
                lErr.Visibility = Visibility.Visible;

                lErr.Content = "Все поля должны быть заполнены!";
            }

        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSell_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            SqlDataReader sqlReader = null;
            //SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE Name LIKE "+@findBox.Text.ToString(), sqlConnection);
            SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE [Name] LIKE N'"+ findBox.Text + "'", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    string s = System.IO.File.ReadAllText(@Convert.ToString(sqlReader["Info"]) + ".txt", Encoding.Default).Replace("\n", " ");

                    textBlock1.Text = s;
                    textBlock2.Text = "Id: " + Convert.ToString(sqlReader["Id"]) + "\nЦена: " + Convert.ToString(sqlReader["Price"]) + " грн.\nКласс: " + Convert.ToString(sqlReader["Class"]) + "\nПодкласс: " + Convert.ToString(sqlReader["Subclass"]) + "\nКол-во на складе: " + Convert.ToString(sqlReader["Quantity"]);
                    //
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            addBox1.Text = "";
            addBox2.Text = "";
            addBox3.Text = "";
            addBox4.Text = "";
            addBox5.Text = "";
            addBox6.Text = "";
            addBox7.Text = "";
        }


        private class Item
        {
            public string Class { get; set; }
            public string Subclass { get; set; }
            public string Name { get; set; }
            public string Info { get; set; }
        }

        private async void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString =
                   string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True",
                       Path.GetFullPath("Database1.mdf"));

            sqlConnection = new SqlConnection(connectionString);

            await sqlConnection.OpenAsync();

            SqlDataReader sqlReader = null;
            //SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE Name LIKE "+@findBox.Text.ToString(), sqlConnection);
            SqlCommand command = new SqlCommand("SELECT * FROM [Table]", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                var items = new List<Item>();

                while (await sqlReader.ReadAsync())
                {
                    /*TreeViewItem item = new TreeViewItem();
                    

                    item.Header = Convert.ToString(sqlReader["Class"]);
                    item.ItemsSource = new string[] { Convert.ToString(sqlReader["Subclass"]), Convert.ToString(sqlReader["Name"]) };*/

                    items.Add(new Item
                    {
                        Class = Convert.ToString(sqlReader["Class"]).Trim(),
                        Subclass = Convert.ToString(sqlReader["Subclass"]),
                        Name = Convert.ToString(sqlReader["Name"]),
                        Info = Convert.ToString(sqlReader["Info"])
                    });





                    /*// ... Get TreeView reference and add both items.
                        var tree = sender as TreeView;

                        tree.Items.Add(item);
                        */
                }



                foreach (var item in items.GroupBy(x => x.Class))
                {
                    var treeViewItem = new TreeViewItem { Header = item.Key };
                    foreach (var subItem in item.GroupBy(x => x.Subclass))
                    {
                        var subclassTreeViewItem = new TreeViewItem
                        {
                            Header = subItem.Key,
                            ItemsSource = subItem.Select(x => x.Name)

                        };

                        treeViewItem.Items.Add(subclassTreeViewItem);
                    }

                    tree.Items.Add(treeViewItem);
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

        }
        private void TreeView_SelectedItemChanged(object sender,
           RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tree.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                var item = tree.SelectedItem as TreeViewItem;
                this.Title = "Selected header: " + item.Header.ToString();
            }
            else if (tree.SelectedItem is string)
            {
                // ... Handle a string.
                this.Title = "Selected: " + tree.SelectedItem.ToString();
            }
        }

    }

    }

    

