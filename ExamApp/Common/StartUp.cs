using ExamApp.Data.SQlite;
using System.Threading.Tasks;

namespace ExamApp.Common
{
    public static class StartUp
    {
        public static SplashPage splash;
        public static async Task Init()
        {
            await SQLiteTools.CheckDB();
        }
    }
}
