using AutoMapper;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;

namespace NewsStacksAPI.Mappers
{
    public class NewsStacksMapper : Profile
    {
        public NewsStacksMapper()
        {
            CreateMap<Article, ArticleWriterDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<Article, ReaderDto>().ReverseMap();
        }
    }
}
