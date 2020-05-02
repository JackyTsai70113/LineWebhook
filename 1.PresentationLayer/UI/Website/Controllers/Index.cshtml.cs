using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Website.Data;

namespace Website.Controllers
{
    public class IndexModel : PageModel
    {
        private readonly Website.Data.LineWebhookContext _context;

        public IndexModel(Website.Data.LineWebhookContext context)
        {
            _context = context;
        }

        public IList<Note> Note { get;set; }

        public async Task OnGetAsync()
        {
            Note = await _context.Note.ToListAsync();
        }
    }
}
