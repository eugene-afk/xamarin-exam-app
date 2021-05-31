using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using ExamApp.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExamAppTest
{
    public class ModifyTest
    {
        [Fact]
        public async Task CreateAndUpdateAndDeleteExamTest()
        {
            await SQLiteTools.CheckDB();
            ModifyExamViewModel modExamVM = new ModifyExamViewModel(null, null, null);

            //create
            modExamVM.editableText = "Test_" + CommonData.RandomString(6);
            await modExamVM.ModifyExam();
            Assert.NotEqual(0, modExamVM.currentExam.id);

            //update
            modExamVM.editableText += CommonData.RandomString(6);
            await modExamVM.ModifyExam();
            var exam = await SQLiteTools.repo.GetExamByID(modExamVM.currentExam.id);
            Assert.Equal(modExamVM.editableText, exam.name);

            //delete
            var exs = await SQLiteTools.repo.GetExams(10,0);
            Assert.NotEmpty(exs);

            await SQLiteTools.repo.DeleteExam(exam);
            exam = await SQLiteTools.repo.GetExamByID(modExamVM.currentExam.id);
            Assert.True(exam.isDeleted);
        }

        [Fact]
        public async Task CreateAndUpdateAndDeleteQuestionAndVariant()
        {
            await SQLiteTools.CheckDB();
            ModifyExamViewModel modExamVM = new ModifyExamViewModel(null, null, null);

            modExamVM.editableText = "Test_" + CommonData.RandomString(6);
            await modExamVM.ModifyExam();

            var exs = await SQLiteTools.repo.GetExams(10, 0);
            Exam exam = exs[0];

            //create question
            ModifyQuestionViewModel qVM = new ModifyQuestionViewModel(new ObservableCollection<ExamQuestion>(), exam, null, null);
            qVM.editableText = "Test_" + CommonData.RandomString(6);
            await qVM.ModifyQuestion();
            Assert.NotEqual(0, qVM._currentQuestion.id);

            //update question
            qVM.editableText += CommonData.RandomString(6);
            await qVM.ModifyQuestion();
            var qs = await SQLiteTools.repo.GetQuestionsByExamID(exam.id);
            Assert.Equal(qVM.editableText, qs.Where(i => i.id == qVM._currentQuestion.id).First().name);

            //create variant
            string vName = "Test_" + CommonData.RandomString(6);
            await qVM.CreateVariant(vName);
            Assert.NotEqual(0, qVM._currentVariant.id);

            //update variant
            vName += CommonData.RandomString(6);
            qVM._currentVariant.name = vName;
            await SQLiteTools.repo.UpdateQuestionVariant(qVM._currentVariant);
            var vs = await SQLiteTools.repo.GetQuestionVariantsByQuestionID(qVM._currentQuestion.id);
            Assert.Equal(vName, vs.Where(i => i.id == qVM._currentVariant.id).First().name);

            //delete variant
            await SQLiteTools.repo.DeleteQuestion(qVM._currentQuestion, true);
            Assert.True(qVM._currentQuestion.isDeleted);

            await SQLiteTools.repo.DeleteVariant(qVM._currentVariant);
            vs = await SQLiteTools.repo.GetQuestionVariantsByQuestionID(qVM._currentQuestion.id);
            Assert.False(vs.Where(i => i.id == qVM._currentVariant.id).Any());

            //delete question
            await SQLiteTools.repo.DeleteQuestion(qVM._currentQuestion, true);
            Assert.True(qVM._currentQuestion.isDeleted);

            await SQLiteTools.repo.DeleteQuestion(qVM._currentQuestion);
            qs = await SQLiteTools.repo.GetQuestionsByExamID(exam.id);
            Assert.False(qs.Where(i => i.id == qVM._currentQuestion.id).Any());
        }

    }
}
