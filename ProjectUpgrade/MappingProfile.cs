using AutoMapper;
using ProjectUpgrade.Configration;
using ProjectUpgrade.Upgrade.Models;

namespace ProjectUpgrade
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EnrichParameters, OptionsModel>();
        }
    }
}
