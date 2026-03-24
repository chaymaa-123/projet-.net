using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Readify.Models;

namespace Readify.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtilisateursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Affiche la liste avec le nom du rôle
        public async Task<IActionResult> Index()
        {
            var users = _context.Utilisateurs.Include(u => u.Role);
            return View(await users.ToListAsync());
        }

        // GET: Préparation de la création
        public IActionResult Create()
        {
            // Affiche "Admin" ou "Client" au lieu de l'ID technique
            ViewData["RoleID"] = new SelectList(_context.Roles, "RoleID", "NomRole");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UtilisateurID,NomUtilisateur,Email,MotDePasseHash,RoleID")] Utilisateur utilisateur)
        {
            // Supprime la validation de l'objet de navigation pour éviter les blocages
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                // Définit la date système automatiquement
                utilisateur.DateInscription = DateTime.Now;

                _context.Add(utilisateur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoleID"] = new SelectList(_context.Roles, "RoleID", "NomRole", utilisateur.RoleID);
            return View(utilisateur);
        }

        // GET: Préparation de la modification
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null) return NotFound();

            ViewData["RoleID"] = new SelectList(_context.Roles, "RoleID", "NomRole", utilisateur.RoleID);
            return View(utilisateur);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtilisateurID,NomUtilisateur,Email,MotDePasseHash,RoleID")] Utilisateur utilisateur)
        {
            if (id != utilisateur.UtilisateurID) return NotFound();

            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                try
                {
                    // Récupère les données existantes pour préserver la date d'inscription
                    var dbUser = await _context.Utilisateurs.AsNoTracking().FirstOrDefaultAsync(u => u.UtilisateurID == id);
                    if (dbUser != null) utilisateur.DateInscription = dbUser.DateInscription;

                    _context.Update(utilisateur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Utilisateurs.Any(e => e.UtilisateurID == utilisateur.UtilisateurID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleID"] = new SelectList(_context.Roles, "RoleID", "NomRole", utilisateur.RoleID);
            return View(utilisateur);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null) _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}