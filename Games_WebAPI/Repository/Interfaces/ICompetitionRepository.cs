using System;
using System.Collections.Generic;
using Games_WebAPI.Entities;

namespace Games_WebAPI.Repository;

public interface ICompetitionRepository
{
    IEnumerable<Competition> GetAll();
    Competition GetById(Guid id);
    void Add(Competition competition);
    void Update(Competition competition);
    void Delete(Guid id);
}