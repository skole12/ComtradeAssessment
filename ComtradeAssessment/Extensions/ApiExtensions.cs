using ComtradeAssessment.Interfaces;
using SoapCore;

namespace ComtradeAssessment.Extensions;

public static class ApiExtensions
{
    public static IEndpointRouteBuilder MapSoapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.UseSoapEndpoint<IUserService>(
            "/User.svc",
            new SoapEncoderOptions(),
            SoapSerializer.DataContractSerializer
        );

        endpoints.UseSoapEndpoint<ICampaignService>(
            "/Campaign.svc",
            new SoapEncoderOptions(),
            SoapSerializer.DataContractSerializer
        );

        endpoints.UseSoapEndpoint<ICampaignOfferService>(
            "/CampaignOffer.svc",
            new SoapEncoderOptions(),
            SoapSerializer.DataContractSerializer
        );

        return endpoints;
    }
}
