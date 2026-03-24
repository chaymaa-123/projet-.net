using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Readify.Models;

namespace Readify.Controllers
{
    public class LivresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public LivresController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Livres
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Livres.Include(l => l.Genre);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Livres/Affichage
        public async Task<IActionResult> Affichage(int? genreId)
        {
            var livresQuery = _context.Livres.Include(l => l.Genre).AsQueryable();

            if (genreId.HasValue && genreId.Value != 0)
            {
                livresQuery = livresQuery.Where(l => l.GenreID == genreId.Value);
            }

            var livresFiltres = await livresQuery.ToListAsync();
            var genres = await _context.Genres.OrderBy(g => g.NomGenre).ToListAsync();

            ViewData["GenresPourFiltre"] = new SelectList(genres, "GenreID", "NomGenre", genreId);
            return View(livresFiltres);
        }

        // GET: Livres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var livre = await _context.Livres.Include(l => l.Genre).FirstOrDefaultAsync(m => m.LivreId == id);
            if (livre == null) return NotFound();
            return View(livre);
        }

        // GET: Livres/Create
        public IActionResult Create()
        {
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreID", "NomGenre");
            return View();
        }

        // POST: Livres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LivreId,Titre,Resume,Prix,DatePublication,EstPubliee,GenreID,ImageFile")] Livre livre)
        {
            ModelState.Remove("Genre"); // Ignore validation Genre

            if (ModelState.IsValid)
            {
                // SAUVEGARDE IMAGE
                if (livre.ImageFile != null)
                {
                    string dossierImages = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                    string nomFichier = Guid.NewGuid().ToString() + "_" + livre.ImageFile.FileName;
                    string cheminComplet = Path.Combine(dossierImages, nomFichier);

                    using (var fileStream = new FileStream(cheminComplet, FileMode.Create))
                    {
                        await livre.ImageFile.CopyToAsync(fileStream);
                    }
                    livre.ImageUrl = nomFichier;
                }

                _context.Add(livre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Affichage));
            }
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreID", "NomGenre", livre.GenreID);
            return View(livre);
        }

        // GET: Livres/Edit/5
        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var livre = await _context.Livres.FindAsync(id);
            if (livre == null) return NotFound();

            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreID", "NomGenre", livre.GenreID);
            return View(livre); // Envoie vers Edit.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Livre livre)
        {
            // --- AFFICHAGE DANS LA CONSOLE VISUAL STUDIO ---
            System.Diagnostics.Debug.WriteLine("======= DEBUG EDIT =======");
            System.Diagnostics.Debug.WriteLine($"ID URL : {id}");
            System.Diagnostics.Debug.WriteLine($"ID Objet : {livre.LivreId}");
            System.Diagnostics.Debug.WriteLine($"Titre reçu : {livre.Titre}");
            System.Diagnostics.Debug.WriteLine($"Prix reçu : {livre.Prix}");
            System.Diagnostics.Debug.WriteLine($"Resume reçu : {livre.Resume}");
            System.Diagnostics.Debug.WriteLine("==========================");

            ModelState.Remove("Genre");
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbLivre = await _context.Livres.FindAsync(id);
                    if (dbLivre == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERREUR : Livre introuvable en base.");
                        return NotFound();
                    }

                    // Mise à jour manuelle
                    dbLivre.Titre = livre.Titre;
                    dbLivre.Prix = livre.Prix;
                    dbLivre.Resume = livre.Resume;
                    dbLivre.GenreID = livre.GenreID;

                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine("SUCCÈS : Enregistrement réussi !");
                    return RedirectToAction(nameof(Livre));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERREUR SQL : {ex.Message}");
                }
            }
            else
            {
                // Affiche pourquoi la validation échoue
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERREUR VALIDATION : {error.ErrorMessage}");
                    }
                }
            }

            return RedirectToAction(nameof(Livre));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var livre = await _context.Livres.Include(l => l.Genre).FirstOrDefaultAsync(m => m.LivreId == id);
            if (livre == null) return NotFound();
            return View(livre);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre != null) _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Affichage));
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.LivreId == id);
        }
    }
}