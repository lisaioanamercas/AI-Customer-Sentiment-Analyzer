using AISA.Application.BusinessProfiles.DTOs;
using AISA.Application.Reviews.DTOs;
using AISA.Domain.Entities;
using AutoMapper;

namespace AISA.Application.Common.Mappings;

/// <summary>
/// Profilul AutoMapper — definire mapări între entități și DTO-uri.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Review → ReviewDto
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.SentimentLabel, opt => opt.MapFrom(src => src.SentimentResult != null ? src.SentimentResult.Label : (Domain.Enums.SentimentLabel?)null))
            .ForMember(dest => dest.SentimentScore, opt => opt.MapFrom(src => src.SentimentResult != null ? src.SentimentResult.Score : (double?)null));

        // BusinessProfile → BusinessProfileDto
        CreateMap<BusinessProfile, BusinessProfileDto>()
            .ForMember(dest => dest.SubscriptionTier, opt => opt.MapFrom(src => src.SubscriptionTier.ToString()))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));
    }
}
