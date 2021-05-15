using NewsStacksAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Repository.IRepository
{
    public interface IArticlePublisherRepository
    {
        Article GetArticle(int articleId);
        Publisher GetPublisher(string username);
        ICollection<Article> GetPublisherArticles(int publisherId);
        ICollection<Article> GetAllArticles();
        bool Update(Article article);
        Tag GetTag(string title);
        bool CreateTag(Article article, string title);
        bool Save();
        bool CheckSubmitted(Article article);
        bool Assign(Article article, Publisher publisher);

    }
}
