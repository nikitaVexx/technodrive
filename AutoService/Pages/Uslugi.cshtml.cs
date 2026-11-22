using AutoService.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace AutoService.Pages
{
    public class UslugiModel : PageModel
    {
        private readonly Database _áàçàÄàííûõ;

       public List<Usluga> Services { get; set; } = new();

        public UslugiModel()
        {
            Services = _базаДанных.GetServices();
        }

        public void OnGet()
        {
            Services = _базаДанных.GetServices();
        }
    }
}
