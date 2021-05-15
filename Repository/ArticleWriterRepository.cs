﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsStacksAPI.Data;
using NewsStacksAPI.Models;
using NewsStacksAPI.Repository.IRepository;
using NewsStacksAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Repository
{
    public class ArticleWriterRepository : IArticleWriterRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;

        public ArticleWriterRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }
        public bool Assign(Article article, Writer writer)
        {
            try
            {
                var articleWriters = _db.Articles.Include(a => a.Writers).Single(x => x.Id == article.Id);
                articleWriters.Writers.Add(writer);
                return Save();
            }
            catch(Exception e)
            {
                return false;
            }

        }

        public bool CheckWriter(Article article, Writer writer)
        {
            var articleWriters = _db.Articles.Include(a => a.Writers).Single(x => x.Id == article.Id);

            if(articleWriters.Writers == null)
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
                .SingleOrDefault(article => article.Id == articleId);
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