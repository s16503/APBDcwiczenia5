using APBDcwiczenia5.Models;
using System;
using System.Collections.Generic;

namespace APBDcwiczenia5.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;


        static MockDbService()
        {
            _students = new List<Student>
                        {
                            new Student{IdEnrollment=1, FirstName="Jan", LastName  ="Kowalski" },
                            new Student{IdEnrollment=2, FirstName="Błążej", LastName  ="Lol" },
                            new Student{IdEnrollment=3, FirstName="Andrzej", LastName  ="Nowak" },
                        };


        }


        IEnumerable<Student> IDbService.GetStudents()
        {
            throw new NotImplementedException();
        }
    }
}
