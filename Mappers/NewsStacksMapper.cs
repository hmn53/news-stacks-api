using AutoMapper;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Mappers
{
    public class NewsStacksMapper: Profile
    {
        public NewsStacksMapper()
        {
            CreateMap<Article, ArticleWriterDto>().ReverseMap();
        }
    }
}
