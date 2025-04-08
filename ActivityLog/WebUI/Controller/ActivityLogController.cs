using IJOS.Application.ActivityLog.Commands;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.ActivityLog.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IJOS.WebUI.Controllers
{
    public class ActivityLogController : ApiControllerBase
    {
        [HttpGet("{sin}")]
        public async Task<ActionResult<ActivityLogVm>> GetAll(long sin)
        {
            var offadd = await Mediator.Send(new ActivityLogQuery() { sin = sin });
            return offadd;
        }

        [HttpGet("Area")]
        public async Task<ActionResult<List<ActivityLogArea>>> GetActivityLogArea()
        {
            return await Mediator.Send(new ActivityLogAreaQuery());
        }

        [HttpGet("Cbo")]
        public async Task<ActionResult<List<ActivityLogCboDatadto>>> GetActivityLogCBO()
        {
            return await Mediator.Send(new ActivityLogCboDataQuery());
        }

        [HttpGet("DeleteLimit")]
        public async Task<ActionResult<ActivityLogVm>> GetActivityLogDELETELIMIT()
        {
            return await Mediator.Send(new ActivityLogDelLimitQuery());
        }

        [HttpGet("GroupCounts")]
        public async Task<ActionResult<ActivityLogVm>> GetActivityLogGROUPCOUNTS()
        {
            return await Mediator.Send(new ActivityLogGroupsCountsQuery());
        }

        [HttpGet("Radios")]
        public async Task<ActionResult<List<ActivityLogRadios>>> GetActivityLogRADIOS()
        {
            return await Mediator.Send(new ActivityLogRadiosQuery());
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateActivityLogCommand command)
        {
            if (command.activityLogDto == null)
            {
                return BadRequest();
            }
            else
            {
                await Mediator.Send(command);
                return NoContent();
            }
        }

        [HttpDelete("id")]
        public async Task<ActionResult> Delete(long id)
        {
            await Mediator.Send(new DeleteActivityLogCommand() { log_Id = id });
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateActivityLogCommand command)
        {
            if (command.activityLogDto == null)
            {
                return BadRequest();
            }
            await Mediator.Send(command);
            return NoContent();
        }

        public async Task<ActionResult<ActivityLogSearchVM>> Search(string str)
        {
            try
            {
                var offadd = await Mediator.Send(new ActivityLogSearchQuery() { searchString = str });
                return offadd;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
                throw;
            }
        }      
    }
}