using SQLite;

namespace GroupSaver.DateBaseLayer.Model
{ 
    public class Attachment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int PostId { get; set; }

        public int VkId { get; set; }

        [Unique]
        public string Url { get; set; }

        public string Type { get; set; }

        public byte[] File { get; set; }
    }
}