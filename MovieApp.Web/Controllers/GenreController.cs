﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Web.Models;
using MovieApp.Web.Repository.IRepository;

namespace MovieApp.Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepo;
        public GenreController(IGenreRepository genreRepo)
        {
            _genreRepo = genreRepo;
        }
        public IActionResult Index()
        {
            return View(new GenreModel() { });
        }

        public async Task<IActionResult> Upsert(Guid? id)
        {
            GenreModel obj = new GenreModel();
            if (id == null)
            {
                // this would be true for insert or create
                return View(obj);
            }
            // flow will come for update
            obj = await _genreRepo.GetAsync(SD.GenreAPIPath, id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(GenreModel obj)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = await _genreRepo.GetAsync(SD.GenreAPIPath, obj.Id);
                if (obj.Id == Guid.Empty)
                {
                    await _genreRepo.CreateAsync(SD.GenreAPIPath, obj);
                }
                else
                {
                    await _genreRepo.UpdateAsync(SD.GenreAPIPath + obj.Id, obj);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllGenre()
        {
            return Json(new { data = await _genreRepo.GetAllAsync(SD.GenreAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var status = await _genreRepo.DeleteAsync(SD.GenreAPIPath, id);
            if (status)
            {
                return Json(new { success = true, message = "Delete Successful!" });
            }
            return Json(new { success = false, message = "Delete not Successful!" });
        }
    }
}
