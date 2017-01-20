using System;
using Android.Runtime;
using Java.IO;
using SQLite;

namespace GroupSaver.DateBaseLayer.Model
{
    public class Group
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        [Unique]
        public int VkId { get; set; }

        [Unique]
        public string ShortUrl { get; set; }
    }
}