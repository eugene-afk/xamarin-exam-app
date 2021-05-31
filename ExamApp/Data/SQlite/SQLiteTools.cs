using ExamApp.Data.Model;
using ExamApp.View;
using SQLite;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ExamApp.Data.SQlite
{
    public static class SQLiteTools
    {
        internal static SQLiteAsyncConnection database;
        public static DBRepository repo;
        private const string databaseName = "examapp.db";
        private static string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), databaseName);

        public static async Task CheckDB()
        {
            if (!File.Exists(databasePath))
            {
                try
                {
                    database = new SQLiteAsyncConnection(databasePath);
                    await database.CreateTableAsync<Exam>();
                    await database.CreateTableAsync<ExamQuestion>();
                    await database.CreateTableAsync<ExamQuestionVariant>();
                    await database.CreateTableAsync<User>();
                    await database.CreateTableAsync<Result>();
                    await database.CreateTableAsync<ResultBody>();
                    await database.CreateTableAsync<Config>();
                    repo = new DBRepository(database);
                }
                catch (Exception ex)
                {
                    Debug.Print("*CheckDB* msg: " + ex.ToString());
                }
            }
            database = new SQLiteAsyncConnection(databasePath);
            repo = new DBRepository(database);
        }

        //temp
        public static bool DeleteDataBase()
        {
            try
            {
                File.Delete(databasePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("*DeleteDataBase* msg: " + ex.ToString());
            }
            return false;
        }

        //temp
        public static async Task ReCreateDatabase()
        {
            if (!DeleteDataBase())
            {
                return;
            }
            await CheckDB();
            var navPage = new NavigationPage(new LoginPage());
            Application.Current.MainPage = navPage;
        }

    }
}
