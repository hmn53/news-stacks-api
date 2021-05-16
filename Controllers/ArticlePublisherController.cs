using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using NewsStacksAPI.Repository.IRepository;
using System;
using System.Collections.Generic;

namespace NewsStacksAPI.Controllers
{
    [Route("api/publisher")]
    [ApiController]
    [Authorize(Roles = "Publisher")]
    public class ArticlePublisherController : ControllerBase
    {
        private readonly IArticlePublisherRepository _apRepo;
        private readonly IMapper _mapper;
        private readonly TimeZone curTimeZone = TimeZone.CurrentTimeZone;

        public ArticlePublisherController(IArticlePublisherRepository aprepo, IMapper mapper)
        {
            _apRepo = aprepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a single submitted article by articleId 
        /// </summary>
        /// <param name="articleId">Id of submitted article</param>
        /// <returns></returns>
        [HttpGet("articles/{articleId:int}", Name = "GetArticleByIdPublisher")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Article))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetArticleByIdPublisher(int articleId)
        {
            var article = _apRepo.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        /// <summary>
        /// Get all articles assigned to current publisher
        /// </summary>
        /// <returns></returns>
        [HttpGet("articles/assigned")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Article>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetAssignedArticlePublisher()
        {
            var id = _apRepo.GetPublisher(User.Identity.Name).Id;
            List<Article> articles = (List<Article>)_apRepo.GetPublisherArticles(id);
            if(articles == null || articles.Count == 0)
            {
                return NoContent();
            }

            return Ok(articles);
        }

        /// <summary>
        /// Get all submitted articles
        /// </summary>
        /// <returns></returns>
        [HttpGet("articles/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Article>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetAllArticlesPublisher()
        {
            List<Article> articles = (List<Article>)_apRepo.GetAllArticles();

            if (articles == null || articles.Count == 0)
            {
                return NoContent();
            }

            return Ok(articles);
        }

        /// <summary>
        /// Add SEO details to a submitted article
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <param name="model">Metadata tags for SEO</param>
        /// <returns></returns>
        [HttpPost("article/seo/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddSeo(int articleId, [FromBody] PublisherSeoDto model)
        {
            Article article = _apRepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(new { message = "Article doesnot exist or is not submitted" });
            }

            if (model == null)
            {
                return BadRequest();
            }
            Publisher publisher = _apRepo.GetPublisher(User.Identity.Name);

            article.MetaData = model.MetaData;
            if (!_apRepo.Update(article))
            {
                ModelState.AddModelError("", "Error while updating Article");
                return StatusCode(500, ModelState);
            }

            if (!_apRepo.Assign(article, publisher))
            {
                ModelState.AddModelError("", "Error while assigning Article to Publisher");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        /// Add Tags to a submitted article
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <param name="tags">Tags to add to the article</param>
        /// <returns></returns>
        [HttpPost("article/tags/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddTags(int articleId, [FromBody] PublisherTagDto tags)
        {
            Article article = _apRepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(new { message = "Article doesnot exist or is not submitted" });
            }

            if (tags == null)
            {
                return BadRequest();
            }

            if (tags.Tags == null || tags.Tags.Count == 0)
            {
                return BadRequest(new { message = "Enter atleast one tag" });
            }

            foreach (var tag in tags.Tags)
            {
                _apRepo.CreateTag(article, tag.Title);
            }

            return NoContent();
        }

        /// <summary>
        /// Publish a submitted article
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <param name="model">DateTime in ISO to publish the article (Optional)</param>
        /// <returns></returns>
        [HttpPost("article/publish/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PublishArticle(int articleId, [FromBody] PublisherDateTimeDto model)
        {
            Article article = _apRepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(new { message = "Article doesnot exist or is not submitted" });
            }

            if (article.IsPublished == true)
            {
                return BadRequest(new { message = "Article already published" });
            }

            Publisher publisher = _apRepo.GetPublisher(User.Identity.Name);

            DateTime? publishTime = model.publishTime;
            if (publishTime == null)
            {
                publishTime = DateTime.Now;
            }
            publishTime = curTimeZone.ToLocalTime((DateTime)publishTime);

            article.PublishedAt = publishTime;
            article.PublishedBy = publisher.Id;
            article.IsPublished = true;

            if (!_apRepo.Update(article))
            {
                ModelState.AddModelError("", "Error while updating Article");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a submitted and/or published article
        /// </summary>
        /// <param name="articleId">Id of the article</param>
        /// <returns></returns>
        [HttpDelete("article/{articleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteArticle(int articleId)
        {
            Article article = _apRepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(new { message = "Article doesnot exist or is not submitted" });
            }
            if (!_apRepo.Delete(article))
            {
                ModelState.AddModelError("", "Error while deleting Article");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
