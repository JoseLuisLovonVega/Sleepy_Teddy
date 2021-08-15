using System;
using System.Collections.Generic;
using SleepyTeddy.Models;

namespace SleepyTeddy.Data.Interfaces
{
    public interface IStepsRepository
    {
        IEnumerable<Step> GetAll();
        void Add(Step step);
        void RemoveAll();
        DateTime LastAddedDatetime();
    }
}
