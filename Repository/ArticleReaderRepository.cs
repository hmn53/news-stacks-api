using Microsoft.EntityFrameworkCore;
using NewsStacksAPI.Data;
using NewsStacksAPI.Models;
using NewsStacksAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace NewsStacksAPI.Repository
{
    public class ArticleReaderRepository : IArticleReaderRepository
    {
        private readonly ApplicationDbContext _db;

        public ArticleReaderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CheckArticle(Article article)
        {
            var isPublished = article.IsPublished;
            return isPublished == null ? false : (bool)isPublished;
        }

        public Article GetArticle(int articleId)
        {
            var article = _db.Articles.Include(x => x.Tags).SingleOrDefault(x => x.Id == articleId);
            if (article != null && CheckArticle(article))
            {
                return article;
            }
            return null;
        }

        public ICollection<Article> GetArticles()
        {
            return _db.Articles.Where(x => x.IsPublished == true)
                .Include(x => x.Tags)
                .ToList();

        }

        public ICollection<Article> GetArticlesByTags(string tag)
        {
            return _db.Articles.Where(x => x.IsPublished == true)
                .Include(x => x.Tags)
                .Where(x => x.Tags
                .Any(t => t.Title.ToLower()
                .Contains(tag.ToLower()))).ToList();
        }
    }
}
