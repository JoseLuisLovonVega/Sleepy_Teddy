using System.Collections.Generic;
using SleepyTeddy.Models;

namespace SleepyTeddy.Data.Interfaces
{
    public interface IHeartrateRepository
    {
        IEnumerable<Heartrate> GetAll();
        void Add(Heartrate heartrate);
        void RemoveAll();
    }
}