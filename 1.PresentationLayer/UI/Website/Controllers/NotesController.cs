using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Website.Data;

namespace Website.Controllers {

    public class NotesController : Controller {
        private readonly LineWebhookContext _context;

        public NotesController(LineWebhookContext context) {
            _context = context;
        }

        // GET: Notes
        public async Task<IActionResult> Index() {
            var noteList = new List<Note>{ new Note {
                Id = new Guid(),
                Remark = "7989"
            } };
            return View(noteList);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null) {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Remark")] Note note) {
            if (ModelState.IsValid) {
                note.Id = Guid.NewGuid();
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var note = await _context.Note.FindAsync(id);
            if (note == null) {
                return NotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Remark")] Note note) {
            if (id != note.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!NoteExists(note.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null) {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id) {
            var note = await _context.Note.FindAsync(id);
            _context.Note.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(Guid id) {
            return _context.Note.Any(e => e.Id == id);
        }
    }
}