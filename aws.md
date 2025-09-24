# CI/CD Complete Guide: Azure DevOps & AWS
*Comprehensive Step-by-Step Guide with Real-World Examples*

## Table of Contents
1. [Azure DevOps CI/CD Pipeline](#azure-devops-cicd-pipeline)
2. [AWS CI/CD Pipeline](#aws-cicd-pipeline)
3. [Repository Management](#repository-management)
4. [YAML Pipeline Examples](#yaml-pipeline-examples)
5. [Best Practices](#best-practices)
6. [Troubleshooting](#troubleshooting)

---

## Azure DevOps CI/CD Pipeline

### Prerequisites
- Azure DevOps account
- Azure subscription
- Visual Studio/VS Code
- Git installed

### Step 1: Create Azure DevOps Project

```bash
# Login to Azure DevOps
# Go to https://dev.azure.com
# Click "New Project"
```

**Project Configuration:**
```yaml
Project Name: "MyWebApp-CICD"
Description: "Full Stack Web Application with CI/CD"
Visibility: Private
Version Control: Git
Work Item Process: Agile
```

### Step 2: Create Repository and Push Code

#### Initialize Local Repository
```bash
# Create new directory
mkdir my-webapp-cicd
cd my-webapp-cicd

# Initialize Git repository
git init

# Add Azure DevOps remote
git remote add origin https://dev.azure.com/YourOrg/MyWebApp-CICD/_git/MyWebApp-CICD

# Create initial files
touch README.md
echo "# My Web App CI/CD Project" > README.md
```

#### Create Sample .NET Core Application
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

```xml
<!-- MyWebApp.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
  </ItemGroup>

</Project>
```

#### Push Initial Code
```bash
# Stage all files
git add .

# Commit changes
git commit -m "Initial commit: .NET Core Web API project setup"

# Push to Azure DevOps
git push -u origin main
```

### Step 3: Create Pull Request Workflow

#### Create Feature Branch
```bash
# Create and switch to feature branch
git checkout -b feature/add-user-controller

# Make changes (add new controller)
# Controllers/UsersController.cs
```

```csharp
// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> Users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        user.Id = Users.Max(u => u.Id) + 1;
        Users.Add(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

#### Push Feature and Create Pull Request
```bash
# Commit feature changes
git add .
git commit -m "Add Users controller with CRUD operations"

# Push feature branch
git push origin feature/add-user-controller
```

**Create Pull Request in Azure DevOps:**
1. Go to Azure DevOps → Repos → Pull Requests
2. Click "New Pull Request"
3. Source: `feature/add-user-controller`
4. Target: `main`
5. Add reviewers and work items
6. Set completion options (delete source branch, squash merge)

### Step 4: Create Build Pipeline (CI)

#### Azure Pipeline YAML (azure-pipelines.yml)
```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
    - main
    - develop
    - feature/*

pr:
  branches:
    include:
    - main
    - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotNetFramework: 'net8.0'
  dotNetVersion: '8.0.x'
  projectPath: '**/*.csproj'

stages:
- stage: Build
  displayName: 'Build and Test'
  jobs:
  - job: Build
    displayName: 'Build Job'
    steps:
    
    # Install .NET Core SDK
    - task: UseDotNet@2
      displayName: 'Install .NET Core SDK'
      inputs:
        version: $(dotNetVersion)
        includePreviewVersions: false
    
    # Restore NuGet packages
    - task: DotNetCoreCLI@2
      displayName: 'Restore NuGet Packages'
      inputs:
        command: 'restore'
        projects: $(projectPath)
        feedsToUse: 'select'
    
    # Build the application
    - task: DotNetCoreCLI@2
      displayName: 'Build Application'
      inputs:
        command: 'build'
        projects: $(projectPath)
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    # Run unit tests
    - task: DotNetCoreCLI@2
      displayName: 'Run Unit Tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --no-build --collect:"XPlat Code Coverage"'
        publishTestResults: true
    
    # Publish code coverage results
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish Code Coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    
    # Publish the application
    - task: DotNetCoreCLI@2
      displayName: 'Publish Application'
      inputs:
        command: 'publish'
        projects: $(projectPath)
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/app'
        publishWebProjects: true
        zipAfterPublish: true
    
    # Publish build artifacts
    - task: PublishBuildArtifacts@1
      displayName: 'Publish Build Artifacts'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'
        publishLocation: 'Container'

- stage: SecurityScan
  displayName: 'Security Scanning'
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: SecurityScan
    displayName: 'Security Scan Job'
    steps:
    
    # OWASP Dependency Check
    - task: dependency-check-build-task@6
      displayName: 'OWASP Dependency Check'
      inputs:
        projectName: 'MyWebApp'
        scanPath: '$(Build.SourcesDirectory)'
        format: 'ALL'
        additionalArguments: '--enableRetired --enableExperimental'
    
    # Publish security scan results
    - task: PublishTestResults@2
      displayName: 'Publish Security Scan Results'
      inputs:
        testResultsFormat: 'JUnit'
        testResultsFiles: 'dependency-check-junit.xml'
        searchFolder: '$(Common.TestResultsDirectory)'
```

### Step 5: Create Release Pipeline (CD)

#### Multi-Stage Release YAML
```yaml
# azure-release-pipeline.yml
trigger: none # Manual trigger for releases

pr: none

pool:
  vmImage: 'ubuntu-latest'

variables:
- group: 'Production-Variables'
- name: 'azureSubscription'
  value: 'YourAzureSubscription'

stages:
- stage: DeployToDev
  displayName: 'Deploy to Development'
  condition: succeeded()
  jobs:
  - deployment: DeployDev
    displayName: 'Deploy to Dev Environment'
    environment: 'Development'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureWebApp@1
            displayName: 'Deploy to Azure Web App - Dev'
            inputs:
              azureSubscription: $(azureSubscription)
              appType: 'webApp'
              appName: 'mywebapp-dev'
              package: '$(Pipeline.Workspace)/drop/app/*.zip'
              deploymentMethod: 'auto'
          
          - task: AzureAppServiceManage@0
            displayName: 'Start Azure Web App - Dev'
            inputs:
              azureSubscription: $(azureSubscription)
              Action: 'Start Azure App Service'
              WebAppName: 'mywebapp-dev'
          
          # Health check
          - task: PowerShell@2
            displayName: 'Health Check - Dev'
            inputs:
              targetType: 'inline'
              script: |
                $response = Invoke-WebRequest -Uri "https://mywebapp-dev.azurewebsites.net/health" -UseBasicParsing
                if ($response.StatusCode -ne 200) {
                  Write-Error "Health check failed"
                  exit 1
                }
                Write-Host "Health check passed"

- stage: DeployToStaging
  displayName: 'Deploy to Staging'
  dependsOn: DeployToDev
  condition: succeeded()
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging Environment'
    environment: 'Staging'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureWebApp@1
            displayName: 'Deploy to Azure Web App - Staging'
            inputs:
              azureSubscription: $(azureSubscription)
              appType: 'webApp'
              appName: 'mywebapp-staging'
              package: '$(Pipeline.Workspace)/drop/app/*.zip'
              deploymentMethod: 'auto'
              
          # Run integration tests
          - task: DotNetCoreCLI@2
            displayName: 'Run Integration Tests'
            inputs:
              command: 'test'
              projects: '**/*IntegrationTests.csproj'
              arguments: '--configuration Release'

- stage: DeployToProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployToStaging
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production Environment'
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          # Blue-Green Deployment
          - task: AzureWebApp@1
            displayName: 'Deploy to Production Slot'
            inputs:
              azureSubscription: $(azureSubscription)
              appType: 'webApp'
              appName: 'mywebapp-prod'
              package: '$(Pipeline.Workspace)/drop/app/*.zip'
              deployToSlotOrASE: true
              resourceGroupName: 'rg-mywebapp-prod'
              slotName: 'staging'
          
          # Smoke tests on staging slot
          - task: PowerShell@2
            displayName: 'Smoke Tests - Production Staging Slot'
            inputs:
              targetType: 'inline'
              script: |
                # Wait for deployment to settle
                Start-Sleep -Seconds 30
                
                # Test critical endpoints
                $baseUrl = "https://mywebapp-prod-staging.azurewebsites.net"
                $endpoints = @("/health", "/api/users")
                
                foreach ($endpoint in $endpoints) {
                  try {
                    $response = Invoke-WebRequest -Uri "$baseUrl$endpoint" -UseBasicParsing -TimeoutSec 30
                    Write-Host "✓ $endpoint - Status: $($response.StatusCode)"
                  }
                  catch {
                    Write-Error "✗ $endpoint failed: $($_.Exception.Message)"
                    exit 1
                  }
                }
          
          # Swap slots (Blue-Green deployment)
          - task: AzureAppServiceManage@0
            displayName: 'Swap Production Slots'
            inputs:
              azureSubscription: $(azureSubscription)
              Action: 'Swap Slots'
              WebAppName: 'mywebapp-prod'
              ResourceGroupName: 'rg-mywebapp-prod'
              SourceSlot: 'staging'
              TargetSlot: 'production'
```

---

## AWS CI/CD Pipeline

### Step 1: Setup AWS Environment

#### Install AWS CLI
```bash
# Install AWS CLI
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip
sudo ./aws/install

# Configure AWS CLI
aws configure
# AWS Access Key ID: YOUR_ACCESS_KEY
# AWS Secret Access Key: YOUR_SECRET_KEY
# Default region name: us-east-1
# Default output format: json
```

#### Create S3 Bucket for Artifacts
```bash
# Create S3 bucket for build artifacts
aws s3 mb s3://mywebapp-cicd-artifacts-$(date +%s)

# Enable versioning
aws s3api put-bucket-versioning \
  --bucket mywebapp-cicd-artifacts-$(date +%s) \
  --versioning-configuration Status=Enabled
```

### Step 2: Setup CodeCommit Repository

#### Create CodeCommit Repository
```bash
# Create CodeCommit repository
aws codecommit create-repository \
  --repository-name mywebapp-cicd \
  --repository-description "My Web App CI/CD Repository"

# Clone the repository
git clone https://git-codecommit.us-east-1.amazonaws.com/v1/repos/mywebapp-cicd
cd mywebapp-cicd
```

#### Configure Git for CodeCommit
```bash
# Configure Git credentials helper
git config credential.helper '!aws codecommit credential-helper $@'
git config credential.UseHttpPath true

# Add your code and push
git add .
git commit -m "Initial commit: .NET Core Web API"
git push origin main
```

### Step 3: Create CodeBuild Project

#### buildspec.yml for CodeBuild
```yaml
# buildspec.yml
version: 0.2

env:
  variables:
    DOTNET_ROOT: /usr/share/dotnet
    DOTNET_CLI_TELEMETRY_OPTOUT: 1

phases:
  install:
    runtime-versions:
      dotnet: 8.0
    commands:
      - echo "Installing dependencies..."
      - apt-get update
      - apt-get install -y jq
      
  pre_build:
    commands:
      - echo "Pre-build phase started on `date`"
      - echo "Restoring NuGet packages..."
      - dotnet restore
      
  build:
    commands:
      - echo "Build phase started on `date`"
      - echo "Building the application..."
      - dotnet build --configuration Release --no-restore
      
      - echo "Running unit tests..."
      - dotnet test --configuration Release --no-build --logger trx --collect:"XPlat Code Coverage"
      
      - echo "Publishing the application..."
      - dotnet publish --configuration Release --no-build --output ./publish
      
      - echo "Creating deployment package..."
      - cd publish
      - zip -r ../mywebapp-$(date +%Y%m%d-%H%M%S).zip .
      - cd ..
      
  post_build:
    commands:
      - echo "Post-build phase completed on `date`"
      - echo "Uploading artifacts to S3..."
      - aws s3 cp mywebapp-*.zip s3://$ARTIFACTS_BUCKET/builds/

artifacts:
  files:
    - mywebapp-*.zip
  base-directory: .
  
cache:
  paths:
    - '/root/.nuget/packages/**/*'

reports:
  unit-tests:
    files:
      - '**/*.trx'
    file-format: 'VSTS'
  code-coverage:
    files:
      - '**/coverage.cobertura.xml'
    file-format: 'COBERTURA'
```

#### Create CodeBuild Project with CLI
```bash
# Create CodeBuild project
cat > codebuild-project.json << EOF
{
  "name": "mywebapp-build",
  "description": "Build project for My Web App",
  "source": {
    "type": "CODECOMMIT",
    "location": "https://git-codecommit.us-east-1.amazonaws.com/v1/repos/mywebapp-cicd",
    "buildspec": "buildspec.yml"
  },
  "artifacts": {
    "type": "S3",
    "location": "mywebapp-cicd-artifacts/builds"
  },
  "environment": {
    "type": "LINUX_CONTAINER",
    "image": "aws/codebuild/amazonlinux2-x86_64-standard:5.0",
    "computeType": "BUILD_GENERAL1_MEDIUM",
    "environmentVariables": [
      {
        "name": "ARTIFACTS_BUCKET",
        "value": "mywebapp-cicd-artifacts"
      }
    ]
  },
  "serviceRole": "arn:aws:iam::YOUR_ACCOUNT:role/CodeBuildServiceRole"
}
EOF

aws codebuild create-project --cli-input-json file://codebuild-project.json
```

### Step 4: Create CodePipeline

#### CloudFormation Template for Complete Pipeline
```yaml
# pipeline-template.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Complete CI/CD Pipeline for Web Application'

Parameters:
  ApplicationName:
    Type: String
    Default: 'mywebapp'
    Description: 'Name of the application'
  
  GitHubRepoName:
    Type: String
    Default: 'mywebapp-cicd'
    Description: 'GitHub repository name'
  
  GitHubOwner:
    Type: String
    Description: 'GitHub repository owner'
  
  GitHubToken:
    Type: String
    NoEcho: true
    Description: 'GitHub personal access token'

Resources:
  # S3 Bucket for Pipeline Artifacts
  ArtifactsBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub '${ApplicationName}-pipeline-artifacts-${AWS::AccountId}'
      VersioningConfiguration:
        Status: Enabled
      BucketEncryption:
        ServerSideEncryptionConfiguration:
          - ServerSideEncryptionByDefault:
              SSEAlgorithm: AES256

  # CodeBuild Project
  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub '${ApplicationName}-build'
      Description: !Sub 'Build project for ${ApplicationName}'
      ServiceRole: !GetAtt CodeBuildRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_MEDIUM
        Image: aws/codebuild/amazonlinux2-x86_64-standard:5.0
        EnvironmentVariables:
          - Name: ARTIFACTS_BUCKET
            Value: !Ref ArtifactsBucket
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
          - Name: AWS_ACCOUNT_ID
            Value: !Ref AWS::AccountId
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          phases:
            install:
              runtime-versions:
                dotnet: 8.0
              commands:
                - echo "Installing dependencies..."
            pre_build:
              commands:
                - echo "Restoring packages..."
                - dotnet restore
            build:
              commands:
                - echo "Building application..."
                - dotnet build --configuration Release
                - dotnet test --configuration Release --logger trx
                - dotnet publish --configuration Release --output ./publish
            post_build:
              commands:
                - echo "Build completed"
          artifacts:
            files:
              - '**/*'
            base-directory: publish

  # CodeDeploy Application
  DeployApplication:
    Type: AWS::CodeDeploy::Application
    Properties:
      ApplicationName: !Sub '${ApplicationName}-deploy'
      ComputePlatform: Server

  # CodeDeploy Deployment Group
  DeploymentGroup:
    Type: AWS::CodeDeploy::DeploymentGroup
    Properties:
      ApplicationName: !Ref DeployApplication
      DeploymentGroupName: !Sub '${ApplicationName}-deployment-group'
      ServiceRoleArn: !GetAtt CodeDeployRole.Arn
      Ec2TagFilters:
        - Type: KEY_AND_VALUE
          Key: Environment
          Value: Production
      AutoRollbackConfiguration:
        Enabled: true
        Events:
          - DEPLOYMENT_FAILURE
          - DEPLOYMENT_STOP_ON_ALARM

  # CodePipeline
  Pipeline:
    Type: AWS::CodePipeline::Pipeline
    Properties:
      Name: !Sub '${ApplicationName}-pipeline'
      RoleArn: !GetAtt CodePipelineRole.Arn
      ArtifactStore:
        Type: S3
        Location: !Ref ArtifactsBucket
      Stages:
        - Name: Source
          Actions:
            - Name: SourceAction
              ActionTypeId:
                Category: Source
                Owner: ThirdParty
                Provider: GitHub
                Version: 1
              Configuration:
                Owner: !Ref GitHubOwner
                Repo: !Ref GitHubRepoName
                Branch: main
                OAuthToken: !Ref GitHubToken
              OutputArtifacts:
                - Name: SourceOutput

        - Name: Build
          Actions:
            - Name: BuildAction
              ActionTypeId:
                Category: Build
                Owner: AWS
                Provider: CodeBuild
                Version: 1
              Configuration:
                ProjectName: !Ref BuildProject
              InputArtifacts:
                - Name: SourceOutput
              OutputArtifacts:
                - Name: BuildOutput

        - Name: DeployToDev
          Actions:
            - Name: DeployToDevAction
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CodeDeploy
                Version: 1
              Configuration:
                ApplicationName: !Ref DeployApplication
                DeploymentGroupName: !Ref DeploymentGroup
              InputArtifacts:
                - Name: BuildOutput
              Region: !Ref AWS::Region

        - Name: ApprovalForProduction
          Actions:
            - Name: ManualApproval
              ActionTypeId:
                Category: Approval
                Owner: AWS
                Provider: Manual
                Version: 1
              Configuration:
                CustomData: 'Please review and approve deployment to production'

        - Name: DeployToProduction
          Actions:
            - Name: DeployToProdAction
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CodeDeploy
                Version: 1
              Configuration:
                ApplicationName: !Ref DeployApplication
                DeploymentGroupName: !Ref DeploymentGroup
              InputArtifacts:
                - Name: BuildOutput
              Region: !Ref AWS::Region

  # IAM Roles
  CodePipelineRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: PipelineExecutionPolicy
          PolicyDocument:
            Version: '2012-10-17'
            Statement:
              - Effect: Allow
                Action:
                  - s3:GetBucketVersioning
                  - s3:GetObject
                  - s3:GetObjectVersion
                  - s3:PutObject
                  - s3:GetBucketLocation
                  - s3:ListBucket
                Resource:
                  - !Sub '${ArtifactsBucket}'
                  - !Sub '${ArtifactsBucket}/*'
              - Effect: Allow
                Action:
                  - codebuild:BatchGetBuilds
                  - codebuild:StartBuild
                Resource: !GetAtt BuildProject.Arn
              - Effect: Allow
                Action:
                  - codedeploy:CreateDeployment
                  - codedeploy:GetApplication
                  - codedeploy:GetApplicationRevision
                  - codedeploy:GetDeployment
                  - codedeploy:GetDeploymentConfig
                  - codedeploy:RegisterApplicationRevision
                Resource: '*'

  CodeBuildRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codebuild.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: BuildExecutionPolicy
          PolicyDocument:
            Version: '2012-10-17'
            Statement:
              - Effect: Allow
                Action:
                  - logs:CreateLogGroup
                  - logs:CreateLogStream
                  - logs:PutLogEvents
                Resource: !Sub 'arn:aws:logs:${AWS::Region}:${AWS::AccountId}:log-group:/aws/codebuild/*'
              - Effect: Allow
                Action:
                  - s3:GetObject
                  - s3:GetObjectVersion
                  - s3:PutObject
                Resource:
                  - !Sub '${ArtifactsBucket}/*'

  CodeDeployRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codedeploy.amazonaws.com
            Action: sts:AssumeRole
      ManagedPolicyArns:
        - arn:aws:iam::aws:policy/service-role/AWSCodeDeployRole

Outputs:
  PipelineName:
    Description: 'Name of the created pipeline'
    Value: !Ref Pipeline
    Export:
      Name: !Sub '${AWS::StackName}-PipelineName'
  
  ArtifactsBucketName:
    Description: 'Name of the artifacts bucket'
    Value: !Ref ArtifactsBucket
    Export:
      Name: !Sub '${AWS::StackName}-ArtifactsBucket'
```

#### Deploy the Pipeline
```bash
# Deploy the CloudFormation stack
aws cloudformation create-stack \
  --stack-name mywebapp-cicd-pipeline \
  --template-body file://pipeline-template.yml \
  --parameters ParameterKey=GitHubOwner,ParameterValue=YourGitHubUsername \
               ParameterKey=GitHubToken,ParameterValue=YourGitHubToken \
  --capabilities CAPABILITY_IAM

# Wait for stack creation
aws cloudformation wait stack-create-complete \
  --stack-name mywebapp-cicd-pipeline
```

---

## YAML Pipeline Examples

### Advanced Azure DevOps YAML with Multiple Environments

```yaml
# azure-pipelines-advanced.yml
name: $(Date:yyyyMMdd)$(Rev:.r)

trigger:
  branches:
    include:
    - main
    - develop
    - release/*
  paths:
    exclude:
    - README.md
    - docs/*

pr:
  branches:
    include:
    - main
    - develop
  paths:
    exclude:
    - README.md
    - docs/*

variables:
- group: 'Global-Variables'
- group: 'Security-Variables'
- name: 'buildConfiguration'
  value: 'Release'
- name: 'vmImageName'
  value: 'ubuntu-latest'

pool:
  vmImage: $(vmImageName)

stages:
- stage: ValidateAndBuild
  displayName: 'Validate and Build'
  jobs:
  - job: ValidateCode
    displayName: 'Code Validation'
    steps:
    - task: UseDotNet@2
      displayName: 'Install .NET SDK'
      inputs:
        version: '8.0.x'
    
    - script: |
        echo "Running code validation..."
        dotnet format --verify-no-changes --verbosity diagnostic
        echo "Code formatting validation passed"
      displayName: 'Code Format Validation'
    
    - task: SonarCloudPrepare@1
      displayName: 'Prepare SonarCloud Analysis'
      inputs:
        SonarCloud: 'SonarCloud-Connection'
        organization: 'your-org'
        scannerMode: 'MSBuild'
        projectKey: 'mywebapp'
        projectName: 'My Web App'
    
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration) --no-restore
      displayName: 'Build Application'
    
    - script: |
        dotnet test --configuration $(buildConfiguration) \
          --no-build \
          --collect:"XPlat Code Coverage" \
          --logger trx \
          --results-directory $(Agent.TempDirectory)/TestResults
      displayName: 'Run Unit Tests'
    
    - task: SonarCloudAnalyze@1
      displayName: 'Run SonarCloud Analysis'
    
    - task: SonarCloudPublish@1
      displayName: 'Publish SonarCloud Results'
      inputs:
        pollingTimeoutSec: '300'

  - job: SecurityScan
    displayName: 'Security Scanning'
    dependsOn: ValidateCode
    steps:
    - task: WhiteSource@21
      displayName: 'WhiteSource Security Scan'
      inputs:
        cwd: '$(System.DefaultWorkingDirectory)'
    
    - task: CredScan@3
      displayName: 'Credential Scanner'
      inputs:
        toolMajorVersion: 'V2'
    
    - task: AntiMalware@4
      displayName: 'Anti-Malware Scan'
      inputs:
        InputType: 'Basic'
        ScanType: 'CustomScan'
        FileDirPath: '$(Build.StagingDirectory)'
        EnableServices: true
        SupportLogOnError: true
        TreatSignatureUpdateFailureAs: 'Warning'
        SignatureFreshness: 'UpToDate'
        TreatStaleSignatureAs: 'Error'

  - job: BuildAndPackage
    displayName: 'Build and Package'
    dependsOn: 
    - ValidateCode
    - SecurityScan
    condition: succeeded()
    steps:
    - task: UseDotNet@2
      displayName: 'Install .NET SDK'
      inputs:
        version: '8.0.x'
    
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration) --no-restore
        dotnet publish --configuration $(buildConfiguration) --no-build --output $(Build.ArtifactStagingDirectory)/app
      displayName: 'Build and Publish'
    
    - task: ArchiveFiles@2
      displayName: 'Archive Application'
      inputs:
        rootFolderOrFile: '$(Build.ArtifactStagingDirectory)/app'
        includeRootFolder: false
        archiveType: 'zip'
        archiveFile: '$(Build.ArtifactStagingDirectory)/$(Build.BuildId).zip'
        replaceExistingArchive: true
    
    - task: PublishBuildArtifacts@1
      displayName: 'Publish Artifacts'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'

- stage: DeployToDevelopment
  displayName: 'Deploy to Development'
  dependsOn: ValidateAndBuild
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  variables:
  - group: 'Development-Variables'
  jobs:
  - deployment: DeployDev
    displayName: 'Deploy to Development'
    environment: 'Development'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureRmWebAppDeployment@4
            displayName: 'Deploy to Azure Web App'
            inputs:
              ConnectionType: 'AzureRM'
              azureSubscription: '$(azureSubscription)'
              appType: 'webApp'
              WebAppName: '$(webAppName)'
              packageForLinux: '$(Pipeline.Workspace)/drop/$(Build.BuildId).zip'
              RuntimeStack: 'DOTNETCORE|8.0'

- stage: DeployToStaging
  displayName: 'Deploy to Staging'
  dependsOn: ValidateAndBuild
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
  - group: 'Staging-Variables'
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging'
    environment: 'Staging'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureRmWebAppDeployment@4
            displayName: 'Deploy to Azure Web App'
            inputs:
              ConnectionType: 'AzureRM'
              azureSubscription: '$(azureSubscription)'
              appType: 'webApp'
              WebAppName: '$(webAppName)'
              packageForLinux: '$(Pipeline.Workspace)/drop/$(Build.BuildId).zip'
              RuntimeStack: 'DOTNETCORE|8.0'
              deployToSlotOrASE: true
              ResourceGroupName: '$(resourceGroupName)'
              SlotName: 'staging'
          
          - task: AzureAppServiceManage@0
            displayName: 'Warm up Staging Slot'
            inputs:
              azureSubscription: '$(azureSubscription)'
              Action: 'Start Azure App Service'
              WebAppName: '$(webAppName)'
              SpecifySlotOrASE: true
              ResourceGroupName: '$(resourceGroupName)'
              Slot: 'staging'
          
          - task: PowerShell@2
            displayName: 'Run Smoke Tests'
            inputs:
              targetType: 'inline'
              script: |
                $stagingUrl = "https://$(webAppName)-staging.azurewebsites.net"
                $healthEndpoint = "$stagingUrl/health"
                
                # Wait for app to be ready
                Start-Sleep -Seconds 30
                
                # Health check with retry logic
                $maxRetries = 5
                $retryCount = 0
                $success = $false
                
                while ($retryCount -lt $maxRetries -and !$success) {
                  try {
                    $response = Invoke-WebRequest -Uri $healthEndpoint -UseBasicParsing -TimeoutSec 30
                    if ($response.StatusCode -eq 200) {
                      Write-Host "✓ Health check passed - Status: $($response.StatusCode)"
                      $success = $true
                    }
                  }
                  catch {
                    $retryCount++
                    Write-Warning "Health check attempt $retryCount failed: $($_.Exception.Message)"
                    if ($retryCount -lt $maxRetries) {
                      Start-Sleep -Seconds 10
                    }
                  }
                }
                
                if (!$success) {
                  Write-Error "Health check failed after $maxRetries attempts"
                  exit 1
                }

- stage: DeployToProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployToStaging
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
  - group: 'Production-Variables'
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production'
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          # Deploy to staging slot first
          - task: AzureRmWebAppDeployment@4
            displayName: 'Deploy to Production Staging Slot'
            inputs:
              ConnectionType: 'AzureRM'
              azureSubscription: '$(azureSubscription)'
              appType: 'webApp'
              WebAppName: '$(webAppName)'
              packageForLinux: '$(Pipeline.Workspace)/drop/$(Build.BuildId).zip'
              RuntimeStack: 'DOTNETCORE|8.0'
              deployToSlotOrASE: true
              ResourceGroupName: '$(resourceGroupName)'
              SlotName: 'staging'
          
          # Comprehensive testing on staging slot
          - task: PowerShell@2
            displayName: 'Comprehensive Pre-Production Tests'
            inputs:
              targetType: 'inline'
              script: |
                $stagingUrl = "https://$(webAppName)-staging.azurewebsites.net"
                
                # Test suite
                $testEndpoints = @(
                  @{ Url = "$stagingUrl/health"; Name = "Health Check" },
                  @{ Url = "$stagingUrl/api/users"; Name = "Users API" },
                  @{ Url = "$stagingUrl/api/products"; Name = "Products API" }
                )
                
                Write-Host "Starting comprehensive pre-production testing..."
                
                foreach ($test in $testEndpoints) {
                  try {
                    $response = Invoke-WebRequest -Uri $test.Url -UseBasicParsing -TimeoutSec 30
                    Write-Host "✓ $($test.Name) - Status: $($response.StatusCode)"
                  }
                  catch {
                    Write-Error "✗ $($test.Name) failed: $($_.Exception.Message)"
                    exit 1
                  }
                }
                
                # Performance test
                Write-Host "Running basic performance test..."
                $start = Get-Date
                $response = Invoke-WebRequest -Uri "$stagingUrl/api/users" -UseBasicParsing
                $end = Get-Date
                $responseTime = ($end - $start).TotalMilliseconds
                
                if ($responseTime -gt 5000) {
                  Write-Error "Response time too slow: $responseTime ms"
                  exit 1
                } else {
                  Write-Host "✓ Performance test passed - Response time: $responseTime ms"
                }
          
          # Blue-Green deployment - swap slots
          - task: AzureAppServiceManage@0
            displayName: 'Swap to Production'
            inputs:
              azureSubscription: '$(azureSubscription)'
              Action: 'Swap Slots'
              WebAppName: '$(webAppName)'
              ResourceGroupName: '$(resourceGroupName)'
              SourceSlot: 'staging'
              TargetSlot: 'production'
          
          # Post-deployment verification
          - task: PowerShell@2
            displayName: 'Post-Deployment Verification'
            inputs:
              targetType: 'inline'
              script: |
                $prodUrl = "https://$(webAppName).azurewebsites.net"
                
                Write-Host "Verifying production deployment..."
                Start-Sleep -Seconds 15
                
                try {
                  $response = Invoke-WebRequest -Uri "$prodUrl/health" -UseBasicParsing -TimeoutSec 30
                  Write-Host "✓ Production health check passed - Status: $($response.StatusCode)"
                  
                  # Send notification
                  Write-Host "🚀 Deployment to production completed successfully!"
                }
                catch {
                  Write-Error "✗ Production verification failed: $($_.Exception.Message)"
                  
                  # Rollback by swapping slots back
                  Write-Host "Initiating rollback..."
                  # Additional rollback logic would go here
                  exit 1
                }
```

### AWS CodePipeline with Advanced Features

```yaml
# aws-codepipeline-advanced.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Advanced CI/CD Pipeline with Blue-Green Deployment and Automated Testing'

Parameters:
  ApplicationName:
    Type: String
    Default: 'mywebapp'
  
  EnvironmentName:
    Type: String
    Default: 'production'
    AllowedValues: [development, staging, production]
  
  VPCId:
    Type: AWS::EC2::VPC::Id
    Description: 'VPC ID for deployment'
  
  SubnetIds:
    Type: List<AWS::EC2::Subnet::Id>
    Description: 'Subnet IDs for deployment'

Resources:
  # Enhanced S3 Bucket with lifecycle policies
  ArtifactsBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub '${ApplicationName}-artifacts-${AWS::AccountId}-${AWS::Region}'
      VersioningConfiguration:
        Status: Enabled
      LifecycleConfiguration:
        Rules:
          - Id: DeleteOldVersions
            Status: Enabled
            NoncurrentVersionExpirationInDays: 30
          - Id: DeleteIncompleteMultipartUploads
            Status: Enabled
            AbortIncompleteMultipartUpload:
              DaysAfterInitiation: 7
      PublicAccessBlockConfiguration:
        BlockPublicAcls: true
        BlockPublicPolicy: true
        IgnorePublicAcls: true
        RestrictPublicBuckets: true
      BucketEncryption:
        ServerSideEncryptionConfiguration:
          - ServerSideEncryptionByDefault:
              SSEAlgorithm: AES256

  # Advanced CodeBuild with Multiple Environments
  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub '${ApplicationName}-build-${EnvironmentName}'
      Description: !Sub 'Build project for ${ApplicationName} - ${EnvironmentName}'
      ServiceRole: !GetAtt CodeBuildRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_LARGE
        Image: aws/codebuild/amazonlinux2-x86_64-standard:5.0
        PrivilegedMode: true
        EnvironmentVariables:
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
          - Name: AWS_ACCOUNT_ID
            Value: !Ref AWS::AccountId
          - Name: ENVIRONMENT_NAME
            Value: !Ref EnvironmentName
          - Name: APPLICATION_NAME
            Value: !Ref ApplicationName
          - Name: ARTIFACTS_BUCKET
            Value: !Ref ArtifactsBucket
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          
          env:
            shell: bash
            variables:
              DOTNET_ROOT: /usr/share/dotnet
              DOTNET_CLI_TELEMETRY_OPTOUT: 1
              SONAR_SCANNER_VERSION: 4.8.0.2856
          
          phases:
            install:
              runtime-versions:
                dotnet: 8.0
                nodejs: 18
              commands:
                - echo "Installing additional tools..."
                - apt-get update && apt-get install -y jq curl
                
                # Install SonarQube Scanner
                - wget https://github.com/SonarSource/sonar-scanner-cli/archive/refs/tags/${SONAR_SCANNER_VERSION}.tar.gz
                - tar -xzf ${SONAR_SCANNER_VERSION}.tar.gz
                - export PATH=$PATH:$(pwd)/sonar-scanner-cli-${SONAR_SCANNER_VERSION}/bin
                
                # Install Docker (for integration tests)
                - curl -fsSL https://get.docker.com -o get-docker.sh && sh get-docker.sh
                - usermod -aG docker root
                
            pre_build:
              commands:
                - echo "Pre-build phase started on $(date)"
                - echo "Environment: $ENVIRONMENT_NAME"
                
                # Validate environment variables
                - |
                  if [ -z "$APPLICATION_NAME" ] || [ -z "$ENVIRONMENT_NAME" ]; then
                    echo "ERROR: Required environment variables not set"
                    exit 1
                  fi
                
                # Install dependencies
                - echo "Restoring NuGet packages..."
                - dotnet restore --verbosity minimal
                
                # Code quality checks
                - echo "Running code formatting check..."
                - dotnet format --verify-no-changes --verbosity diagnostic
                
            build:
              commands:
                - echo "Build phase started on $(date)"
                
                # Build application
                - echo "Building application..."
                - dotnet build --configuration Release --no-restore --verbosity minimal
                
                # Run unit tests with coverage
                - echo "Running unit tests..."
                - |
                  dotnet test \
                    --configuration Release \
                    --no-build \
                    --logger trx \
                    --collect:"XPlat Code Coverage" \
                    --results-directory ./TestResults \
                    --verbosity minimal
                
                # Run integration tests
                - echo "Running integration tests..."
                - |
                  if [ -f "docker-compose.test.yml" ]; then
                    docker-compose -f docker-compose.test.yml up -d
                    sleep 30
                    dotnet test ./tests/IntegrationTests --configuration Release --logger trx
                    docker-compose -f docker-compose.test.yml down
                  fi
                
                # Security scanning
                - echo "Running security scans..."
                - |
                  # OWASP dependency check
                  if command -v dependency-check &> /dev/null; then
                    dependency-check --project "$APPLICATION_NAME" --scan . --format XML --out ./security-reports/
                  fi
                
                # Code quality analysis
                - echo "Running SonarQube analysis..."
                - |
                  if [ ! -z "$SONAR_TOKEN" ]; then
                    sonar-scanner \
                      -Dsonar.projectKey=$APPLICATION_NAME \
                      -Dsonar.sources=. \
                      -Dsonar.host.url=$SONAR_HOST_URL \
                      -Dsonar.login=$SONAR_TOKEN
                  fi
                
                # Publish application
                - echo "Publishing application..."
                - dotnet publish --configuration Release --no-build --output ./publish
                
                # Create deployment package
                - echo "Creating deployment package..."
                - |
                  # Create appspec.yml for CodeDeploy
                  cat > ./publish/appspec.yml << 'EOF'
                  version: 0.0
                  os: linux
                  files:
                    - source: /
                      destination: /var/www/mywebapp
                  hooks:
                    BeforeInstall:
                      - location: scripts/install_dependencies.sh
                        timeout: 300
                        runas: root
                    ApplicationStart:
                      - location: scripts/start_server.sh
                        timeout: 300
                        runas: root
                    ApplicationStop:
                      - location: scripts/stop_server.sh
                        timeout: 300
                        runas: root
                    ValidateService:
                      - location: scripts/validate_service.sh
                        timeout: 300
                  EOF
                
                # Create deployment scripts
                - mkdir -p ./publish/scripts
                - |
                  cat > ./publish/scripts/install_dependencies.sh << 'EOF'
                  #!/bin/bash
                  # Install .NET runtime if not present
                  if ! command -v dotnet &> /dev/null; then
                    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                    dpkg -i packages-microsoft-prod.deb
                    apt-get update
                    apt-get install -y aspnetcore-runtime-8.0
                  fi
                  
                  # Create application directory
                  mkdir -p /var/www/mywebapp
                  chown -R www-data:www-data /var/www/mywebapp
                  EOF
                
                - |
                  cat > ./publish/scripts/start_server.sh << 'EOF'
                  #!/bin/bash
                  cd /var/www/mywebapp
                  
                  # Start the application
                  export ASPNETCORE_ENVIRONMENT=$ENVIRONMENT_NAME
                  export ASPNETCORE_URLS="http://localhost:5000"
                  
                  nohup dotnet MyWebApp.dll > /var/log/mywebapp.log 2>&1 &
                  echo $! > /var/run/mywebapp.pid
                  
                  # Wait for application to start
                  sleep 10
                  EOF
                
                - |
                  cat > ./publish/scripts/stop_server.sh << 'EOF'
                  #!/bin/bash
                  if [ -f /var/run/mywebapp.pid ]; then
                    PID=$(cat /var/run/mywebapp.pid)
                    if ps -p $PID > /dev/null; then
                      kill $PID
                      rm /var/run/mywebapp.pid
                    fi
                  fi
                  EOF
                
                - |
                  cat > ./publish/scripts/validate_service.sh << 'EOF'
                  #!/bin/bash
                  # Health check
                  max_attempts=10
                  attempt=1
                  
                  while [ $attempt -le $max_attempts ]; do
                    if curl -f http://localhost:5000/health; then
                      echo "Application is healthy"
                      exit 0
                    fi
                    
                    echo "Attempt $attempt failed, waiting..."
                    sleep 10
                    ((attempt++))
                  done
                  
                  echo "Application health check failed"
                  exit 1
                  EOF
                
                # Make scripts executable
                - chmod +x ./publish/scripts/*.sh
                
                # Create deployment archive
                - cd ./publish && zip -r ../deployment-$(date +%Y%m%d-%H%M%S).zip .
                - cd ..
                
            post_build:
              commands:
                - echo "Post-build phase completed on $(date)"
                
                # Upload artifacts to S3
                - |
                  if [ -f deployment-*.zip ]; then
                    aws s3 cp deployment-*.zip s3://$ARTIFACTS_BUCKET/builds/$CODEBUILD_BUILD_ID/
                    echo "Deployment package uploaded successfully"
                  fi
                
                # Generate build report
                - |
                  cat > build-report.json << EOF
                  {
                    "buildId": "$CODEBUILD_BUILD_ID",
                    "buildNumber": "$CODEBUILD_BUILD_NUMBER",
                    "environment": "$ENVIRONMENT_NAME",
                    "application": "$APPLICATION_NAME",
                    "timestamp": "$(date -u +%Y-%m-%dT%H:%M:%SZ)",
                    "status": "SUCCESS",
                    "artifacts": {
                      "deploymentPackage": "deployment-$(date +%Y%m%d-%H%M%S).zip"
                    }
                  }
                  EOF
                
                - aws s3 cp build-report.json s3://$ARTIFACTS_BUCKET/reports/$CODEBUILD_BUILD_ID/
          
          reports:
            unit-tests:
              files:
                - '**/*.trx'
              base-directory: './TestResults'
              file-format: 'VSTS'
            
            code-coverage:
              files:
                - '**/coverage.cobertura.xml'
              base-directory: './TestResults'
              file-format: 'COBERTURA'
            
            security-scan:
              files:
                - '**/*-report.xml'
              base-directory: './security-reports'
              file-format: 'JUNIT'
          
          artifacts:
            files:
              - deployment-*.zip
              - build-report.json
            base-directory: .

  # Advanced CodePipeline with Multiple Stages
  Pipeline:
    Type: AWS::CodePipeline::Pipeline
    Properties:
      Name: !Sub '${ApplicationName}-pipeline-${EnvironmentName}'
      RoleArn: !GetAtt CodePipelineRole.Arn
      ArtifactStore:
        Type: S3
        Location: !Ref ArtifactsBucket
        EncryptionKey:
          Id: !GetAtt PipelineKMSKey.Arn
          Type: KMS
      Stages:
        # Source Stage
        - Name: Source
          Actions:
            - Name: SourceAction
              ActionTypeId:
                Category: Source
                Owner: AWS
                Provider: CodeCommit
                Version: 1
              Configuration:
                RepositoryName: !Sub '${ApplicationName}-repo'
                BranchName: !Sub '${EnvironmentName}'
                PollForSourceChanges: false
              OutputArtifacts:
                - Name: SourceOutput

        # Build and Test Stage
        - Name: BuildAndTest
          Actions:
            - Name: BuildAction
              ActionTypeId:
                Category: Build
                Owner: AWS
                Provider: CodeBuild
                Version: 1
              Configuration:
                ProjectName: !Ref BuildProject
              InputArtifacts:
                - Name: SourceOutput
              OutputArtifacts:
                - Name: BuildOutput
              RunOrder: 1

        # Security and Compliance Stage
        - Name: SecurityCompliance
          Actions:
            - Name: SecurityScan
              ActionTypeId:
                Category: Build
                Owner: AWS
                Provider: CodeBuild
                Version: 1
              Configuration:
                ProjectName: !Ref SecurityScanProject
              InputArtifacts:
                - Name: SourceOutput
              OutputArtifacts:
                - Name: SecurityOutput
              RunOrder: 1

        # Deploy to Development (if not production)
        - !If
          - IsNotProduction
          - Name: DeployToDevelopment
            Actions:
              - Name: DeployToDevAction
                ActionTypeId:
                  Category: Deploy
                  Owner: AWS
                  Provider: CodeDeploy
                  Version: 1
                Configuration:
                  ApplicationName: !Ref CodeDeployApplication
                  DeploymentGroupName: !Ref DevelopmentDeploymentGroup
                InputArtifacts:
                  - Name: BuildOutput
                RunOrder: 1
              
              - Name: RunIntegrationTests
                ActionTypeId:
                  Category: Test
                  Owner: AWS
                  Provider: CodeBuild
                  Version: 1
                Configuration:
                  ProjectName: !Ref IntegrationTestProject
                InputArtifacts:
                  - Name: BuildOutput
                RunOrder: 2
          - !Ref AWS::NoValue

        # Manual Approval for Production
        - !If
          - IsProduction
          - Name: ProductionApproval
            Actions:
              - Name: ManualApproval
                ActionTypeId:
                  Category: Approval
                  Owner: AWS
                  Provider: Manual
                  Version: 1
                Configuration:
                  CustomData: !Sub |
                    Please review the following before approving:
                    1. All tests have passed
                    2. Security scans show no critical issues
                    3. Performance benchmarks are acceptable
                    4. Documentation is up to date
                    
                    Build ID: #{BuildAndTest.BuildAction.BUILD_ID}
                    Environment: ${EnvironmentName}
                    
                  ExternalEntityLink: !Sub 'https://console.aws.amazon.com/codebuild/home?region=${AWS::Region}#/builds/#{BuildAndTest.BuildAction.BUILD_ID}/view/new'
                RunOrder: 1
          - !Ref AWS::NoValue

        # Production Deployment with Blue-Green
        - Name: ProductionDeployment
          Actions:
            - Name: DeployToGreen
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CodeDeploy
                Version: 1
              Configuration:
                ApplicationName: !Ref CodeDeployApplication
                DeploymentGroupName: !Ref ProductionDeploymentGroup
              InputArtifacts:
                - Name: BuildOutput
              RunOrder: 1
            
            - Name: SmokeTests
              ActionTypeId:
                Category: Test
                Owner: AWS
                Provider: CodeBuild
                Version: 1
              Configuration:
                ProjectName: !Ref SmokeTestProject
              InputArtifacts:
                - Name: BuildOutput
              RunOrder: 2
            
            - Name: LoadTesting
              ActionTypeId:
                Category: Test
                Owner: AWS
                Provider: CodeBuild
                Version: 1
              Configuration:
                ProjectName: !Ref LoadTestProject
              InputArtifacts:
                - Name: BuildOutput
              RunOrder: 3

  # Additional resources (KMS, CodeDeploy, etc.)
  PipelineKMSKey:
    Type: AWS::KMS::Key
    Properties:
      Description: 'KMS Key for Pipeline Artifacts'
      KeyPolicy:
        Statement:
          - Sid: Enable IAM User Permissions
            Effect: Allow
            Principal:
              AWS: !Sub 'arn:aws:iam::${AWS::AccountId}:root'
            Action: 'kms:*'
            Resource: '*'
          - Sid: Allow Pipeline Service
            Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action:
              - 'kms:Decrypt'
              - 'kms:DescribeKey'
              - 'kms:Encrypt'
              - 'kms:GenerateDataKey*'
              - 'kms:ReEncrypt*'
            Resource: '*'

  # IAM Roles with comprehensive permissions
  CodePipelineRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action: sts:AssumeRole
      ManagedPolicyArns:
        - arn:aws:iam::aws:policy/AWSCodePipelineFullAccess
      Policies:
        - PolicyName: PipelineExecutionPolicy
          PolicyDocument:
            Version: '2012-10-17'
            Statement:
              - Effect: Allow
                Action:
                  - 's3:GetBucketVersioning'
                  - 's3:GetObject'
                  - 's3:GetObjectVersion'
                  - 's3:PutObject'
                  - 's3:GetBucketLocation'
                  - 's3:ListBucket'
                Resource:
                  - !Sub '${ArtifactsBucket}'
                  - !Sub '${ArtifactsBucket}/*'
              - Effect: Allow
                Action:
                  - 'kms:Decrypt'
                  - 'kms:DescribeKey'
                  - 'kms:Encrypt'
                  - 'kms:GenerateDataKey*'
                  - 'kms:ReEncrypt*'
                Resource: !GetAtt PipelineKMSKey.Arn
              - Effect: Allow
                Action:
                  - 'codebuild:BatchGetBuilds'
                  - 'codebuild:StartBuild'
                Resource:
                  - !GetAtt BuildProject.Arn
                  - !If [HasSecurityScan, !GetAtt SecurityScanProject.Arn, !Ref 'AWS::NoValue']
              - Effect: Allow
                Action:
                  - 'codedeploy:CreateDeployment'
                  - 'codedeploy:GetApplication'
                  - 'codedeploy:GetApplicationRevision'
                  - 'codedeploy:GetDeployment'
                  - 'codedeploy:GetDeploymentConfig'
                  - 'codedeploy:RegisterApplicationRevision'
                Resource: '*// filepath: CI-CD-Azure-AWS-Guide.md
# CI/CD Complete Guide: Azure DevOps & AWS
*Comprehensive Step-by-Step Guide with Real-World Examples*

## Table of Contents
1. [Azure DevOps CI/CD Pipeline](#azure-devops-cicd-pipeline)
2. [AWS CI/CD Pipeline](#aws-cicd-pipeline)
3. [Repository Management](#repository-management)
4. [YAML Pipeline Examples](#yaml-pipeline-examples)
5. [Best Practices](#best-practices)
6. [Troubleshooting](#troubleshooting)

---

## Azure DevOps CI/CD Pipeline

### Prerequisites
- Azure DevOps account
- Azure subscription
- Visual Studio/VS Code
- Git installed

### Step 1: Create Azure DevOps Project

```bash
# Login to Azure DevOps
# Go to https://dev.azure.com
# Click "New Project"
```

**Project Configuration:**
```yaml
Project Name: "MyWebApp-CICD"
Description: "Full Stack Web Application with CI/CD"
Visibility: Private
Version Control: Git
Work Item Process: Agile
```

### Step 2: Create Repository and Push Code

#### Initialize Local Repository
```bash
# Create new directory
mkdir my-webapp-cicd
cd my-webapp-cicd

# Initialize Git repository
git init

# Add Azure DevOps remote
git remote add origin https://dev.azure.com/YourOrg/MyWebApp-CICD/_git/MyWebApp-CICD

# Create initial files
touch README.md
echo "# My Web App CI/CD Project" > README.md
```

#### Create Sample .NET Core Application
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

```xml
<!-- MyWebApp.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
  </ItemGroup>

</Project>
```

#### Push Initial Code
```bash
# Stage all files
git add .

# Commit changes
git commit -m "Initial commit: .NET Core Web API project setup"

# Push to Azure DevOps
git push -u origin main
```

### Step 3: Create Pull Request Workflow

#### Create Feature Branch
```bash
# Create and switch to feature branch
git checkout -b feature/add-user-controller

# Make changes (add new controller)
# Controllers/UsersController.cs
```

```csharp
// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> Users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        user.Id = Users.Max(u => u.Id) + 1;
        Users.Add(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

#### Push Feature and Create Pull Request
```bash
# Commit feature changes
git add .
git commit -m "Add Users controller with CRUD operations"

# Push feature branch
git push origin feature/add-user-controller
```

**Create Pull Request in Azure DevOps:**
1. Go to Azure DevOps → Repos → Pull Requests
2. Click "New Pull Request"
3. Source: `feature/add-user-controller`
4. Target: `main`
5. Add reviewers and work items
6. Set completion options (delete source branch, squash merge)

### Step 4: Create Build Pipeline (CI)

#### Azure Pipeline YAML (azure-pipelines.yml)
```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
    - main
    - develop
    - feature/*

pr:
  branches:
    include:
    - main
    - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotNetFramework: 'net8.0'
  dotNetVersion: '8.0.x'
  projectPath: '**/*.csproj'

stages:
- stage: Build
  displayName: 'Build and Test'
  jobs:
  - job: Build
    displayName: 'Build Job'
    steps:
    
    # Install .NET Core SDK
    - task: UseDotNet@2
      displayName: 'Install .NET Core SDK'
      inputs:
        version: $(dotNetVersion)
        includePreviewVersions: false
    
    # Restore NuGet packages
    - task: DotNetCoreCLI@2
      displayName: 'Restore NuGet Packages'
      inputs:
        command: 'restore'
        projects: $(projectPath)
        feedsToUse: 'select'
    
    # Build the application
    - task: DotNetCoreCLI@2
      displayName: 'Build Application'
      inputs:
        command: 'build'
        projects: $(projectPath)
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    # Run unit tests
    - task: DotNetCoreCLI@2
      displayName: 'Run Unit Tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --no-build --collect:"XPlat Code Coverage"'
        publishTestResults: true
    
    # Publish code coverage results
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish Code Coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    
    # Publish the application
    - task: DotNetCoreCLI@2
      displayName: 'Publish Application'
      inputs:
        command: 'publish'
        projects: $(projectPath)
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/app'
        publishWebProjects: true
        zipAfterPublish: true
    
    # Publish build artifacts
    - task: PublishBuildArtifacts@1
      displayName: 'Publish Build Artifacts'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'
        publishLocation: 'Container'

- stage: SecurityScan
  displayName: 'Security Scanning'
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: SecurityScan
    displayName: 'Security Scan Job'
    steps:
    
    # OWASP Dependency Check
    - task: dependency-check-build-task@6
      displayName: 'OWASP Dependency Check'
      inputs:
        projectName: 'MyWebApp'
        scanPath: '$(Build.SourcesDirectory)'
        format: 'ALL'
        additionalArguments: '--enableRetired --enableExperimental'
    
    # Publish security scan results
    - task: PublishTestResults@2
      displayName: 'Publish Security Scan Results'
      inputs:
        testResultsFormat: 'JUnit'
        testResultsFiles: 'dependency-check-junit.xml'
        searchFolder: '$(Common.TestResultsDirectory)'
```

### Step 5: Create Release Pipeline (CD)

#### Multi-Stage Release YAML
```yaml
# azure-release-pipeline.yml
trigger: none # Manual trigger for releases

pr: none

pool:
  vmImage: 'ubuntu-latest'

variables:
- group: 'Production-Variables'
- name: 'azureSubscription'
  value: 'YourAzureSubscription'

stages:
- stage: DeployToDev
  displayName: 'Deploy to Development'
  condition: succeeded()
  jobs:
  - deployment: DeployDev
    displayName: 'Deploy to Dev Environment'
    environment: 'Development'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureWebApp@1
            displayName: 'Deploy to Azure Web App - Dev'
            inputs:
              azureSubscription: $(azureSubscription)
              appType: 'webApp'
              appName: 'mywebapp-dev'
              package: '$(Pipeline.Workspace)/drop/app/*.zip'
              deploymentMethod: 'auto'
          
          - task: AzureAppServiceManage@0
            displayName: 'Start Azure Web App - Dev'
            inputs:
              azureSubscription: $(azureSubscription)
              Action: 'Start Azure App Service'
              WebAppName: 'mywebapp-dev'
          
          # Health check
          - task: PowerShell@2
            displayName: 'Health Check - Dev'
            inputs:
              targetType: 'inline'
              script: |
                $response = Invoke-WebRequest -Uri "https://mywebapp-dev.azurewebsites.net/health" -UseBasicParsing
                if ($response.StatusCode -ne 200) {
                  Write-Error "Health check failed"
                  exit 1
                }
                Write-Host "Health check passed"

- stage: DeployToStaging
  displayName: 'Deploy to Staging'
  dependsOn: DeployToDev
  condition: succeeded()
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging Environment'
    environment: 'Staging'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureWebApp@1
            displayName: 'Deploy to Azure Web App - Staging'
            inputs:
              azureSubscription: $(azureSubscription)
              appType: 'webApp'
              appName: 'mywebapp-staging'
              package: '$(Pipeline.Workspace)/drop/app/*.zip'
              deploymentMethod: 'auto'
              
          # Run integration tests
          - task: DotNetCoreCLI@2
            displayName: 'Run Integration Tests'
            inputs:
              command: 'test'
              projects: '**/*IntegrationTests.csproj'
              arguments: '--configuration Release'

- stage: DeployToProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployToStaging
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production Environment'
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          # Blue-Green Deployment
          - task: AzureWebApp@1
            displayName: 'Deploy to Production Slot'
            inputs:
              azureSubscription: $(azureSubscription)
              appType: 'webApp'
              appName: 'mywebapp-prod'
              package: '$(Pipeline.Workspace)/drop/app/*.zip'
              deployToSlotOrASE: true
              resourceGroupName: 'rg-mywebapp-prod'
              slotName: 'staging'
          
          # Smoke tests on staging slot
          - task: PowerShell@2
            displayName: 'Smoke Tests - Production Staging Slot'
            inputs:
              targetType: 'inline'
              script: |
                # Wait for deployment to settle
                Start-Sleep -Seconds 30
                
                # Test critical endpoints
                $baseUrl = "https://mywebapp-prod-staging.azurewebsites.net"
                $endpoints = @("/health", "/api/users")
                
                foreach ($endpoint in $endpoints) {
                  try {
                    $response = Invoke-WebRequest -Uri "$baseUrl$endpoint" -UseBasicParsing -TimeoutSec 30
                    Write-Host "✓ $endpoint - Status: $($response.StatusCode)"
                  }
                  catch {
                    Write-Error "✗ $endpoint failed: $($_.Exception.Message)"
                    exit 1
                  }
                }
          
          # Swap slots (Blue-Green deployment)
          - task: AzureAppServiceManage@0
            displayName: 'Swap Production Slots'
            inputs:
              azureSubscription: $(azureSubscription)
              Action: 'Swap Slots'
              WebAppName: 'mywebapp-prod'
              ResourceGroupName: 'rg-mywebapp-prod'
              SourceSlot: 'staging'
              TargetSlot: 'production'
```

---

## AWS CI/CD Pipeline

### Step 1: Setup AWS Environment

#### Install AWS CLI
```bash
# Install AWS CLI
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip
sudo ./aws/install

# Configure AWS CLI
aws configure
# AWS Access Key ID: YOUR_ACCESS_KEY
# AWS Secret Access Key: YOUR_SECRET_KEY
# Default region name: us-east-1
# Default output format: json
```

#### Create S3 Bucket for Artifacts
```bash
# Create S3 bucket for build artifacts
aws s3 mb s3://mywebapp-cicd-artifacts-$(date +%s)

# Enable versioning
aws s3api put-bucket-versioning \
  --bucket mywebapp-cicd-artifacts-$(date +%s) \
  --versioning-configuration Status=Enabled
```

### Step 2: Setup CodeCommit Repository

#### Create CodeCommit Repository
```bash
# Create CodeCommit repository
aws codecommit create-repository \
  --repository-name mywebapp-cicd \
  --repository-description "My Web App CI/CD Repository"

# Clone the repository
git clone https://git-codecommit.us-east-1.amazonaws.com/v1/repos/mywebapp-cicd
cd mywebapp-cicd
```

#### Configure Git for CodeCommit
```bash
# Configure Git credentials helper
git config credential.helper '!aws codecommit credential-helper $@'
git config credential.UseHttpPath true

# Add your code and push
git add .
git commit -m "Initial commit: .NET Core Web API"
git push origin main
```

### Step 3: Create CodeBuild Project

#### buildspec.yml for CodeBuild
```yaml
# buildspec.yml
version: 0.2

env:
  variables:
    DOTNET_ROOT: /usr/share/dotnet
    DOTNET_CLI_TELEMETRY_OPTOUT: 1

phases:
  install:
    runtime-versions:
      dotnet: 8.0
    commands:
      - echo "Installing dependencies..."
      - apt-get update
      - apt-get install -y jq
      
  pre_build:
    commands:
      - echo "Pre-build phase started on `date`"
      - echo "Restoring NuGet packages..."
      - dotnet restore
      
  build:
    commands:
      - echo "Build phase started on `date`"
      - echo "Building the application..."
      - dotnet build --configuration Release --no-restore
      
      - echo "Running unit tests..."
      - dotnet test --configuration Release --no-build --logger trx --collect:"XPlat Code Coverage"
      
      - echo "Publishing the application..."
      - dotnet publish --configuration Release --no-build --output ./publish
      
      - echo "Creating deployment package..."
      - cd publish
      - zip -r ../mywebapp-$(date +%Y%m%d-%H%M%S).zip .
      - cd ..
      
  post_build:
    commands:
      - echo "Post-build phase completed on `date`"
      - echo "Uploading artifacts to S3..."
      - aws s3 cp mywebapp-*.zip s3://$ARTIFACTS_BUCKET/builds/

artifacts:
  files:
    - mywebapp-*.zip
  base-directory: .
  
cache:
  paths:
    - '/root/.nuget/packages/**/*'

reports:
  unit-tests:
    files:
      - '**/*.trx'
    file-format: 'VSTS'
  code-coverage:
    files:
      - '**/coverage.cobertura.xml'
    file-format: 'COBERTURA'
```

#### Create CodeBuild Project with CLI
```bash
# Create CodeBuild project
cat > codebuild-project.json << EOF
{
  "name": "mywebapp-build",
  "description": "Build project for My Web App",
  "source": {
    "type": "CODECOMMIT",
    "location": "https://git-codecommit.us-east-1.amazonaws.com/v1/repos/mywebapp-cicd",
    "buildspec": "buildspec.yml"
  },
  "artifacts": {
    "type": "S3",
    "location": "mywebapp-cicd-artifacts/builds"
  },
  "environment": {
    "type": "LINUX_CONTAINER",
    "image": "aws/codebuild/amazonlinux2-x86_64-standard:5.0",
    "computeType": "BUILD_GENERAL1_MEDIUM",
    "environmentVariables": [
      {
        "name": "ARTIFACTS_BUCKET",
        "value": "mywebapp-cicd-artifacts"
      }
    ]
  },
  "serviceRole": "arn:aws:iam::YOUR_ACCOUNT:role/CodeBuildServiceRole"
}
EOF

aws codebuild create-project --cli-input-json file://codebuild-project.json
```

### Step 4: Create CodePipeline

#### CloudFormation Template for Complete Pipeline
```yaml
# pipeline-template.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Complete CI/CD Pipeline for Web Application'

Parameters:
  ApplicationName:
    Type: String
    Default: 'mywebapp'
    Description: 'Name of the application'
  
  GitHubRepoName:
    Type: String
    Default: 'mywebapp-cicd'
    Description: 'GitHub repository name'
  
  GitHubOwner:
    Type: String
    Description: 'GitHub repository owner'
  
  GitHubToken:
    Type: String
    NoEcho: true
    Description: 'GitHub personal access token'

Resources:
  # S3 Bucket for Pipeline Artifacts
  ArtifactsBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub '${ApplicationName}-pipeline-artifacts-${AWS::AccountId}'
      VersioningConfiguration:
        Status: Enabled
      BucketEncryption:
        ServerSideEncryptionConfiguration:
          - ServerSideEncryptionByDefault:
              SSEAlgorithm: AES256

  # CodeBuild Project
  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub '${ApplicationName}-build'
      Description: !Sub 'Build project for ${ApplicationName}'
      ServiceRole: !GetAtt CodeBuildRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_MEDIUM
        Image: aws/codebuild/amazonlinux2-x86_64-standard:5.0
        EnvironmentVariables:
          - Name: ARTIFACTS_BUCKET
            Value: !Ref ArtifactsBucket
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
          - Name: AWS_ACCOUNT_ID
            Value: !Ref AWS::AccountId
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          phases:
            install:
              runtime-versions:
                dotnet: 8.0
              commands:
                - echo "Installing dependencies..."
            pre_build:
              commands:
                - echo "Restoring packages..."
                - dotnet restore
            build:
              commands:
                - echo "Building application..."
                - dotnet build --configuration Release
                - dotnet test --configuration Release --logger trx
                - dotnet publish --configuration Release --output ./publish
            post_build:
              commands:
                - echo "Build completed"
          artifacts:
            files:
              - '**/*'
            base-directory: publish

  # CodeDeploy Application
  DeployApplication:
    Type: AWS::CodeDeploy::Application
    Properties:
      ApplicationName: !Sub '${ApplicationName}-deploy'
      ComputePlatform: Server

  # CodeDeploy Deployment Group
  DeploymentGroup:
    Type: AWS::CodeDeploy::DeploymentGroup
    Properties:
      ApplicationName: !Ref DeployApplication
      DeploymentGroupName: !Sub '${ApplicationName}-deployment-group'
      ServiceRoleArn: !GetAtt CodeDeployRole.Arn
      Ec2TagFilters:
        - Type: KEY_AND_VALUE
          Key: Environment
          Value: Production
      AutoRollbackConfiguration:
        Enabled: true
        Events:
          - DEPLOYMENT_FAILURE
          - DEPLOYMENT_STOP_ON_ALARM

  # CodePipeline
  Pipeline:
    Type: AWS::CodePipeline::Pipeline
    Properties:
      Name: !Sub '${ApplicationName}-pipeline'
      RoleArn: !GetAtt CodePipelineRole.Arn
      ArtifactStore:
        Type: S3
        Location: !Ref ArtifactsBucket
      Stages:
        - Name: Source
          Actions:
            - Name: SourceAction
              ActionTypeId:
                Category: Source
                Owner: ThirdParty
                Provider: GitHub
                Version: 1
              Configuration:
                Owner: !Ref GitHubOwner
                Repo: !Ref GitHubRepoName
                Branch: main
                OAuthToken: !Ref GitHubToken
              OutputArtifacts:
                - Name: SourceOutput

        - Name: Build
          Actions:
            - Name: BuildAction
              ActionTypeId:
                Category: Build
                Owner: AWS
                Provider: CodeBuild
                Version: 1
              Configuration:
                ProjectName: !Ref BuildProject
              InputArtifacts:
                - Name: SourceOutput
              OutputArtifacts:
                - Name: BuildOutput

        - Name: DeployToDev
          Actions:
            - Name: DeployToDevAction
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CodeDeploy
                Version: 1
              Configuration:
                ApplicationName: !Ref DeployApplication
                DeploymentGroupName: !Ref DeploymentGroup
              InputArtifacts:
                - Name: BuildOutput
              Region: !Ref AWS::Region

        - Name: ApprovalForProduction
          Actions:
            - Name: ManualApproval
              ActionTypeId:
                Category: Approval
                Owner: AWS
                Provider: Manual
                Version: 1
              Configuration:
                CustomData: 'Please review and approve deployment to production'

        - Name: DeployToProduction
          Actions:
            - Name: DeployToProdAction
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CodeDeploy
                Version: 1
              Configuration:
                ApplicationName: !Ref DeployApplication
                DeploymentGroupName: !Ref DeploymentGroup
              InputArtifacts:
                - Name: BuildOutput
              Region: !Ref AWS::Region

  # IAM Roles
  CodePipelineRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: PipelineExecutionPolicy
          PolicyDocument:
            Version: '2012-10-17'
            Statement:
              - Effect: Allow
                Action:
                  - s3:GetBucketVersioning
                  - s3:GetObject
                  - s3:GetObjectVersion
                  - s3:PutObject
                  - s3:GetBucketLocation
                  - s3:ListBucket
                Resource:
                  - !Sub '${ArtifactsBucket}'
                  - !Sub '${ArtifactsBucket}/*'
              - Effect: Allow
                Action:
                  - codebuild:BatchGetBuilds
                  - codebuild:StartBuild
                Resource: !GetAtt BuildProject.Arn
              - Effect: Allow
                Action:
                  - codedeploy:CreateDeployment
                  - codedeploy:GetApplication
                  - codedeploy:GetApplicationRevision
                  - codedeploy:GetDeployment
                  - codedeploy:GetDeploymentConfig
                  - codedeploy:RegisterApplicationRevision
                Resource: '*'

  CodeBuildRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codebuild.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: BuildExecutionPolicy
          PolicyDocument:
            Version: '2012-10-17'
            Statement:
              - Effect: Allow
                Action:
                  - logs:CreateLogGroup
                  - logs:CreateLogStream
                  - logs:PutLogEvents
                Resource: !Sub 'arn:aws:logs:${AWS::Region}:${AWS::AccountId}:log-group:/aws/codebuild/*'
              - Effect: Allow
                Action:
                  - s3:GetObject
                  - s3:GetObjectVersion
                  - s3:PutObject
                Resource:
                  - !Sub '${ArtifactsBucket}/*'

  CodeDeployRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codedeploy.amazonaws.com
            Action: sts:AssumeRole
      ManagedPolicyArns:
        - arn:aws:iam::aws:policy/service-role/AWSCodeDeployRole

Outputs:
  PipelineName:
    Description: 'Name of the created pipeline'
    Value: !Ref Pipeline
    Export:
      Name: !Sub '${AWS::StackName}-PipelineName'
  
  ArtifactsBucketName:
    Description: 'Name of the artifacts bucket'
    Value: !Ref ArtifactsBucket
    Export:
      Name: !Sub '${AWS::StackName}-ArtifactsBucket'
```

#### Deploy the Pipeline
```bash
# Deploy the CloudFormation stack
aws cloudformation create-stack \
  --stack-name mywebapp-cicd-pipeline \
  --template-body file://pipeline-template.yml \
  --parameters ParameterKey=GitHubOwner,ParameterValue=YourGitHubUsername \
               ParameterKey=GitHubToken,ParameterValue=YourGitHubToken \
  --capabilities CAPABILITY_IAM

# Wait for stack creation
aws cloudformation wait stack-create-complete \
  --stack-name mywebapp-cicd-pipeline
```

---

## YAML Pipeline Examples

### Advanced Azure DevOps YAML with Multiple Environments

```yaml
# azure-pipelines-advanced.yml
name: $(Date:yyyyMMdd)$(Rev:.r)

trigger:
  branches:
    include:
    - main
    - develop
    - release/*
  paths:
    exclude:
    - README.md
    - docs/*

pr:
  branches:
    include:
    - main
    - develop
  paths:
    exclude:
    - README.md
    - docs/*

variables:
- group: 'Global-Variables'
- group: 'Security-Variables'
- name: 'buildConfiguration'
  value: 'Release'
- name: 'vmImageName'
  value: 'ubuntu-latest'

pool:
  vmImage: $(vmImageName)

stages:
- stage: ValidateAndBuild
  displayName: 'Validate and Build'
  jobs:
  - job: ValidateCode
    displayName: 'Code Validation'
    steps:
    - task: UseDotNet@2
      displayName: 'Install .NET SDK'
      inputs:
        version: '8.0.x'
    
    - script: |
        echo "Running code validation..."
        dotnet format --verify-no-changes --verbosity diagnostic
        echo "Code formatting validation passed"
      displayName: 'Code Format Validation'
    
    - task: SonarCloudPrepare@1
      displayName: 'Prepare SonarCloud Analysis'
      inputs:
        SonarCloud: 'SonarCloud-Connection'
        organization: 'your-org'
        scannerMode: 'MSBuild'
        projectKey: 'mywebapp'
        projectName: 'My Web App'
    
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration) --no-restore
      displayName: 'Build Application'
    
    - script: |
        dotnet test --configuration $(buildConfiguration) \
          --no-build \
          --collect:"XPlat Code Coverage" \
          --logger trx \
          --results-directory $(Agent.TempDirectory)/TestResults
      displayName: 'Run Unit Tests'
    
    - task: SonarCloudAnalyze@1
      displayName: 'Run SonarCloud Analysis'
    
    - task: SonarCloudPublish@1
      displayName: 'Publish SonarCloud Results'
      inputs:
        pollingTimeoutSec: '300'

  - job: SecurityScan
    displayName: 'Security Scanning'
    dependsOn: ValidateCode
    steps:
    - task: WhiteSource@21
      displayName: 'WhiteSource Security Scan'
      inputs:
        cwd: '$(System.DefaultWorkingDirectory)'
    
    - task: CredScan@3
      displayName: 'Credential Scanner'
      inputs:
        toolMajorVersion: 'V2'
    
    - task: AntiMalware@4
      displayName: 'Anti-Malware Scan'
      inputs:
        InputType: 'Basic'
        ScanType: 'CustomScan'
        FileDirPath: '$(Build.StagingDirectory)'
        EnableServices: true
        SupportLogOnError: true
        TreatSignatureUpdateFailureAs: 'Warning'
        SignatureFreshness: 'UpToDate'
        TreatStaleSignatureAs: 'Error'

  - job: BuildAndPackage
    displayName: 'Build and Package'
    dependsOn: 
    - ValidateCode
    - SecurityScan
    condition: succeeded()
    steps:
    - task: UseDotNet@2
      displayName: 'Install .NET SDK'
      inputs:
        version: '8.0.x'
    
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration) --no-restore
        dotnet publish --configuration $(buildConfiguration) --no-build --output $(Build.ArtifactStagingDirectory)/app
      displayName: 'Build and Publish'
    
    - task: ArchiveFiles@2
      displayName: 'Archive Application'
      inputs:
        rootFolderOrFile: '$(Build.ArtifactStagingDirectory)/app'
        includeRootFolder: false
        archiveType: 'zip'
        archiveFile: '$(Build.ArtifactStagingDirectory)/$(Build.BuildId).zip'
        replaceExistingArchive: true
    
    - task: PublishBuildArtifacts@1
      displayName: 'Publish Artifacts'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'

- stage: DeployToDevelopment
  displayName: 'Deploy to Development'
  dependsOn: ValidateAndBuild
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  variables:
  - group: 'Development-Variables'
  jobs:
  - deployment: DeployDev
    displayName: 'Deploy to Development'
    environment: 'Development'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureRmWebAppDeployment@4
            displayName: 'Deploy to Azure Web App'
            inputs:
              ConnectionType: 'AzureRM'
              azureSubscription: '$(azureSubscription)'
              appType: 'webApp'
              WebAppName: '$(webAppName)'
              packageForLinux: '$(Pipeline.Workspace)/drop/$(Build.BuildId).zip'
              RuntimeStack: 'DOTNETCORE|8.0'

- stage: DeployToStaging
  displayName: 'Deploy to Staging'
  dependsOn: ValidateAndBuild
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
  - group: 'Staging-Variables'
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging'
    environment: 'Staging'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          - task: AzureRmWebAppDeployment@4
            displayName: 'Deploy to Azure Web App'
            inputs:
              ConnectionType: 'AzureRM'
              azureSubscription: '$(azureSubscription)'
              appType: 'webApp'
              WebAppName: '$(webAppName)'
              packageForLinux: '$(Pipeline.Workspace)/drop/$(Build.BuildId).zip'
              RuntimeStack: 'DOTNETCORE|8.0'
              deployToSlotOrASE: true
              ResourceGroupName: '$(resourceGroupName)'
              SlotName: 'staging'
          
          - task: AzureAppServiceManage@0
            displayName: 'Warm up Staging Slot'
            inputs:
              azureSubscription: '$(azureSubscription)'
              Action: 'Start Azure App Service'
              WebAppName: '$(webAppName)'
              SpecifySlotOrASE: true
              ResourceGroupName: '$(resourceGroupName)'
              Slot: 'staging'
          
          - task: PowerShell@2
            displayName: 'Run Smoke Tests'
            inputs:
              targetType: 'inline'
              script: |
                $stagingUrl = "https://$(webAppName)-staging.azurewebsites.net"
                $healthEndpoint = "$stagingUrl/health"
                
                # Wait for app to be ready
                Start-Sleep -Seconds 30
                
                # Health check with retry logic
                $maxRetries = 5
                $retryCount = 0
                $success = $false
                
                while ($retryCount -lt $maxRetries -and !$success) {
                  try {
                    $response = Invoke-WebRequest -Uri $healthEndpoint -UseBasicParsing -TimeoutSec 30
                    if ($response.StatusCode -eq 200) {
                      Write-Host "✓ Health check passed - Status: $($response.StatusCode)"
                      $success = $true
                    }
                  }
                  catch {
                    $retryCount++
                    Write-Warning "Health check attempt $retryCount failed: $($_.Exception.Message)"
                    if ($retryCount -lt $maxRetries) {
                      Start-Sleep -Seconds 10
                    }
                  }
                }
                
                if (!$success) {
                  Write-Error "Health check failed after $maxRetries attempts"
                  exit 1
                }

- stage: DeployToProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployToStaging
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
  - group: 'Production-Variables'
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production'
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: drop
          
          # Deploy to staging slot first
          - task: AzureRmWebAppDeployment@4
            displayName: 'Deploy to Production Staging Slot'
            inputs:
              ConnectionType: 'AzureRM'
              azureSubscription: '$(azureSubscription)'
              appType: 'webApp'
              WebAppName: '$(webAppName)'
              packageForLinux: '$(Pipeline.Workspace)/drop/$(Build.BuildId).zip'
              RuntimeStack: 'DOTNETCORE|8.0'
              deployToSlotOrASE: true
              ResourceGroupName: '$(resourceGroupName)'
              SlotName: 'staging'
          
          # Comprehensive testing on staging slot
          - task: PowerShell@2
            displayName: 'Comprehensive Pre-Production Tests'
            inputs:
              targetType: 'inline'
              script: |
                $stagingUrl = "https://$(webAppName)-staging.azurewebsites.net"
                
                # Test suite
                $testEndpoints = @(
                  @{ Url = "$stagingUrl/health"; Name = "Health Check" },
                  @{ Url = "$stagingUrl/api/users"; Name = "Users API" },
                  @{ Url = "$stagingUrl/api/products"; Name = "Products API" }
                )
                
                Write-Host "Starting comprehensive pre-production testing..."
                
                foreach ($test in $testEndpoints) {
                  try {
                    $response = Invoke-WebRequest -Uri $test.Url -UseBasicParsing -TimeoutSec 30
                    Write-Host "✓ $($test.Name) - Status: $($response.StatusCode)"
                  }
                  catch {
                    Write-Error "✗ $($test.Name) failed: $($_.Exception.Message)"
                    exit 1
                  }
                }
                
                # Performance test
                Write-Host "Running basic performance test..."
                $start = Get-Date
                $response = Invoke-WebRequest -Uri "$stagingUrl/api/users" -UseBasicParsing
                $end = Get-Date
                $responseTime = ($end - $start).TotalMilliseconds
                
                if ($responseTime -gt 5000) {
                  Write-Error "Response time too slow: $responseTime ms"
                  exit 1
                } else {
                  Write-Host "✓ Performance test passed - Response time: $responseTime ms"
                }
          
          # Blue-Green deployment - swap slots
          - task: AzureAppServiceManage@0
            displayName: 'Swap to Production'
            inputs:
              azureSubscription: '$(azureSubscription)'
              Action: 'Swap Slots'
              WebAppName: '$(webAppName)'
              ResourceGroupName: '$(resourceGroupName)'
              SourceSlot: 'staging'
              TargetSlot: 'production'
          
          # Post-deployment verification
          - task: PowerShell@2
            displayName: 'Post-Deployment Verification'
            inputs:
              targetType: 'inline'
              script: |
                $prodUrl = "https://$(webAppName).azurewebsites.net"
                
                Write-Host "Verifying production deployment..."
                Start-Sleep -Seconds 15
                
                try {
                  $response = Invoke-WebRequest -Uri "$prodUrl/health" -UseBasicParsing -TimeoutSec 30
                  Write-Host "✓ Production health check passed - Status: $($response.StatusCode)"
                  
                  # Send notification
                  Write-Host "🚀 Deployment to production completed successfully!"
                }
                catch {
                  Write-Error "✗ Production verification failed: $($_.Exception.Message)"
                  
                  # Rollback by swapping slots back
                  Write-Host "Initiating rollback..."
                  # Additional rollback logic would go here
                  exit 1
                }
```

### AWS CodePipeline with Advanced Features

```yaml
# aws-codepipeline-advanced.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Advanced CI/CD Pipeline with Blue-Green Deployment and Automated Testing'

Parameters:
  ApplicationName:
    Type: String
    Default: 'mywebapp'
  
  EnvironmentName:
    Type: String
    Default: 'production'
    AllowedValues: [development, staging, production]
  
  VPCId:
    Type: AWS::EC2::VPC::Id
    Description: 'VPC ID for deployment'
  
  SubnetIds:
    Type: List<AWS::EC2::Subnet::Id>
    Description: 'Subnet IDs for deployment'

Resources:
  # Enhanced S3 Bucket with lifecycle policies
  ArtifactsBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub '${ApplicationName}-artifacts-${AWS::AccountId}-${AWS::Region}'
      VersioningConfiguration:
        Status: Enabled
      LifecycleConfiguration:
        Rules:
          - Id: DeleteOldVersions
            Status: Enabled
            NoncurrentVersionExpirationInDays: 30
          - Id: DeleteIncompleteMultipartUploads
            Status: Enabled
            AbortIncompleteMultipartUpload:
              DaysAfterInitiation: 7
      PublicAccessBlockConfiguration:
        BlockPublicAcls: true
        BlockPublicPolicy: true
        IgnorePublicAcls: true
        RestrictPublicBuckets: true
      BucketEncryption:
        ServerSideEncryptionConfiguration:
          - ServerSideEncryptionByDefault:
              SSEAlgorithm: AES256

  # Advanced CodeBuild with Multiple Environments
  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub '${ApplicationName}-build-${EnvironmentName}'
      Description: !Sub 'Build project for ${ApplicationName} - ${EnvironmentName}'
      ServiceRole: !GetAtt CodeBuildRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_LARGE
        Image: aws/codebuild/amazonlinux2-x86_64-standard:5.0
        PrivilegedMode: true
        EnvironmentVariables:
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
          - Name: AWS_ACCOUNT_ID
            Value: !Ref AWS::AccountId
          - Name: ENVIRONMENT_NAME
            Value: !Ref EnvironmentName
          - Name: APPLICATION_NAME
            Value: !Ref ApplicationName
          - Name: ARTIFACTS_BUCKET
            Value: !Ref ArtifactsBucket
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          
          env:
            shell: bash
            variables:
              DOTNET_ROOT: /usr/share/dotnet
              DOTNET_CLI_TELEMETRY_OPTOUT: 1
              SONAR_SCANNER_VERSION: 4.8.0.2856
          
          phases:
            install:
              runtime-versions:
                dotnet: 8.0
                nodejs: 18
              commands:
                - echo "Installing additional tools..."
                - apt-get update && apt-get install -y jq curl
                
                # Install SonarQube Scanner
                - wget https://github.com/SonarSource/sonar-scanner-cli/archive/refs/tags/${SONAR_SCANNER_VERSION}.tar.gz
                - tar -xzf ${SONAR_SCANNER_VERSION}.tar.gz
                - export PATH=$PATH:$(pwd)/sonar-scanner-cli-${SONAR_SCANNER_VERSION}/bin
                
                # Install Docker (for integration tests)
                - curl -fsSL https://get.docker.com -o get-docker.sh && sh get-docker.sh
                - usermod -aG docker root
                
            pre_build:
              commands:
                - echo "Pre-build phase started on $(date)"
                - echo "Environment: $ENVIRONMENT_NAME"
                
                # Validate environment variables
                - |
                  if [ -z "$APPLICATION_NAME" ] || [ -z "$ENVIRONMENT_NAME" ]; then
                    echo "ERROR: Required environment variables not set"
                    exit 1
                  fi
                
                # Install dependencies
                - echo "Restoring NuGet pack…