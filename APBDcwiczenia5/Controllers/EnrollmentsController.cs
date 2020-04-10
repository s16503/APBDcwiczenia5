using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBDcwiczenia5.DOTs.Requests;
using APBDcwiczenia5.Models;
using APBDcwiczenia5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBDcwiczenia5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
     
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

  
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request) //nowy student
        {

            try
            {
                return Ok(_service.EnrollStudent(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

         
        }

        [HttpPost("{promotions}")]
        public IActionResult PromoteStudents(PromotionStudentRequest request, string promotions)
        {
            if (promotions.Equals("promotions"))
            {
                try
                {

                return Ok(_service.PromoteStudents(request.Semester,request.Studies));


                }catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                
            }
            else
                return BadRequest("Bad request!");

        }
    }
}