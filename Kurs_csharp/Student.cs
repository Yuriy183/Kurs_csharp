using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kurs_csharp
{
    class Student
    {
        string name; // переменная ФИО студента
        string faculty; //переменная факультета
        int recordbook;//переменная номера зачётки
        int course;//переменная курса

        //конструктор по умолчанию
        public Student()
        {

        }
        //конструктор с входными параметрами
        public Student(string name, string faculty, int course, int recordbook)
        {
            this.name = name;
            this.faculty = faculty;
            this.course = course;
            this.recordbook = recordbook;
        }
        //инкапсуляция
        public string Name { get => name; set => name = value; }
        public string Faculty { get => faculty; set => faculty = value; }
        public int Course { get => course; set => course = value; }
        public int Recordbook { get => recordbook; set => recordbook = value; }

        //метод записи в файл
        public void Write(BinaryWriter bw)
        {
            bw.Write(Name);
            bw.Write(Faculty);
            bw.Write(Course);
            bw.Write(Recordbook);
        }
        //метод чтения из файла
        public static Student Read(BinaryReader br)
        {
            Student q = new Student()
            {
                Name = br.ReadString(),
                Faculty = br.ReadString(),
                Course = br.ReadInt32(),
                Recordbook = br.ReadInt32()
            };
            return q;
        }
    }
}
