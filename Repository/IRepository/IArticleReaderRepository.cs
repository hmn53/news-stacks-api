using NewsStacksAPI.Models;
using System.Collections.Generic;

namespace NewsStacksAPI.Repository.IRepository
{
    public interface IArticleReaderRepository
    {
        public ICollection<Article> GetArticles();
        public ICollection<Article> GetArticlesByTags(string tag);
        public Article GetArticle(int articleId);
        public bool CheckArticle(Article article);
    }
}
