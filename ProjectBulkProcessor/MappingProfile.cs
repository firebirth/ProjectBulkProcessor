using AutoMapper;
using ProjectBulkProcessor.Configration;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EnrichParameters, OptionsModel>();
        }
    }
}
