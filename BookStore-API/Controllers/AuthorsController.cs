using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted Get All authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully returned all authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message}: {e.InnerException}");
            }
        }
        
        /// <summary>
        /// Get author by ID
        /// </summary>
        /// <param name="id">Author's database ID</param>
        /// <returns>An author's record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted Get author with id:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Author with id: {id} was not found");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully returned author with id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message}: {e.InnerException}");
            }
        }
        
        /// <summary>
        /// Creates an author record
        /// </summary>
        /// <param name="authorDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDto)
        {
            try
            {
                _logger.LogInfo("Author submission attempted");
                if (authorDto == null)
                {
                    _logger.LogWarn("Empty request was submitted");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Author data was incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDto);
                var success = await _authorRepository.Create(author);
                if (!success)
                {
                    return InternalError("Failed to create author record");
                }
                _logger.LogInfo("Author created");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message}: {e.InnerException}");
            }
        }

        /// <summary>
        /// Update author's record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdateDto)
        {
            try
            {
                _logger.LogInfo($"Attempt Author update with id: {id}");
                if (id < 1 || authorUpdateDto == null || id != authorUpdateDto.Id)
                {
                    _logger.LogWarn("Bad update request submitted");
                    return BadRequest();
                }

                var exists = await _authorRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"Author id: {id} does not exist");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorUpdateDto);
                var success = await _authorRepository.Update(author);
                if (!success)
                {
                    return InternalError($"Update operation failed");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message}: {e.InnerException}");
            }
        }
        
        /// <summary>
        /// Delete author's record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo("Attempt Author delete");
                if (id < 1)
                {
                    _logger.LogWarn("Bad delete request submitted");
                    return BadRequest();
                }

                var exists = await _authorRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"Author with id: {id} not found");
                    return NotFound();
                }

                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    return NotFound();
                }
                else
                {
                    var success = await _authorRepository.Delete(author);
                    if (!success)
                    {
                        return InternalError($"Author delete operation failed");
                    }
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message}: {e.InnerException}");
            }
        }
        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Shit got fucked up. Call the Whitehouse");
        }
    }
}