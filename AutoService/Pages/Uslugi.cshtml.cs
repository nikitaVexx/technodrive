using AutoService.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace AutoService.Pages
{
    public class UslugiModel : PageModel
    {
        private readonly Database _базаДанных;

        public List<Usluga> Услуги { get; set; } = new();

        public UslugiModel()
        {
            _базаДанных = new Database();
        }

        public void OnGet()
        {
            Услуги = _базаДанных.GetServices();
        }
    }
}
