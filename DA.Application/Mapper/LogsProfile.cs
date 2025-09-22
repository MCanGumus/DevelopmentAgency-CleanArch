using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Entities;
//using DA.Domain.Dtos.LogsDto.Logs;
//using DA.Domain.Entities.Logs;

namespace DA.Application.Mapper
{
    public class LogsProfile : Profile
    {
        public LogsProfile()
        {
            #region LogEntry
            CreateMap<LogEntry, LogEntryDto>().ReverseMap();
            CreateMap<LogEntry, UpdateLogEntryDto>().ReverseMap();
            CreateMap<LogEntry, SaveLogEntryDto>().ReverseMap();
            #endregion
        }
    }
}
