using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kurs_csharp
{
    class Session
    {
        Student student; // переменная студента
        string teacher; // переменная преподавателя
        string subject; // переменная предмета
        int mark; // переменная оценки

        //конструктор по умолчанию
        public Session()
        {

        }
        //конструктор с входными параметрами
        public Session(Student student, string teacher, string subject, int mark)
        {
            this.Student = student;
            this.Teacher = teacher;
            this.Subject = subject;
            this.Mark = mark;
        }

        //инкапсуляция
        public string Teacher { get => teacher; set => teacher = value; }
        public string Subject { get => subject; set => subject = value; }
        public int Mark { get => mark; set => mark = value; }
        internal Student Student { get => student; set => student = value; }

        //метод записи в файл
        public void Write(BinaryWriter bw)
        {
            student.Write(bw);
            bw.Write(Teacher);
            bw.Write(Subject);
            bw.Write(Mark);
        }
        //метод чтения из файла
        public static Session Read(BinaryReader br)
        {
            Session q = new Session();
            q.student = Student.Read(br);
            q.Teacher = br.ReadString();
            q.Subject = br.ReadString();
            q.Mark = br.ReadInt32();
            return q;
        }
    }
}
