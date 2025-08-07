using AutoMapper;
using DomainRepository.IRepositories;
using DomainRepository.Mapping;
using DomainRepository.Repositories;
using K.Common;
using KS.Library.EFDB;
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
    [ApiVersion("1.0")]
    [ApiController]   
    public class BidController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IPFClientRepository _repository;
        private readonly IMapper _mapper;
        //   private readonly LinkGenerator _linkGenerator;
        private ILogger<BidController> _logger;
        private IConfiguration _config;
        private string _curDataModelName = "Bid";

        public BidController(IMapper mapper, ILogger<BidController> logger//, LinkGenerator linkGenerator
                                      , IConfiguration config)
        {
            //_repository = repository;
            _mapper = mapper;
            _logger = logger;
           // _linkGenerator = linkGenerator;
            _config = config;
        }

        /// <summary>
        /// Get a list of Bid Model
        /// </summary>
        /// <param name="qp_page">Page number</param>
        /// <param name="qp_pagesize">Page size</param>
        /// <param name="qp_orderby">Order by</param>
        /// <param name="qp_includeallchildrendata">Whether get all children data</param>
        /// <param name="qp_includedata">Specific children data that need to be get; one level lower only; split by [,]</param>
        /// <param name="qp_filter">Filter condition</param>
        /// <returns>An ActionResult of PagedData for existing Bid Models</returns>
        //[Authorize(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne)]
        [HttpGet("{houseId}", Name = SystemStatics.Route_GetList_Bid)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Bid>>> GetList(int houseId)
        {
            try
            {
                BidRepository theRepository = new BidRepository();
                var theResult = theRepository.GetBids(houseId);
                return Ok(theResult);


                //QueryParameter theQueryParameter = new QueryParameter(qp_page, qp_pagesize, qp_orderby, qp_includeallchildrendata, qp_includedata, qp_filter
                //                                                      , Url.Link(SystemStatics.Route_GetAll_Bid, null));
                ////CreateRepositoryInstance(User, _config, _mapper)
                //using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
                //{
                //    var theResult = await _repository_clientdb.CreatePagedResults<Bid, Bid>(theQueryParameter);
                //    return Ok(theResult);
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
            }
        }

        /// <summary>
        /// Create a new Bid Model
        /// </summary>
        /// <param name="model">Bid Model to be created</param>
        /// <returns>An ActionResult of newly created Bid Model</returns>
        /// <remarks>
        /// Sample request:
        ///     POST                         \
        ///     {                            \
        ///        "name": "Item1",          \
        ///        "code": "example code"    \
        ///     }                            
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the request/data is not valid</response>       
        /// <response code="500">If error happened at server side</response>
        /// <response code="422">If the provided data is not valid</response>               
        //[Authorize(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne)]
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
            Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        public async Task<ActionResult<Bid>> Post(Bid model)
        {
            try
            {
                BidRepository theRepository = new BidRepository();
                theRepository.Add(model);

                return Ok(model);

                //using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
                //{
                //    //Verify DB constraints
                //    if (!VerificationHelper.VerifyDBconstraints(_repository_clientdb, model, out string msg))
                //    {
                //        return BadRequest(msg);
                //    }

                //    // Name should be Unique
                //    //if (_repository_clientdb.GetExists<Bid>($"Name=='{model.Name}'"))
                //    //{
                //    //    return BadRequest($"Name [{model.Name}] is in Use;");
                //    //}

                //    // Create new 
                //    model.Id = Guid.Empty;
                //    Bid entity = await MappingHelper.StarndardMap<Bid, Bid>(_repository_clientdb, _mapper, model, Guid.Empty);

                //    //regular operation
                //    _repository_clientdb.Create(entity);
                //    if (!await _repository_clientdb.SaveChangesAsync(this.User))
                //    {
                //        return this.StatusCode(StatusCodes.Status500InternalServerError, null);
                //    }

                //    var newUri = Url.Link(SystemStatics.Route_GetOne_Bid, new { id = entity.Id });
                //    return Created(newUri, _mapper.Map<Bid>(entity));
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
            }
        }

        //   /// <summary>
        //   /// Get a list of Bid Model
        //   /// </summary>
        //   /// <param name="qp_page">Page number</param>
        //   /// <param name="qp_pagesize">Page size</param>
        //   /// <param name="qp_orderby">Order by</param>
        //   /// <param name="qp_includeallchildrendata">Whether get all children data</param>
        //   /// <param name="qp_includedata">Specific children data that need to be get; one level lower only; split by [,]</param>
        //   /// <param name="qp_filter">Filter condition</param>
        //   /// <returns>An ActionResult of PagedData for existing Bid Models</returns>
        ////   [Authorize(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne)]
        //   [HttpGet(Name = SystemStatics.Route_GetAll_Bid)]
        //   [ProducesResponseType(StatusCodes.Status200OK)]
        //   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //   public async Task<ActionResult<PagedData<Bid>>> Get(int qp_page = PFAPIStatics.SYS_Default_QP_Page
        //                                                           , int qp_pagesize = PFAPIStatics.SYS_Default_QP_Pagesize
        //                                                           , string qp_orderby = PFAPIStatics.SYS_Default_QP_Orderby
        //                                                           , bool qp_includeallchildrendata = PFAPIStatics.SYS_Default_QP_IncludeAllChildrenData
        //                                                           , string qp_includedata = PFAPIStatics.SYS_Default_QP_IncludeData
        //                                                           , string qp_filter = PFAPIStatics.SYS_Default_QP_Filter)
        //   {
        //       try
        //       {
        //           QueryParameter theQueryParameter = new QueryParameter(qp_page, qp_pagesize, qp_orderby, qp_includeallchildrendata, qp_includedata, qp_filter
        //                                                                 , Url.Link(SystemStatics.Route_GetAll_Bid, null));
        //           //CreateRepositoryInstance(User, _config, _mapper)
        //           using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
        //           {
        //               var theResult = await _repository_clientdb.CreatePagedResults<Bid, Bid>(theQueryParameter);
        //               return Ok(theResult);
        //           }
        //       }
        //       catch (Exception e)
        //       {
        //           _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
        //           return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //       }
        //   }

        //   /// <summary>
        //   /// Get one Bid Model
        //   /// </summary>
        //   /// <param name="id">Bid ID</param>
        //   /// <param name="qp_includeallchildrendata">Whether get all children data</param>
        //   /// <param name="qp_includedata">Specific children data that need to be get; one level lower only; split by [,]</param>
        //   /// <returns>An ActionResult of existing Bid Model</returns>
        //   [Authorize(Policy4ModuleOperations.P_AccountAccessLevel.AccessLevel_EveryOne)]
        //   [HttpGet("{id}", Name = SystemStatics.Route_GetOne_Bid)]
        //   //[Consumes("application/json")]
        //   [ProducesResponseType(StatusCodes.Status200OK)]
        //   [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //   [ProducesResponseType(StatusCodes.Status404NotFound)]

        //   public async Task<ActionResult<Bid>> Get(int id, bool qp_includeallchildrendata = PFAPIStatics.SYS_Default_QP_IncludeAllChildrenData
        //                                                           , string qp_includedata = PFAPIStatics.SYS_Default_QP_IncludeData)
        //   {
        //       try
        //       {
        //           QueryParameterMin theQueryParameter = new QueryParameterMin(qp_includeallchildrendata, qp_includedata);

        //           // work with Client Database
        //           using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
        //           {
        //               IQueryable<Bid> theQuery = _repository_clientdb.GetQueryable<Bid>();
        //               theQuery = theQuery.Where(o => o.Id == id);

        //               Bid theDataModel = await _repository_clientdb.GetOneDataModel<Bid, Bid>(theQueryParameter, theQuery);
        //               if (theDataModel == null)
        //                   return NotFound();

        //               return Ok(theDataModel);
        //           }
        //       }
        //       catch (Exception e)
        //       {
        //           _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
        //           return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //       }
        //   }




        ///// <summary>
        ///// Update a existing Bid Model
        ///// </summary>
        ///// <param name="id">Bid ID</param>
        ///// <param name="model">Bid Model to be updated</param>
        ///// <returns>An ActionResult of updated Bid Model</returns>
        //////[HttpPatch("{id}")]
        //[Authorize(Policy4ModuleOperations.P_Admin.AdminManageOperation)]
        //[HttpPut("{id}")]
        //[Consumes("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status304NotModified)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
        //    Type = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary))]
        //public async Task<ActionResult<Bid>> Put(Guid id, Bid model)
        //{
        //    try
        //    {
        //        // using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(User, _config, _mapper))
        //        using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
        //        {
        //            var existingentity = _repository_clientdb.GetById<Bid>(id);
        //            if (existingentity == null) return NotFound($"Could not find {_curDataModelName} with id of {id}");

        //            if (model.Id != id)
        //                model.Id = id;

        //            //// Name should be Unique
        //            //if (_repository_clientdb.GetExists<Bid>($"Name=='{model.Name}' and ID != GUID('{model.Id}')"))
        //            //{
        //            //    return BadRequest($"Name [{model.Name}] is in Use;");
        //            //}

        //            //Verify DB constraints
        //            if (!VerificationHelper.VerifyDBconstraints(_repository_clientdb, model, out string msg))
        //            {
        //                return BadRequest(msg);
        //            }

        //            existingentity = await MappingHelper.StarndardMap<Bid, Bid>(_repository_clientdb, _mapper, model, id);
        //            if (existingentity == null) return BadRequest($"Failed to map {_curDataModelName} with id of {id}");

        //            if (!_repository_clientdb.HasChangesToDB())
        //            {
        //                if (_config["ValidationCheckOptions:Apply200ForNotModifiedRecord"].MeaningTrue())
        //                {
        //                    return _mapper.Map<Bid>(existingentity);
        //                }
        //                else
        //                {
        //                    return this.StatusCode(StatusCodes.Status304NotModified);
        //                }
        //            }
        //            if (await _repository_clientdb.SaveChangesAsync(this.User))
        //            {
        //                return Ok(_mapper.Map<Bid>(existingentity));
        //            }
        //            return StatusCode(StatusCodes.Status304NotModified, model);
        //        }
        //    }
        //    catch (FormatException e)
        //    {
        //        _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
        //        return BadRequest(e.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //    }
        //}

        ///// <summary>
        ///// Delete a existing Bid Model
        ///// </summary>
        ///// <param name="id">Bid ID</param>
        ///// <returns>An ActionResult</returns>
        //[Authorize(Policy4ModuleOperations.P_Admin.AdminManageOperation)]
        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    try
        //    {
        //        // using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(User, _config, _mapper))
        //        using (IPFClientRepository _repository_clientdb = PFClientRepository.CreateRepositoryInstance(_config, _mapper, Guid.Empty))
        //        {
        //            var existingentity = _repository_clientdb.GetById<Bid>(id);
        //            if (existingentity == null) return NotFound($"Could not find {_curDataModelName} with id of {id}");

        //            //delete schedule control
        //            _repository_clientdb.Delete(existingentity);
        //            if (!await _repository_clientdb.SaveChangesAsync(this.User))
        //            {
        //                _logger.LogWarning($"Failed to delete {_curDataModelName} with id of {id}");
        //                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Failed to delete {_curDataModelName} with id of {id}");
        //            }

        //            return Ok();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(LoggingEvents.Other, e, "Exception thrown;", this.User);
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, $"{e.toExceptionString()}");
        //    }
        //}
    }
}
