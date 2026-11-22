using AutoService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoService.Pages
{
    [Authorize]
    public class ZayavkiModel : PageModel
    {
        private readonly Database _базаДанных = new Database();

        public List<ZayavkaView> Заявки { get; set; } = new();
        public string Search { get; set; } = "";
        public int ВсегоЗаявок { get; set; }
        public int ЗаявокСегодня { get; set; }
        public int КоличествоУслуг { get; set; }

        public void OnGet(string? search)
        {
            Search = search?.Trim() ?? "";
            var всеЗаявки = _базаДанных.GetRequests();

            ВсегоЗаявок = всеЗаявки.Count;
            ЗаявокСегодня = всеЗаявки.Count(z =>
                DateTime.TryParse(z.ДатаЗаявки, out var дата) &&
                дата.Date == DateTime.Today);
            КоличествоУслуг = _базаДанных.GetServices().Count;
            Заявки = всеЗаявки;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Заявки = Заявки.Where(z =>
                    z.ИмяКлиента.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    z.Телефон.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public IActionResult OnPostDelete(int идентификатор)
        {
            _базаДанных.DeleteRequest(идентификатор);
            return RedirectToPage("/Zayavki");
        }
    }
}
