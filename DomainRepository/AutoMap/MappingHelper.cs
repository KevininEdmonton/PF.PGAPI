using AutoMapper;
using DomainRepository.IRepositories;
using K.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DomainRepository.Mapping
{
    // should add data access control here to ensure user will not be able to crete/update data that don't have access
    // for data reading, should control when include children data
    public static class MappingHelper
    {
        public static async Task<TReturn> StarndardMap<T, TReturn>(IPFClientRepository repository, IMapper mapper, T model, Guid theID) where T : class where TReturn : class
        {
            if (model == null)
                return null;

            TReturn theEntity = null;
            if (theID.HasValue() && theID != Guid.Empty)
                theEntity = await repository.GetByIdAsync<TReturn>(theID);

            if (theEntity == null)
            {
                theEntity = mapper.Map<TReturn>(model);
            }
            else
            {
                mapper.Map(model, theEntity);
            }
            return theEntity;
        }


    }
}
