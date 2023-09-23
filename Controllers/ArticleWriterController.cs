using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsStacksAPI.Data;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using NewsStacksAPI.Repository.IRepository;
using System;
using System.Collections.Generic;

namespace NewsStacksAPI.Controllers
{
    [Route("api/writer")]
    [ApiController]
    [Authorize(Roles = "Writer")]
    public class ArticleWriterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IArticleWriterRepository _awrepo;
        private readonly IMapper _mapper;
        private readonly TimeZone curTimeZone = TimeZone.CurrentTimeZone;

        public ArticleWriterController(IArticleWriterRepository awrepo, IMapper mapper, ApplicationDbContext context)
        {
            _context = context;
            _awrepo = awrepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a single article by Id
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <returns></returns>
        [HttpGet("articles/{articleId:int}"
            , Name = "GetArticleByIdWriter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Article))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetArticleByIdWriter([FromRoute(Name = "articleId")] int articleId)
        {
            var article = _awrepo.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        /// <summary>
        /// Get articles assigned to current writer
        /// </summary>
        /// <returns></returns>
        [HttpGet("articles/assigned")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Article>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetAssignedArticleWriter()
        {
            var id = _awrepo.GetWriter(User.Identity.Name).Id;
            List<Article> articles = (List<Article>)_awrepo.GetWriterArticles(id);
            if (articles == null || articles.Count == 0)
            {
                return NoContent();
            }

            return Ok(articles);
        }

        /// <summary>
        /// Get all articles
        /// </summary>
        /// <returns></returns>
        [HttpGet("articles/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Article>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetAllArticlesWriter()
        {
            List<Article> articles = (List<Article>)_awrepo.GetAllArticles();
            if (articles == null || articles.Count == 0)
            {
                return NoContent();
            }

            return Ok(articles);
        }

        /// <summary>
        /// Create an article
        /// </summary>
        /// <param name="model">Article properties</param>
        /// <returns></returns>
        [HttpPost("article")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateArticle([FromForm] ArticleWriterDto model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Article cannot be null" });
            }
            var writer = _awrepo.GetWriter(User.Identity.Name);
            Article article = _mapper.Map<Article>(model);
            article.CreatedBy = writer.Id;
            article.LastModifiedBy = writer.Id;
            article.CreatedAt = curTimeZone.ToLocalTime(DateTime.Now);
            article.LastModified = curTimeZone.ToLocalTime(DateTime.Now);
            article.Tags = null;

            if (!_awrepo.Create(article))
            {
                ModelState.AddModelError("", "Error while creating Article");
                return StatusCode(500, ModelState);
            }


            if (!_awrepo.Assign(article, writer))
            {
                ModelState.AddModelError("", "Error while assigning article to writer");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetArticleByIdWriter", new { articleId = article.Id }, article);
        }

        /// <summary>
        /// Update an article 
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <param name="model">Article properties</param>
        /// <returns></returns>
        [HttpPut("article/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateArticle([FromRoute(Name = "articleId")] int articleId, [FromForm] ArticleWriterUpdateDto model)
        {
            var article = _awrepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(ModelState);
            }

            if (article.IsSubmitted == true)
            {
                return BadRequest(new { message = "Cannot update article after submitting" });
            }

            if (model == null)
            {
                return BadRequest(new { message = "Article cannot be null" });
            }

            var writer = _awrepo.GetWriter(User.Identity.Name);
            if (!_awrepo.CheckWriter(article, writer))
            {
                return BadRequest(new { message = "Not Authorised" });
            }

            _context.Database.BeginTransaction();

            try
            {
                article.Headline = model.Headline ?? article.Headline;
                article.Description = model.Description ?? article.Description;
                article.Body = model.Body ?? article.Body;

                article.LastModifiedBy = writer.Id;
                article.LastModified = curTimeZone.ToLocalTime(DateTime.Now);

                _awrepo.Edit(article);

                _awrepo.Assign(article, writer);

                _context.Database.CommitTransaction();
            }
            catch
            {
                _context.Database.RollbackTransaction();
                ModelState.AddModelError("", "Error while editing Article");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an article
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <returns></returns>
        [HttpDelete("article/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteArticle([FromRoute(Name = "articleId")] int articleId)
        {
            var article = _awrepo.GetArticle(articleId);
            if (article == null)
            {
                ModelState.AddModelError("", "Article with given id doesn't exists");
                return StatusCode(404, ModelState);
            }

            var writer = _awrepo.GetWriter(User.Identity.Name);
            if (!_awrepo.CheckWriter(article, writer))
            {
                return BadRequest(new { message = "Not Authorised" });
            }

            if (article.IsSubmitted == true)
            {
                return BadRequest(new { message = "Cannot delete article after submitting" });
            }

            if (!_awrepo.Delete(article))
            {
                ModelState.AddModelError("", "Error while deleting Article");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Submit an article for publishing
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpPost("article/submit/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SubmitArticle([FromRoute(Name = "articleId")] int articleId)
        {
            var article = _awrepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(new { message = "Article with given id doesn't exists" });
            }

            var writer = _awrepo.GetWriter(User.Identity.Name);
            if (!_awrepo.CheckWriter(article, writer))
            {
                return BadRequest(new { message = "Not Authorised" });
            }

            if (article.IsSubmitted == true)
            {
                return BadRequest(new { message = "Article already submitted" });
            }

            article.IsSubmitted = true;
            article.SubmittedAt = curTimeZone.ToLocalTime(DateTime.Now);
            article.SubmittedBy = writer.Id;
            if (!_awrepo.Edit(article))
            {
                ModelState.AddModelError("", "Error while editing Article");
                return StatusCode(500, ModelState);
            }
            return Ok();

        }

    }
}
