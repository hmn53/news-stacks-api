using NewsStacksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Repository.IRepository
{
    public interface IArticleWriterRepository
    {
        Article GetArticle(int articleId);
        ICollection<Article> GetWriterArticles(int writerId);
        ICollection<Article> GetAllArticles();
        bool Create(Article model);
        bool Edit(Article model);
        bool Delete(Article article);
        bool Assign(Article article, Writer writer);
        public Writer GetWriter(string username);
        bool CheckWriter(Article article, Writer writer);
        bool Save();

        Tag GetTag(string title);
        bool CreateTag(Article article, string title);

    }
}
