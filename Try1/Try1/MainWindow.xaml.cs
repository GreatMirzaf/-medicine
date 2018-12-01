﻿using System;
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
            string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = D:\Archive\Project\Могилянка\С#\git\-medicine\Try1\Try1\Database1.mdf;Integrated Security=True";

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
    }
}
