using Module_06_Dashboards_Reporting.Models;

namespace Module_06_Dashboards_Reporting.Services;

public sealed class ExecutiveSummaryService
{
	public ExecutiveSummary Build(ReportDocument document)
	{
		var strengths = document.Highlights
			.Where(kpi => kpi.Delta < 0)
			.Select(kpi => $"{kpi.Name} improved by {Math.Abs(kpi.Delta):F2}.")
			.ToList();

		var risks = document.Highlights
			.Where(kpi => kpi.Delta > 0)
			.Select(kpi => $"{kpi.Name} increased by {kpi.Delta:F2}.")
			.ToList();

		if (strengths.Count == 0)
		{
			strengths.Add("No improving metrics detected in the window.");
		}

		if (risks.Count == 0)
		{
			risks.Add("No degrading metrics detected in the window.");
		}

		var opportunities = document.Sections
			.Where(section => section.Table.Count > 0)
			.Select(section => $"Investigate {section.Title} segments with highest variance.")
			.Distinct()
			.ToList();

		if (opportunities.Count == 0)
		{
			opportunities.Add("Leverage dashboard filters to uncover optimization opportunities.");
		}

		return new ExecutiveSummary
		{
			Overview = $"Report covers {document.Sections.Count} metrics with {document.Highlights.Count} KPI callouts.",
			Strengths = strengths,
			Risks = risks,
			Opportunities = opportunities,
			NextActions = "Prioritize mitigating high-risk KPIs and track improvements on the default dashboard."
		};
	}
}
