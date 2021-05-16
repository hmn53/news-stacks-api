using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using NewsStacksAPI.Repository.IRepository;
using System.Collections.Generic;

namespace NewsStacksAPI.Controllers
{
    [Route("api/reader/articles")]
    [ApiController]
    [Authorize]
    public class ArticleReaderController : ControllerBase
    {
        private readonly IArticleReaderRepository _arrepo;
        private readonly IMapper _mapper;

        public ArticleReaderController(IArticleReaderRepository arrepo, IMapper mapper)
        {
            _arrepo = arrepo;
            _mapper = mapper;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public IActionResult GetAllArticles()
        {
            List<Article> articles = (List<Article>)_arrepo.GetArticles();
            if (articles == null || articles.Count == 0)
            {
                return NoContent();
            }
            List<ReaderDto> readerArticles = new List<ReaderDto>();
            foreach (var article in articles)
            {
                var readerArticle = _mapper.Map<ReaderDto>(article);
                readerArticles.Add(readerArticle);
            }
            if (readerArticles.Count == 0)
            {
                return NoContent();
            }
            return Ok(readerArticles);
        }

        [HttpGet("{articleId:int}")]
        public IActionResult GetArticle(int articleId)
        {
            var article = _arrepo.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            ReaderDto readerArticle = _mapper.Map<ReaderDto>(article);
            return Ok(readerArticle);
        }

        [HttpGet("{tag}")]
        public IActionResult GetArticle(string tag)
        {
            var articles = _arrepo.GetArticlesByTags(tag);
            if (articles == null || articles.Count == 0)
            {
                return NoContent();
            }
            List<ReaderDto> readerArticles = new List<ReaderDto>();
            foreach (var article in articles)
            {
                var readerArticle = _mapper.Map<ReaderDto>(article);
                readerArticles.Add(readerArticle);
            }
            if (readerArticles.Count == 0)
            {
                return NoContent();
            }
            return Ok(readerArticles);
        }
    }
}
