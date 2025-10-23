using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Module_04_Scaling_Strategies.Models;
using Module_04_Scaling_Strategies.Services;

namespace Module_04_Scaling_Strategies.Controllers;

[ApiController]
[Route("api/scaling")]
public sealed class ScalingController : ControllerBase
{
    private readonly ScalingProfileService _profiles;
    private readonly ScalingRecommendationService _recommendations;
    private readonly CapacityForecastService _forecast;

    public ScalingController(
        ScalingProfileService profiles,
        ScalingRecommendationService recommendations,
        CapacityForecastService forecast)
    {
        _profiles = profiles;
        _recommendations = recommendations;
        _forecast = forecast;
    }

    [HttpGet("profiles")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ScalingProfile>), StatusCodes.Status200OK)]
    public IActionResult GetProfiles()
    {
        return Ok(_profiles.GetAll());
    }

    [HttpGet("profiles/{name}")]
    [ProducesResponseType(typeof(ScalingProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetProfile([FromRoute] string name)
    {
        var profile = _profiles.Get(name);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPost("profiles")]
    [ProducesResponseType(typeof(ScalingProfile), StatusCodes.Status201Created)]
    public IActionResult UpsertProfile([FromBody] ScalingProfile profile)
    {
        var saved = _profiles.Upsert(profile);
        return CreatedAtAction(nameof(GetProfile), new { name = saved.Name }, saved);
    }

    [HttpDelete("profiles/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteProfile([FromRoute] string name)
    {
        _profiles.Delete(name);
        return NoContent();
    }

    [HttpPost("plan")]
    [ProducesResponseType(typeof(ScalingPlan), StatusCodes.Status200OK)]
    public IActionResult BuildPlan([FromBody] ScalingPlanRequest request)
    {
        request ??= new ScalingPlanRequest();
        request.Snapshot ??= new WorkloadSnapshot();
        var plan = _recommendations.BuildPlan(request);
        return Ok(plan);
    }

    [HttpPost("forecast")]
    [ProducesResponseType(typeof(CapacityForecast), StatusCodes.Status200OK)]
    public IActionResult Forecast([FromBody] ScalingPlanRequest request)
    {
        request ??= new ScalingPlanRequest();
        request.Snapshot ??= new WorkloadSnapshot();
        var profile = string.IsNullOrWhiteSpace(request.ProfileName)
            ? _profiles.GetAll().First()
            : _profiles.Get(request.ProfileName) ?? _profiles.GetAll().First();

        var forecast = _forecast.BuildForecast(profile, request.Snapshot, request.Scenario, request.ForecastHorizonMinutes);
        return Ok(forecast);
    }
}
