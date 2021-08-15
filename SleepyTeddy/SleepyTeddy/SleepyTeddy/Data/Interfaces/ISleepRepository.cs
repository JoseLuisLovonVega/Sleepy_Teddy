using System.Collections.Generic;
using SleepyTeddy.Models;

namespace SleepyTeddy.Data.Interfaces
{
    public interface ISleepRepository
    {
        IEnumerable<Sleep> GetAll();
        void Add(Sleep sleep);
        void RemoveAll();
    }
}