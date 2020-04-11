using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBDcwiczenia5.DAL;
using APBDcwiczenia5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using APBDcwiczenia5.Services;

namespace APBDcwiczenia5.Controllers
{

    [ApiController]     //definiuje zestaw standardowych cech dla api ,, walidować modele
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16503;Integrated Security=True;";
        private readonly IStudentDbService studentDbService;


        public StudentsController(IStudentDbService db)
        {
            studentDbService = db;
        }

        //2 q
        [HttpGet]       // odpowiada na żądanie GET
        public IActionResult GetStudents(string orderBy) //action methos
        {
            List<Student> list = studentDbService.GetStudents();
            return Ok(list);
        }


        // ze struktury tabel w bazie wynika ze student może mieć tylko jeden (aktualny) wpis na studia
        // studenci w bazie mają index bez "s-ki"
        [HttpGet("{index}")]
        public IActionResult GetStudentEnrollments(string index)
        {

            //List<Enrollment> listaWpisow = new List<Enrollment>();

            Enrollment enrollment = null;

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT Enrollment.IdEnrollment, Semester, Studies.Name, StartDate" +
                    " FROM Enrollment " +
                    "JOIN Student ON Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy " +
                    "WHERE Student.IndexNumber = @index;";

                com.Parameters.AddWithValue("index", index);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    enrollment = new Enrollment();
                    enrollment.IdEnrollment = (int)dr["IdEnrollment"];
                    enrollment.Semester = (int)dr["Semester"];
                    enrollment.Study = dr["Name"].ToString();
                    enrollment.StartDate = dr["StartDate"].ToString();

                    // listaWpisow.Add(enrollment);
                }

            }

            if (enrollment != null)
                return Ok(enrollment);



            return NotFound("Not found");
        }


        ////1 spososób przekazywania danych
        //[HTTPGET("{indexNumber}")]
        //public IActionResult getstudent(int id) //zwraca rezultat z metody action. ...
        //{

        //    if (id == 1)
        //        return ok("jan");
        //    else if (id == 2)
        //        return ok("andrzej");

        //    return notfound("not found");
        //}


        //3 przekazanie dancyh w cile żądania POST
        //[HttpPost]
        //public IActionResult CreateStudent(Student student) //nowy student
        //{
        //    student.IndexNumber = $"s{new Random().Next(1, 20000)}";

        //    ((List<Student>)studentDbService.GetStudents()).Add(student);

        //    return Ok(student);
        //}

        [HttpPut("{id}")]
        public IActionResult UpdateStudnet(string id)  //aktualizacja
        {
            var list = studentDbService.GetStudents();


            foreach (Student st in list)
            {
                if (st.IndexNumber == id)
                {
                    st.IndexNumber = "s6666";
                    return Ok("Aktualizacja dokończona");
                }
            }
            // Console.WriteLine(st.FirstName);

            return NotFound("id not found");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)      //usuwanie
        {
            foreach (Student st in studentDbService.GetStudents())
            {
                if (st.IndexNumber == id)
                {
                    ((List<Student>)studentDbService.GetStudents()).Remove(st);
                    return Ok("Usuwanie ukończone");
                }
            }
            return NotFound("nie znaleziono studenta o id: " + id);
        }

    }
}