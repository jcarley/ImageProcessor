using Domain.Commands;
using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Api.Contributions;

[Route("api/[controller]")]
[ApiController]
public class ContributionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContributionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Contribution
    [HttpGet]
    public IEnumerable<string> Get()
    {
        Contribution contribution = new()
        {
            Name = "Sample",
            ImageType = "application/jpeg",
            Exif = new ExifData
            {
                Device = "Canon",
                FNumber = "f/4.6",
                Exposure = "1/180",
                Dimensions = "1656x1242",
                Kind = "JPEG Image",
            },
        };

        return new[] { "value1", "value2" };
    }

    // GET: api/Contribution/5
    [HttpGet("{id}", Name = "GetContribution")]
    public async Task<ActionResult<GetContributionQuery>> GetContribution(Guid id)
    {
        try
        {
            GetContributionQuery criteria = new() { Id = id };
            GetContributionResult contributionResult = await _mediator.Send(criteria);

            if (!contributionResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, string.Join("", contributionResult.Errors));
            }

            return StatusCode(StatusCodes.Status200OK, contributionResult.Data);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    // POST: api/Contribution
    [HttpPost]
    public async Task<ActionResult<GetContributionQuery>> Post([FromBody] AddContributionCommand contributionCommand)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Invalid request");
        }

        try
        {
            AddContributionResult addContributionResult = await _mediator.Send(contributionCommand);
            if (!addContributionResult.Succeeded)
            {
                return BadRequest(string.Join(" ", addContributionResult.Errors));
            }

            Contribution? contribution = addContributionResult.Data;
            if (contribution == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Missing contribution");
            }

            // map to return entity; this could use AutoMapper
            GetContributionQuery getContributionQuery = new()
            {
                Id = contribution.Id,
                Name = contribution.Name,
                Size = contribution.Size,
                ImageType = contribution.ImageType,
                ThumbnailUrl = contribution.ThumbnailUrl,
                HiResUrl = contribution.HiResUrl,
                SampleHiResUrl = contribution.SampleHiResUrl,
            };

            return CreatedAtRoute(
                nameof(GetContribution),
                new { contribution.Id },
                getContributionQuery
            );
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    // PUT: api/Contribution/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE: api/Contribution/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}