using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Student
    {
        public int StudentID { get; set; }

        public string StudentName { get; set; }

        public int Age { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IList<Student> studentList = new List<Student>() {
            new Student() { StudentID = 1, StudentName = "John", Age = 18 } ,
            new Student() { StudentID = 2, StudentName = "Steve",  Age = 15 } ,
            new Student() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
            new Student() { StudentID = 4, StudentName = "Ram" , Age = 20 } ,
            new Student() { StudentID = 5, StudentName = "Ron" , Age = 19 }
            };

            var b = studentList.OrderBy(x => x.StudentName);

            var a = studentList.OrderByDescending(x => x.StudentName);

            foreach (var item in b)
            {
                Console.WriteLine(item.StudentName);
            }

            Console.WriteLine();
            Console.WriteLine();

            foreach (var item in a)
            {
                Console.WriteLine(item.StudentName);
            }

            Console.ReadKey();
        }
    }
}
