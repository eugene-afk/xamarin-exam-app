using SQLite;

namespace ExamApp.Data.Model
{
    [Table("USERS")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id { get; set; }
        public string name { get; set; }
        public string uid { get; set; }
        public int parentID { get; set; }
        public UserType type { get; set; }

    }
}
