using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
[AuthorizeServiceByRole(ERole.SalesManager)]
public interface ICampaignService : IBaseEntityService<Campaign, CampaignResponseDto>
{
    [OperationContract]
    Task<CampaignResponseDto> Create(CreateCampaignRequest request);

    //[AuthorizeByRole(ERole.SalesManager)] besides auth on whole service, we can add authorization individually by method
    [OperationContract]
    Task<CampaignResponseDto> Update(UpdateCampaignRequest request);

    [OperationContract]
    Task Delete(int campaignId);

    [OperationContract]
    Task<CampaignDetailsResponseDto> Details(int campaignId);

    [OperationContract]
    Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request);

    [OperationContract]
    Task<CampaignJobResponseDto> JobStatus(Guid jobId);

    [OperationContract]
    Task<DownloadResultsFileResponse> DownloadResultsFile(int campaignId);

    [OperationContract]
    new Task<PagedResult<CampaignResponseDto>> GetAll(BaseRequest request);
}
