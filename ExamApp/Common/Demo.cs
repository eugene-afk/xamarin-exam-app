using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamApp.Tests
{
    public class Demo
    {
        private Exam _exam { get; set; }
        private List<ExamQuestion> _questions { get; set; }
        private Result _result { get; set; }
        private List<ResultBody> _results { get; set; }
        private List<LocalRes> _real { get; set; }

        private int cycle = 2;
        private int exams = 10;

        public Demo()
        {
            _questions = new List<ExamQuestion>();
            _real = new List<LocalRes>();
            _result = new Result();
            _results = new List<ResultBody>();
        }

        public async Task FillDemoData()
        {
            for (int i = 0; i < exams; i++){
                await CreateExam();
                await CreateQuestionsForExam();
                await CreateVariantsForQuestion();

                int rightCoef = 2;
                if (i % 5 == 0)
                {
                    rightCoef = 1;
                }
                if(i % 3 == 0)
                {
                    rightCoef = 3;
                }

                for (int ii = 0; ii < cycle; ii++)
                {
                    await ImitatePlay(rightCoef);
                    await FinishTest();
                    _exam.playedTimes += 1;
                }
                await SQLiteTools.repo.UpdateExam(_exam);
            }
        }

        private async Task CreateExam()
        {
            Exam e = new Exam() { name = "UnitTest_" + Common.CommonData.RandomString(6) };
            e.isDraft = true;
            e.ownerID = Common.CommonData.userID;
            e.isDraft = false;
            await SQLiteTools.repo.InsertExam(e);
            _exam = e;
        }

        private async Task CreateQuestionsForExam()
        {
            for(int i = 0; i < 10; i++)
            {
                ExamQuestion eq = new ExamQuestion() { name = "UnitTest_" + Common.CommonData.RandomString(6) };
                eq.examID = _exam.id;
                eq.position = _questions.Count + 1;
                await SQLiteTools.repo.InsertExamQuestion(eq);
                _questions.Add(eq);
            }
        }

        private async Task CreateVariantsForQuestion()
        {
            Random rnd = new Random();
            for(int i = 0; i < _questions.Count; i++)
            {
                var qID = _questions[i].id;
                List<ExamQuestionVariant> variants = new List<ExamQuestionVariant>();
                for (int ii = 0; ii < 4; ii++)
                {
                    ExamQuestionVariant eqv = new ExamQuestionVariant();
                    eqv.position = ii + 1;
                    eqv.name = "UnitTest_" + Common.CommonData.RandomString(6);
                    eqv.questionID = qID;
                    await SQLiteTools.repo.InsertExamQuestionVariant(eqv);
                    variants.Add(eqv);
                }
                int r = rnd.Next(0, 4);
                variants[r].isRight = true;
                _real.Add(new LocalRes() { qID = qID, rightID = variants[r].id });
                await SQLiteTools.repo.UpdateQuestionVariant(variants[r]);
            }
        }

        private async Task<List<ExamQuestionVariant>> GetVariants(int qID)
        {
            return await SQLiteTools.repo.GetQuestionVariantsByQuestionID(qID);
        }

        private async Task ImitatePlay(int coef)
        {
            _results.Clear();

            _result.examID = _exam.id;
            _result.userID = Common.CommonData.userID;

            Random rnd = new Random();
            for(int i = 0; i < _questions.Count; i++)
            {
                ResultBody rb = new ResultBody();
                List<ExamQuestionVariant> variants = await GetVariants(_questions[i].id);
                rb.chosenVariantID = variants[rnd.Next(0, 4)].id;
                if (i % coef == 0)
                {
                    int rightID = _real.Where(x => x.qID == _questions[i].id).First().rightID;
                    if(rb.chosenVariantID != rightID)
                    {
                        rb.chosenVariantID = rightID;
                    }
                }
                rb.questionID = _questions[i].id;
                _results.Add(rb);
                _real.Where(x => x.qID == _questions[i].id).Select(x => { x.choosenID = rb.chosenVariantID; return x; }).ToList();
            }
        }

        private async Task FinishTest()
        {
            int rightAnswers = 0;

            foreach(var i in _real)
            {
                if(i.choosenID == i.rightID)
                {
                    rightAnswers++;
                }
            }
            int realPercent = (int)Math.Round((double)(100 * rightAnswers) / _questions.Count);
            rightAnswers = 0;
            foreach (var i in _results)
            {
                var r = await SQLiteTools.repo.GetQuestionRightVariantByQuestionID(i.questionID);
                if (r == i.chosenVariantID)
                {
                    i.isRight = true;
                    rightAnswers++;
                }
            }
            _result.percent = (int)Math.Round((double)(100 * rightAnswers) / _questions.Count);

            _result.dateEND = DateTime.Now;
            await SQLiteTools.repo.InsertResult(_result);
            _results.Select(i => { i.headID = _result.id; return i; }).ToList();
            await SQLiteTools.repo.InsertResultBodiesRange(_results);
        }

    }

    internal class LocalRes
    {
        public int qID { get; set; }
        public int rightID { get; set; }
        public int choosenID { get; set; }
    }
}
