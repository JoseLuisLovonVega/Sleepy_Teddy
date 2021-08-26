using System.Collections.Generic;
using SleepyTeddy.Data.Interfaces;
using SleepyTeddy.Models;


namespace SleepyTeddy.Data.Repository
{
    public class SleepRepository : ISleepRepository
    {
        private readonly Database _database;
        public SleepRepository(Database database)
        {
            _database = database;
        }

        public void Add(Sleep sleep)
        {
            var query = "INSERT INTO Sleep(DateTime, SleepType) VALUES(?,?)";
            var command = _database.Instance.CreateCommand(query, new object[] { sleep.DateTime, sleep.SleepType });
            command.ExecuteNonQuery();
        }
        //IEnumerable
        public List<Sleep> GetAll()
        {
            return _database.Instance.Table<Sleep>().OrderBy(x => x.DateTime).ToList();
        }

        public void RemoveAll()
        {
            var sleeps = this.GetAll();
            foreach (var sleep in sleeps)
            {
                var query = "DELETE FROM Sleep WHERE Id = ?";
                var command = _database.Instance.CreateCommand(query, new object[] { sleep.Id });
                command.ExecuteNonQuery();
            }
        }
    }
}