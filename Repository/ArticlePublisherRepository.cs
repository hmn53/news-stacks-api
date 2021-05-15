using Microsoft.EntityFrameworkCore;
using NewsStacksAPI.Data;
using NewsStacksAPI.Models;
using NewsStacksAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Repository
{
    public class ArticlePublisherRepository : IArticlePublisherRepository
    {
        private readonly ApplicationDbContext _db;

        public ArticlePublisherRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateTag(Article article, string title)
        {
            Tag tag = GetTag(title);
            if (tag == null)
            {
                tag = new Tag
                {
                    Title = title
                };
                _db.Tags.Add(tag);
                Save();
            }
            var articleTags = _db.Articles.Include(a => a.Tags)
                .Single(x => x.Id == article.Id);
            if (!articleTags.Tags.Contains(tag))
            {
                articleTags.Tags.Add(tag);
            }
            return Save();
        }

        public ICollection<Article> GetAllArticles()
        {
            return _db.Articles.Where(article => article.IsSubmitted == true).ToList();
        }

        public Article GetArticle(int articleId)
        {
            return _db.Articles.Where(a => a.IsSubmitted == true)
                .Include(x => x.Publishers)
                .Include(x => x.Tags)
                .SingleOrDefault(a => a.Id == articleId);
        }

        public Tag GetTag(string title)
        {
            return _db.Tags.SingleOrDefault(tag => tag.Title.Equals(title));
        }

        public ICollection<Article> GetPublisherArticles(int publisherId)
        {
            return _db.Articles
                .Where(article => article.Publishers
                .Any(p => p.Id == publisherId)).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public bool Update(Article article)
        {
            _db.Update(article);
            return Save();
        }

        public Publisher GetPublisher(string username)
        {
            return _db.Publishers.SingleOrDefault(x => x.Account.Username == username);
        }

        public bool CheckSubmitted(Article article)
        {
            return (bool)article.IsSubmitted; 
        }

        public bool Assign(Article article, Publisher publisher)
        {
            try
            {
                var articlePublishers = _db.Articles
                    .Include(a => a.Publishers)
                    .Single(x => x.Id == article.Id);

                articlePublishers.Publishers.Add(publisher);
                return Save();
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
