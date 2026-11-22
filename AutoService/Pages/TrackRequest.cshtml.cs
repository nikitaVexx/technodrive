using AutoService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace AutoService.Pages
{
    public class TrackRequestModel : PageModel
    {
        private readonly Database _базаДанных = new Database();

        [BindProperty]
        public string Телефон { get; set; } = "";

        public string Сообщение { get; set; } = "";
        public List<ZayavkaView> Заявки { get; set; } = new List<ZayavkaView>();
        public bool ВыполненПоиск { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            НайтиЗаявки();
        }

        public IActionResult OnPostCancel(int идентификатор)
        {
            string телефонДляПоиска = new string((Телефон ?? "").Where(char.IsDigit).ToArray());
            var заявка = _базаДанных.GetRequests().FirstOrDefault(z =>
                z.Идентификатор == идентификатор &&
                new string(z.Телефон.Where(char.IsDigit).ToArray()) == телефонДляПоиска);

            if (заявка == null)
            {
                Сообщение = "Не удалось найти заявку. Повторите поиск.";
                НайтиЗаявки();
                return Page();
            }

            if (заявка.Статус == "Отменена" || заявка.Статус == "Завершена")
            {
                Сообщение = "Эту заявку нельзя отменить.";
                НайтиЗаявки();
                return Page();
            }

            if (_базаДанных.CancelRequest(идентификатор, заявка.Телефон))
                Сообщение = "Заявка отменена.";
            else
                Сообщение = "Не удалось отменить заявку.";

            НайтиЗаявки();
            return Page();
        }

        private void НайтиЗаявки()
        {
            ВыполненПоиск = true;
            string телефонДляПоиска = new string((Телефон ?? "").Where(char.IsDigit).ToArray());
            if (телефонДляПоиска.Length < 10)
            {
                Сообщение = "Введите корректный номер телефона.";
                return;
            }

            Заявки = _базаДанных.GetRequests()
                .Where(z => new string(z.Телефон.Where(char.IsDigit).ToArray()) == телефонДляПоиска)
                .ToList();

            if (Заявки.Count == 0 && string.IsNullOrEmpty(Сообщение))
                Сообщение = "Заявок с таким номером телефона не найдено.";
        }
    }
}
