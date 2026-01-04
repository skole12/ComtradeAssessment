using AutoMapper;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;

namespace ComtradeAssessment.Mappers;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Campaign, CampaignResponseDto>();
    }
}
