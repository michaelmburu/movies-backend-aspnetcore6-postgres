﻿using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_API.DTO.Actor;
using Movies_API.DTO.PaginationDTO;
using Movies_API.Helpers;
using Movies_API.Models.Actor;
using Movies_API.MovieContext;

namespace Movies_API.Controllers
{
    [Route("api/actors")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly MovieDBContext _movieDBContext;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "actors";
        
        public ActorsController(MovieDBContext movieDBContext, IMapper mapper, IFileStorageService fileStorageService)
        {
            _movieDBContext = movieDBContext;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            //Pagination
            var queryable = _movieDBContext.Actors.AsQueryable();
            await HttpContext.InsertParametersPaginationFromHeader(queryable);

            var actors = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<ActorDTO>>(actors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var actor = await _movieDBContext.Actors.FirstOrDefaultAsync(a => a.Id == id);
            if(actor == null)
            {
                return NotFound(id);
            }
            return _mapper.Map<ActorDTO>(actor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreationDTO);
            if(actorCreationDTO.Picture != null)
            {
                actor.Picture = await _fileStorageService.SaveFile(containerName, actorCreationDTO.Picture);
            }

            _movieDBContext.Add(actor);
            await _movieDBContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromForm] ActorCreationDTO actorCreationDTO)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await _movieDBContext.Actors.FirstOrDefaultAsync(a => a.Id == id);
            if(actor == null)
            {
                return NotFound();
            }
            _movieDBContext.Actors.Remove(actor);
            await _movieDBContext.SaveChangesAsync();
            return NoContent();
        }
            
    }
}
