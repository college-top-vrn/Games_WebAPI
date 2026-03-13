using System;
using System.Collections.Generic;
using Games_WebAPI.Entities;

namespace Games_WebAPI.Repository;

public interface ICompetitionResultRepository
{
    IEnumerable<CompetitionResult> GetAll();
    CompetitionResult GetResultById(Guid id);
    IEnumerable<CompetitionResult> GetAllResultsConcreteCompetition(Guid id);

    void Add(CompetitionResult result);
    void Update(CompetitionResult result);
    void Delete(Guid id);
}