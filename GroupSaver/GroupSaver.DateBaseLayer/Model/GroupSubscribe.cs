namespace GroupSaver.DateBaseLayer.Model
{
    using SQLite;

    namespace GroupSaver.Model
    {
        public class GroupSubcsribe
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public int PersonId { get; set; }

            public int GroupId { get; set; }
        }
    }
}