﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.API.Models;
using MovieApp.API.Models.DTOs;
using MovieApp.API.Repository.IRepository;

namespace MovieApp.API.Controllers
{
    [Route("api/v{version:apiVersion}/Genres")]
    //[Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class GenresController : Controller
    {
        private readonly IGenreRepository _genreRepo;
        private readonly IMapper _mapper;

        public GenresController(IGenreRepository genreRepo, 
            IMapper mapper)
        {
            _genreRepo = genreRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a list of all the Genres in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<GenreDTO>))]
        public IActionResult GetAllGenre()
        {
            var objList = _genreRepo.GetGenre();

            var objDto = new List<GenreDTO>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<GenreDTO>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Gets individual genre from database
        /// </summary>
        /// <param name="genreId">The id of the genre</param>
        /// <returns></returns>
        [HttpGet("{genreId:Guid}", Name ="GetGenreById")]
        [ProducesResponseType(200, Type = typeof(List<GenreDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetGenreById(Guid genreId)
        {
            var obj = _genreRepo.GetGenre(genreId);

            if (obj is null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<GenreDTO>(obj);

            return Ok(objDto);
        }

        /// <summary>
        /// Create a new genre
        /// </summary>
        /// <param name="genreDto">Genre Data transfer object</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(List<GenreDTO>))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateGenre([FromBody] GenreDTO genreDto)
        {
            if (genreDto is null)
            {
                return BadRequest(ModelState);
            }

            if (_genreRepo.GenreExist(genreDto.Name))
            {
                ModelState.AddModelError("", "Genre already exist!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var genreObj = _mapper.Map<GenreModel>(genreDto);

            if (!_genreRepo.CreateGenre(genreObj))
            {
                ModelState.AddModelError("", $"Something wrong occured when trying to save record {genreObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetGenreById", new { genreId = genreObj.Id }, genreObj);
        }
        /// <summary>
        /// Updates existing genre in the database by passing genre Id
        /// </summary>
        /// <param name="genreId">Genre id</param>
        /// <param name="genreDto">Genre DTO</param>
        /// <returns></returns>
        [HttpPatch("{genreId:Guid}", Name = "UpdateGenre")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateGenre(Guid genreId, [FromBody]GenreDTO genreDto)
        {
            if (genreDto == null || genreId != genreDto.Id)
            {
                return BadRequest(ModelState);
            }

            var genreObj = _mapper.Map<GenreModel>(genreDto);

            if (!_genreRepo.UpdateGenre(genreObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {genreObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete genre from database by passing genre id
        /// </summary>
        /// <param name="genreId">Genre id</param>
        /// <returns></returns>
        [HttpDelete("{genreId:Guid}", Name = "DeleteGenre")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteGenre(Guid genreId)
        {
            if (!_genreRepo.GenreExist(genreId))
            {
                return NotFound();
            }

            var genreObj = _genreRepo.GetGenre(genreId);

            if (!_genreRepo.DeleteGenre(genreObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {genreObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
