using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using NewsStacksAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Controllers
{
    [Route("api/writer")]
    [ApiController]
    [Authorize(Roles ="Writer")]
    public class ArticleWriterController : ControllerBase
    {
        private readonly IArticleWriterRepository _awrepo;
        private readonly IMapper _mapper;
        private readonly TimeZone curTimeZone = TimeZone.CurrentTimeZone;


        public ArticleWriterController(IArticleWriterRepository awrepo, IMapper mapper)
        {
            _awrepo = awrepo;
            _mapper = mapper;
        }

        [HttpGet("articles/{articleId:int}", Name ="GetArticleByIdWriter")]
        public IActionResult GetArticleByIdWriter(int articleId)
        {
            var article = _awrepo.GetArticle(articleId);
            if(article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpGet("articles/assigned")]
        public IActionResult GetAssignedArticleWriter()
        {
            var id = _awrepo.GetWriter(User.Identity.Name).Id;
            List<Article> articles = (List<Article>)_awrepo.GetWriterArticles(id);

            return Ok(articles);
        }

        [HttpGet("articles/all")]
        public IActionResult GetAllArticlesWriter()
        {
            List<Article> articles = (List<Article>)_awrepo.GetAllArticles();

            return Ok(articles);
        }

        [HttpPost("article")]
        public IActionResult CreateArticle(ArticleWriterDto model)
        {
            if(model == null)
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

        [HttpPut("article/{articleId:int}")]
        public IActionResult UpdateArticle(int articleId, [FromBody] ArticleWriterDto model)
        {
            var article = _awrepo.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest(ModelState);
            }

            if (article.IsSubmitted == true)
            {
                return BadRequest(new { message= "Cannot update article after submitting" });
            }

            if(model == null)
            {
                return BadRequest(new { message = "Article cannot be null" });
            }
            var writer = _awrepo.GetWriter(User.Identity.Name);
            article.Headline = model.Headline;
            article.Description = model.Description;
            article.Body = model.Body;

            
            article.LastModifiedBy = writer.Id;
            article.LastModified = curTimeZone.ToLocalTime(DateTime.Now);

            if (!_awrepo.Edit(article))
            {
                ModelState.AddModelError("", "Error while editing Article");
                return StatusCode(500, ModelState);
            }

            //foreach (var tag in model.Tags)
            //{
            //    _awrepo.CreateTag(article, tag.Title);
            //}

            if (!_awrepo.Assign(article, writer))
            {
                ModelState.AddModelError("", "Error while assigning article to writer");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("article/{articleId:int}")]
        public IActionResult DeleteArticle(int articleId)
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

        [HttpPost("article/submit/{articleId:int}")]
        public IActionResult SubmitArticle(int articleId)
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

            if(article.IsSubmitted == true)
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
