using Medinova.Dtos;
using Medinova.Enums;
using Medinova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Controllers
{

    [AllowAnonymous]
    public class DefaultController : Controller
    {
        MedinovaDbEntities1 context = new MedinovaDbEntities1();

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]  
        public PartialViewResult DefaultAppointment() 
        {
            var setting = context.AppointmentSettings.FirstOrDefault();
            ViewBag.AppointmentSetting = setting;

            // Veritabanından tüm departmanları çek ve listeye al
            var departments = context.Departments.ToList();

            // Departmanları dropdown için SelectListItem formatına çevir
            ViewBag.Departments = (from department in departments
                                   select new SelectListItem
                                   {
                                       Text = department.Name,                        // Dropdown'da görünecek isim
                                       Value = department.DepartmentId.ToString()     // Arka planda gönderilecek ID
                                   }).ToList();

            // Tarih listesi için boş bir liste oluştur
            var dateList = new List<SelectListItem>();

            // Bugünden itibaren 7 günlük tarih listesi oluştur
            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(i);  // Bugün + i gün (0=bugün, 1=yarın ...)

                dateList.Add(new SelectListItem
                {
                    Text = date.ToString("dd.MMMM.dddd"),   // Görünen format: "30.Mart.Pazartesi"
                    Value = date.ToString("yyyy-MM-dd")      // Gönderilen format: "2026-03-30"
                });
            }

            // Tarih listesini View'e gönder
            ViewBag.Dates = dateList;

            // PartialView'i render et ve döndür
            return PartialView();
        }

        [HttpPost]
        public ActionResult MakeAppointment(Appointments appointment) 
        { 
            
            appointment.IsActive= true;
            context.Appointments.Add(appointment);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        //Json türünde veri döndüren metot
        public JsonResult GetDoctorsByDepartmentId(int departmentId)
        {
            //Sadece DepartmentId = gelen departmentId olan doktorlar, o bölüme ait doktorlar.
            //Gelen doktor listesini farklı bir formata çeviriyor.Her doktoru bir SelectListItem haline getiriyor.SelectListItem genelde dropdown (select box) için kullanılır.
            var doctors = context.Doctors.Where(x => x.DepartmentId == departmentId)
                                          .Select(doctor => new SelectListItem
                                          {
                                              Text = doctor.FullName,
                                              Value = doctor.DoctorId.ToString()
                                          }).ToList();

            return Json(doctors, JsonRequestBehavior.AllowGet);//doctors listesini JSON olarak geri döndürür. AJAX bunu alıp kullanabilir.
           //JsonRequestBehavior.AllowGetGET isteğiyle JSON döndürmeye izin verirASP.NET MVC güvenlik kuralı gereği yazılır.

        }
        [HttpPost] 
        public JsonResult GetAvailableHours(DateTime selectedDate, int doctorId)
        {
            // Seçilen doktorun, seçilen tarihteki dolu randevu saatlerini veritabanından çek
            var bookedTimes = context.Appointments
                .Where(x => x.DoctorId == doctorId && x.AppointmentDate == selectedDate) // O doktora ve tarihe ait randevuları filtrele
                .Select(x => x.AppointmentTime) // Sadece saat bilgisini al
                .ToList(); // Listeye çevir

            // Sonuç listesi oluştur (her saat için müsait mi dolu mu bilgisi tutacak)
            var dtoList = new List<AppointmentAvailabilityDto>();

            // Tüm çalışma saatlerini döngüyle gez (Times.AppointmentHours = sabit saat listesi)
            foreach (var hour in Times.AppointmentHours)
            {
                var dto = new AppointmentAvailabilityDto(); // Her saat için yeni bir DTO nesnesi oluştur
                dto.Time = hour; // Saati ata

                if (bookedTimes.Contains(hour))
                {
                    dto.IsBooked = true;  // Bu saat dolu
                }
                else
                {
                    dto.IsBooked = false; // Bu saat müsait
                }

                dtoList.Add(dto); // Listeye ekle
            }

            // Listeyi JSON formatında döndür (jQuery AJAX bunu okuyacak)
            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult DefaultHero()
        {
            return PartialView();
        }

        public PartialViewResult DefaultAbout()
        {
            var abouts = context.Abouts.FirstOrDefault(); 
            return PartialView(abouts);
        }

        public PartialViewResult DefaultService()
        {
            var services = context.Services.ToList();
            return PartialView(services);
        }

        public PartialViewResult DefaultPricingPlan()
        {
            return PartialView();
        }
        public PartialViewResult DefaultTeam()
        {
            var doctors = context.Doctors.ToList();
            return PartialView(doctors);
        }
        public PartialViewResult DefaultSearch()
        {
            return PartialView();
        }
        public PartialViewResult DefaultTestimonial()
        {
            return PartialView();
        }
        public PartialViewResult DefaultBlog()
        {
            return PartialView();
        }
        public PartialViewResult DefaultAboutItems()
        {
            var items = context.AboutItems.ToList();
            return PartialView(items);
        }

    }
}
