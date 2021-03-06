﻿using APBDcwiczenia5.Controllers;
using APBDcwiczenia5.DOTs.Requests;
using APBDcwiczenia5.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace APBDcwiczenia5.Services
{
    public class SqlServerDbService : IStudentDbService
    {

        private string myConnection = "Data Source=db-mssql;Initial Catalog=s16503;Integrated Security=True;";
        public SqlServerDbService(/*.. */ )
        {

        }

       

        public Student GetStudent(string index)
        {

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.Parameters.AddWithValue("Index", index);
                com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @Index;";

                con.Open();
                var dr = com.ExecuteReader();

                if (dr.Read())
                {
                    Student st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        IdEnrollment = (int)dr["IdEnrollment"]

                    };

                    return st;
                }
            }

            return null;
        }

        public List<Student> GetStudents()
        {
            List<Student> resList = new List<Student>();

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM Student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.IdEnrollment = (int)dr["IdEnrollment"];
                    st.BirthDate = dr["BirthDate"].ToString();
                    ///  st.IdStudy = (int)dr["IdStudy"];

                    resList.Add(st);
                }

            }

            return resList;
        }

        public Enrollment EnrollStudent(EnrollStudentRequest request) //nowy student
        {


            var enrollment = new Enrollment();

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {


                com.Connection = con;
                com.CommandText = "select IdStudy from Studies where name=@name";
                com.Parameters.AddWithValue("name", request.Studies);
                con.Open();

                var tran = con.BeginTransaction("SampleTransaction");
                com.Transaction = tran;


                //1. Czy studia istnieja?
                //com.CommandText = "select IdStudy from Studies where name=@name";

                var dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    dr.Close();
                    tran.Rollback();
                    throw new Exception("Studia nie istnieją");
                    //...
                }
                int idstudies = (int)dr["IdStudy"];

                // return Ok(idstudies);
                //com.Parameters.AddWithValue("idStud", idstudies);

                com.CommandText = "SELECT IdEnrollment FROM Enrollment " +

                                      "WHERE IdStudy =" + idstudies + " AND Enrollment.Semester = 1 ORDER BY StartDate;";


                com.Parameters.AddWithValue("Index", request.IndexNumber);
                com.Parameters.AddWithValue("Fname", request.FirstName);
                com.Parameters.AddWithValue("Lname", request.LastName);
                com.Parameters.AddWithValue("BirthDate", request.BirthDate);

                dr.Close();
                dr = com.ExecuteReader();

                int newIdEnrollment;
                if (dr.Read())
                {
                    newIdEnrollment = (int)dr["IdEnrollment"];

                    // return Ok(newIdEnrollment);

                    // com.Parameters.AddWithValue("IdEnroll", dr["IdEnrollment"].ToString());

                    //  com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName,BirthDate, IdEnrollment) VALUES(@Index, @Fname, @Lname,@BirthDate, @IdEnroll )";
                    //  com.ExecuteNonQuery();
                }
                else
                {
                    com.CommandText = "SELECT max(IdEnrollment) as id FROM Enrollment;";
                    dr.Close();
                    dr = com.ExecuteReader();
                    dr.Read();

                    newIdEnrollment = ((int)dr["id"]) + 1;

                    // return Ok(newIdEnrollment);
                    com.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,  IdStudy, StartDate) " +
                                      "VALUES(" + newIdEnrollment + ", 1," + idstudies + ",'" + DateTime.Now + "');";

                    dr.Close();
                    com.ExecuteNonQuery();
                    //   tran.Commit();
                }
                com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName,BirthDate, IdEnrollment) " +
                        "VALUES(@Index, @Fname, @Lname, CONVERT(DATETIME,@BirthDate,104), " + newIdEnrollment + " ) ";
                dr.Close();

                com.ExecuteNonQuery();
                //x.Dodanie studenta
                //com.CommandText = "INSERT INTO Student(IndexNumber, FirstName) VALUES(@Index, @Fname)";
                //com.Parameters.AddWithValue("index", request.IndexNumber);
                //...

                tran.Commit();


                com.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment = " + newIdEnrollment + " ;";
                dr.Close();
                dr = com.ExecuteReader();
                dr.Read();

                enrollment.IdEnrollment = newIdEnrollment;
                enrollment.Semester = (int)dr["Semester"];
                enrollment.StartDate = dr["StartDate"].ToString();
                enrollment.Study = request.Studies;

            }

            return enrollment;
        }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            int idStud;

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {

                com.Connection = con;
                com.CommandText = "SELECT Studies.IdStudy FROm Studies WHERE Studies.Name = '" + studies + "';";

                con.Open();
                var dr = com.ExecuteReader();


                if (dr.Read())
                {
                    idStud = (int)dr["IdStudy"];
                }
                else
                    throw new Exception("Nie znaleziono studiów o tej nazwie");


                //  var tran = con.BeginTransaction("SampleTransaction");
                //  com.Transaction = tran;

                com.CommandText = "select IdEnrollment from Enrollment where IdStudy=" + idStud + " AND Semester= " + semester + ";";
                // con.Open();
                //  dr = com.ExecuteReader();
                dr.Close();
                dr = com.ExecuteReader();
                if (!dr.Read())
                    throw new Exception("Nie znaleziono wpisu");

                con.Close();
                dr.Close();
            }

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand("Promotions", con))
            {
                com.Connection = con;
                con.Open();
             
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.Add("@Studies", SqlDbType.VarChar).Value = studies;
                    com.Parameters.Add("@Semester", SqlDbType.Int).Value = semester;
                    com.ExecuteNonQuery();

                    // NIECH PROCEDURA ZWRACA ENROLLMENT (selectem ) << reultSet
                    con.Close();
                              
            }


            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM Enrollment WHERE Semester = " + (semester + 1) + " AND IdStudy = " + idStud + " ORDER BY IdEnrollment;";

                con.Open();
                var dr = com.ExecuteReader();

                var enrollment = new Enrollment();

                if (dr.Read())
                {
                    enrollment.IdEnrollment = (int)dr["IdEnrollment"];
                    enrollment.Semester = (int)dr["Semester"];
                    enrollment.Study = studies;
                    enrollment.StartDate = dr["StartDate"].ToString();
                }
                else
                    throw new Exception("nie mogę zwrócić nowego wpisu");

                return enrollment;

            }
        }

        // sprawdzam czy istnieje student + hasło
        public Student GetStudent(string index, string haslo)
        {
            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                 com.Parameters.AddWithValue("Index", index);
                 com.Parameters.AddWithValue("Password", haslo);
                

                com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @Index AND password = @Password;";

                con.Open();
                var dr = com.ExecuteReader();

                if (dr.Read())
                {
                    Student st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        IdEnrollment = (int)dr["IdEnrollment"]

                    };

                    return st;
                }
            }

            return null;
        }

        public void UpdateRefreshToken(string index, string _refreshToken)
        {
            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Parameters.AddWithValue("Index", index);
                com.Parameters.AddWithValue("RefreshToken", _refreshToken);
                com.Connection = con;
                com.CommandText = "UPDATE Student SET refreshToken = @RefreshToken WHERE IndexNumber = @Index;";
                con.Open();
                var tran = con.BeginTransaction("SampleTransaction");
                com.Transaction = tran;

                com.ExecuteNonQuery();

                tran.Commit();



            }
        }



        public string GetSalt(string index)
        {
            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Parameters.AddWithValue("Index", index);
                com.Connection = con;
                com.CommandText = "SELECT salt FROM Student WHERE IndexNumber = @Index;";
                con.Open();
                var dr = com.ExecuteReader();



                if (dr.Read())
                {
                    return dr["salt"].ToString();
                }
                else
                    return null;
               
            }
        }

        public void UpdateSalt(string index, string salt)
        {
            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Parameters.AddWithValue("Index", index);
                com.Parameters.AddWithValue("Salt", salt);
                com.Connection = con;
                com.CommandText = "UPDATE Student SET salt = @Salt WHERE IndexNumber = @Index;";
                con.Open();
                var tran = con.BeginTransaction("SampleTransaction");
                com.Transaction = tran;

                com.ExecuteNonQuery();

                tran.Commit();

              

            }
        }

        public void UpdatePassword(string index, string passw)
        {

          
           using (var con = new SqlConnection(myConnection))
                    using (var com = new SqlCommand())
                    {
                    com.Parameters.AddWithValue("Index", index);
                    com.Parameters.AddWithValue("Passw", passw);
                    com.Connection = con;
                    com.CommandText = "UPDATE Student SET password = @Passw WHERE IndexNumber = @Index;";
                        con.Open();
                        var tran = con.BeginTransaction("UpdatePasswordTran");
                        com.Transaction = tran;

                        com.ExecuteNonQuery();

                        tran.Commit();

                    }





        }

        public string getPassword(string index)
        {

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Parameters.AddWithValue("Index", index);
              
                com.Connection = con;
                com.CommandText = "SELECT password FROM Student WHERE IndexNumber = @Index;";
                con.Open();
            
                var dr = com.ExecuteReader();

                if (dr.Read())
                {
                    return dr["password"].ToString();
                }

                return null;

            }

        }

        public string getRefreshToken(string refreshToken)
        {

            using (var con = new SqlConnection(myConnection))
            using (var com = new SqlCommand())
            {
                com.Parameters.AddWithValue("RefreshToken", refreshToken);

                com.Connection = con;
                com.CommandText = "SELECT * FROM Student WHERE refreshToken = @RefreshToken;";
                con.Open();

                var dr = com.ExecuteReader();

                if (dr.Read())
                {
                    Student st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        IdEnrollment = (int)dr["IdEnrollment"]

                    };
                    return st.IndexNumber;
                }

               

            }

            return null;

        }



    }
}
