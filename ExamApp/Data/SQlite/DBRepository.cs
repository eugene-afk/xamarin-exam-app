using ExamApp.Common;
using ExamApp.Data.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ExamApp.Data.SQlite
{
    public class DBRepository
    {
        private SQLiteAsyncConnection _database;

        public DBRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        #region Basic

        public async Task<Tuple<bool, int>> IsSigned()
        {
            try
            {
                Config cfg = await _database.GetAsync<Config>("select * from CONFIG");
                return new Tuple<bool, int>(cfg.signed, cfg.userID);
            }
            catch (Exception ex)
            {
                Debug.Print("*IsSigned* msg: " + ex.ToString());
                return new Tuple<bool, int>(false, 0);
            }
        }

        public async Task<int> InsertUser(User user)
        {
            try
            {
                return await _database.InsertAsync(user);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertUser* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task UpdateConfig(Config cfg)
        {
            try
            {
                await _database.UpdateAsync(cfg);
            }
            catch (Exception ex)
            {
                Debug.Print("*UpdateConfig* msg: " + ex.ToString());
            }
        }

        public async Task InsertConfig(Config cfg)
        {
            try
            {
                await _database.InsertAsync(cfg);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertConfig* msg: " + ex.ToString());
            }
        }

        public async Task<string> GetUserNameByID(int id)
        {
            try
            {
                return await _database.ExecuteScalarAsync<string>("select name from USERS where id = " + id);
            }
            catch (Exception ex)
            {
                Debug.Print("*GetUserNameByID* msg: " + ex.ToString());
                return "Unknown";
            }
        }
        #endregion

        #region Exams
        public async Task<List<Exam>> GetExams(int limit, int offset)
        {
            try
            {
                return await _database.QueryAsync<Exam>("select * from EXAMS where isDeleted <> 1 order by id desc limit " + limit + " offset " + offset);
            }
            catch (Exception ex)
            {
                Debug.Print("*GetExams* msg: " + ex.ToString());
                return new List<Exam>();
            }
        }

        public async Task<int> DeleteExam(Exam exam)
        {
            try
            {
                await _database.ExecuteAsync("update EXAMS set isDeleted = 1 where id = " + exam.id);
                
                return 1;
            }
            catch (Exception ex)
            {
                Debug.Print("*DeleteExamByID* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task UpdateExam(Exam exam)
        {
            try
            {
                if (await _database.ExecuteScalarAsync<int>("select id from EXAMS where name = '" + exam.name + "' and id <> " + exam.id + "") != 0)
                {
                    exam.name = exam.name + "_" + CommonData.RandomString(3);
                }
                await _database.UpdateAsync(exam);
            }
            catch (Exception ex)
            {
                Debug.Print("*UpdateExam* msg: " + ex.ToString());
            }
        }

        public async Task<Exam> GetExamByID(int id)
        {
            try
            {
                return await _database.GetAsync<Exam>(id);
            }
            catch (Exception ex)
            {
                Debug.Print("*GetExamByID* msg: " + ex.ToString());
                return new Exam();
            }
        }

        public async Task<int> InsertExam(Exam item)
        {
            try
            {
                if (await _database.ExecuteScalarAsync<int>("select id from EXAMS where name = '" + item.name + "'") != 0)
                {
                    item.name = item.name + "_" + CommonData.RandomString(3);
                }
                return await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertExam* msg: " + ex.ToString());
            }
            return 0;
        }
        #endregion

        #region Questions
        public async Task UpdateQuestion(ExamQuestion question)
        {
            try
            {
                await _database.UpdateAsync(question);
            }
            catch (Exception ex)
            {
                Debug.Print("*UpdateQuestion* msg: " + ex.ToString());
            }
        }

        public async Task<List<ExamQuestion>> GetQuestionsByExamID(int id)
        {
            try
            {
                var l = await _database.QueryAsync<ExamQuestion>("select * from EXAM_QUESTIONS " +
                    "where examID = " + id + " order by position");
                return await _database.QueryAsync<ExamQuestion>("select * from EXAM_QUESTIONS " +
                    "where examID = " + id + " order by position");
            }
            catch (Exception ex)
            {
                Debug.Print("*GetQuestionsByExamID* msg: " + ex.ToString());
                return new List<ExamQuestion>();
            }
        }

        public async Task<int> InsertExamQuestion(ExamQuestion item)
        {
            try
            {
                return await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertExamQuestion* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task<int> DeleteQuestion(ExamQuestion q, bool isPlayed = false)
        {
            try
            {
                var variants = await GetQuestionVariantsByQuestionID(q.id);
                if (isPlayed)
                {
                    q.isDeleted = true;
                    variants.Select(i => { i.isDeleted = true; return i; });
                    await _database.UpdateAllAsync(variants);
                    return await _database.UpdateAsync(q);
                }
                for(int i = 0; i < variants.Count; i++)
                {
                    await DeleteVariant(variants[i]);
                }
                return await _database.DeleteAsync(q);
            }
            catch (Exception ex)
            {
                Debug.Print("*DeleteQuestion* msg: " + ex.ToString());
                return 0;
            }
        }

        #endregion

        #region Variants
        public async Task<int> UpdateQuestionVariant(ExamQuestionVariant variant)
        {
            try
            {
                return await _database.UpdateAsync(variant);
            }
            catch (Exception ex)
            {
                Debug.Print("*UpdateQuestionVariant* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task<List<ExamQuestionVariant>> GetQuestionVariantsByQuestionID(int id)
        {
            try
            {
                return await _database.QueryAsync<ExamQuestionVariant>("select * from EXAM_QUESTION_VARIANTS " +
                    "where questionID = " + id + " order by position");
            }
            catch (Exception ex)
            {
                Debug.Print("*GetQuestionVariantsByQuestionID* msg: " + ex.ToString());
                return new List<ExamQuestionVariant>();
            }
        }

        public async Task<int> GetQuestionRightVariantByQuestionID(int id)
        {
            try
            {
                return await _database.ExecuteScalarAsync<int>("select id from EXAM_QUESTION_VARIANTS where questionID = " + id + " and isRight = 1");
            }
            catch (Exception ex)
            {
                Debug.Print("*GetQuestionRightVariantByQuestionID* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task<int> InsertExamQuestionVariant(ExamQuestionVariant item)
        {
            try
            {
                return await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertExamQuestionVariant* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task ChangeQuestionsOrder(int id, int newIndex, int oldIndex)
        {
            try
            {
                newIndex += 1;
                oldIndex += 1;
                await _database.QueryAsync<ExamQuestion>("update EXAM_QUESTIONS set position = " + newIndex + " where id = " + id + "");
                if (newIndex > oldIndex )
                {
                    await _database.QueryAsync<ExamQuestion>("update EXAM_QUESTIONS set position = position - 1 where position <= " + newIndex + " AND position >= " + oldIndex + " " +
                             "AND id <> " + id + " AND position <> 1");
                    return;
                }
                await _database.QueryAsync<ExamQuestion>("update EXAM_QUESTIONS set position = position + 1 where position >= " + newIndex + " AND position <= " + oldIndex + " " +
                            "AND id <> " + id);
            }
            catch (Exception ex)
            {
                Debug.Print("*ChangeQuestionsOrder* msg: " + ex.ToString());
            }
        }

        public async Task ChangeVariantsOrder(int id, int newIndex, int oldIndex)
        {
            try
            {
                newIndex += 1;
                oldIndex += 1;
                await _database.QueryAsync<ExamQuestion>("update EXAM_QUESTION_VARIANTS set position = " + newIndex + " where id = " + id + "");
                if (newIndex > oldIndex)
                {
                    await _database.QueryAsync<ExamQuestionVariant>("update EXAM_QUESTION_VARIANTS set position = position - 1 where position <= " + newIndex + " AND position >= " + oldIndex + " " +
                             "AND id <> " + id + " AND position <> 1");
                    return;
                }
                await _database.QueryAsync<ExamQuestionVariant>("update EXAM_QUESTION_VARIANTS set position = position + 1 where position >= " + newIndex + " AND position <= " + oldIndex + " " +
                            "AND id <> " + id);
            }
            catch (Exception ex)
            {
                Debug.Print("*ChangeVariantsOrder* msg: " + ex.ToString());
            }
        }

        public async Task<int> DeleteVariant(ExamQuestionVariant v, bool isPlayed = false)
        {
            try
            {
                if (isPlayed)
                {
                    v.isDeleted = true;
                    return await _database.UpdateAsync(v);
                }
                return await _database.DeleteAsync(v);
            }
            catch (Exception ex)
            {
                Debug.Print("*DeleteVariantByID* msg: " + ex.ToString());
                return 0;
            }
        }

        #endregion

        #region Results

        public async Task<List<ResultBodyView>> GetResultBodiesByResultHeadID(int id, int limit, int offset)
        {
            try
            {
                return await _database.QueryAsync<ResultBodyView>("select a.isRight as isRight, b.name as questionName, c.name as chosenVariantName " +
                    "from RESULTS_BODY as a " +
                    "inner join EXAM_QUESTIONS as b on a.questionID = b.id " +
                    "inner join EXAM_QUESTION_VARIANTS as c on a.chosenVariantID = c.id " +
                    "where a.headID = " + id + " limit " + limit + " offset " + offset);
            }
            catch (Exception ex)
            {
                Debug.Print("*GetResultBodiesByResultHeadID* msg: " + ex.ToString());
                return new List<ResultBodyView>();
            }
        }

        public async Task<List<ResultView>> GetReultsByUserID(int id, bool topRes = false, int limit = 0, int offset = 0)
        {
            try
            {
                string qtyStr = " order by ";
                if (topRes)
                {
                    qtyStr += "a.percent desc limit 5";
                }
                else
                {
                    qtyStr += "a.dateEND desc limit " + limit + " offset " + offset;
                }
                return await _database.QueryAsync<ResultView>("select a.*, b.name as label from RESULTS_HEAD as a inner join EXAMS as b on a.examID = b.id where a.userID = "
                    + id + qtyStr);
            }
            catch (Exception ex)
            {
                Debug.Print("*GetReultsByUserID* msg: " + ex.ToString());
                return new List<ResultView>();
            }
        }

        public async Task<ResultView> GetReultByUserID(int id, string desc = "", bool last = false)
        {
            try
            {
                string orderby = " order by ";
                if (last)
                {
                    orderby += "a.dateEND ";
                }
                else
                {
                    orderby += "a.percent ";
                }
                var res = await _database.QueryAsync<ResultView>("select a.*, b.name as label from RESULTS_HEAD as a inner join EXAMS as b on a.examID = b.id where a.userID = "
                    + id + orderby + desc + " limit 1");
                return res[0];
            }
            catch (Exception ex)
            {
                Debug.Print("*GetReultByUserID* msg: " + ex.ToString());
                return new ResultView();
            }
        }

        public async Task<ResultView> GetResultByID(int id)
        {
            try
            {
                var res = await _database.QueryAsync<ResultView>("select a.*, b.name as label from RESULTS_HEAD as a inner join EXAMS as b on a.examID = b.id where a.id = "
                    + id + " limit 1");
                return res[0];
            }
            catch (Exception ex)
            {
                Debug.Print("*GetResultByID* msg: " + ex.ToString());
                return new ResultView();
            }
        }

        public async Task<int> GetAVGResultByUserID(int id)
        {
            try
            {
                return await _database.ExecuteScalarAsync<int>("select AVG(percent) from RESULTS_HEAD where userID = " + id);
            }
            catch (Exception ex)
            {
                Debug.Print("*GetAVGResult* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task<int> InsertResult(Result item)
        {
            try
            {
                return await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertResult* msg: " + ex.ToString());
                return 0;
            }
        }

        public async Task<int> InsertResultBodiesRange(List<ResultBody> items)
        {
            try
            {
                return await _database.InsertAllAsync(items);
            }
            catch (Exception ex)
            {
                Debug.Print("*InsertResultsRange* msg: " + ex.ToString());
                return 0;
            }
        }

        #endregion

    }
}
