using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("articles/{articleId:int}", Name = "GetArticleByIdPublisher")]
        public IActionResult GetArticleByIdPublisher(int articleId)
        {
            var article = _apRepo.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpGet("articles/assigned")]
        public IActionResult GetAssignedArticlePublisher()
        {
            var id = _apRepo.GetPublisher(User.Identity.Name).Id;
            List<Article> articles = (List<Article>)_apRepo.GetPublisherArticles(id);

            return Ok(articles);
        }

        [HttpGet("articles/all")]
        public IActionResult GetAllArticlesPublisher()
        {
            List<Article> articles = (List<Article>)_apRepo.GetAllArticles();

            return Ok(articles);
        }

        [HttpPost("article/seo/{articleId:int}")]
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

        [HttpPost("article/tags/{articleId:int}")]
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

        [HttpPost("article/publish/{articleId:int}")]
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
    }
}
