using ExamApp.Common;
using ExamApp.ViewModel;
using SQLite;

namespace ExamApp.Data.Model
{
    [Table("EXAM_QUESTIONS")]
    public class ExamQuestion : Base, IOrderableItem
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id { get; set; }
        public int examID { get; set; }
        public int position { get; set; }
        [Ignore]
        private string _name { get; set; }
        public string name 
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("nameView"); }
        }
        public int rightVariantID { get; set; }
        public QuestionType type { get; set; } = QuestionType.Standart;

        [Ignore]
        private string _nameView { get; set; }
        [Ignore]
        public string nameView
        {
            get
            {
                _nameView = name;
                if (name.Length > 10)
                {
                    _nameView = name.Substring(0, 10) + "...";
                }
                return _nameView;
            }
            set
            {
                _nameView = value;
                OnPropertyChanged();
            }
        }

        public bool isDeleted { get; set; } = false;

    }
}
