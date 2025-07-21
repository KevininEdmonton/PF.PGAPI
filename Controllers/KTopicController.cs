using K.Common;
using KS.Library.Interface.PFAPI.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFAPI.SupportModels;
using PFAPI.utility;
using System;

namespace PFAPI.Controllers
{

    [Route("[controller]")]
    [Produces("application/json")]
   // [ApiVersion("1.0")]
    [ApiController]   
    public class KTopicController : BaseController
    {
        //private readonly IHoBOCenterRepository _repository;
       // private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        private ILogger<KTopicController> _logger;
        private IConfigurationRoot _config;
        private string _curDataModelName = "KTopic";

        public KTopicController(
            //IHoBOCenterRepository repository, IMapper mapper
                                       ILogger<KTopicController> logger, LinkGenerator linkGenerator
                                      , IConfigurationRoot config)
        {
            //_repository = repository;
            //_mapper = mapper;
            _logger = logger;
            _linkGenerator = linkGenerator;
            _config = config;
        }

        /// <summary>
        /// Get a list of KTopic Model
        /// </summary>
        /// <param name="qp_page">Page number</param>
        /// <param name="qp_pagesize">Page size</param>
        /// <param name="qp_orderby">Order by</param>
        /// <param name="qp_includeallchildrendata">Whether get all children data</param>
        /// <param name="qp_includedata">Specific children data that need to be get; one level lower only; split by [,]</param>
        /// <param name="qp_filter">Filter condition</param>
        /// <returns>An ActionResult of PagedData for existing KTopic Models</returns>
       // [Authorize(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne)]
        [HttpGet(Name = SystemStatics.Route_GetAll_KTopic)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedData<KTopicModel>>> Get(int qp_page = PFAPIStatics.SYS_Default_QP_Page
                                                                , int qp_pagesize = PFAPIStatics.SYS_Default_QP_Pagesize
                                                                , string qp_orderby = PFAPIStatics.SYS_Default_QP_Orderby
                                                                , bool qp_includeallchildrendata = PFAPIStatics.SYS_Default_QP_IncludeAllChildrenData
                                                                , string qp_includedata = PFAPIStatics.SYS_Default_QP_IncludeData
                                                                , string qp_filter = PFAPIStatics.SYS_Default_QP_Filter)
        {
            try
            {
                QueryParameter theQueryParameter = new QueryParameter(qp_page, qp_pagesize, qp_orderby, qp_includeallchildrendata, qp_includedata, qp_filter
                                                                      , Url.Link(SystemStatics.Route_GetAll_KTopic, null));

                int thePageSize = theQueryParameter.pagesize;
                if(thePageSize<1 || thePageSize>100)
                {
                    thePageSize = PFAPIStatics.SYS_Default_QP_Pagesize;
                }

                //// work with Client Database
                //using (IHoBOClientRepository _repository_clientdb = HoBOClientRepository.CreateRepositoryInstance(User, _config, _mapper))
                //{
                //    var theResult = await _repository_clientdb.CreatePagedResults<KTopic, KTopicModel>(theQueryParameter);
                //    return Ok(theResult);
                //}

                List<KTopicModel> topics = DataHelper.LoadKTopicsFromJson();
                if (!topics.HasData() )
                {
                    return NotFound("No KTopic data found.");
                }
                List<KTopicModel> validtopics = new List<KTopicModel>();
                if (topics.Count > thePageSize)
                {   // Filter topics based on the page size
                    validtopics = topics.OrderBy(d=>d.Name).ToList().Skip((theQueryParameter.page - 1) * thePageSize)
                                        .Take(thePageSize)
                                        .ToList();
                }
                else
                {
                    validtopics = topics;
                }


                PagedData<KTopicModel> result = new PagedData<KTopicModel>
                {
                    Data = validtopics,
                    PageNum = theQueryParameter.page,
                    PageSize = thePageSize,
                    RecordsCount = topics.Count,
                    PagesCount = (int)Math.Ceiling((double)topics.Count / theQueryParameter.pagesize),
                    OrderBy = theQueryParameter.orderby,
                };

                return Ok(result);

            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
            }
        }


        /// <summary>
        /// Get one KTopic Model
        /// </summary>
        /// <param name="id">KTopic ID</param>
        /// <param name="qp_includeallchildrendata">Whether get all children data</param>
        /// <param name="qp_includedata">Specific children data that need to be get; one level lower only; split by [,]</param>
        /// <returns>An ActionResult of existing KTopic Model</returns>
        [Authorize(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne)]
        [HttpGet("{id}", Name = SystemStatics.Route_GetOne_KTopic)]
        //[Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<KTopicModel>> Get(Guid id, bool qp_includeallchildrendata = PFAPIStatics.SYS_Default_QP_IncludeAllChildrenData
                                                                , string qp_includedata = PFAPIStatics.SYS_Default_QP_IncludeData)
        {
            try
            {
                QueryParameterMin theQueryParameter = new QueryParameterMin(qp_includeallchildrendata, qp_includedata);

                return Ok();

                //// work with Client Database
                //using (IHoBOClientRepository _repository_clientdb = HoBOClientRepository.CreateRepositoryInstance(User, _config, _mapper))
                //{
                //    IQueryable<KTopic> theQuery = _repository_clientdb.GetQueryable<KTopic>();
                //    theQuery = theQuery.Where(o => o.Id == id);

                //    KTopicModel theDataModel = await _repository_clientdb.GetOneDataModel<KTopic, KTopicModel>(theQueryParameter, theQuery);
                //    if (theDataModel == null)
                //        return NotFound();

                //    return Ok(theDataModel);
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
            }
        }


     //   /// <summary>
     //   /// Create a new KTopic Model
     //   /// </summary>
     //   /// <param name="model">KTopic Model to be created</param>
     //   /// <returns>An ActionResult of newly created KTopic Model</returns>
     //   /// <remarks>
     //   /// Sample request:
     //   ///     POST                         \
     //   ///     {                            \
     //   ///        "name": "Item1",          \
     //   ///        "code": "example code"    \
     //   ///     }                            
     //   /// </remarks>
     //   /// <response code="201">Returns the newly created item</response>
     //   /// <response code="400">If the request/data is not valid</response>       
     //   /// <response code="500">If error happened at server side</response>
     //   /// <response code="422">If the provided data is not valid</response>               
     ////   [Authorize(Policy4ModuleOperations.P_Sales.SalesTaxRateManageOperation)]
     //   [HttpPost]
     //   [Consumes("application/json")]
     //   [ProducesResponseType(StatusCodes.Status200OK)]
     //   [ProducesResponseType(StatusCodes.Status201Created)]
     //   [ProducesResponseType(StatusCodes.Status400BadRequest)]
     //   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     //   [ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
     //       Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
     //   public async Task<ActionResult<KTopicModel>> Post(KTopicModel model)
     //   {
     //       try
     //       {
     //           using (IHoBOClientRepository _repository_clientdb = HoBOClientRepository.CreateRepositoryInstance(User, _config, _mapper))
     //           {
     //               //Verify DB constraints
     //               if (!VerificationHelper.VerifyDBconstraints(_repository_clientdb, model, out string msg))
     //               {
     //                   return BadRequest(msg);
     //               }

     //               // Name should be Unique
     //               if (_repository_clientdb.GetExists<KTopic>($"Name=='{model.Name}'"))
     //               {
     //                   return BadRequest($"Name [{model.Name}] is in Use;");
     //               }

     //               // Create new 
     //               model.ID = Guid.Empty;
     //               KTopic entity = await MappingHelper.MaptoEntity(_repository_clientdb, _mapper, model);

     //               //regular operation
     //               _repository_clientdb.Create(entity);
     //               if (!await _repository_clientdb.SaveChangesAsync(this.User))
     //               {
     //                   return this.StatusCode(StatusCodes.Status500InternalServerError, null);
     //               }

     //               var newUri = Url.Link(SystemStatics.Route_GetOne_KTopic, new { id = entity.Id });
     //               return Created(newUri, _mapper.Map<KTopicModel>(entity));
     //           }
     //       }
     //       catch (Exception e)
     //       {
     //           _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
     //           return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
     //       }
     //   }

     //   /// <summary>
     //   /// Update a existing KTopic Model
     //   /// </summary>
     //   /// <param name="id">KTopic ID</param>
     //   /// <param name="model">KTopic Model to be updated</param>
     //   /// <returns>An ActionResult of updated KTopic Model</returns>
     //   ////[HttpPatch("{id}")]
     // //  [Authorize(Policy4ModuleOperations.P_Sales.SalesTaxRateManageOperation)]
     //   [HttpPut("{id}")]
     //   [Consumes("application/json")]
     //   [ProducesResponseType(StatusCodes.Status200OK)]
     //   [ProducesResponseType(StatusCodes.Status304NotModified)]
     //   [ProducesResponseType(StatusCodes.Status404NotFound)]
     //   [ProducesResponseType(StatusCodes.Status400BadRequest)]
     //   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     //   [ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
     //       Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
     //   public async Task<ActionResult<KTopicModel>> Put(Guid id, KTopicModel model)
     //   {
     //       try
     //       {
     //           using (IHoBOClientRepository _repository_clientdb = HoBOClientRepository.CreateRepositoryInstance(User, _config, _mapper))
     //           {
     //               var existingentity = _repository_clientdb.GetById<KTopic>(id);
     //               if (existingentity == null) return NotFound($"Could not find {_curDataModelName} with id of {id}");

     //               if (model.ID != id)
     //                   model.ID = id;

     //               // Name should be Unique
     //               if (_repository_clientdb.GetExists<KTopic>($"Name=='{model.Name}' and ID != GUID('{model.ID}')"))
     //               {
     //                   return BadRequest($"Name [{model.Name}] is in Use;");
     //               }

     //               //Verify DB constraints
     //               if (!VerificationHelper.VerifyDBconstraints(_repository_clientdb, model, out string msg))
     //               {
     //                   return BadRequest(msg);
     //               }

     //               existingentity = await MappingHelper.MaptoEntity(_repository_clientdb, _mapper, model);
     //               if (existingentity == null) return BadRequest($"Failed to map {_curDataModelName} with id of {id}");

     //               if (!_repository_clientdb.HasChangesToDB())
     //               {
     //                   if (_config["ValidationCheckOptions:Apply200ForNotModifiedRecord"].MeaningTrue())
     //                   {
     //                       return _mapper.Map<KTopicModel>(existingentity);
     //                   }
     //                   else
     //                   {
     //                       return this.StatusCode(StatusCodes.Status304NotModified);
     //                   }
     //               }
     //               if (await _repository_clientdb.SaveChangesAsync(this.User))
     //               {
     //                   return Ok(_mapper.Map<KTopicModel>(existingentity));
     //               }
     //               return StatusCode(StatusCodes.Status304NotModified, model);
     //           }
     //       }
     //       catch (FormatException e)
     //       {
     //           _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
     //           return BadRequest(e.Message);
     //       }
     //       catch (Exception e)
     //       {
     //           _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
     //           return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
     //       }
     //   }

     //   /// <summary>
     //   /// Delete a existing KTopic Model
     //   /// </summary>
     //   /// <param name="id">KTopic ID</param>
     //   /// <returns>An ActionResult</returns>
     //  // [Authorize(Policy4ModuleOperations.P_Sales.SalesTaxRateManageOperation)]
     //   [HttpDelete("{id}")]
     //   [ProducesResponseType(StatusCodes.Status200OK)]
     //   [ProducesResponseType(StatusCodes.Status400BadRequest)]
     //   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     //   [ProducesResponseType(StatusCodes.Status404NotFound)]
     //   public async Task<IActionResult> Delete(Guid id)
     //   {
     //       try
     //       {
     //           using (IHoBOClientRepository _repository_clientdb = HoBOClientRepository.CreateRepositoryInstance(User, _config, _mapper))
     //           {
     //               var existingentity = _repository_clientdb.GetById<KTopic>(id);
     //               if (existingentity == null) return NotFound($"Could not find {_curDataModelName} with id of {id}");

     //               //delete schedule control
     //               _repository_clientdb.Delete(existingentity);
     //               if (!await _repository_clientdb.SaveChangesAsync(this.User))
     //               {
     //                   _logger.LogWarning($"Failed to delete {_curDataModelName} with id of {id}");
     //                   return this.StatusCode(StatusCodes.Status500InternalServerError, $"Failed to delete {_curDataModelName} with id of {id}");
     //               }

     //               return Ok();
     //           }
     //       }
     //       catch (Exception e)
     //       {
     //           _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
     //           return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
     //       }
     //   }
    }
}
