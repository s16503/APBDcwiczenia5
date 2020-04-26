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
using Microsoft.AspNetCore.Authorization;
using APBDcwiczenia5.DOTs.Requests;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace APBDcwiczenia5.Controllers
{

    [ApiController]     //definiuje zestaw standardowych cech dla api ,, walidować modele
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16503;Integrated Security=True;";
        private readonly IStudentDbService studentDbService;

        public IConfiguration Configuration { get; set; }

        public StudentsController(IStudentDbService db, IConfiguration configuration)
        {
            studentDbService = db;
            Configuration = configuration;
        }

        //2 q
        [HttpGet]       // odpowiada na żądanie GET
        [Authorize(Roles = "employee")]
        public IActionResult GetStudents() //action methos
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



        [HttpPut("{idPassword}")]
        public IActionResult UpdateStudnet(string idPassword)  // ustawienie hasła i salt
        {
            string index = idPassword.Split(" ")[0];
            string password = idPassword.Split(" ")[1];

    
            
            var list = studentDbService.GetStudents();


            foreach (Student st in list)
            {
                if (st.IndexNumber == index)
                {

                    if (studentDbService.GetSalt(index) == null || studentDbService.GetSalt(index).Equals(""))                 // jesli nie ma soli to ją dodaje
                        studentDbService.UpdateSalt(index,CreateSalt());
                    
                    string hashPassw = Create(password, studentDbService.GetSalt(index));

                
                   studentDbService.UpdatePassword(index, hashPassw);




                    return Ok(studentDbService.getPassword(index));
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

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {

         
            var studPass = studentDbService.getPassword(request.Login);

            if(studPass == null)
                return BadRequest("Błędny login lub hasło!");

            var salt = studentDbService.GetSalt(request.Login);
            

           
            if (!Validate(request.Password,salt,studPass))
            {
                return BadRequest("Błędny login lub hasło!");
            }

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name,request.Login),
                new Claim(ClaimTypes.Role, "employee"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );


            
            var refreshToken = Guid.NewGuid();
            studentDbService.UpdateRefreshToken(request.Login, refreshToken.ToString());

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken        
            });
        }


        [HttpPost("refresh-token/{refToken}")]
        public IActionResult RefreshToken(string refToken)
        {
            var index = studentDbService.getRefreshToken(refToken);
            if (index == null)
            {
                return BadRequest("nie ma tokenu");
            }

            var salt = studentDbService.GetSalt(index);
          

            var claims = new[]
         {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name,index),
                new Claim(ClaimTypes.Role, "employee"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );



            var refreshToken = Guid.NewGuid();
            studentDbService.UpdateRefreshToken(index, refreshToken.ToString());

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken
            });

        }



        ///////////////////////////////////////////////////////////

        public static string Create(string value, string salt)
        {
            var valuesBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                );
            return Convert.ToBase64String(valuesBytes);
        }

        public static bool Validate(string value, string salt, string hash)
            => Create(value, salt) == hash;

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }

        }
        ///////////////////////////////////////////////////////////////////
    }
}