using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IMissionService : IBaseService<Mission, UpdateMissionDto, SaveMissionDto, MissionDto>
    {
        List<MissionDto> GetAllMissionsOfMe(Guid gid);

        MissionDto GetMission(Guid gid);

        MissionDto GetMissionWithDocId(string docId);

        List<MissionDto> GetAllMissionsMonthly(DateTime filter);

        List<MissionDto> GetAllMissions(DateTime startDate, DateTime endDate);

        List<MissionDto> GetAllMissionsDepartment(Guid idDepartment, DateTime startDate, DateTime endDate);

        List<MissionDto> GetAllMissionsByProxy(Guid id);
    }
}
