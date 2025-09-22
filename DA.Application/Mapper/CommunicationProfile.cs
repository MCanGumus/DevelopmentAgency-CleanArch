using AutoMapper;
using DA.Domain.Dtos;
using DA.Domain.Entities;

namespace DA.Application.Mapper
{
    public class CommunicationProfile : Profile
    {
        public CommunicationProfile()
        {
            #region Address
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<Address, UpdateAddressDto>().ReverseMap();
            CreateMap<Address, SaveAddressDto>().ReverseMap();
            #endregion

            #region GSMNumber
            CreateMap<GSMNumber, GSMNumberDto>().ReverseMap();
            CreateMap<GSMNumber, UpdateGSMNumberDto>().ReverseMap();
            CreateMap<GSMNumber, SaveGSMNumberDto>().ReverseMap();
            #endregion

            #region EMail
            CreateMap<EMail, EMailDto>().ReverseMap();
            CreateMap<EMail, UpdateEMailDto>().ReverseMap();
            CreateMap<EMail, SaveEMailDto>().ReverseMap();
            #endregion


        }
    }
}
