using AutoService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AutoService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Database _базаДанных = new Database();

        [BindProperty]
        public string ИмяКлиента { get; set; } = "";

        [BindProperty]
        public string Телефон { get; set; } = "";

        [BindProperty]
        public string ОписаниеПроблемы { get; set; } = "";

        [BindProperty]
        public int ИдентификаторУслуги { get; set; }

        [BindProperty]
        public string ДатаОбслуживания { get; set; } = "";

        [BindProperty]
        public string ВремяОбслуживания { get; set; } = "";

        public string Сообщение { get; set; } = "";
        public List<Usluga> Услуги { get; set; } = new();

        public List<string> ВремяЗаписи { get; set; } = new()
        {
            "09:00", "09:30", "10:00", "10:30",
            "11:00", "11:30", "12:00", "12:30",
            "13:00", "13:30", "14:00", "14:30",
            "15:00", "15:30", "16:00", "16:30",
            "17:00", "17:30", "18:00", "18:30",
            "19:00", "19:30"
        };

        public void OnGet()
        {
            Услуги = _базаДанных.GetServices();
        }

        public IActionResult OnPost()
        {
            Услуги = _базаДанных.GetServices();

            if (string.IsNullOrWhiteSpace(ИмяКлиента) ||
                string.IsNullOrWhiteSpace(Телефон) ||
                string.IsNullOrWhiteSpace(ОписаниеПроблемы))
            {
                Сообщение = "Заполните все поля.";
                return Page();
            }

            if (!Услуги.Any(x => x.Идентификатор == ИдентификаторУслуги))
            {
                Сообщение = "Выберите услугу из списка.";
                return Page();
            }

            if (!DateTime.TryParseExact(ДатаОбслуживания, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime дата) ||
                дата.Date < DateTime.Today)
            {
                Сообщение = "Выберите корректную дату обслуживания.";
                return Page();
            }

            if (!ВремяЗаписи.Contains(ВремяОбслуживания))
            {
                Сообщение = "Выберите время из списка.";
                return Page();
            }

            TimeSpan время = TimeSpan.Parse(ВремяОбслуживания);

            if (дата.DayOfWeek == DayOfWeek.Sunday &&
                (время < new TimeSpan(10, 0, 0) || время >= new TimeSpan(17, 0, 0)))
            {
                Сообщение = "В воскресенье запись доступна с 10:00 до 17:00.";
                return Page();
            }

            if (дата.Date == DateTime.Today && время <= DateTime.Now.TimeOfDay)
            {
                Сообщение = "Выберите время, которое ещё не наступило.";
                return Page();
            }

            bool добавлена = _базаДанных.AddRequest(
                ИмяКлиента.Trim(), Телефон.Trim(), ОписаниеПроблемы.Trim(),
                ИдентификаторУслуги, ДатаОбслуживания, ВремяОбслуживания);

            if (!добавлена)
            {
                Сообщение = "Это время уже занято. Выберите другое.";
                return Page();
            }

            TempData["СообщениеУспеха"] = "Ваша заявка успешно отправлена!";
            return RedirectToPage("/Index");
        }
    }
}
