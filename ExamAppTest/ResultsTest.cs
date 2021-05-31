using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using ExamApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExamAppTest
{
    public class ResultsTest
    {
        [Fact]
        public async Task TestResults()
        {
            await SQLiteTools.CheckDB();

            //Create exam
            Exam e = new Exam() { name = "Test_" + CommonData.RandomString(6) };
            e.isDraft = true;
            e.ownerID = CommonData.userID;
            e.isDraft = false;
            await SQLiteTools.repo.InsertExam(e);

            Assert.NotEqual(0, e.id);

            //add questions
            List<ExamQuestion> qs = new List<ExamQuestion>();
            for (int i = 0; i < 10; i++)
            {
                ExamQuestion eq = new ExamQuestion() { name = "Test_" + CommonData.RandomString(6) };
                eq.examID = e.id;
                eq.position = qs.Count + 1;
                await SQLiteTools.repo.InsertExamQuestion(eq);
                qs.Add(eq);
                Assert.NotEqual(0, eq.id);
            }

            //add variants
            List<LocalRes> real = new List<LocalRes>();
            Random rnd = new Random();
            for (int i = 0; i < qs.Count; i++)
            {
                var qID = qs[i].id;
                List<ExamQuestionVariant> variants = new List<ExamQuestionVariant>();
                for (int ii = 0; ii < 4; ii++)
                {
                    ExamQuestionVariant eqv = new ExamQuestionVariant();
                    eqv.position = ii + 1;
                    eqv.name = "Test_" + CommonData.RandomString(6);
                    eqv.questionID = qID;
                    await SQLiteTools.repo.InsertExamQuestionVariant(eqv);
                    variants.Add(eqv);
                    Assert.NotEqual(0, eqv.id);
                }
                int r = rnd.Next(0, 4);
                variants[r].isRight = true;
                real.Add(new LocalRes() { qID = qID, rightID = variants[r].id });
                await SQLiteTools.repo.UpdateQuestionVariant(variants[r]);
            }

            ExamViewModel exVM = new ExamViewModel(e.id, e.name);
            await Task.Delay(1000);
            await exVM.GetQuestionVariants(qs[0].id);
            int vCount = exVM.variants.Count - 1;
            Assert.Equal(3, vCount);
            bool isEnd = false;
            while(!isEnd)
            {
                int chosenID = rnd.Next(0, vCount);
                exVM.variants[chosenID].isChecked = true;
                ExamQuestion eqs = qs.Where(i => i.name == exVM.currentQName).First();
                real.Where(x => x.qID == eqs.id).Select(x => { x.choosenID = exVM.variants[chosenID].id; return x; }).ToList();
                await exVM.Next();
                if (!exVM.mainVisible)
                {
                    isEnd = true;
                }
            }

            int rightAnswers = 0;

            foreach (var i in real)
            {
                if (i.choosenID == i.rightID)
                {
                    rightAnswers++;
                }
            }
            int realPercent = (int)Math.Round((double)(100 * rightAnswers) / qs.Count);

            Assert.NotNull(exVM.resultView);
            Assert.Equal(realPercent, exVM.resultView.percent);
        }
    }

    internal class LocalRes
    {
        public int qID { get; set; }
        public int rightID { get; set; }
        public int choosenID { get; set; }
    }
}
