using SQLite;

namespace GroupSaver.DateBaseLayer.Model
{
    public class PostAttachment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int AttachmentId { get; set; }
    }
}