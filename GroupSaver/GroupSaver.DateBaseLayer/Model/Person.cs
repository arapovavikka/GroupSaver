using SQLite;

namespace GroupSaver.DateBaseLayer.Model
{
    public class Person
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public int VkId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}