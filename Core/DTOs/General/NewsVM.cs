using DataLayer.Entities.Blogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class NewsVM
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalNewsCount { get; set; }
        public int NewsPerPage { get; set; }
        public List<News> AllNews { get; set; }
        public List<News> PageNews { get; set; }
        public List<NewsGroup> NewsGroups { get; set; }
        public List<News> LastNews { get; set; }
        public List<string> Tags { get; set; }
       
        public int? GId { get; set; }
        public NewsGroup NewsGroup { get; set; }
    }
}
