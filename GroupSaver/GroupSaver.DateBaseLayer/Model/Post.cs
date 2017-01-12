using System;
using SQLite;

namespace GroupSaver.DateBaseLayer.Model
{
    public class Post
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public int VkId { get; set; }

        public int GroupId { get; set; }

        public string Text { get; set; }

        public DateTime TimeDate { get; set; }
    }
}