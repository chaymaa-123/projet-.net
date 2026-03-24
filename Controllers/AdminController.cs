using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Readify.Models;

namespace Readify.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Livre()
        {
            var livres = _context.Livres.Include(l => l.Genre).ToList();
            ViewBag.TotalLivres = livres.Count;
            ViewBag.PrixMoyen = livres.Any() ? (double)livres.Average(l => l.Prix) : 0;
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreID", "NomGenre");
            return View(livres);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Livre livre)
        {
            // On ignore la validation automatique pour permettre les champs vides
            ModelState.Clear();

            try
            {
                string folder = Path.Combine(_hostEnvironment.WebRootPath, "img");

                // GESTION DE L'IMAGE
                if (livre.ImageFile != null)
                {
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(livre.ImageFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await livre.ImageFile.CopyToAsync(stream);
                    }

                    if (livre.LivreId != 0)
                    {
                        var oldLivre = await _context.Livres.AsNoTracking().FirstOrDefaultAsync(l => l.LivreId == livre.LivreId);
                        if (oldLivre != null && !string.IsNullOrEmpty(oldLivre.ImageUrl))
                        {
                            string oldPath = Path.Combine(folder, oldLivre.ImageUrl);
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }
                    }
                    livre.ImageUrl = fileName;
                }

                if (livre.LivreId == 0)
                {
                    // AJOUT : Le titre est quand même requis pour un nouveau livre
                    if (string.IsNullOrEmpty(livre.Titre)) return RedirectToAction(nameof(Livre));
                    _context.Add(livre);
                }
                else
                {
                    // MODIFICATION PARTIELLE : On récupère l'existant
                    var dbLivre = await _context.Livres.AsNoTracking().FirstOrDefaultAsync(l => l.LivreId == livre.LivreId);
                    if (dbLivre == null) return NotFound();

                    // On ne remplace que si l'utilisateur a saisi quelque chose
                    livre.Titre = string.IsNullOrWhiteSpace(livre.Titre) ? dbLivre.Titre : livre.Titre;
                    livre.Prix = (livre.Prix == null || livre.Prix <= 0) ? dbLivre.Prix : livre.Prix;
                    livre.Resume = string.IsNullOrWhiteSpace(livre.Resume) ? dbLivre.Resume : livre.Resume;
                    livre.GenreID = (livre.GenreID == null || livre.GenreID == 0) ? dbLivre.GenreID : livre.GenreID;

                    if (livre.ImageFile == null) livre.ImageUrl = dbLivre.ImageUrl;

                    _context.Update(livre);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur : {ex.Message}");
            }

            return RedirectToAction(nameof(Livre));
        }

        // --- SECTION GENRES (Correction erreur image_ea14e6) ---



        public IActionResult Genre()
        {
            var genres = _context.Genres.Include(g => g.Livres).ToList();
            ViewBag.TotalGenres = genres.Count;
            return View(genres);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertGenre(Genre genre)
        {
            // Empêche le crash si le genre existe déjà
            bool exists = await _context.Genres.AnyAsync(g => g.NomGenre == genre.NomGenre && g.GenreID != genre.GenreID);
            if (exists) return RedirectToAction(nameof(Genre));

            if (genre.GenreID == 0) _context.Add(genre);
            else _context.Update(genre);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Genre));
        }

        // Reste de ton code (Delete, etc.)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre != null)
            {
                if (!string.IsNullOrEmpty(livre.ImageUrl))
                {
                    string path = Path.Combine(_hostEnvironment.WebRootPath, "img", livre.ImageUrl);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                _context.Livres.Remove(livre);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Livre));
        }
        // Action pour afficher la liste des utilisateurs
        // --- SECTION UTILISATEURS ---

        [HttpGet]
        [HttpGet]
        public IActionResult Utilisateurs()
        {
            // On récupère les utilisateurs pour le tableau
            var liste = _context.Utilisateurs.Include(u => u.Role).ToList();

            // --- RÉCUPÉRATION DES RÔLES ---
            // On crée une SelectList : RoleID est la valeur, NomRole est le texte affiché
            ViewBag.Roles = new SelectList(_context.Roles, "RoleID", "NomRole");

            // Optionnel : Récupérer l'ID de l'Admin pour le mettre par défaut
            var adminRole = _context.Roles.FirstOrDefault(r => r.NomRole == "Admin");
            ViewBag.AdminRoleId = adminRole?.RoleID ?? 0;

            return View(liste);
        }

        // Optionnel : Supprimer un utilisateur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Utilisateurs.FindAsync(id);
            if (user != null)
            {
                _context.Utilisateurs.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Utilisateurs));
        }
        [HttpGet]


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertUtilisateur(Utilisateur utilisateur)
        {
            ModelState.Remove("Role"); // Important pour ne pas valider l'objet lié

            if (utilisateur.UtilisateurID == 0)
            {
                // Logique Ajout...
                utilisateur.DateInscription = DateTime.Now;
                _context.Add(utilisateur);
            }
            else
            {
                // LOGIQUE UPDATE (Image_ebf144)
                var dbUser = await _context.Utilisateurs.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UtilisateurID == utilisateur.UtilisateurID);

                if (dbUser == null) return NotFound();

                // On garde l'ancien mot de passe si le champ est vide
                if (string.IsNullOrWhiteSpace(utilisateur.MotDePasseHash))
                    utilisateur.MotDePasseHash = dbUser.MotDePasseHash;

                utilisateur.DateInscription = dbUser.DateInscription;
                _context.Update(utilisateur);
            }

            await _context.SaveChangesAsync(); // C'est ici que l'erreur FK arrivait (Image_ebf144)
            return RedirectToAction(nameof(Utilisateurs));
        }
        // --- GESTION DES COMMANDES (CRUD) ---

        // --- GESTION DES COMMANDES (CRUD FINAL) ---

        public IActionResult Commandes()
        {
            var commandes = _context.Commandes
                .Include(c => c.Utilisateur)
                .OrderByDescending(c => c.DateCommande)
                .ToList();

            ViewBag.TotalCommandes = commandes.Count;
            // Utilisation de MontantTotal pour le calcul
            ViewBag.ChiffreAffaire = commandes.Sum(c => c.MontantTotal);
            ViewBag.Users = new SelectList(_context.Utilisateurs, "UtilisateurID", "NomUtilisateur");

            return View(commandes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCommande(Commande commande)
        {
            ModelState.Remove("Utilisateur");

            if (commande.CommandeID == 0)
            {
                // CREATE
                commande.DateCommande = DateTime.Now;
                if (string.IsNullOrEmpty(commande.Statut)) commande.Statut = "En attente";
                _context.Add(commande);
            }
            else
            {
                // UPDATE
                var dbOrder = await _context.Commandes.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CommandeID == commande.CommandeID);

                if (dbOrder == null) return NotFound();

                commande.DateCommande = dbOrder.DateCommande; // On préserve la date initiale
                _context.Update(commande);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Commandes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande != null)
            {
                _context.Commandes.Remove(commande);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Commandes));
        }
        public IActionResult Statistiques()
        {
            // 1. Statistiques Globales
            ViewBag.TotalLivres = _context.Livres.Count();
            ViewBag.TotalUsers = _context.Utilisateurs.Count();
            ViewBag.TotalCommandes = _context.Commandes.Count();

            // 2. Chiffre d'affaires total (Utilise MontantTotal de votre modèle Commande)
            ViewBag.ChiffreAffaire = _context.Commandes.Sum(c => (decimal?)c.MontantTotal) ?? 0m;

            // 3. Prix moyen d'un livre (Correction avec 0m pour le type decimal)
            ViewBag.PrixMoyen = _context.Livres.Any() ? (double)_context.Livres.Average(l => l.Prix) : 0;

            // 4. Données pour un graphique (Ventes par mois par exemple)
            var ventesParMois = _context.Commandes
                .GroupBy(c => c.DateCommande.Value.Month)
                .Select(g => new { Mois = g.Key, Total = g.Sum(c => c.MontantTotal) })
                .OrderBy(g => g.Mois)
                .ToList();

            return View();
        }
    }
}