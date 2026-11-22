using AutoService.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace AutoService.Pages
{
    public class UslugiModel : PageModel
    {
        private readonly Database _базаƒанных;

        public List<Usluga> ”слуги { get; set; } = new();

        public UslugiModel()
        {
            _базаƒанных = new Database();
        }

        public void OnGet()
        {
            ”слуги = _базаƒанных.GetServices();
        }
    }
}
