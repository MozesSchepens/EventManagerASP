using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventManagerADV.Data;
using EventManagerADV.ViewModels;
using EventManagerADV.Models;
using Microsoft.AspNetCore.Identity;

namespace EventManagerADV.Controllers
{
    public class UserViewModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserViewModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserViewModels
        public async Task<IActionResult> Index()
        {
            List<UserViewModel> users = await (from Users user in _context.Users
                                               select new UserViewModel(user, _context))
                                       .ToListAsync();
            
            return View(users);
        }


        // GET: UserViewModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel(await _context.Users.FindAsync(id), _context);
            ViewData["Rollen"] =  new MultiSelectList(_context.Roles.ToList(), "Id", "Id", userViewModel.Roles);
            if (userViewModel == null)
            {
                return NotFound();
            }
            return View(userViewModel);
        }

        // POST: UserViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserName,UserEmail,Name,Roles,IsBlocked")] UserViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // wijzig usergegevens
                    Users user = await _context.Users.FindAsync(model.UserId);
                    user.Email = model.UserEmail;
                    user.LockoutEnd = model.IsBlocked ? DateTime.MaxValue : DateTime.Now;
                    _context.Update(user);

                    // haal bestaande rollen weg
                    foreach (IdentityRole role in _context.Roles)
                    {
                        IdentityUserRole<string> userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == model.UserId && ur.RoleId == role.Id);
                        if (userRole != null) 
                            _context.UserRoles.Remove(userRole);
                    }    

                    // voeg nieuwe rollen toe
                    foreach (string roleId in model.Roles)
                        _context.UserRoles.Add(new IdentityUserRole<string> { RoleId = roleId, UserId = model.UserId });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

    }
}
