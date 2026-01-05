using AutoMapper;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Mappers;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Campaign, CampaignResponseDto>();
        CreateMap<CampaignResponseDto, Campaign>();
        CreateMap<CampaignOffer, CampaignOfferResponseDto>();
        CreateMap<BackgroundJobStatus, CampaignJobResponseDto>();
    }
}
