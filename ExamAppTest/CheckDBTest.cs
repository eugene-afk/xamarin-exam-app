using ExamApp.Data.SQlite;
using System.Threading.Tasks;
using Xunit;

namespace ExamAppTest
{
    public class CheckDbTest
    {
        [Fact]
        public async Task CreateDataBaseTest()
        {
            const int expected = 8;

            bool isDeleted = SQLiteTools.DeleteDataBase();
            await SQLiteTools.CheckDB();

            var list = await SQLiteTools.database.QueryAsync<TableName>("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1");

            Assert.True(isDeleted);
            Assert.Equal(expected, list.Count);
        }
    }

    internal class TableName
    {
        public string name { get; set; }
    }

}
