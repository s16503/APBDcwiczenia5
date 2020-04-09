using APBDcwiczenia5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBDcwiczenia5.DAL
{
    interface IDbService
    {
        public IEnumerable<Student> GetStudents();
    }
}
