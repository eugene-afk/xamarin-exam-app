using ExamApp.Common;
using ExamApp.ViewModel;
using SQLite;

namespace ExamApp.Data.Model
{
    [Table("EXAM_QUESTION_VARIANTS")]
    public class ExamQuestionVariant : Base, IOrderableItem
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id { get; set; }
        public int questionID { get; set; }
        public string name { get; set; }
        [Ignore]
        private bool _isRight { get; set; }
        public bool isRight { get { return _isRight; } set { _isRight = value; OnPropertyChanged("icon"); } }
        public int position { get; set; }
        public VariantType type { get; set; } = VariantType.Text;
        [Ignore]
        public string icon 
        { 
            get
            {
                if (isRight)
                {
                    return "star.png";
                }
                return "empty_star.png";
            }
        }
        [Ignore]
        private bool _isChecked{ get; set; }
        [Ignore]
        public bool isChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; OnPropertyChanged(); }
        }
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
