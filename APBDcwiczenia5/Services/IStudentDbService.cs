using APBDcwiczenia5.DOTs.Requests;
using APBDcwiczenia5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBDcwiczenia5.Services
{
    public interface IStudentDbService
    {
        public Enrollment EnrollStudent(EnrollStudentRequest request);
        public Enrollment PromoteStudents(int semester, string studies);
        public List<Student> GetStudents();
        public Student GetStudent(string index);
        public Student GetStudent(string index, string haslo);
    }
}
