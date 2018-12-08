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

        private class Item
        {
            public string Class { get; set; }
            public string Subclass { get; set; }
            public string Name { get; set; }

        }


        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            string connectionString = string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", Path.GetFullPath("Database1.mdf"));

            //string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename \Database1.mdf;Integrated Security=True";
            //////////////a1789e93be27de4f96ad4055e6d5f6715dd0b3a8
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
                System.IO.File.WriteAllText(addBox6.Text + ".txt", lines, Encoding.UTF8);


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

        private async void BtnChange_Click(object sender, RoutedEventArgs e)
        {

            try { 
            if (lErr_.Visibility == Visibility.Visible)
                lErr_.Visibility = Visibility.Hidden;

            if (!string.IsNullOrEmpty(addBox1_.Text) && !string.IsNullOrWhiteSpace(addBox1_.Text) &&
                !string.IsNullOrEmpty(addBox2_.Text) && !string.IsNullOrWhiteSpace(addBox2_.Text) &&
                !string.IsNullOrEmpty(addBox5_.Text) && !string.IsNullOrWhiteSpace(addBox5_.Text))
            {
                SqlCommand command = new SqlCommand("UPDATE [Table] SET [Name]=@Name, [Price]=@Price, [Quantity]=@Quantity WHERE [Name]=N'" + comboMedName.SelectedItem.ToString() + "'", sqlConnection);

                command.Parameters.AddWithValue("Name", addBox1_.Text);
                command.Parameters.AddWithValue("Price", addBox2_.Text);
                command.Parameters.AddWithValue("Quantity", addBox5_.Text);

                
                await command.ExecuteNonQueryAsync();
                
                addBox1_.Text = "";
                addBox2_.Text = "";
               
                addBox5_.Text = "";
                
               
            }
            else
            {
                lErr_.Visibility = Visibility.Visible;

                lErr_.Content = "Все поля должны быть заполнены!";
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            comboMedName.SelectedItem = null;



        }

        private async void BtnSell_Click(object sender, RoutedEventArgs e)
        {
            int q = 0;
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT [Quantity] FROM [Table] WHERE [Name] LIKE N'" + nameLb.Content.ToString() + "'", sqlConnection);

            try
            {
                if (!string.IsNullOrEmpty(sellBox.Text) && !string.IsNullOrWhiteSpace(sellBox.Text))
                {
                    sqlReader = await command.ExecuteReaderAsync();
                    while (await sqlReader.ReadAsync())
                    {
                        if(Convert.ToInt32(sellBox.Text)<= Convert.ToInt32(sqlReader["Quantity"]))
                        {
                            q = Convert.ToInt32(sqlReader["Quantity"]) - Convert.ToInt32(sellBox.Text);
                        }
                        else
                        {
                            q = Convert.ToInt32(sqlReader["Quantity"]);
                            MessageBox.Show("Введенное кол-во продаваемого препарата превышает максимальное кол-во препарата в наличии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    SqlCommand command2 = new SqlCommand("UPDATE [Table] SET [Quantity]=@Quantity WHERE [Name]=N'" + nameLb.Content.ToString() + "'", sqlConnection);
                    command2.Parameters.AddWithValue("Quantity", q);

                    //qunLb.Content = Convert.ToString(sqlReader["Quantity"]);
                    if (sqlReader != null)
                    sqlReader.Close();

                    await command2.ExecuteNonQueryAsync();

                    sellBox.Text = "";
                }
                else
                {
                    MessageBox.Show("Введите кол-во продаваемого препарата!", "Требуется заполнить поле", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

            SqlCommand command3 = new SqlCommand("SELECT * FROM [Table] WHERE [Name] LIKE N'" + findBox.Text + "'", sqlConnection);
            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    qunLb.Content = Convert.ToString(sqlReader["Quantity"]);
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
                    string s = System.IO.File.ReadAllText(Convert.ToString(sqlReader["Info"]) + ".txt", Encoding.Default).Replace("\n", " ");

                    textBlock1.Text = s;
                    nameLb.Content = Convert.ToString(sqlReader["Name"]);
                    priceLb.Content = Convert.ToString(sqlReader["Price"]);
                    classLb.Content = Convert.ToString(sqlReader["Class"]);
                    sbLb.Content = Convert.ToString(sqlReader["Subclass"]);
                    qunLb.Content = Convert.ToString(sqlReader["Quantity"]);
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

        private async void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SqlDataReader sqlReader = null;
                //SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE Name LIKE "+@findBox.Text.ToString(), sqlConnection);
                SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE [Name] LIKE N'" + findBox.Text + "'", sqlConnection);

                try
                {
                    sqlReader = await command.ExecuteReaderAsync();

                    while (await sqlReader.ReadAsync())
                    {
                        string s = System.IO.File.ReadAllText(Convert.ToString(sqlReader["Info"]) + ".txt", Encoding.Default).Replace("\n", " ");

                        textBlock1.Text = s;
                        nameLb.Content = Convert.ToString(sqlReader["Name"]);
                        priceLb.Content = Convert.ToString(sqlReader["Price"]);
                        classLb.Content = Convert.ToString(sqlReader["Class"]);
                        sbLb.Content = Convert.ToString(sqlReader["Subclass"]);
                        qunLb.Content = Convert.ToString(sqlReader["Quantity"]);
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

            addBox1_.Text = "";
            addBox2_.Text = "";
            
        }

        private async void ComboMedName_Loaded(object sender, RoutedEventArgs e)
        {
            comboMedName.Items.Clear();
           
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT [Name] FROM [Table]", sqlConnection);
            try
            {
                
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {

                    comboMedName.Items.Add(Convert.ToString(sqlReader["Name"]));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }

        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlDataReader sqlReader = null;
            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE [Name] LIKE N'" + comboMedName.SelectedItem.ToString() + "'", sqlConnection);

                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    //string s = Convert.ToString(System.IO.File.ReadAllText(Convert.ToString(sqlReader["Info"]) + ".txt", Encoding.Default).Replace("\n", " "));

                    
                    addBox1_.Text = Convert.ToString(sqlReader["Name"]);
                    addBox2_.Text = Convert.ToString(sqlReader["Price"]);
                   
                    addBox5_.Text = Convert.ToString(sqlReader["Quantity"]);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error); Мне похуй!
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
             
        }

        

        private async void ComboDel_Loaded(object sender, RoutedEventArgs e)
        {
            comboDel.Items.Clear();

            SqlDataReader sqlReader = null;

            try
            {
                SqlCommand command = new SqlCommand("SELECT [Name] FROM [Table]", sqlConnection);
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {

                    comboDel.Items.Add(Convert.ToString(sqlReader["Name"]));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
        }

        private async void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            

            SqlCommand command = new SqlCommand("DELETE FROM [Table] WHERE [Name]= @Name", sqlConnection);
            command.Parameters.AddWithValue("Name", comboDel.SelectedItem.ToString());
            await command.ExecuteNonQueryAsync();
            comboDel.SelectedItem = null;
            //}
            /*catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }


        

        private async void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            tree.Items.Clear();

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
                        Name = Convert.ToString(sqlReader["Name"])
                       
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

        private async void DblClick(object sender, RoutedEventArgs e)
        {
            SqlDataReader sqlReader = null;
            //SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE Name LIKE "+@findBox.Text.ToString(), sqlConnection);
            SqlCommand command = new SqlCommand("SELECT * FROM [Table] WHERE [Name] LIKE N'" + tree.SelectedItem.ToString() + "'", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    string s = System.IO.File.ReadAllText(Convert.ToString(sqlReader["Info"]) + ".txt", Encoding.Default).Replace("\n", " ");

                    textBlock1.Text = s;
                    nameLb.Content = Convert.ToString(sqlReader["Name"]);
                    priceLb.Content = Convert.ToString(sqlReader["Price"]);
                    classLb.Content = Convert.ToString(sqlReader["Class"]);
                    sbLb.Content = Convert.ToString(sqlReader["Subclass"]);
                    qunLb.Content = Convert.ToString(sqlReader["Quantity"]);
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
           

       }

}

  

    

