using Microsoft.EntityFrameworkCore;
using NewsStacksAPI.Data;
using NewsStacksAPI.Models;
using NewsStacksAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsStacksAPI.Repository
{
    public class ArticleWriterRepository : IArticleWriterRepository
    {
        private readonly ApplicationDbContext _db;

        public ArticleWriterRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Assign(Article article, Writer writer)
        {
            try
            {
                var articleWriters = _db.Articles.Include(a => a.Writers).Single(x => x.Id == article.Id);
                articleWriters.Writers.Add(writer);
                return Save();
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool CheckWriter(Article article, Writer writer)
        {
            var articleWriters = _db.Articles.Include(a => a.Writers).Single(x => x.Id == article.Id);

            if (articleWriters.Writers == null)
            {
                return false;
            }
            if (articleWriters.Writers.Contains(writer))
            {
                return true;
            }
            return false;
        }

        public bool Create(Article model)
        {
            var article = _db.Articles.Add(model);
            return Save();
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

        public bool Delete(Article article)
        {
            _db.Articles.Remove(article);
            return Save();
        }


        public bool Edit(Article model)
        {
            _db.Articles.Update(model);
            return Save();
        }

        public ICollection<Article> GetAllArticles()
        {
            return _db.Articles.ToList();
        }

        public Article GetArticle(int articleId)
        {
            return _db.Articles
                .Include(x => x.Tags)
                .Include(x => x.Writers)
                .SingleOrDefault(article => article.Id == articleId);
        }

        public Tag GetTag(string title)
        {
            return _db.Tags.SingleOrDefault(tag => tag.Title.Equals(title));
        }

        public Writer GetWriter(string username)
        {
            return _db.Writers
                .SingleOrDefault(
                    writer => writer.Account.Username == username
                );
        }

        public ICollection<Article> GetWriterArticles(int writerId)
        {
            return _db.Articles
                .Where(article => article.Writers
                .Any(writer => writer.Id == writerId)).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }


    }
}
