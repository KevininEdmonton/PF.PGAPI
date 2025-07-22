using AutoMapper;
using Azure;
using K.Common;
using KS.Library.EFDB;
using KS.Library.Interface.PFAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DomainRepository.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ///------- some mapping rules ------///
            // 1. each BaseModel Model, always add .ForMember(c => c.LinkedModels, opt => opt.MapFrom<LinkedModelsResolver>())
            // 2. for mapping, specify children model-properties' MapFrom; example: .ForMember(c => c.Operations, opt => opt.MapFrom<PagedModuleOperationsResolver>())  ////.ForMember(c => c.Operations, o => o.MapFrom(m => m.ZzOperations))
            // 3. for any reverseMap, ignore all children mapping
            // 4. for any reverseMap, add following ignores to ignore system-related data mapping
            //////.ForMember(s => s.ValidFrom, opt => opt.Ignore())
            //////.ForMember(s => s.ValidTo, opt => opt.Ignore())
            ////.ForMember(s => s.CreatedAtUtc, opt => opt.Ignore())
            ////.ForMember(s => s.CreatedByUserId, opt => opt.Ignore())
            ////.ForMember(s => s.CreatedThrough, opt => opt.Ignore())
            ////.ForMember(s => s.LastUpdatedAtUtc, opt => opt.Ignore())
            ////.ForMember(s => s.LastUpdatedByUserId, opt => opt.Ignore())
            ////.ForMember(s => s.LastUpdatedThrough, opt => opt.Ignore())
            ///

            this.CreateMap<Ktopic, KTopicModel>()
                //.ForMember(c => c.Operations, opt => opt.MapFrom<PagedDataResolver<ZzOperation, ZZOperationModel>>())
                //.ForMember(c => c.ClientModules, o => o.MapFrom<PagedDataResolver<CenterDB.Entities.ZClientModuleCenter, ZClientModuleModel>>())
                 .ForMember(d => d.CommentList, opt => opt.MapFrom(s=>s.KtopicComments))
                .ReverseMap()
                .ForMember(s => s.KtopicComments, opt => opt.Ignore())
              //  .ForMember(s => s.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(s => s.CreatedByUserId, opt => opt.Ignore())
              //  .ForMember(s => s.LastUpdatedAtUtc, opt => opt.Ignore())
                .ForMember(s => s.LastUpdatedByUserId, opt => opt.Ignore())
                ;

            this.CreateMap<KtopicComment, KTopicCommentModel>()
                //.ForMember(c => c.Operations, opt => opt.MapFrom<PagedDataResolver<ZzOperation, ZZOperationModel>>())
                //.ForMember(c => c.ClientModules, o => o.MapFrom<PagedDataResolver<CenterDB.Entities.ZClientModuleCenter, ZClientModuleModel>>())
                .ReverseMap()
              //  .ForMember(s => s.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(s => s.CreatedByUserId, opt => opt.Ignore())
              //  .ForMember(s => s.LastUpdatedAtUtc, opt => opt.Ignore())
                .ForMember(s => s.LastUpdatedByUserId, opt => opt.Ignore())
                ;


            //this.CreateMap<ZzModule, ZZModuleModel>()
            //        .ForMember(c => c.Operations, opt => opt.MapFrom<PagedDataResolver<ZzOperation, ZZOperationModel>>())
            //        .ForMember(c => c.ClientModules, o => o.MapFrom<PagedDataResolver<CenterDB.Entities.ZClientModuleCenter, ZClientModuleModel>>())
            //        .ReverseMap()
            //        .ForMember(s => s.CreatedAtUtc, opt => opt.Ignore())
            //        .ForMember(s => s.CreatedByUserId, opt => opt.Ignore())
            //        .ForMember(s => s.LastUpdatedAtUtc, opt => opt.Ignore())
            //        .ForMember(s => s.LastUpdatedByUserId, opt => opt.Ignore())
            //;

        }

        public static T MapEnum<T>(string grade)
        {
            return EnumHelper<T>.Parse(grade, true);
        }
    }
}
