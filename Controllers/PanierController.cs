using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readify.Models;

namespace Readify.Controllers
{
    public class PanierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PanierController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var items = await _context.Paniers.Include(p => p.Livre).ToListAsync();
            return View(items);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Ajouter(int livreId)
        {
            var itemExistant = await _context.Paniers.FirstOrDefaultAsync(p => p.LivreId == livreId);

            if (itemExistant != null)
            {
                itemExistant.Quantite++;
            }
            else
            {
                var nouvelItem = new Panier { LivreId = livreId, Quantite = 1, DateAjout = DateTime.Now };
                _context.Paniers.Add(nouvelItem);
            }

            await _context.SaveChangesAsync();

            // RECTIFICATION ICI : Rediriger vers la vue Details du livre ajouté
            return RedirectToAction("Details", "Livres", new { id = livreId });
        }
        // Action AJAX pour mettre à jour sans refresh
        [HttpPost]
        public async Task<JsonResult> UpdateQuantiteAjax(int id, int changement)
        {
            var item = await _context.Paniers.Include(p => p.Livre).FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                item.Quantite += changement;
                if (item.Quantite <= 0) _context.Paniers.Remove(item);

                await _context.SaveChangesAsync();

                // On renvoie les nouvelles valeurs pour mettre à jour l'interface
                return Json(new
                {
                    success = true,
                    nouvelleQty = item.Quantite,
                    nouveauSousTotal = (item.Quantite * item.Livre.Prix).ToString("N2"),
                    nouveauTotalGeneral = _context.Paniers.Sum(p => p.Quantite * p.Livre.Prix).ToString("N2")
                });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Supprimer(int id)
        {
            var item = await _context.Paniers.FindAsync(id);
            if (item != null) { _context.Paniers.Remove(item); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Vider()
        {
            _context.Paniers.RemoveRange(_context.Paniers);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}