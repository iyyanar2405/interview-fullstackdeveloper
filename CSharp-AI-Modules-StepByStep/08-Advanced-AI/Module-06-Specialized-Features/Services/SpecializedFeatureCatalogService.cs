using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Module_06_Specialized_Features.Models;

namespace Module_06_Specialized_Features.Services;

public sealed class SpecializedFeatureCatalogService
{
    private readonly ConcurrentDictionary<string, CodeTemplateDefinition> _codeTemplates = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DataAnalysisWorkflowDefinition> _analysisWorkflows = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ContentStyleDefinition> _contentStyles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DocumentPipelineDefinition> _documentPipelines = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ImageModelDefinition> _imageModels = new(StringComparer.OrdinalIgnoreCase);

    public SpecializedFeatureCatalogService(IOptions<SpecializedFeaturesOptions> options)
    {
        foreach (var template in options.Value.CodeTemplates)
        {
            _codeTemplates[template.TemplateId] = Clone(template);
        }

        foreach (var workflow in options.Value.AnalysisWorkflows)
        {
            _analysisWorkflows[workflow.WorkflowId] = Clone(workflow);
        }

        foreach (var style in options.Value.ContentStyles)
        {
            _contentStyles[style.StyleId] = Clone(style);
        }

        foreach (var pipeline in options.Value.DocumentPipelines)
        {
            _documentPipelines[pipeline.PipelineId] = Clone(pipeline);
        }

        foreach (var model in options.Value.ImageModels)
        {
            _imageModels[model.ModelId] = Clone(model);
        }
    }

    public IReadOnlyCollection<CodeTemplateDefinition> GetCodeTemplates()
    {
        return _codeTemplates.Values.Select(Clone).ToArray();
    }

    public CodeTemplateDefinition? GetCodeTemplate(string templateId)
    {
        return _codeTemplates.TryGetValue(templateId, out var template) ? Clone(template) : null;
    }

    public IReadOnlyCollection<DataAnalysisWorkflowDefinition> GetAnalysisWorkflows()
    {
        return _analysisWorkflows.Values.Select(Clone).ToArray();
    }

    public DataAnalysisWorkflowDefinition? GetAnalysisWorkflow(string workflowId)
    {
        return _analysisWorkflows.TryGetValue(workflowId, out var workflow) ? Clone(workflow) : null;
    }

    public IReadOnlyCollection<ContentStyleDefinition> GetContentStyles()
    {
        return _contentStyles.Values.Select(Clone).ToArray();
    }

    public ContentStyleDefinition? GetContentStyle(string styleId)
    {
        return _contentStyles.TryGetValue(styleId, out var style) ? Clone(style) : null;
    }

    public IReadOnlyCollection<DocumentPipelineDefinition> GetDocumentPipelines()
    {
        return _documentPipelines.Values.Select(Clone).ToArray();
    }

    public DocumentPipelineDefinition? GetDocumentPipeline(string pipelineId)
    {
        return _documentPipelines.TryGetValue(pipelineId, out var pipeline) ? Clone(pipeline) : null;
    }

    public IReadOnlyCollection<ImageModelDefinition> GetImageModels()
    {
        return _imageModels.Values.Select(Clone).ToArray();
    }

    public ImageModelDefinition? GetImageModel(string modelId)
    {
        return _imageModels.TryGetValue(modelId, out var model) ? Clone(model) : null;
    }

    private static CodeTemplateDefinition Clone(CodeTemplateDefinition template)
    {
        return new CodeTemplateDefinition
        {
            TemplateId = template.TemplateId,
            Name = template.Name,
            Language = template.Language,
            Description = template.Description,
            Scaffolding = template.Scaffolding.ToList(),
            TestHints = template.TestHints.ToList(),
            QualityChecks = template.QualityChecks.ToList()
        };
    }

    private static DataAnalysisWorkflowDefinition Clone(DataAnalysisWorkflowDefinition definition)
    {
        return new DataAnalysisWorkflowDefinition
        {
            WorkflowId = definition.WorkflowId,
            Name = definition.Name,
            Dataset = definition.Dataset,
            Objective = definition.Objective,
            Steps = definition.Steps.ToList(),
            Metrics = definition.Metrics.ToList()
        };
    }

    private static ContentStyleDefinition Clone(ContentStyleDefinition style)
    {
        return new ContentStyleDefinition
        {
            StyleId = style.StyleId,
            Name = style.Name,
            Tone = style.Tone,
            Guidelines = style.Guidelines.ToList(),
            SampleOpeners = style.SampleOpeners.ToList()
        };
    }

    private static DocumentPipelineDefinition Clone(DocumentPipelineDefinition pipeline)
    {
        return new DocumentPipelineDefinition
        {
            PipelineId = pipeline.PipelineId,
            Name = pipeline.Name,
            Description = pipeline.Description,
            Stages = pipeline.Stages.ToList(),
            OutputArtifacts = pipeline.OutputArtifacts.ToList()
        };
    }

    private static ImageModelDefinition Clone(ImageModelDefinition model)
    {
        return new ImageModelDefinition
        {
            ModelId = model.ModelId,
            Name = model.Name,
            Modalities = model.Modalities.ToList(),
            Capabilities = model.Capabilities.ToList(),
            SamplePrompts = model.SamplePrompts.ToList()
        };
    }
}
