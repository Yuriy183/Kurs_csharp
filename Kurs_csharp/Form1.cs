
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kurs_csharp
{
    public partial class Form1 : Form
    {
        List<Student> students = new List<Student>(); //список студентов
        List<Session> session = new List<Session>();//список результатов сесси
        OpenFileDialog openFileDialog;//открывает файл
        SaveFileDialog saveFileDialog;//сохраняет файл

        public Form1()
        {
            InitializeComponent();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
        }
        //ф-я для проверки является ли соседние ячейки одинаковые
        bool IsTheSameCellValue(int column, int row)
        {
            DataGridViewCell cell1 = dataGridView3[column, row];
            DataGridViewCell cell2 = dataGridView3[column, row - 1];
            if (cell1.Value == null || cell2.Value == null)
            {
                return false;
            }
            return cell1.Value.ToString() == cell2.Value.ToString();
        }
        //ф-я обновления таблицы
        void UpdateTable()
        {
            dataGridView1.Rows.Clear();
            foreach (var student in students)
            {
                dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
            }
            foreach (var student in session)
            {
                dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
            }
        }
        //добавить информацию по предмету
        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog.FileName;

            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);

            int length = session.Count;
            bw.Write(length);

            foreach (var student in session)
            {
                student.Write(bw);
            }
            // Закрываем потоки
            bw.Close();
            fs.Close();
            MessageBox.Show("Файл сохранен");
        }
        //удаление из списка студентов записей
        private void Button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0 && this.dataGridView1.SelectedRows[0].Index != this.dataGridView1.Rows.Count - 1)
            {
                this.dataGridView1.Rows.RemoveAt(this.dataGridView1.SelectedRows[0].Index);
                int i = this.dataGridView1.SelectedRows[0].Index;
                students.RemoveAt(i);
            }
            UpdateTable();
        }
        //открытие файла
        private void ОткрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog.FileName;
            FileStream fsR = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsR, Encoding.UTF8);
            //чтение из файла
            int length = br.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                session.Add(Session.Read(br));
            }

            dataGridView1.Rows.Clear();
            dataGridView3.Rows.Clear();

            List<Student> temp = new List<Student>();
            foreach (var student in session)
            {
                dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);

                temp.Add(new Student
                {
                    Name = student.Student.Name,
                    Recordbook = student.Student.Recordbook,
                    Faculty = student.Student.Faculty,
                    Course = student.Student.Course
                });
            }
            //из списка сессии получается список студент
            var studentsTemp = temp.GroupBy(x => x.Name).Select(x => x.First()); 
            foreach (var student in studentsTemp)
            {
                students.Add(student);
            }

            foreach (var student in students)
            {
                dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
            }
            br.Close();
            fsR.Close();
        }
        //ф-я проверки на позитивные числа
        bool InputIsPositiveDigit(string st)
        {
            bool check = false;
            int temp;
            if (int.TryParse(st, out temp) == true && temp > 0)
                check = true;
            return check;
        }
        //ф-я для вывода студентов у которых есть двойки
        private void Button6_Click_1(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            foreach (var student in session)
            {
                if (student.Mark < 30)
                {
                    dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
                }
            }
        }
        //ф-я вывода студентов с двойками в таблицу
        private void Button8_Click_1(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            foreach (var student in session)
            {
                if (student.Mark < 30)
                {
                    session = session.Where(x => x.Student.Name != student.Student.Name).ToList();
                    students = students.Where(x => x.Name != student.Student.Name).ToList();
                }
            }
            UpdateTable();
        }
        //ф-я добавления в таблицу студент
        private void Add_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
            else
            {
                if (InputIsPositiveDigit(textBox2.Text) && InputIsPositiveDigit(textBox4.Text))
                {
                    students.Add(new Student
                    {
                        Name = textBox1.Text,
                        Recordbook = Convert.ToInt32(textBox2.Text),
                        Faculty = textBox3.Text,
                        Course = Convert.ToInt32(textBox4.Text)
                    });
                    dataGridView3.Rows.Clear();
                    UpdateTable();
                }
                else
                {
                    MessageBox.Show("В поля \"курс\" и \"номер зачётки\" должны вводится положительные числа");
                }
            }
        }
        //ф-я для форматирования ячеек, которые одинаковые
        private void DataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0)
                return;
            if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
            {
                e.Value = "";
                e.FormattingApplied = true;
            }
        }
        //ф-я для форматирования ячеек, которые одинаковые
        private void DataGridView3_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            if (e.RowIndex < 1 || e.ColumnIndex < 0)
                return;
            if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            }
            else
            {
                e.AdvancedBorderStyle.Top = dataGridView1.AdvancedCellBorderStyle.Top;
            }
        }
        //ф-я удаления из списка студентов
        private void Delete_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0 && this.dataGridView1.SelectedRows[0].Index != this.dataGridView1.Rows.Count - 1)
            {
                int indx = this.dataGridView1.SelectedRows[0].Index;
                students.RemoveAt(indx);
                dataGridView3.Rows.Clear();
                UpdateTable();
            }
        }
        //ф-я добавления в таблицу результат сессии
        private void Button2_Click_1(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0 && this.dataGridView1.SelectedRows[0].Index != this.dataGridView1.Rows.Count - 1)
            {
                if (textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "")
                {
                    MessageBox.Show("Все поля должны быть заполнены");
                }
                else
                {
                    int indx = this.dataGridView1.SelectedRows[0].Index;
                    if (InputIsPositiveDigit(textBox5.Text))
                    {
                        session.Add(new Session
                        {
                            Student = new Student(students[indx].Name, students[indx].Faculty, students[indx].Course, students[indx].Recordbook),
                            Teacher = textBox7.Text,
                            Subject = textBox6.Text,
                            Mark = Convert.ToInt32(textBox5.Text)
                        });
                        dataGridView3.Rows.Clear();
                        UpdateTable();
                    }
                    else
                    {
                        MessageBox.Show("Оценка должна быть положительным числом");
                    }
                }
            }
        }
        //ф-я вывода отличников в таблицу
        private void Button5_Click_1(object sender, EventArgs e)
        {
            double avg = 0;
            double count = 0;
            dataGridView1.Rows.Clear();

            for (int i = 0; i < students.Count; i++)
            {
                foreach (var student in session)
                {
                    if (student.Student.Name == students[i].Name && student.Mark >= 90)
                    {
                        avg += student.Mark;
                        count++;
                    }
                    else
                    {
                        avg = 0;
                        count = 0;
                        continue;
                    }
                }
                if (avg / count >= 90)
                {
                    dataGridView1.Rows.Add(students[i].Name, students[i].Recordbook, students[i].Faculty, students[i].Course);
                }
                avg = 0;
                count = 0;
            }
        }
        //ф-я измениния для таблицы студент
        private void Button4_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0 &&
          this.dataGridView1.SelectedRows[0].Index !=
          this.dataGridView1.Rows.Count - 1)
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Все поля должны быть заполнены");
                }
                else
                {
                    int indx = this.dataGridView1.SelectedRows[0].Index;

                    students[indx].Name = textBox1.Text;
                    students[indx].Recordbook = Convert.ToInt32(textBox2.Text);
                    students[indx].Faculty = textBox3.Text;
                    students[indx].Course = Convert.ToInt32(textBox4.Text);

                    foreach (var student in session)
                    {
                        if (student.Student.Name == students[indx].Name)
                        {
                            student.Student.Name = students[indx].Name;
                            student.Student.Recordbook = students[indx].Recordbook;
                            student.Student.Faculty = students[indx].Faculty;
                            student.Student.Course = students[indx].Course;
                        }
                    }
                }
            }
            dataGridView3.Rows.Clear();
            UpdateTable();
        }
        //ф-я удаления в таблице результат сессии
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView3.SelectedRows.Count > 0 && this.dataGridView3.SelectedRows[0].Index != this.dataGridView3.Rows.Count - 1)
            {
                int indx = this.dataGridView3.SelectedRows[0].Index;

                session.RemoveAt(indx);
                dataGridView3.Rows.Clear();
                UpdateTable();
            }
        }
        //ф-я для поиска в таблице студент
        private void button3_Click_1(object sender, EventArgs e)
        {
            string poisk = textBox8.Text;
            dataGridView1.Rows.Clear();
            switch (comboBox1.Text)
            {
                case "ФИО студента":
                    foreach (var student in students)
                    {
                        if (student.Name == poisk)
                        {
                            dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
                        }
                    }
                    break;
                case "Факультет":
                    foreach (var student in students)
                    {
                        if (student.Faculty == poisk)
                        {
                            dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
                        }
                    }
                    break;
                case "Номер зачётки":
                    foreach (var student in students)
                    {
                        if (student.Recordbook == Convert.ToInt32(poisk))
                        {
                            dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
                        }
                    }
                    break;
                case "Курс":
                    foreach (var student in students)
                    {
                        if (student.Course == Convert.ToInt32(poisk))
                        {
                            dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        //ф-я для обновления в таблице студент
        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            foreach (var student in students)
            {
                dataGridView1.Rows.Add(student.Name, student.Recordbook, student.Faculty, student.Course);
            }
        }
        //ф-я для обновления в таблице результат сессии
        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            foreach (var student in session)
            {
                dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
            }
        }
        //ф-я для поиска в таблице результат сессии
        private void button11_Click(object sender, EventArgs e)
        {
            string poisk2 = textBox9.Text;
            dataGridView3.Rows.Clear();
            switch (comboBox2.Text)
            {
                case "ФИО студента":
                    foreach (var student in session)
                    {
                        if (student.Student.Name == poisk2)
                        {
                            dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
                        }
                    }
                    break;
                case "Преподаватель":
                    foreach (var student in session)
                    {
                        if (student.Teacher == poisk2)
                        {
                            dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
                        }
                    }
                    break;
                case "Предмет":
                    foreach (var student in session)
                    {
                        if (student.Subject == poisk2)
                        {
                            dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
                        }
                    }
                    break;
                case "Оценка":
                    foreach (var student in session)
                    {
                        if (student.Mark == Convert.ToInt32(poisk2))
                        {
                            dataGridView3.Rows.Add(student.Student.Name, student.Teacher, student.Subject, student.Mark);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
