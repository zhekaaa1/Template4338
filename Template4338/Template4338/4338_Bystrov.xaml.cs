﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Shapes;
using Newtonsoft.Json;

using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace Template4338
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        //импорт Excel
        private void ImportExcel_ButtonCliked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Реализуйте код загрузки данных из файла Excel и сохранения их в БД
                // Например:
                DataTable dataTable = LoadDataFromExcel(filePath);
                SaveDataToDatabase(dataTable);
            }
        }
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable servicesDataTable = GetServicesFromDatabase();

            if (servicesDataTable.Rows.Count > 0)
            {
                // Сортировка данных по стоимости в порядке возрастания
                DataView dv = servicesDataTable.DefaultView;
                dv.Sort = "ServiceCost ASC";
                DataTable sortedServicesDataTable = dv.ToTable();

                SaveDataToExcel(sortedServicesDataTable);
            }
            else
            {
                MessageBox.Show("Нет данных для экспорта.");
            }
        }

        private void SaveDataToDatabase(DataTable dataTable)
        {
            using (var connection = new SqlConnection(@"Data Source=ZHEKAAA1;Initial Catalog=LR34ISRPO;Integrated Security=true"))
            {
                connection.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    string query = @"INSERT INTO Service (ID, ServiceName, ServiceType, ServiceCode, ServiceCost) 
                             VALUES (@ID, @ServiceName, @ServiceType, @ServiceCode, @ServiceCost)";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Параметры для вставки данных из каждой строки DataTable
                    command.Parameters.AddWithValue("@ID", row["ID"]);
                    command.Parameters.AddWithValue("@ServiceName", row["Наименование услуги"]);
                    command.Parameters.AddWithValue("@ServiceType", row["Вид услуги"]);
                    command.Parameters.AddWithValue("@ServiceCode", row["Код услуги"]);
                    command.Parameters.AddWithValue("@ServiceCost", row["Стоимость, руб за час"]);

                    // Выполнение команды INSERT INTO
                    command.ExecuteNonQuery();
                }
            }
        }
        //экспорт Excel
        private DataTable GetServicesFromDatabase()
        {
            DataTable servicesDataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(@"Data Source=ZHEKAAA1;Initial Catalog=LR34ISRPO;Integrated Security=true"))
            {
                string query = "SELECT ID, ServiceName, ServiceCost FROM Service";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    adapter.Fill(servicesDataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных из базы данных: " + ex.Message);
                }
            }

            return servicesDataTable;
        }
        
        private DataTable LoadDataFromExcel(string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(filePath);
            Excel.Worksheet worksheet = workbook.Worksheets[1];

            DataTable dataTable = new DataTable();

            int rows = worksheet.UsedRange.Rows.Count;
            int columns = worksheet.UsedRange.Columns.Count;

            // Заполнение данных из Excel в DataTable
            for (int i = 1; i <= rows; i++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int j = 1; j <= columns; j++)
                {
                    if (i == 1) // Заполнение заголовков
                    {
                        dataTable.Columns.Add((worksheet.Cells[i, j] as Excel.Range).Value.ToString());
                    }
                    else // Заполнение данных
                    {
                        dataRow[j - 1] = (worksheet.Cells[i, j] as Excel.Range).Value;
                    }
                }
                if (i != 1)
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            workbook.Close();
            excelApp.Quit();

            return dataTable;
        }

        private void SaveDataToExcel(DataTable dataTable)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Worksheets[1];

            // Заполнение заголовков
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
            }

            // Заполнение данных
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataTable.Rows[i][j];
                }
            }

            // Сохранение файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
            }

            excelApp.Quit();
        }

        //импорт json
        private void ImportJSON_ButtonClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Реализация загрузки данных из файла JSON и сохранения их в БД
                DataTable dataTable = LoadDataFromJson(filePath);
                SaveDataFromJsonToDatabase(dataTable); // Измененный вызов метода
            }
        }

        private DataTable LoadDataFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json);
                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных из файла JSON: " + ex.Message);
                return null;
            }
        }

        private void SaveDataFromJsonToDatabase(DataTable dataTable) // Переименованный метод
        {
            try
            {
                using (var connection = new SqlConnection(@"Data Source=ZHEKAAA1;Initial Catalog=LR34ISRPO;Integrated Security=true"))
                {
                    connection.Open();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string query = @"INSERT INTO ServicesJson (Id, ServiceName, ServiceType, ServiceCode, ServiceCost) 
                                 VALUES (@Id, @ServiceName, @ServiceType, @ServiceCode, @ServiceCost)";
                        SqlCommand command = new SqlCommand(query, connection);

                        // Параметры для вставки данных из каждой строки DataTable
                        command.Parameters.AddWithValue("@Id", row["IdServices"]);
                        command.Parameters.AddWithValue("@ServiceName", row["NameServices"]);
                        command.Parameters.AddWithValue("@ServiceType", row["TypeOfService"]);
                        command.Parameters.AddWithValue("@ServiceCode", row["CodeService"]);
                        command.Parameters.AddWithValue("@ServiceCost", row["Cost"]);

                        // Выполнение команды INSERT INTO
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Данные успешно импортированы в таблицу ServicesJson.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при импортировании данных в таблицу ServicesJson: " + ex.Message);
            }
        }


        //экспорт json
        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            DataTable servicesDataTable = GetServicesFromDatabase();

            if (servicesDataTable.Rows.Count > 0)
            {
                // Сортировка данных по стоимости в порядке возрастания
                DataView dv = servicesDataTable.DefaultView;
                dv.Sort = "ServiceCost ASC";
                DataTable sortedServicesDataTable = dv.ToTable();

                ExportToWord(sortedServicesDataTable);
            }
            else
            {
                MessageBox.Show("Нет данных для экспорта.");
            }
        }

        private void ExportToWord(DataTable dataTable)
        {
            // Создание нового документа Word
            Word.Application wordApp = new Word.Application();
            object missing = System.Reflection.Missing.Value;
            Word.Document doc = wordApp.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            // Добавление заголовков в таблицу Word
            Word.Table table = doc.Tables.Add(doc.Range(), dataTable.Rows.Count + 1, dataTable.Columns.Count, ref missing, ref missing);

            // Заполнение заголовков таблицы
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                table.Cell(1, i + 1).Range.Text = dataTable.Columns[i].ColumnName;
            }

            // Заполнение данных
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    table.Cell(i + 2, j + 1).Range.Text = dataTable.Rows[i][j].ToString();
                }
            }

            // Сохранение файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word Files (*.docx)|*.docx";
            if (saveFileDialog.ShowDialog() == true)
            {
                doc.SaveAs(saveFileDialog.FileName);
                wordApp.Visible = true;
            }
            else
            {
                doc.Close();
                wordApp.Quit();
            }
        }
    }
}
