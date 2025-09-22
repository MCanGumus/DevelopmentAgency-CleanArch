using DA.Domain.Entities;
using DA.Domain.Dtos;

namespace DA.Application.Abstractions
{
    public interface IEMailService : IBaseService<EMail, UpdateEMailDto, SaveEMailDto, EMailDto>
    {
        List<EMailDto> GetAllMyMails(Guid id);
    }
}
