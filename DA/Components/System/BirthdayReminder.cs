using DA.Application.Abstractions;
using DA.Domain.Entities;
using DA.Persistence.Services;

namespace DA.Components.System
{
    public class BirthdayReminder
    {
        private readonly IServiceProvider _serviceProvider;
        

        public BirthdayReminder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void CheckSpecialDates()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();

                var today = DateTime.Now.Date;
                var employees = employeeService.GetAll();

                var usersWithBirthdayToday = employees
                    .Where(u => u.DateOfBirth.Day == today.Day && u.DateOfBirth.Month == today.Month)
                    .ToList();

                var usersWithStartDayToday = employees
                    .Where(u => u.DateOfStart.Day == today.Day && u.DateOfStart.Month == today.Month)
                    .ToList();

                string toWho = "";
                string subject = "";
                string body = "";

                foreach (var user in usersWithBirthdayToday)
                {

                    toWho = user.Email;
                    subject = "Doğum Gününüz Kutlu Olsun";
                    body = $"Merhaba {user.Name + " " + user.Surname},\n"+ Constants.ProjectFullName +" ailesi adına doğum gününüzü kutlarız. Yeni yaşınızın size sağlık, mutluluk ve şans getirmesi dileğiyle...\nDaha nice yaşlara.";

                    MailSenderService.SendEmail(toWho, subject, body);
                }

                foreach (var user in usersWithStartDayToday)
                {
                    toWho = user.Email;
                    subject = "Çalışma Yıldönümü Kutlaması";

                    body = $"Merhaba {user.Name + " " + user.Surname},\nKıymetli çalışma arkadaşımız, ajansımızda sizinle birlikte çalışıyor olmanın {DateTime.Now.Year - user.DateOfStart.Year}. yılını kutlamak büyük bir mutluluk ve gurur kaynağı! Bu süreçte gösterdiğiniz özveri ve gayret için "+ Constants.ProjectFullName + " ailesi adına size teşekkür ederiz. \nDaha nice yıllara ve nice başarılara birlikte ulaşmak dileğiyle.";
                    MailSenderService.SendEmail(toWho, subject, body);

                    Employee employeeDto = employeeService.GetEntityById(user.Id);

                    employeeDto.TotalYearlyLeave += employeeDto.RefresherYearlyLeave;

                    employeeService.UpdateEntity(employeeDto);
                }
            }
        }
    }
}
