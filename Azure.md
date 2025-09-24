# Complete CI/CD Guide: Azure DevOps & AWS
*Comprehensive Step-by-Step Implementation with Real-World Examples*

## Table of Contents
1. [Azure DevOps Complete Implementation](#azure-devops-complete-implementation)
2. [AWS CodePipeline Complete Implementation](#aws-codepipeline-complete-implementation)
3. [YAML Pipeline Configurations](#yaml-pipeline-configurations)
4. [Best Practices & Troubleshooting](#best-practices--troubleshooting)

---

## Azure DevOps Complete Implementation

### 🏗️ Step 1: Create Azure DevOps Organization & Project

#### 1.1 Create Organization
```bash
# Navigate to Azure DevOps
https://dev.azure.com

# Create new organization
Organization Name: YourCompany-DevOps
Region: Central US (or your preferred region)
```

#### 1.2 Create Project
```json
{
  "projectName": "ECommerceApp",
  "description": "E-commerce application with microservices",
  "visibility": "private",
  "processTemplate": "Agile",
  "versionControl": "Git"
}
```

### 🗂️ Step 2: Repository Setup & Code Management

#### 2.1 Create Repository
```bash
# In Azure DevOps Project
# Navigate to Repos → Files → Initialize with README

# Repository Structure
ECommerceApp/
├── src/
│   ├── WebAPI/
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Startup.cs
│   ├── Frontend/
│   └── Database/
├── tests/
├── infrastructure/
│   ├── terraform/
│   └── arm-templates/
├── pipelines/
│   ├── azure-pipelines.yml
│   ├── build-pipeline.yml
│   └── release-pipeline.yml
└── docker/
    └── Dockerfile
```

#### 2.2 Clone Repository Locally
```bash
# Clone the repository
git clone https://dev.azure.com/YourOrg/ECommerceApp/_git/ECommerceApp
cd ECommerceApp

# Configure Git
git config user.name "Your Name"
git config user.email "your.email@company.com"
```

#### 2.3 Create Sample .NET Core Application
```csharp
// src/WebAPI/Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m },
            new Product { Id = 2, Name = "Phone", Price = 699.99m }
        };
        return Ok(products);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### 2.4 Create Dockerfile
```dockerfile
# docker/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WebAPI/WebAPI.csproj", "src/WebAPI/"]
RUN dotnet restore "src/WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
```

### 📤 Step 3: Push Code to Repository

#### 3.1 Initial Commit and Push
```bash
# Add all files
git add .

# Commit changes
git commit -m "feat: Initial project setup with .NET Core API

- Added ProductController with basic CRUD operations
- Configured Dockerfile for containerization
- Set up project structure for microservices
- Added basic unit tests framework

Closes #1"

# Push to main branch
git push origin main
```

#### 3.2 Create Feature Branch
```bash
# Create and switch to feature branch
git checkout -b feature/add-user-authentication

# Make changes (add authentication)
# ... code changes ...

# Commit and push feature
git add .
git commit -m "feat: Add JWT authentication

- Implemented JWT token generation
- Added user registration and login endpoints
- Configured authentication middleware
- Added authorization policies

Related to #2"

git push origin feature/add-user-authentication
```

### 🔄 Step 4: Create Pull Request

#### 4.1 Create Pull Request via Azure DevOps UI
```yaml
# Pull Request Details
Title: "Add JWT Authentication to Web API"
Description: |
  ## Changes Made
  - ✅ Implemented JWT token generation and validation
  - ✅ Added user registration and login endpoints
  - ✅ Configured authentication middleware
  - ✅ Added comprehensive unit tests
  
  ## Testing
  - [x] Unit tests pass (95% coverage)
  - [x] Integration tests pass
  - [x] Manual testing completed
  
  ## Security Considerations
  - JWT tokens expire in 24 hours
  - Passwords hashed using BCrypt
  - Rate limiting implemented for auth endpoints
  
  Closes #2

Source Branch: feature/add-user-authentication
Target Branch: main
Reviewers: team-leads, security-team
Work Items: #2
```

#### 4.2 Pull Request Policies
```json
{
  "branchPolicies": {
    "minimumNumberOfReviewers": 2,
    "creatorVoteCounts": false,
    "allowDownvotes": true,
    "resetOnSourcePush": true,
    "requireBuildToPass": true,
    "pathBasedPolicies": [
      {
        "path": "/src/WebAPI/*",
        "requiredReviewers": ["security-team"]
      }
    ]
  }
}
```

### 🔧 Step 5: Build Pipeline Creation

#### 5.1 Azure Pipeline YAML (azure-pipelines.yml)
```yaml
# pipelines/azure-pipelines.yml
trigger:
  branches:
    include:
    - main
    - develop
    - feature/*
  paths:
    include:
    - src/*
    - tests/*

variables:
  buildConfiguration: 'Release'
  vmImageName: 'ubuntu-latest'
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'

stages:
- stage: Build
  displayName: 'Build and Test'
  jobs:
  - job: Build
    displayName: 'Build Application'
    pool:
      vmImage: $(vmImageName)
    
    steps:
    - checkout: self
      fetchDepth: 0  # Shallow clones disabled for better analysis
    
    # Install .NET SDK
    - task: UseDotNet@2
      displayName: 'Install .NET 6 SDK'
      inputs:
        packageType: 'sdk'
        version: '6.0.x'
    
    # Restore NuGet packages
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '$(solution)'
        feedsToUse: 'select'
    
    # Build solution
    - task: DotNetCoreCLI@2
      displayName: 'Build solution'
      inputs:
        command: 'build'
        projects: '$(solution)'
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    # Run unit tests
    - task: DotNetCoreCLI@2
      displayName: 'Run unit tests'
      inputs:
        command: 'test'
        projects: '**/tests/**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage" --logger trx --no-build'
        publishTestResults: true
    
    # Code coverage
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish code coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    
    # Security scan
    - task: SonarCloudPrepare@1
      displayName: 'Prepare SonarCloud analysis'
      inputs:
        SonarCloud: 'SonarCloud-Connection'
        organization: 'your-org'
        scannerMode: 'MSBuild'
        projectKey: 'ecommerce-app'
    
    - task: SonarCloudAnalyze@1
      displayName: 'Run SonarCloud analysis'
    
    - task: SonarCloudPublish@1
      displayName: 'Publish SonarCloud results'
    
    # Docker build and push
    - task: Docker@2
      displayName: 'Build Docker image'
      inputs:
        containerRegistry: 'ACR-Connection'
        repository: 'ecommerce/webapi'
        command: 'buildAndPush'
        Dockerfile: 'docker/Dockerfile'
        tags: |
          $(Build.BuildNumber)
          latest
    
    # Publish artifacts
    - task: PublishBuildArtifacts@1
      displayName: 'Publish build artifacts'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'
        publishLocation: 'Container'

- stage: SecurityScan
  displayName: 'Security & Quality Gates'
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: SecurityScan
    displayName: 'Security Scanning'
    pool:
      vmImage: $(vmImageName)
    
    steps:
    - task: WhiteSource@21
      displayName: 'WhiteSource Scan'
      inputs:
        cwd: '$(System.DefaultWorkingDirectory)'
    
    - task: AzureKeyVault@2
      displayName: 'Get secrets from Key Vault'
      inputs:
        azureSubscription: 'Azure-Subscription'
        KeyVaultName: 'ecommerce-keyvault'
        SecretsFilter: '*'
        RunAsPreJob: false
```

#### 5.2 Build Pipeline Variables
```yaml
# Pipeline Variables (Set in Azure DevOps UI)
variables:
  # Build Configuration
  - name: buildConfiguration
    value: Release
  
  # Container Registry
  - name: containerRegistry
    value: ecommerceacr.azurecr.io
  
  # Service connections (created in Azure DevOps)
  - name: azureSubscription
    value: 'Azure-Subscription-Connection'
  
  # SonarCloud configuration
  - name: sonarCloudOrganization
    value: 'your-organization'
  
  # Environment-specific variables
  - name: environmentName
    value: development
```

### 🚀 Step 6: Release Pipeline Creation

#### 6.1 Release Pipeline YAML (release-pipeline.yml)
```yaml
# pipelines/release-pipeline.yml
trigger: none # Triggered by build pipeline completion

resources:
  pipelines:
  - pipeline: buildPipeline
    source: 'ECommerceApp-CI'
    trigger:
      branches:
        include:
        - main

variables:
  azureSubscription: 'Azure-Subscription-Connection'
  resourceGroupName: 'rg-ecommerce-$(environmentName)'
  webAppName: 'app-ecommerce-$(environmentName)'
  containerRegistry: 'ecommerceacr.azurecr.io'

stages:
- stage: DeployDevelopment
  displayName: 'Deploy to Development'
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  variables:
    environmentName: 'dev'
    deploymentSlot: 'staging'
  
  jobs:
  - deployment: DeployDev
    displayName: 'Deploy to Development Environment'
    environment: 'ecommerce-development'
    pool:
      vmImage: 'ubuntu-latest'
    
    strategy:
      runOnce:
        deploy:
          steps:
          - checkout: none
          
          # Download build artifacts
          - download: buildPipeline
            artifact: drop
          
          # Azure CLI setup
          - task: AzureCLI@2
            displayName: 'Deploy Infrastructure'
            inputs:
              azureSubscription: $(azureSubscription)
              scriptType: 'bash'
              scriptLocation: 'inlineScript'
              inlineScript: |
                # Create resource group
                az group create --name $(resourceGroupName) --location "East US"
                
                # Deploy ARM template
                az deployment group create \
                  --resource-group $(resourceGroupName) \
                  --template-file $(Pipeline.Workspace)/buildPipeline/drop/infrastructure/arm-templates/webapp.json \
                  --parameters webAppName=$(webAppName) environment=$(environmentName)
          
          # Deploy to Azure Web App
          - task: AzureWebAppContainer@1
            displayName: 'Deploy to Azure Web App'
            inputs:
              azureSubscription: $(azureSubscription)
              appName: $(webAppName)
              slotName: $(deploymentSlot)
              containers: '$(containerRegistry)/ecommerce/webapi:$(Build.BuildNumber)'
          
          # Run smoke tests
          - task: AzureCLI@2
            displayName: 'Run Smoke Tests'
            inputs:
              azureSubscription: $(azureSubscription)
              scriptType: 'bash'
              scriptLocation: 'inlineScript'
              inlineScript: |
                # Wait for app to be ready
                sleep 30
                
                # Health check
                response=$(curl -s -o /dev/null -w "%{http_code}" https://$(webAppName)-$(deploymentSlot).azurewebsites.net/health)
                if [ $response -eq 200 ]; then
                  echo "Health check passed"
                else
                  echo "Health check failed with status code: $response"
                  exit 1
                fi
          
          # Swap deployment slots
          - task: AzureAppServiceManage@0
            displayName: 'Swap Deployment Slots'
            inputs:
              azureSubscription: $(azureSubscription)
              WebAppName: $(webAppName)
              ResourceGroupName: $(resourceGroupName)
              SourceSlot: $(deploymentSlot)
              SwapWithProduction: true

- stage: DeployStaging
  displayName: 'Deploy to Staging'
  dependsOn: []
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
    environmentName: 'staging'
  
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging Environment'
    environment: 'ecommerce-staging'
    pool:
      vmImage: 'ubuntu-latest'
    
    strategy:
      runOnce:
        deploy:
          steps:
          # Similar to dev deployment with staging-specific configurations
          - template: templates/deploy-template.yml
            parameters:
              environment: 'staging'
              azureSubscription: $(azureSubscription)
              resourceGroupName: $(resourceGroupName)
              webAppName: $(webAppName)
              runLoadTests: true

- stage: DeployProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployStaging
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
    environmentName: 'prod'
  
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production Environment'
    environment: 'ecommerce-production'
    pool:
      vmImage: 'ubuntu-latest'
    
    strategy:
      runOnce:
        deploy:
          steps:
          # Pre-deployment approvals configured in environment
          - template: templates/deploy-template.yml
            parameters:
              environment: 'production'
              azureSubscription: $(azureSubscription)
              resourceGroupName: $(resourceGroupName)
              webAppName: $(webAppName)
              enableBlueGreenDeployment: true
              runSecurityTests: true
```

#### 6.2 Deployment Template (templates/deploy-template.yml)
```yaml
# pipelines/templates/deploy-template.yml
parameters:
- name: environment
  type: string
- name: azureSubscription
  type: string
- name: resourceGroupName
  type: string
- name: webAppName
  type: string
- name: runLoadTests
  type: boolean
  default: false
- name: enableBlueGreenDeployment
  type: boolean
  default: false
- name: runSecurityTests
  type: boolean
  default: false

steps:
- checkout: none

# Download artifacts
- download: buildPipeline
  artifact: drop

# Get secrets from Key Vault
- task: AzureKeyVault@2
  displayName: 'Get secrets from Key Vault'
  inputs:
    azureSubscription: ${{ parameters.azureSubscription }}
    KeyVaultName: 'kv-ecommerce-${{ parameters.environment }}'
    SecretsFilter: '*'

# Deploy infrastructure
- task: AzureResourceManagerTemplateDeployment@3
  displayName: 'Deploy ARM Template'
  inputs:
    deploymentScope: 'Resource Group'
    azureResourceManagerConnection: ${{ parameters.azureSubscription }}
    subscriptionId: '$(subscriptionId)'
    action: 'Create Or Update Resource Group'
    resourceGroupName: ${{ parameters.resourceGroupName }}
    location: 'East US'
    templateLocation: 'Linked artifact'
    csmFile: '$(Pipeline.Workspace)/buildPipeline/drop/infrastructure/arm-templates/main.json'
    overrideParameters: |
      -environment "${{ parameters.environment }}"
      -webAppName "${{ parameters.webAppName }}"
      -databaseConnectionString "$(DatabaseConnectionString)"

# Deploy application
- task: AzureWebAppContainer@1
  displayName: 'Deploy to Azure Web App'
  inputs:
    azureSubscription: ${{ parameters.azureSubscription }}
    appName: ${{ parameters.webAppName }}
    containers: '$(containerRegistry)/ecommerce/webapi:$(Build.BuildNumber)'
    appSettings: |
      -ASPNETCORE_ENVIRONMENT "${{ parameters.environment }}"
      -ConnectionStrings:DefaultConnection "$(DatabaseConnectionString)"
      -JwtSettings:SecretKey "$(JwtSecretKey)"

# Run load tests (if enabled)
- ${{ if eq(parameters.runLoadTests, true) }}:
  - task: AzureLoadTest@1
    displayName: 'Run Load Tests'
    inputs:
      azureSubscription: ${{ parameters.azureSubscription }}
      loadTestConfigFile: '$(Pipeline.Workspace)/buildPipeline/drop/tests/load-tests/config.yaml'
      loadTestResource: 'loadtest-ecommerce-${{ parameters.environment }}'

# Run security tests (if enabled)
- ${{ if eq(parameters.runSecurityTests, true) }}:
  - task: AzureCLI@2
    displayName: 'Run Security Tests'
    inputs:
      azureSubscription: ${{ parameters.azureSubscription }}
      scriptType: 'bash'
      scriptLocation: 'inlineScript'
      inlineScript: |
        # Run OWASP ZAP security scan
        docker run -t owasp/zap2docker-stable zap-baseline.py \
          -t https://${{ parameters.webAppName }}.azurewebsites.net \
          -J zap-report.json
```

---

## AWS CodePipeline Complete Implementation

### 🏗️ Step 1: AWS Setup & Repository Creation

#### 1.1 Create AWS CodeCommit Repository
```bash
# Install AWS CLI
aws configure
# Enter your Access Key ID, Secret Access Key, Region, and Output format

# Create CodeCommit repository
aws codecommit create-repository \
    --repository-name ecommerce-app \
    --repository-description "E-commerce application with microservices"

# Output will include clone URL
{
    "repositoryMetadata": {
        "repositoryName": "ecommerce-app",
        "cloneUrlHttp": "https://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app",
        "cloneUrlSsh": "ssh://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app"
    }
}
```

#### 1.2 Setup Local Repository
```bash
# Clone repository
git clone https://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app
cd ecommerce-app

# Create same project structure as Azure DevOps example
mkdir -p src/WebAPI tests infrastructure/cloudformation buildspec
```

#### 1.3 Create buildspec.yml for CodeBuild
```yaml
# buildspec/buildspec.yml
version: 0.2

phases:
  pre_build:
    commands:
      - echo "Starting pre-build phase"
      - echo "Logging in to Amazon ECR..."
      - aws ecr get-login-password --region $AWS_DEFAULT_REGION | docker login --username AWS --password-stdin $AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com
      - REPOSITORY_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com/$IMAGE_REPO_NAME
      - COMMIT_HASH=$(echo $CODEBUILD_RESOLVED_SOURCE_VERSION | cut -c 1-7)
      - IMAGE_TAG=${COMMIT_HASH:=latest}
      - echo "Repository URI = $REPOSITORY_URI"
      - echo "Image tag = $IMAGE_TAG"
      
  build:
    commands:
      - echo "Starting build phase"
      - echo "Installing .NET 6 SDK"
      - wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
      - dpkg -i packages-microsoft-prod.deb
      - apt-get update
      - apt-get install -y dotnet-sdk-6.0
      
      - echo "Restoring NuGet packages"
      - dotnet restore src/WebAPI/WebAPI.csproj
      
      - echo "Building application"
      - dotnet build src/WebAPI/WebAPI.csproj --configuration Release --no-restore
      
      - echo "Running unit tests"
      - dotnet test tests/ --configuration Release --no-build --verbosity normal
      
      - echo "Building Docker image"
      - docker build -t $IMAGE_REPO_NAME:$IMAGE_TAG -f docker/Dockerfile .
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:$IMAGE_TAG
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:latest
      
  post_build:
    commands:
      - echo "Starting post-build phase"
      - echo "Pushing Docker image to ECR"
      - docker push $REPOSITORY_URI:$IMAGE_TAG
      - docker push $REPOSITORY_URI:latest
      - echo "Writing image definitions file"
      - printf '[{"name":"ecommerce-webapi","imageUri":"%s"}]' $REPOSITORY_URI:$IMAGE_TAG > imagedefinitions.json
      - echo "Running security scan"
      - aws ecr start-image-scan --repository-name $IMAGE_REPO_NAME --image-id imageTag=$IMAGE_TAG

artifacts:
  files:
    - imagedefinitions.json
    - infrastructure/cloudformation/**/*
  name: BuildArtifact-$(date +%Y-%m-%d)

cache:
  paths:
    - '/root/.nuget/packages/**/*'
    - '/var/lib/docker/**/*'
```

### 📤 Step 2: Push Code to AWS CodeCommit

#### 2.1 Add Sample Application Code
```csharp
// src/WebAPI/Program.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddAWSProvider();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```

#### 2.2 Create CloudFormation Templates
```yaml
# infrastructure/cloudformation/ecs-service.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'ECS Service for E-commerce Application'

Parameters:
  Environment:
    Type: String
    Default: development
    AllowedValues:
      - development
      - staging
      - production
  
  ImageURI:
    Type: String
    Description: 'Docker image URI'
  
  VpcId:
    Type: String
    Description: 'VPC ID for the ECS service'
  
  SubnetIds:
    Type: CommaDelimitedList
    Description: 'Subnet IDs for the ECS service'

Resources:
  # ECS Cluster
  ECSCluster:
    Type: AWS::ECS::Cluster
    Properties:
      ClusterName: !Sub 'ecommerce-cluster-${Environment}'
      CapacityProviders:
        - FARGATE
        - FARGATE_SPOT
      DefaultCapacityProviderStrategy:
        - CapacityProvider: FARGATE
          Weight: 1
        - CapacityProvider: FARGATE_SPOT
          Weight: 2
  
  # Task Definition
  TaskDefinition:
    Type: AWS::ECS::TaskDefinition
    Properties:
      Family: !Sub 'ecommerce-webapi-${Environment}'
      NetworkMode: awsvpc
      RequiresCompatibilities:
        - FARGATE
      Cpu: 256
      Memory: 512
      ExecutionRoleArn: !Ref ExecutionRole
      TaskRoleArn: !Ref TaskRole
      ContainerDefinitions:
        - Name: ecommerce-webapi
          Image: !Ref ImageURI
          PortMappings:
            - ContainerPort: 80
              Protocol: tcp
          LogConfiguration:
            LogDriver: awslogs
            Options:
              awslogs-group: !Ref LogGroup
              awslogs-region: !Ref AWS::Region
              awslogs-stream-prefix: ecs
          Environment:
            - Name: ASPNETCORE_ENVIRONMENT
              Value: !Ref Environment
            - Name: AWS_REGION
              Value: !Ref AWS::Region
          HealthCheck:
            Command:
              - CMD-SHELL
              - "curl -f http://localhost:80/health || exit 1"
            Interval: 30
            Timeout: 5
            Retries: 3
            StartPeriod: 60
  
  # ECS Service
  ECSService:
    Type: AWS::ECS::Service
    DependsOn: LoadBalancerListener
    Properties:
      ServiceName: !Sub 'ecommerce-webapi-${Environment}'
      Cluster: !Ref ECSCluster
      TaskDefinition: !Ref TaskDefinition
      DesiredCount: 2
      LaunchType: FARGATE
      NetworkConfiguration:
        AwsvpcConfiguration:
          SecurityGroups:
            - !Ref ServiceSecurityGroup
          Subnets: !Ref SubnetIds
          AssignPublicIp: ENABLED
      LoadBalancers:
        - ContainerName: ecommerce-webapi
          ContainerPort: 80
          TargetGroupArn: !Ref TargetGroup
      DeploymentConfiguration:
        MaximumPercent: 200
        MinimumHealthyPercent: 50
        DeploymentCircuitBreaker:
          Enable: true
          Rollback: true
  
  # Application Load Balancer
  LoadBalancer:
    Type: AWS::ElasticLoadBalancingV2::LoadBalancer
    Properties:
      Name: !Sub 'ecommerce-alb-${Environment}'
      Subnets: !Ref SubnetIds
      SecurityGroups:
        - !Ref LoadBalancerSecurityGroup
      Type: application
      IpAddressType: ipv4
      
  LoadBalancerListener:
    Type: AWS::ElasticLoadBalancingV2::Listener
    Properties:
      DefaultActions:
        - Type: forward
          TargetGroupArn: !Ref TargetGroup
      LoadBalancerArn: !Ref LoadBalancer
      Port: 80
      Protocol: HTTP
  
  TargetGroup:
    Type: AWS::ElasticLoadBalancingV2::TargetGroup
    Properties:
      Name: !Sub 'ecommerce-tg-${Environment}'
      Port: 80
      Protocol: HTTP
      VpcId: !Ref VpcId
      TargetType: ip
      HealthCheckPath: /health
      HealthCheckProtocol: HTTP
      HealthCheckIntervalSeconds: 30
      HealthCheckTimeoutSeconds: 5
      HealthyThresholdCount: 2
      UnhealthyThresholdCount: 5

  # Auto Scaling
  AutoScalingTarget:
    Type: AWS::ApplicationAutoScaling::ScalableTarget
    Properties:
      MaxCapacity: 10
      MinCapacity: 2
      ResourceId: !Sub 'service/${ECSCluster}/${ECSService.Name}'
      RoleARN: !GetAtt AutoScalingRole.Arn
      ScalableDimension: ecs:service:DesiredCount
      ServiceNamespace: ecs
  
  AutoScalingPolicy:
    Type: AWS::ApplicationAutoScaling::ScalingPolicy
    Properties:
      PolicyName: !Sub 'ecommerce-scaling-policy-${Environment}'
      PolicyType: TargetTrackingScaling
      ScalingTargetId: !Ref AutoScalingTarget
      TargetTrackingScalingPolicyConfiguration:
        PredefinedMetricSpecification:
          PredefinedMetricType: ECSServiceAverageCPUUtilization
        TargetValue: 70
        ScaleOutCooldown: 300
        ScaleInCooldown: 300

  # IAM Roles
  ExecutionRole:
    Type: AWS::IAM::Role
    Properties:
      RoleName: !Sub 'ecommerce-execution-role-${Environment}'
      AssumeRolePolicyDocument:
        Statement:
          - Effect: Allow
            Principal:
              Service: ecs-tasks.amazonaws.com
            Action: 'sts:AssumeRole'
      ManagedPolicyArns:
        - 'arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy'

  TaskRole:
    Type: AWS::IAM::Role
    Properties:
      RoleName: !Sub 'ecommerce-task-role-${Environment}'
      AssumeRolePolicyDocument:
        Statement:
          - Effect: Allow
            Principal:
              Service: ecs-tasks.amazonaws.com
            Action: 'sts:AssumeRole'
      Policies:
        - PolicyName: CloudWatchLogs
          PolicyDocument:
            Statement:
              - Effect: Allow
                Action:
                  - 'logs:CreateLogGroup'
                  - 'logs:CreateLogStream'
                  - 'logs:PutLogEvents'
                Resource: '*'

  # Security Groups
  ServiceSecurityGroup:
    Type: AWS::EC2::SecurityGroup
    Properties:
      GroupDescription: Security group for ECS service
      VpcId: !Ref VpcId
      SecurityGroupIngress:
        - IpProtocol: tcp
          FromPort: 80
          ToPort: 80
          SourceSecurityGroupId: !Ref LoadBalancerSecurityGroup

  LoadBalancerSecurityGroup:
    Type: AWS::EC2::SecurityGroup
    Properties:
      GroupDescription: Security group for Application Load Balancer
      VpcId: !Ref VpcId
      SecurityGroupIngress:
        - IpProtocol: tcp
          FromPort: 80
          ToPort: 80
          CidrIp: 0.0.0.0/0

  # CloudWatch Log Group
  LogGroup:
    Type: AWS::Logs::LogGroup
    Properties:
      LogGroupName: !Sub '/ecs/ecommerce-webapi-${Environment}'
      RetentionInDays: 14

Outputs:
  LoadBalancerURL:
    Description: 'Load Balancer URL'
    Value: !Sub 'http://${LoadBalancer.DNSName}'
    Export:
      Name: !Sub '${AWS::StackName}-LoadBalancerURL'
  
  ECSClusterName:
    Description: 'ECS Cluster Name'
    Value: !Ref ECSCluster
    Export:
      Name: !Sub '${AWS::StackName}-ECSCluster'
```

#### 2.3 Commit and Push Code
```bash
# Add all files
git add .

# Commit changes
git commit -m "feat: Initial AWS infrastructure setup

- Added .NET Core Web API with health checks
- Configured ECS Fargate deployment with CloudFormation
- Implemented Application Load Balancer with auto-scaling
- Added comprehensive buildspec.yml for CodeBuild
- Configured CloudWatch logging and monitoring

Features:
- Auto-scaling based on CPU utilization
- Blue-green deployment capability
- Security groups with least privilege access
- Multi-AZ deployment for high availability"

# Push to main branch
git push origin main
```

### 🔧 Step 3: Create AWS CodeBuild Project

#### 3.1 Create CodeBuild Project via AWS CLI
```bash
# Create build project
aws codebuild create-project \
    --name "ecommerce-build" \
    --description "Build project for E-commerce application" \
    --source '{
        "type": "CODECOMMIT",
        "location": "https://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app",
        "buildspec": "buildspec/buildspec.yml"
    }' \
    --artifacts '{
        "type": "S3",
        "location": "ecommerce-build-artifacts",
        "name": "build-output"
    }' \
    --environment '{
        "type": "LINUX_CONTAINER",
        "image": "aws/codebuild/amazonlinux2-x86_64-standard:3.0",
        "computeType": "BUILD_GENERAL1_MEDIUM",
        "privilegedMode": true,
        "environmentVariables": [
            {
                "name": "AWS_DEFAULT_REGION",
                "value": "us-east-1"
            },
            {
                "name": "AWS_ACCOUNT_ID",
                "value": "123456789012"
            },
            {
                "name": "IMAGE_REPO_NAME",
                "value": "ecommerce-webapi"
            }
        ]
    }' \
    --service-role "arn:aws:iam::123456789012:role/service-role/codebuild-service-role"
```

#### 3.2 Advanced buildspec.yml with Multiple Environments
```yaml
# buildspec/buildspec-advanced.yml
version: 0.2

env:
  variables:
    DOTNET_CORE_RUNTIME: '6.0'
  parameter-store:
    SONAR_TOKEN: '/ecommerce/sonar/token'
    SNYK_TOKEN: '/ecommerce/snyk/token'
  exported-variables:
    - IMAGE_TAG
    - REPOSITORY_URI

phases:
  install:
    runtime-versions:
      docker: 20
    commands:
      - echo "Installing additional dependencies"
      - curl -fsSL https://get.docker.com -o get-docker.sh
      - sh get-docker.sh
      - pip3 install awscli --upgrade
      
  pre_build:
    commands:
      - echo "Starting pre-build phase on `date`"
      - echo "Current directory: $(pwd)"
      - echo "Listing files:"
      - ls -la
      
      # Login to ECR
      - echo "Logging in to Amazon ECR..."
      - aws ecr get-login-password --region $AWS_DEFAULT_REGION | docker login --username AWS --password-stdin $AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com
      
      # Set image variables
      - REPOSITORY_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com/$IMAGE_REPO_NAME
      - COMMIT_HASH=$(echo $CODEBUILD_RESOLVED_SOURCE_VERSION | cut -c 1-7)
      - BUILD_NUMBER=${CODEBUILD_BUILD_NUMBER}
      - IMAGE_TAG=${COMMIT_HASH}-${BUILD_NUMBER}
      
      # Create ECR repository if it doesn't exist
      - aws ecr describe-repositories --repository-names $IMAGE_REPO_NAME || aws ecr create-repository --repository-name $IMAGE_REPO_NAME
      
      # Install .NET SDK
      - echo "Installing .NET SDK"
      - wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
      - dpkg -i packages-microsoft-prod.deb
      - apt-get update
      - apt-get install -y dotnet-sdk-6.0
      
      # Install SonarScanner
      - echo "Installing SonarScanner"
      - dotnet tool install --global dotnet-sonarscanner
      - export PATH="$PATH:/root/.dotnet/tools"
      
  build:
    commands:
      - echo "Starting build phase on `date`"
      
      # Start SonarQube analysis
      - echo "Starting SonarQube analysis"
      - dotnet sonarscanner begin /k:"ecommerce-app" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login="$SONAR_TOKEN" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
      
      # Restore dependencies
      - echo "Restoring NuGet packages"
      - dotnet restore src/WebAPI/WebAPI.csproj --verbosity normal
      
      # Build application
      - echo "Building application"
      - dotnet build src/WebAPI/WebAPI.csproj --configuration Release --no-restore --verbosity normal
      
      # Run unit tests with coverage
      - echo "Running unit tests with coverage"
      - dotnet test tests/ --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./TestResults/ --logger trx
      
      # Convert coverage to OpenCover format for SonarQube
      - dotnet tool install -g dotnet-reportgenerator-globaltool
      - reportgenerator "-reports:TestResults/*/coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:OpenCover"
      
      # Complete SonarQube analysis
      - dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
      
      # Security scanning with Snyk
      - echo "Running security scan with Snyk"
      - npm install -g snyk
      - snyk auth $SNYK_TOKEN
      - snyk test --file=src/WebAPI/WebAPI.csproj --severity-threshold=high
      
      # Build Docker image
      - echo "Building Docker image"
      - echo "Repository URI: $REPOSITORY_URI"
      - echo "Image tag: $IMAGE_TAG"
      - docker build -t $IMAGE_REPO_NAME:$IMAGE_TAG -f docker/Dockerfile .
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:$IMAGE_TAG
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:latest
      
      # Docker security scan
      - echo "Running Docker security scan"
      - docker run --rm -v /var/run/docker.sock:/var/run/docker.sock -v $(pwd):/app anchore/syft /app -o json > sbom.json
      - docker run --rm -v $(pwd):/app anchore/grype sbom:sbom.json
      
  post_build:
    commands:
      - echo "Starting post-build phase on `date`"
      
      # Push image to ECR
      - echo "Pushing Docker image to ECR"
      - docker push $REPOSITORY_URI:$IMAGE_TAG
      - docker push $REPOSITORY_URI:latest
      
      # Start ECR image scan
      - echo "Starting ECR image vulnerability scan"
      - aws ecr start-image-scan --repository-name $IMAGE_REPO_NAME --image-id imageTag=$IMAGE_TAG || true
      
      # Wait for scan to complete and get results
      - echo "Waiting for vulnerability scan to complete"
      - sleep 30
      - aws ecr describe-image-scan-findings --repository-name $IMAGE_REPO_NAME --image-id imageTag=$IMAGE_TAG || true
      
      # Create image definitions for CodeDeploy
      - echo "Creating image definitions file"
      - printf '[{"name":"ecommerce-webapi","imageUri":"%s"}]' $REPOSITORY_URI:$IMAGE_TAG > imagedefinitions.json
      - cat imagedefinitions.json
      
      # Update parameter store with new image URI
      - echo "Updating parameter store with new image URI"
      - aws ssm put-parameter --name "/ecommerce/webapi/image-uri" --value "$REPOSITORY_URI:$IMAGE_TAG" --overwrite
      
      # Create CloudFormation parameters file
      - echo "Creating CloudFormation parameters file"
      - cat > infrastructure/parameters.json << EOF
        [
          {
            "ParameterKey": "ImageURI",
            "ParameterValue": "$REPOSITORY_URI:$IMAGE_TAG"
          },
          {
            "ParameterKey": "Environment",
            "ParameterValue": "${ENVIRONMENT:-development}"
          }
        ]
        EOF
      
      # Package CloudFormation templates
      - echo "Packaging CloudFormation templates"
      - aws cloudformation package \
          --template-file infrastructure/cloudformation/ecs-service.yml \
          --s3-bucket ecommerce-cloudformation-templates \
          --output-template-file infrastructure/packaged-template.yml

reports:
  SonarQubeReport:
    files:
      - 'sonar-report.json'
    base-directory: '.'
  TestResults:
    files:
      - '**/*'
    base-directory: 'TestResults'
    file-format: 'VisualStudioTrx'
  CodeCoverage:
    files:
      - 'coverage/Cobertura.xml'
    file-format: 'CoberturaXml'

artifacts:
  files:
    - imagedefinitions.json
    - infrastructure/packaged-template.yml
    - infrastructure/parameters.json
    - infrastructure/cloudformation/**/*
  secondary-artifacts:
    BuildArtifact:
      files:
        - imagedefinitions.json
        - infrastructure/packaged-template.yml
        - infrastructure/parameters.json
      name: BuildArtifact-$IMAGE_TAG
    SourceArtifact:
      files:
        - '**/*'
      name: SourceArtifact-$IMAGE_TAG

cache:
  paths:
    - '/root/.nuget/packages/**/*'
    - '/root/.dotnet/tools/**/*'
    - '/var/lib/docker/**/*'
    - 'node_modules/**/*'
```

### 🚀 Step 4: Create AWS CodePipeline

#### 4.1 CodePipeline CloudFormation Template
```yaml
# infrastructure/cloudformation/codepipeline.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Complete CI/CD Pipeline for E-commerce Application'

Parameters:
  GitHubOwner:
    Type: String
    Description: 'GitHub repository owner'
    Default: 'your-github-username'
  
  GitHubRepo:
    Type: String
    Description: 'GitHub repository name'
    Default: 'ecommerce-app'
  
  GitHubToken:
    Type: String
    NoEcho: true
    Description: 'GitHub personal access token'
  
  SlackWebhookUrl:
    Type: String
    NoEcho: true
    Description: 'Slack webhook URL for notifications'

Resources:
  # S3 Bucket for artifacts
  ArtifactsBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub 'ecommerce-pipeline-artifacts-${AWS::AccountId}-${AWS::Region}'
      VersioningConfiguration:
        Status: Enabled
      BucketEncryption:
        ServerSideEncryptionConfiguration:
          - ServerSideEncryptionByDefault:
              SSEAlgorithm: AES256
      PublicAccessBlockConfiguration:
        BlockPublicAcls: true
        BlockPublicPolicy: true
        IgnorePublicAcls: true
        RestrictPublicBuckets: true
      LifecycleConfiguration:
        Rules:
          - Id: DeleteOldArtifacts
            Status: Enabled
            ExpirationInDays: 30

  # CodePipeline Service Role
  CodePipelineServiceRole:
    Type: AWS::IAM::Role
    Properties:
      RoleName: !Sub 'CodePipelineServiceRole-${AWS::StackName}'
      AssumeRolePolicyDocument:
        Statement:
          - Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: CodePipelineServiceRolePolicy
          PolicyDocument:
            Statement:
              - Effect: Allow
                Action:
                  - s3:GetBucketVersioning
                  - s3:GetObject
                  - s3:GetObjectVersion
                  - s3:PutObject
                Resource:
                  - !Sub '${ArtifactsBucket}/*'
                  - !Ref ArtifactsBucket
              - Effect: Allow
                Action:
                  - codebuild:BatchGetBuilds
                  - codebuild:StartBuild
                Resource: '*'
              - Effect: Allow
                Action:
                  - cloudformation:CreateStack
                  - cloudformation:DeleteStack
                  - cloudformation:DescribeStacks
                  - cloudformation:UpdateStack
                  - cloudformation:CreateChangeSet
                  - cloudformation:DeleteChangeSet
                  - cloudformation:DescribeChangeSet
                  - cloudformation:ExecuteChangeSet
                  - cloudformation:SetStackPolicy
                  - cloudformation:ValidateTemplate
                Resource: '*'
              - Effect: Allow
                Action:
                  - iam:PassRole
                Resource: '*'
              - Effect: Allow
                Action:
                  - ecs:DescribeServices
                  - ecs:DescribeTaskDefinition
                  - ecs:DescribeTasks
                  - ecs:ListTasks
                  - ecs:RegisterTaskDefinition
                  - ecs:UpdateService
                Resource: '*'
              - Effect: Allow
                Action:
                  - sns:Publish
                Resource: '*'

  # CodeBuild Projects
  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub 'ecommerce-build-${AWS::StackName}'
      Description: 'Build project for E-commerce application'
      ServiceRole: !GetAtt CodeBuildServiceRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_MEDIUM
        Image: aws/codebuild/amazonlinux2-x86_64-standard:3.0
        PrivilegedMode: true
        EnvironmentVariables:
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
          - Name: AWS_ACCOUNT_ID
            Value: !Ref AWS::AccountId
          - Name: IMAGE_REPO_NAME
            Value: ecommerce-webapi
          - Name: ENVIRONMENT
            Value: development
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          phases:
            pre_build:
              commands:
                - echo "Starting pre-build phase"
                - aws ecr get-login-password --region $AWS_DEFAULT_REGION | docker login --username AWS --password-stdin $AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com
                - REPOSITORY_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com/$IMAGE_REPO_NAME
                - COMMIT_HASH=$(echo $CODEBUILD_RESOLVED_SOURCE_VERSION | cut -c 1-7)
                - IMAGE_TAG=${COMMIT_HASH:=latest}
            build:
              commands:
                - echo "Starting build phase"
                - docker build -t $IMAGE_REPO_NAME:$IMAGE_TAG .
                - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:$IMAGE_TAG
                - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:latest
            post_build:
              commands:
                - echo "Starting post-build phase"
                - docker push $REPOSITORY_URI:$IMAGE_TAG
                - docker push $REPOSITORY_URI:latest
                - printf '[{"name":"ecommerce-webapi","imageUri":"%s"}]' $REPOSITORY_URI:$IMAGE_TAG > imagedefinitions.json
          artifacts:
            files:
              - imagedefinitions.json
              - infrastructure/**/*

  # Deploy to Development Environment
  DeployDevProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub 'ecommerce-deploy-dev-${AWS::StackName}'
      Description: 'Deploy to development environment'
      ServiceRole: !GetAtt CodeBuildServiceRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_SMALL
        Image: aws/codebuild/amazonlinux2-x86_64-standard:3.0
        EnvironmentVariables:
          - Name: ENVIRONMENT
            Value: development
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          phases:
            pre_build:
              commands:
                - echo "Preparing deployment to development environment"
            build:
              commands:
                - echo "Deploying CloudFormation stack"
                - aws cloudformation deploy
                    --template-file infrastructure/cloudformation/ecs-service.yml
                    --stack-name ecommerce-dev-stack
                    --parameter-overrides Environment=development
                    --capabilities CAPABILITY_IAM
                    --region $AWS_DEFAULT_REGION
            post_build:
              commands:
                - echo "Deployment completed"
                - aws ecs wait services-stable --cluster ecommerce-cluster-development --services ecommerce-webapi-development

  # CodePipeline
  Pipeline:
    Type: AWS::CodePipeline::Pipeline
    Properties:
      Name: !Sub 'ecommerce-pipeline-${AWS::StackName}'
      RoleArn: !GetAtt CodePipelineServiceRole.Arn
      ArtifactStore:
        Type: S3
        Location: !Ref ArtifactsBucket
      Stages:
        # Source Stage
        - Name: Source
          Actions:
            - Name: SourceAction
              ActionTypeId:
                Category: Source
                Owner: ThirdParty
                Provider: GitHub
                Version: '1'
              Configuration:
                Owner: !Ref GitHubOwner
                Repo: !Ref GitHubRepo
                Branch: main
                OAuthToken: !Ref GitHubToken
                PollForSourceChanges: false
              OutputArtifacts:
                - Name: SourceOutput

        # Build Stage
        - Name: Build
          Actions:
            - Name: BuildAction
              ActionTypeId:
                Category: Build
                Owner: AWS
                Provider: CodeBuild
                Version: '1'
              Configuration:
                ProjectName: !Ref BuildProject
              InputArtifacts:
                - Name: SourceOutput
              OutputArtifacts:
                - Name: BuildOutput
              RunOrder: 1

        # Security and Quality Gates
        - Name: SecurityScan
          Actions:
            - Name: SecurityScanAction
              ActionTypeId:
                Category: Invoke
                Owner: AWS
                Provider: Lambda
                Version: '1'
              Configuration:
                FunctionName: !Ref SecurityScanFunction
              InputArtifacts:
                - Name: BuildOutput
              RunOrder: 1

        # Deploy to Development
        - Name: DeployDev
          Actions:
            - Name: CreateChangeSet
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CloudFormation
                Version: '1'
              Configuration:
                ActionMode: CHANGE_SET_REPLACE
                Capabilities: CAPABILITY_IAM
                ChangeSetName: pipeline-changeset
                StackName: ecommerce-dev-stack
                TemplatePath: BuildOutput::infrastructure/cloudformation/ecs-service.yml
                ParameterOverrides: |
                  {
                    "Environment": "development",
                    "ImageURI": { "Fn::GetParam": ["BuildOutput", "imagedefinitions.json", "imageUri"] }
                  }
                RoleArn: !GetAtt CloudFormationServiceRole.Arn
              InputArtifacts:
                - Name: BuildOutput
              RunOrder: 1

            - Name: ExecuteChangeSet
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CloudFormation
                Version: '1'
              Configuration:
                ActionMode: CHANGE_SET_EXECUTE
                ChangeSetName: pipeline-changeset
                StackName: ecommerce-dev-stack
              RunOrder: 2

        # Manual Approval for Production
        - Name: ApprovalForProduction
          Actions:
            - Name: ManualApproval
              ActionTypeId:
                Category: Approval
                Owner: AWS
                Provider: Manual
                Version: '1'
              Configuration:
                NotificationArn: !Ref ApprovalTopic
                CustomData: |
                  Please review the application in development environment and approve for production deployment.
                  Development URL: http://ecommerce-alb-development.us-east-1.elb.amazonaws.com
              RunOrder: 1

        # Deploy to Production
        - Name: DeployProd
          Actions:
            - Name: CreateProdChangeSet
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CloudFormation
                Version: '1'
              Configuration:
                ActionMode: CHANGE_SET_REPLACE
                Capabilities: CAPABILITY_IAM
                ChangeSetName: prod-pipeline-changeset
                StackName: ecommerce-prod-stack
                TemplatePath: BuildOutput::infrastructure/cloudformation/ecs-service.yml
                ParameterOverrides: |
                  {
                    "Environment": "production",
                    "ImageURI": { "Fn::GetParam": ["BuildOutput", "imagedefinitions.json", "imageUri"] }
                  }
                RoleArn: !GetAtt CloudFormationServiceRole.Arn
              InputArtifacts:
                - Name: BuildOutput
              RunOrder: 1

            - Name: ExecuteProdChangeSet
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: CloudFormation
                Version: '1'
              Configuration:
                ActionMode: CHANGE_SET_EXECUTE
                ChangeSetName: prod-pipeline-changeset
                StackName: ecommerce-prod-stack
              RunOrder: 2

  # GitHub Webhook
  GitHubWebhook:
    Type: AWS::CodePipeline::Webhook
    Properties:
      Authentication: GITHUB_HMAC
      AuthenticationConfiguration:
        SecretToken: !Ref GitHubToken
      Filters:
        - JsonPath: $.ref
          MatchEquals: 'refs/heads/main'
      TargetPipeline: !Ref Pipeline
      TargetAction: SourceAction
      TargetPipelineVersion: !GetAtt Pipeline.Version
      RegisterWithThirdParty: true

  # SNS Topic for notifications
  ApprovalTopic:
    Type: AWS::SNS::Topic
    Properties:
      TopicName: !Sub 'ecommerce-approvals-${AWS::StackName}'
      Subscription:
        - Protocol: email
          Endpoint: admin@company.com

  # Lambda function for security scanning
  SecurityScanFunction:
    Type: AWS::Lambda::Function
    Properties:
      FunctionName: !Sub 'ecommerce-security-scan-${AWS::StackName}'
      Runtime: python3.9
      Handler: index.lambda_handler
      Role: !GetAtt SecurityScanRole.Arn
      Code:
        ZipFile: |
          import json
          import boto3
          import logging

          logger = logging.getLogger()
          logger.setLevel(logging.INFO)

          def lambda_handler(event, context):
              """
              Perform security scanning on the build artifacts
              """
              try:
                  codepipeline = boto3.client('codepipeline')
                  job_id = event['CodePipeline.job']['id']
                  
                  # Perform security checks here
                  # This is a simplified example - in production, you'd integrate with
                  # tools like Snyk, OWASP Dependency Check, etc.
                  
                  security_passed = True
                  
                  if security_passed:
                      logger.info("Security scan passed")
                      codepipeline.put_job_success_result(jobId=job_id)
                  else:
                      logger.error("Security scan failed")
                      codepipeline.put_job_failure_result(
                          jobId=job_id,
                          failureDetails={'message': 'Security vulnerabilities detected', 'type': 'JobFailed'}
                      )
                  
                  return {
                      'statusCode': 200,
                      'body': json.dumps('Security scan completed')
                  }
                  
              except Exception as e:
                  logger.error(f"Error in security scan: {str(e)}")
                  codepipeline.put_job_failure_result(
                      jobId=event['CodePipeline.job']['id'],
                      failureDetails={'message': str(e), 'type': 'JobFailed'}
                  )
                  return {
                      'statusCode': 500,
                      'body': json.dumps(f'Error: {str(e)}')
                  }

Outputs:
  PipelineName:
    Description: 'Name of the created pipeline'
    Value: !Ref Pipeline
    Export:
      Name: !Sub '${AWS::StackName}-PipelineName'
  
  ArtifactsBucket:
    Description: 'S3 bucket for pipeline artifacts'
    Value: !Ref ArtifactsBucket
    Export:
      Name: !Sub '${AWS::StackName}-ArtifactsBucket'
```

### 📊 Step 5: Monitoring and Notifications

#### 5.1 CloudWatch Monitoring Setup
```yaml
# infrastructure/cloudformation/monitoring.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Monitoring and alerting for E-commerce CI/CD pipeline'

Parameters:
  SlackWebhookUrl:
    Type: String
    NoEcho: true
    Description: 'Slack webhook URL for notifications'

Resources:
  # CloudWatch Dashboard
  PipelineDashboard:
    Type: AWS::CloudWatch::Dashboard
    Properties:
      DashboardName: 'ECommerce-CICD-Dashboard'
      DashboardBody: !Sub |
        {
          "widgets": [
            {
              "type": "metric",
              "x": 0,
              "y": 0,
              "width": 12,
              "height": 6,
              "properties": {
                "metrics": [
                  [ "AWS/CodePipeline", "PipelineExecutionSuccess", "PipelineName", "ecommerce-pipeline" ],
                  [ ".", "PipelineExecutionFailure", ".", "." ]
                ],
                "view": "timeSeries",
                "stacked": false,
                "region": "${AWS::Region}",
                "title": "Pipeline Execution Status",
                "period": 300
              }
            },
            {
              "type": "metric",
              "x": 0,
              "y": 6,
              "width": 12,
              "height": 6,
              "properties": {
                "metrics": [
                  [ "AWS/CodeBuild", "Builds", "ProjectName", "ecommerce-build" ],
                  [ ".", "FailedBuilds", ".", "." ],
                  [ ".", "SucceededBuilds", ".", "." ]
                ],
                "view": "timeSeries",
                "stacked": false,
                "region": "${AWS::Region}",
                "title": "Build Status",
                "period": 300
              }
            },
            {
              "type": "log",
              "x": 0,
              "y": 12,
              "width": 24,
              "height": // filepath: CI-CD-Azure-AWS-Guide.md
# Complete CI/CD Guide: Azure DevOps & AWS
*Comprehensive Step-by-Step Implementation with Real-World Examples*

## Table of Contents
1. [Azure DevOps Complete Implementation](#azure-devops-complete-implementation)
2. [AWS CodePipeline Complete Implementation](#aws-codepipeline-complete-implementation)
3. [YAML Pipeline Configurations](#yaml-pipeline-configurations)
4. [Best Practices & Troubleshooting](#best-practices--troubleshooting)

---

## Azure DevOps Complete Implementation

### 🏗️ Step 1: Create Azure DevOps Organization & Project

#### 1.1 Create Organization
```bash
# Navigate to Azure DevOps
https://dev.azure.com

# Create new organization
Organization Name: YourCompany-DevOps
Region: Central US (or your preferred region)
```

#### 1.2 Create Project
```json
{
  "projectName": "ECommerceApp",
  "description": "E-commerce application with microservices",
  "visibility": "private",
  "processTemplate": "Agile",
  "versionControl": "Git"
}
```

### 🗂️ Step 2: Repository Setup & Code Management

#### 2.1 Create Repository
```bash
# In Azure DevOps Project
# Navigate to Repos → Files → Initialize with README

# Repository Structure
ECommerceApp/
├── src/
│   ├── WebAPI/
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Startup.cs
│   ├── Frontend/
│   └── Database/
├── tests/
├── infrastructure/
│   ├── terraform/
│   └── arm-templates/
├── pipelines/
│   ├── azure-pipelines.yml
│   ├── build-pipeline.yml
│   └── release-pipeline.yml
└── docker/
    └── Dockerfile
```

#### 2.2 Clone Repository Locally
```bash
# Clone the repository
git clone https://dev.azure.com/YourOrg/ECommerceApp/_git/ECommerceApp
cd ECommerceApp

# Configure Git
git config user.name "Your Name"
git config user.email "your.email@company.com"
```

#### 2.3 Create Sample .NET Core Application
```csharp
// src/WebAPI/Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m },
            new Product { Id = 2, Name = "Phone", Price = 699.99m }
        };
        return Ok(products);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### 2.4 Create Dockerfile
```dockerfile
# docker/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WebAPI/WebAPI.csproj", "src/WebAPI/"]
RUN dotnet restore "src/WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
```

### 📤 Step 3: Push Code to Repository

#### 3.1 Initial Commit and Push
```bash
# Add all files
git add .

# Commit changes
git commit -m "feat: Initial project setup with .NET Core API

- Added ProductController with basic CRUD operations
- Configured Dockerfile for containerization
- Set up project structure for microservices
- Added basic unit tests framework

Closes #1"

# Push to main branch
git push origin main
```

#### 3.2 Create Feature Branch
```bash
# Create and switch to feature branch
git checkout -b feature/add-user-authentication

# Make changes (add authentication)
# ... code changes ...

# Commit and push feature
git add .
git commit -m "feat: Add JWT authentication

- Implemented JWT token generation
- Added user registration and login endpoints
- Configured authentication middleware
- Added authorization policies

Related to #2"

git push origin feature/add-user-authentication
```

### 🔄 Step 4: Create Pull Request

#### 4.1 Create Pull Request via Azure DevOps UI
```yaml
# Pull Request Details
Title: "Add JWT Authentication to Web API"
Description: |
  ## Changes Made
  - ✅ Implemented JWT token generation and validation
  - ✅ Added user registration and login endpoints
  - ✅ Configured authentication middleware
  - ✅ Added comprehensive unit tests
  
  ## Testing
  - [x] Unit tests pass (95% coverage)
  - [x] Integration tests pass
  - [x] Manual testing completed
  
  ## Security Considerations
  - JWT tokens expire in 24 hours
  - Passwords hashed using BCrypt
  - Rate limiting implemented for auth endpoints
  
  Closes #2

Source Branch: feature/add-user-authentication
Target Branch: main
Reviewers: team-leads, security-team
Work Items: #2
```

#### 4.2 Pull Request Policies
```json
{
  "branchPolicies": {
    "minimumNumberOfReviewers": 2,
    "creatorVoteCounts": false,
    "allowDownvotes": true,
    "resetOnSourcePush": true,
    "requireBuildToPass": true,
    "pathBasedPolicies": [
      {
        "path": "/src/WebAPI/*",
        "requiredReviewers": ["security-team"]
      }
    ]
  }
}
```

### 🔧 Step 5: Build Pipeline Creation

#### 5.1 Azure Pipeline YAML (azure-pipelines.yml)
```yaml
# pipelines/azure-pipelines.yml
trigger:
  branches:
    include:
    - main
    - develop
    - feature/*
  paths:
    include:
    - src/*
    - tests/*

variables:
  buildConfiguration: 'Release'
  vmImageName: 'ubuntu-latest'
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'

stages:
- stage: Build
  displayName: 'Build and Test'
  jobs:
  - job: Build
    displayName: 'Build Application'
    pool:
      vmImage: $(vmImageName)
    
    steps:
    - checkout: self
      fetchDepth: 0  # Shallow clones disabled for better analysis
    
    # Install .NET SDK
    - task: UseDotNet@2
      displayName: 'Install .NET 6 SDK'
      inputs:
        packageType: 'sdk'
        version: '6.0.x'
    
    # Restore NuGet packages
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '$(solution)'
        feedsToUse: 'select'
    
    # Build solution
    - task: DotNetCoreCLI@2
      displayName: 'Build solution'
      inputs:
        command: 'build'
        projects: '$(solution)'
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    # Run unit tests
    - task: DotNetCoreCLI@2
      displayName: 'Run unit tests'
      inputs:
        command: 'test'
        projects: '**/tests/**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage" --logger trx --no-build'
        publishTestResults: true
    
    # Code coverage
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish code coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    
    # Security scan
    - task: SonarCloudPrepare@1
      displayName: 'Prepare SonarCloud analysis'
      inputs:
        SonarCloud: 'SonarCloud-Connection'
        organization: 'your-org'
        scannerMode: 'MSBuild'
        projectKey: 'ecommerce-app'
    
    - task: SonarCloudAnalyze@1
      displayName: 'Run SonarCloud analysis'
    
    - task: SonarCloudPublish@1
      displayName: 'Publish SonarCloud results'
    
    # Docker build and push
    - task: Docker@2
      displayName: 'Build Docker image'
      inputs:
        containerRegistry: 'ACR-Connection'
        repository: 'ecommerce/webapi'
        command: 'buildAndPush'
        Dockerfile: 'docker/Dockerfile'
        tags: |
          $(Build.BuildNumber)
          latest
    
    # Publish artifacts
    - task: PublishBuildArtifacts@1
      displayName: 'Publish build artifacts'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'drop'
        publishLocation: 'Container'

- stage: SecurityScan
  displayName: 'Security & Quality Gates'
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: SecurityScan
    displayName: 'Security Scanning'
    pool:
      vmImage: $(vmImageName)
    
    steps:
    - task: WhiteSource@21
      displayName: 'WhiteSource Scan'
      inputs:
        cwd: '$(System.DefaultWorkingDirectory)'
    
    - task: AzureKeyVault@2
      displayName: 'Get secrets from Key Vault'
      inputs:
        azureSubscription: 'Azure-Subscription'
        KeyVaultName: 'ecommerce-keyvault'
        SecretsFilter: '*'
        RunAsPreJob: false
```

#### 5.2 Build Pipeline Variables
```yaml
# Pipeline Variables (Set in Azure DevOps UI)
variables:
  # Build Configuration
  - name: buildConfiguration
    value: Release
  
  # Container Registry
  - name: containerRegistry
    value: ecommerceacr.azurecr.io
  
  # Service connections (created in Azure DevOps)
  - name: azureSubscription
    value: 'Azure-Subscription-Connection'
  
  # SonarCloud configuration
  - name: sonarCloudOrganization
    value: 'your-organization'
  
  # Environment-specific variables
  - name: environmentName
    value: development
```

### 🚀 Step 6: Release Pipeline Creation

#### 6.1 Release Pipeline YAML (release-pipeline.yml)
```yaml
# pipelines/release-pipeline.yml
trigger: none # Triggered by build pipeline completion

resources:
  pipelines:
  - pipeline: buildPipeline
    source: 'ECommerceApp-CI'
    trigger:
      branches:
        include:
        - main

variables:
  azureSubscription: 'Azure-Subscription-Connection'
  resourceGroupName: 'rg-ecommerce-$(environmentName)'
  webAppName: 'app-ecommerce-$(environmentName)'
  containerRegistry: 'ecommerceacr.azurecr.io'

stages:
- stage: DeployDevelopment
  displayName: 'Deploy to Development'
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  variables:
    environmentName: 'dev'
    deploymentSlot: 'staging'
  
  jobs:
  - deployment: DeployDev
    displayName: 'Deploy to Development Environment'
    environment: 'ecommerce-development'
    pool:
      vmImage: 'ubuntu-latest'
    
    strategy:
      runOnce:
        deploy:
          steps:
          - checkout: none
          
          # Download build artifacts
          - download: buildPipeline
            artifact: drop
          
          # Azure CLI setup
          - task: AzureCLI@2
            displayName: 'Deploy Infrastructure'
            inputs:
              azureSubscription: $(azureSubscription)
              scriptType: 'bash'
              scriptLocation: 'inlineScript'
              inlineScript: |
                # Create resource group
                az group create --name $(resourceGroupName) --location "East US"
                
                # Deploy ARM template
                az deployment group create \
                  --resource-group $(resourceGroupName) \
                  --template-file $(Pipeline.Workspace)/buildPipeline/drop/infrastructure/arm-templates/webapp.json \
                  --parameters webAppName=$(webAppName) environment=$(environmentName)
          
          # Deploy to Azure Web App
          - task: AzureWebAppContainer@1
            displayName: 'Deploy to Azure Web App'
            inputs:
              azureSubscription: $(azureSubscription)
              appName: $(webAppName)
              slotName: $(deploymentSlot)
              containers: '$(containerRegistry)/ecommerce/webapi:$(Build.BuildNumber)'
          
          # Run smoke tests
          - task: AzureCLI@2
            displayName: 'Run Smoke Tests'
            inputs:
              azureSubscription: $(azureSubscription)
              scriptType: 'bash'
              scriptLocation: 'inlineScript'
              inlineScript: |
                # Wait for app to be ready
                sleep 30
                
                # Health check
                response=$(curl -s -o /dev/null -w "%{http_code}" https://$(webAppName)-$(deploymentSlot).azurewebsites.net/health)
                if [ $response -eq 200 ]; then
                  echo "Health check passed"
                else
                  echo "Health check failed with status code: $response"
                  exit 1
                fi
          
          # Swap deployment slots
          - task: AzureAppServiceManage@0
            displayName: 'Swap Deployment Slots'
            inputs:
              azureSubscription: $(azureSubscription)
              WebAppName: $(webAppName)
              ResourceGroupName: $(resourceGroupName)
              SourceSlot: $(deploymentSlot)
              SwapWithProduction: true

- stage: DeployStaging
  displayName: 'Deploy to Staging'
  dependsOn: []
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
    environmentName: 'staging'
  
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging Environment'
    environment: 'ecommerce-staging'
    pool:
      vmImage: 'ubuntu-latest'
    
    strategy:
      runOnce:
        deploy:
          steps:
          # Similar to dev deployment with staging-specific configurations
          - template: templates/deploy-template.yml
            parameters:
              environment: 'staging'
              azureSubscription: $(azureSubscription)
              resourceGroupName: $(resourceGroupName)
              webAppName: $(webAppName)
              runLoadTests: true

- stage: DeployProduction
  displayName: 'Deploy to Production'
  dependsOn: DeployStaging
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  variables:
    environmentName: 'prod'
  
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production Environment'
    environment: 'ecommerce-production'
    pool:
      vmImage: 'ubuntu-latest'
    
    strategy:
      runOnce:
        deploy:
          steps:
          # Pre-deployment approvals configured in environment
          - template: templates/deploy-template.yml
            parameters:
              environment: 'production'
              azureSubscription: $(azureSubscription)
              resourceGroupName: $(resourceGroupName)
              webAppName: $(webAppName)
              enableBlueGreenDeployment: true
              runSecurityTests: true
```

#### 6.2 Deployment Template (templates/deploy-template.yml)
```yaml
# pipelines/templates/deploy-template.yml
parameters:
- name: environment
  type: string
- name: azureSubscription
  type: string
- name: resourceGroupName
  type: string
- name: webAppName
  type: string
- name: runLoadTests
  type: boolean
  default: false
- name: enableBlueGreenDeployment
  type: boolean
  default: false
- name: runSecurityTests
  type: boolean
  default: false

steps:
- checkout: none

# Download artifacts
- download: buildPipeline
  artifact: drop

# Get secrets from Key Vault
- task: AzureKeyVault@2
  displayName: 'Get secrets from Key Vault'
  inputs:
    azureSubscription: ${{ parameters.azureSubscription }}
    KeyVaultName: 'kv-ecommerce-${{ parameters.environment }}'
    SecretsFilter: '*'

# Deploy infrastructure
- task: AzureResourceManagerTemplateDeployment@3
  displayName: 'Deploy ARM Template'
  inputs:
    deploymentScope: 'Resource Group'
    azureResourceManagerConnection: ${{ parameters.azureSubscription }}
    subscriptionId: '$(subscriptionId)'
    action: 'Create Or Update Resource Group'
    resourceGroupName: ${{ parameters.resourceGroupName }}
    location: 'East US'
    templateLocation: 'Linked artifact'
    csmFile: '$(Pipeline.Workspace)/buildPipeline/drop/infrastructure/arm-templates/main.json'
    overrideParameters: |
      -environment "${{ parameters.environment }}"
      -webAppName "${{ parameters.webAppName }}"
      -databaseConnectionString "$(DatabaseConnectionString)"

# Deploy application
- task: AzureWebAppContainer@1
  displayName: 'Deploy to Azure Web App'
  inputs:
    azureSubscription: ${{ parameters.azureSubscription }}
    appName: ${{ parameters.webAppName }}
    containers: '$(containerRegistry)/ecommerce/webapi:$(Build.BuildNumber)'
    appSettings: |
      -ASPNETCORE_ENVIRONMENT "${{ parameters.environment }}"
      -ConnectionStrings:DefaultConnection "$(DatabaseConnectionString)"
      -JwtSettings:SecretKey "$(JwtSecretKey)"

# Run load tests (if enabled)
- ${{ if eq(parameters.runLoadTests, true) }}:
  - task: AzureLoadTest@1
    displayName: 'Run Load Tests'
    inputs:
      azureSubscription: ${{ parameters.azureSubscription }}
      loadTestConfigFile: '$(Pipeline.Workspace)/buildPipeline/drop/tests/load-tests/config.yaml'
      loadTestResource: 'loadtest-ecommerce-${{ parameters.environment }}'

# Run security tests (if enabled)
- ${{ if eq(parameters.runSecurityTests, true) }}:
  - task: AzureCLI@2
    displayName: 'Run Security Tests'
    inputs:
      azureSubscription: ${{ parameters.azureSubscription }}
      scriptType: 'bash'
      scriptLocation: 'inlineScript'
      inlineScript: |
        # Run OWASP ZAP security scan
        docker run -t owasp/zap2docker-stable zap-baseline.py \
          -t https://${{ parameters.webAppName }}.azurewebsites.net \
          -J zap-report.json
```

---

## AWS CodePipeline Complete Implementation

### 🏗️ Step 1: AWS Setup & Repository Creation

#### 1.1 Create AWS CodeCommit Repository
```bash
# Install AWS CLI
aws configure
# Enter your Access Key ID, Secret Access Key, Region, and Output format

# Create CodeCommit repository
aws codecommit create-repository \
    --repository-name ecommerce-app \
    --repository-description "E-commerce application with microservices"

# Output will include clone URL
{
    "repositoryMetadata": {
        "repositoryName": "ecommerce-app",
        "cloneUrlHttp": "https://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app",
        "cloneUrlSsh": "ssh://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app"
    }
}
```

#### 1.2 Setup Local Repository
```bash
# Clone repository
git clone https://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app
cd ecommerce-app

# Create same project structure as Azure DevOps example
mkdir -p src/WebAPI tests infrastructure/cloudformation buildspec
```

#### 1.3 Create buildspec.yml for CodeBuild
```yaml
# buildspec/buildspec.yml
version: 0.2

phases:
  pre_build:
    commands:
      - echo "Starting pre-build phase"
      - echo "Logging in to Amazon ECR..."
      - aws ecr get-login-password --region $AWS_DEFAULT_REGION | docker login --username AWS --password-stdin $AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com
      - REPOSITORY_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com/$IMAGE_REPO_NAME
      - COMMIT_HASH=$(echo $CODEBUILD_RESOLVED_SOURCE_VERSION | cut -c 1-7)
      - IMAGE_TAG=${COMMIT_HASH:=latest}
      - echo "Repository URI = $REPOSITORY_URI"
      - echo "Image tag = $IMAGE_TAG"
      
  build:
    commands:
      - echo "Starting build phase"
      - echo "Installing .NET 6 SDK"
      - wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
      - dpkg -i packages-microsoft-prod.deb
      - apt-get update
      - apt-get install -y dotnet-sdk-6.0
      
      - echo "Restoring NuGet packages"
      - dotnet restore src/WebAPI/WebAPI.csproj
      
      - echo "Building application"
      - dotnet build src/WebAPI/WebAPI.csproj --configuration Release --no-restore
      
      - echo "Running unit tests"
      - dotnet test tests/ --configuration Release --no-build --verbosity normal
      
      - echo "Building Docker image"
      - docker build -t $IMAGE_REPO_NAME:$IMAGE_TAG -f docker/Dockerfile .
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:$IMAGE_TAG
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:latest
      
  post_build:
    commands:
      - echo "Starting post-build phase"
      - echo "Pushing Docker image to ECR"
      - docker push $REPOSITORY_URI:$IMAGE_TAG
      - docker push $REPOSITORY_URI:latest
      - echo "Writing image definitions file"
      - printf '[{"name":"ecommerce-webapi","imageUri":"%s"}]' $REPOSITORY_URI:$IMAGE_TAG > imagedefinitions.json
      - echo "Running security scan"
      - aws ecr start-image-scan --repository-name $IMAGE_REPO_NAME --image-id imageTag=$IMAGE_TAG

artifacts:
  files:
    - imagedefinitions.json
    - infrastructure/cloudformation/**/*
  name: BuildArtifact-$(date +%Y-%m-%d)

cache:
  paths:
    - '/root/.nuget/packages/**/*'
    - '/var/lib/docker/**/*'
```

### 📤 Step 2: Push Code to AWS CodeCommit

#### 2.1 Add Sample Application Code
```csharp
// src/WebAPI/Program.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddAWSProvider();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```

#### 2.2 Create CloudFormation Templates
```yaml
# infrastructure/cloudformation/ecs-service.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'ECS Service for E-commerce Application'

Parameters:
  Environment:
    Type: String
    Default: development
    AllowedValues:
      - development
      - staging
      - production
  
  ImageURI:
    Type: String
    Description: 'Docker image URI'
  
  VpcId:
    Type: String
    Description: 'VPC ID for the ECS service'
  
  SubnetIds:
    Type: CommaDelimitedList
    Description: 'Subnet IDs for the ECS service'

Resources:
  # ECS Cluster
  ECSCluster:
    Type: AWS::ECS::Cluster
    Properties:
      ClusterName: !Sub 'ecommerce-cluster-${Environment}'
      CapacityProviders:
        - FARGATE
        - FARGATE_SPOT
      DefaultCapacityProviderStrategy:
        - CapacityProvider: FARGATE
          Weight: 1
        - CapacityProvider: FARGATE_SPOT
          Weight: 2
  
  # Task Definition
  TaskDefinition:
    Type: AWS::ECS::TaskDefinition
    Properties:
      Family: !Sub 'ecommerce-webapi-${Environment}'
      NetworkMode: awsvpc
      RequiresCompatibilities:
        - FARGATE
      Cpu: 256
      Memory: 512
      ExecutionRoleArn: !Ref ExecutionRole
      TaskRoleArn: !Ref TaskRole
      ContainerDefinitions:
        - Name: ecommerce-webapi
          Image: !Ref ImageURI
          PortMappings:
            - ContainerPort: 80
              Protocol: tcp
          LogConfiguration:
            LogDriver: awslogs
            Options:
              awslogs-group: !Ref LogGroup
              awslogs-region: !Ref AWS::Region
              awslogs-stream-prefix: ecs
          Environment:
            - Name: ASPNETCORE_ENVIRONMENT
              Value: !Ref Environment
            - Name: AWS_REGION
              Value: !Ref AWS::Region
          HealthCheck:
            Command:
              - CMD-SHELL
              - "curl -f http://localhost:80/health || exit 1"
            Interval: 30
            Timeout: 5
            Retries: 3
            StartPeriod: 60
  
  # ECS Service
  ECSService:
    Type: AWS::ECS::Service
    DependsOn: LoadBalancerListener
    Properties:
      ServiceName: !Sub 'ecommerce-webapi-${Environment}'
      Cluster: !Ref ECSCluster
      TaskDefinition: !Ref TaskDefinition
      DesiredCount: 2
      LaunchType: FARGATE
      NetworkConfiguration:
        AwsvpcConfiguration:
          SecurityGroups:
            - !Ref ServiceSecurityGroup
          Subnets: !Ref SubnetIds
          AssignPublicIp: ENABLED
      LoadBalancers:
        - ContainerName: ecommerce-webapi
          ContainerPort: 80
          TargetGroupArn: !Ref TargetGroup
      DeploymentConfiguration:
        MaximumPercent: 200
        MinimumHealthyPercent: 50
        DeploymentCircuitBreaker:
          Enable: true
          Rollback: true
  
  # Application Load Balancer
  LoadBalancer:
    Type: AWS::ElasticLoadBalancingV2::LoadBalancer
    Properties:
      Name: !Sub 'ecommerce-alb-${Environment}'
      Subnets: !Ref SubnetIds
      SecurityGroups:
        - !Ref LoadBalancerSecurityGroup
      Type: application
      IpAddressType: ipv4
      
  LoadBalancerListener:
    Type: AWS::ElasticLoadBalancingV2::Listener
    Properties:
      DefaultActions:
        - Type: forward
          TargetGroupArn: !Ref TargetGroup
      LoadBalancerArn: !Ref LoadBalancer
      Port: 80
      Protocol: HTTP
  
  TargetGroup:
    Type: AWS::ElasticLoadBalancingV2::TargetGroup
    Properties:
      Name: !Sub 'ecommerce-tg-${Environment}'
      Port: 80
      Protocol: HTTP
      VpcId: !Ref VpcId
      TargetType: ip
      HealthCheckPath: /health
      HealthCheckProtocol: HTTP
      HealthCheckIntervalSeconds: 30
      HealthCheckTimeoutSeconds: 5
      HealthyThresholdCount: 2
      UnhealthyThresholdCount: 5

  # Auto Scaling
  AutoScalingTarget:
    Type: AWS::ApplicationAutoScaling::ScalableTarget
    Properties:
      MaxCapacity: 10
      MinCapacity: 2
      ResourceId: !Sub 'service/${ECSCluster}/${ECSService.Name}'
      RoleARN: !GetAtt AutoScalingRole.Arn
      ScalableDimension: ecs:service:DesiredCount
      ServiceNamespace: ecs
  
  AutoScalingPolicy:
    Type: AWS::ApplicationAutoScaling::ScalingPolicy
    Properties:
      PolicyName: !Sub 'ecommerce-scaling-policy-${Environment}'
      PolicyType: TargetTrackingScaling
      ScalingTargetId: !Ref AutoScalingTarget
      TargetTrackingScalingPolicyConfiguration:
        PredefinedMetricSpecification:
          PredefinedMetricType: ECSServiceAverageCPUUtilization
        TargetValue: 70
        ScaleOutCooldown: 300
        ScaleInCooldown: 300

  # IAM Roles
  ExecutionRole:
    Type: AWS::IAM::Role
    Properties:
      RoleName: !Sub 'ecommerce-execution-role-${Environment}'
      AssumeRolePolicyDocument:
        Statement:
          - Effect: Allow
            Principal:
              Service: ecs-tasks.amazonaws.com
            Action: 'sts:AssumeRole'
      ManagedPolicyArns:
        - 'arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy'

  TaskRole:
    Type: AWS::IAM::Role
    Properties:
      RoleName: !Sub 'ecommerce-task-role-${Environment}'
      AssumeRolePolicyDocument:
        Statement:
          - Effect: Allow
            Principal:
              Service: ecs-tasks.amazonaws.com
            Action: 'sts:AssumeRole'
      Policies:
        - PolicyName: CloudWatchLogs
          PolicyDocument:
            Statement:
              - Effect: Allow
                Action:
                  - 'logs:CreateLogGroup'
                  - 'logs:CreateLogStream'
                  - 'logs:PutLogEvents'
                Resource: '*'

  # Security Groups
  ServiceSecurityGroup:
    Type: AWS::EC2::SecurityGroup
    Properties:
      GroupDescription: Security group for ECS service
      VpcId: !Ref VpcId
      SecurityGroupIngress:
        - IpProtocol: tcp
          FromPort: 80
          ToPort: 80
          SourceSecurityGroupId: !Ref LoadBalancerSecurityGroup

  LoadBalancerSecurityGroup:
    Type: AWS::EC2::SecurityGroup
    Properties:
      GroupDescription: Security group for Application Load Balancer
      VpcId: !Ref VpcId
      SecurityGroupIngress:
        - IpProtocol: tcp
          FromPort: 80
          ToPort: 80
          CidrIp: 0.0.0.0/0

  # CloudWatch Log Group
  LogGroup:
    Type: AWS::Logs::LogGroup
    Properties:
      LogGroupName: !Sub '/ecs/ecommerce-webapi-${Environment}'
      RetentionInDays: 14

Outputs:
  LoadBalancerURL:
    Description: 'Load Balancer URL'
    Value: !Sub 'http://${LoadBalancer.DNSName}'
    Export:
      Name: !Sub '${AWS::StackName}-LoadBalancerURL'
  
  ECSClusterName:
    Description: 'ECS Cluster Name'
    Value: !Ref ECSCluster
    Export:
      Name: !Sub '${AWS::StackName}-ECSCluster'
```

#### 2.3 Commit and Push Code
```bash
# Add all files
git add .

# Commit changes
git commit -m "feat: Initial AWS infrastructure setup

- Added .NET Core Web API with health checks
- Configured ECS Fargate deployment with CloudFormation
- Implemented Application Load Balancer with auto-scaling
- Added comprehensive buildspec.yml for CodeBuild
- Configured CloudWatch logging and monitoring

Features:
- Auto-scaling based on CPU utilization
- Blue-green deployment capability
- Security groups with least privilege access
- Multi-AZ deployment for high availability"

# Push to main branch
git push origin main
```

### 🔧 Step 3: Create AWS CodeBuild Project

#### 3.1 Create CodeBuild Project via AWS CLI
```bash
# Create build project
aws codebuild create-project \
    --name "ecommerce-build" \
    --description "Build project for E-commerce application" \
    --source '{
        "type": "CODECOMMIT",
        "location": "https://git-codecommit.us-east-1.amazonaws.com/v1/repos/ecommerce-app",
        "buildspec": "buildspec/buildspec.yml"
    }' \
    --artifacts '{
        "type": "S3",
        "location": "ecommerce-build-artifacts",
        "name": "build-output"
    }' \
    --environment '{
        "type": "LINUX_CONTAINER",
        "image": "aws/codebuild/amazonlinux2-x86_64-standard:3.0",
        "computeType": "BUILD_GENERAL1_MEDIUM",
        "privilegedMode": true,
        "environmentVariables": [
            {
                "name": "AWS_DEFAULT_REGION",
                "value": "us-east-1"
            },
            {
                "name": "AWS_ACCOUNT_ID",
                "value": "123456789012"
            },
            {
                "name": "IMAGE_REPO_NAME",
                "value": "ecommerce-webapi"
            }
        ]
    }' \
    --service-role "arn:aws:iam::123456789012:role/service-role/codebuild-service-role"
```

#### 3.2 Advanced buildspec.yml with Multiple Environments
```yaml
# buildspec/buildspec-advanced.yml
version: 0.2

env:
  variables:
    DOTNET_CORE_RUNTIME: '6.0'
  parameter-store:
    SONAR_TOKEN: '/ecommerce/sonar/token'
    SNYK_TOKEN: '/ecommerce/snyk/token'
  exported-variables:
    - IMAGE_TAG
    - REPOSITORY_URI

phases:
  install:
    runtime-versions:
      docker: 20
    commands:
      - echo "Installing additional dependencies"
      - curl -fsSL https://get.docker.com -o get-docker.sh
      - sh get-docker.sh
      - pip3 install awscli --upgrade
      
  pre_build:
    commands:
      - echo "Starting pre-build phase on `date`"
      - echo "Current directory: $(pwd)"
      - echo "Listing files:"
      - ls -la
      
      # Login to ECR
      - echo "Logging in to Amazon ECR..."
      - aws ecr get-login-password --region $AWS_DEFAULT_REGION | docker login --username AWS --password-stdin $AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com
      
      # Set image variables
      - REPOSITORY_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com/$IMAGE_REPO_NAME
      - COMMIT_HASH=$(echo $CODEBUILD_RESOLVED_SOURCE_VERSION | cut -c 1-7)
      - BUILD_NUMBER=${CODEBUILD_BUILD_NUMBER}
      - IMAGE_TAG=${COMMIT_HASH}-${BUILD_NUMBER}
      
      # Create ECR repository if it doesn't exist
      - aws ecr describe-repositories --repository-names $IMAGE_REPO_NAME || aws ecr create-repository --repository-name $IMAGE_REPO_NAME
      
      # Install .NET SDK
      - echo "Installing .NET SDK"
      - wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
      - dpkg -i packages-microsoft-prod.deb
      - apt-get update
      - apt-get install -y dotnet-sdk-6.0
      
      # Install SonarScanner
      - echo "Installing SonarScanner"
      - dotnet tool install --global dotnet-sonarscanner
      - export PATH="$PATH:/root/.dotnet/tools"
      
  build:
    commands:
      - echo "Starting build phase on `date`"
      
      # Start SonarQube analysis
      - echo "Starting SonarQube analysis"
      - dotnet sonarscanner begin /k:"ecommerce-app" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login="$SONAR_TOKEN" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
      
      # Restore dependencies
      - echo "Restoring NuGet packages"
      - dotnet restore src/WebAPI/WebAPI.csproj --verbosity normal
      
      # Build application
      - echo "Building application"
      - dotnet build src/WebAPI/WebAPI.csproj --configuration Release --no-restore --verbosity normal
      
      # Run unit tests with coverage
      - echo "Running unit tests with coverage"
      - dotnet test tests/ --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./TestResults/ --logger trx
      
      # Convert coverage to OpenCover format for SonarQube
      - dotnet tool install -g dotnet-reportgenerator-globaltool
      - reportgenerator "-reports:TestResults/*/coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:OpenCover"
      
      # Complete SonarQube analysis
      - dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
      
      # Security scanning with Snyk
      - echo "Running security scan with Snyk"
      - npm install -g snyk
      - snyk auth $SNYK_TOKEN
      - snyk test --file=src/WebAPI/WebAPI.csproj --severity-threshold=high
      
      # Build Docker image
      - echo "Building Docker image"
      - echo "Repository URI: $REPOSITORY_URI"
      - echo "Image tag: $IMAGE_TAG"
      - docker build -t $IMAGE_REPO_NAME:$IMAGE_TAG -f docker/Dockerfile .
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:$IMAGE_TAG
      - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:latest
      
      # Docker security scan
      - echo "Running Docker security scan"
      - docker run --rm -v /var/run/docker.sock:/var/run/docker.sock -v $(pwd):/app anchore/syft /app -o json > sbom.json
      - docker run --rm -v $(pwd):/app anchore/grype sbom:sbom.json
      
  post_build:
    commands:
      - echo "Starting post-build phase on `date`"
      
      # Push image to ECR
      - echo "Pushing Docker image to ECR"
      - docker push $REPOSITORY_URI:$IMAGE_TAG
      - docker push $REPOSITORY_URI:latest
      
      # Start ECR image scan
      - echo "Starting ECR image vulnerability scan"
      - aws ecr start-image-scan --repository-name $IMAGE_REPO_NAME --image-id imageTag=$IMAGE_TAG || true
      
      # Wait for scan to complete and get results
      - echo "Waiting for vulnerability scan to complete"
      - sleep 30
      - aws ecr describe-image-scan-findings --repository-name $IMAGE_REPO_NAME --image-id imageTag=$IMAGE_TAG || true
      
      # Create image definitions for CodeDeploy
      - echo "Creating image definitions file"
      - printf '[{"name":"ecommerce-webapi","imageUri":"%s"}]' $REPOSITORY_URI:$IMAGE_TAG > imagedefinitions.json
      - cat imagedefinitions.json
      
      # Update parameter store with new image URI
      - echo "Updating parameter store with new image URI"
      - aws ssm put-parameter --name "/ecommerce/webapi/image-uri" --value "$REPOSITORY_URI:$IMAGE_TAG" --overwrite
      
      # Create CloudFormation parameters file
      - echo "Creating CloudFormation parameters file"
      - cat > infrastructure/parameters.json << EOF
        [
          {
            "ParameterKey": "ImageURI",
            "ParameterValue": "$REPOSITORY_URI:$IMAGE_TAG"
          },
          {
            "ParameterKey": "Environment",
            "ParameterValue": "${ENVIRONMENT:-development}"
          }
        ]
        EOF
      
      # Package CloudFormation templates
      - echo "Packaging CloudFormation templates"
      - aws cloudformation package \
          --template-file infrastructure/cloudformation/ecs-service.yml \
          --s3-bucket ecommerce-cloudformation-templates \
          --output-template-file infrastructure/packaged-template.yml

reports:
  SonarQubeReport:
    files:
      - 'sonar-report.json'
    base-directory: '.'
  TestResults:
    files:
      - '**/*'
    base-directory: 'TestResults'
    file-format: 'VisualStudioTrx'
  CodeCoverage:
    files:
      - 'coverage/Cobertura.xml'
    file-format: 'CoberturaXml'

artifacts:
  files:
    - imagedefinitions.json
    - infrastructure/packaged-template.yml
    - infrastructure/parameters.json
    - infrastructure/cloudformation/**/*
  secondary-artifacts:
    BuildArtifact:
      files:
        - imagedefinitions.json
        - infrastructure/packaged-template.yml
        - infrastructure/parameters.json
      name: BuildArtifact-$IMAGE_TAG
    SourceArtifact:
      files:
        - '**/*'
      name: SourceArtifact-$IMAGE_TAG

cache:
  paths:
    - '/root/.nuget/packages/**/*'
    - '/root/.dotnet/tools/**/*'
    - '/var/lib/docker/**/*'
    - 'node_modules/**/*'
```

### 🚀 Step 4: Create AWS CodePipeline

#### 4.1 CodePipeline CloudFormation Template
```yaml
# infrastructure/cloudformation/codepipeline.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'Complete CI/CD Pipeline for E-commerce Application'

Parameters:
  GitHubOwner:
    Type: String
    Description: 'GitHub repository owner'
    Default: 'your-github-username'
  
  GitHubRepo:
    Type: String
    Description: 'GitHub repository name'
    Default: 'ecommerce-app'
  
  GitHubToken:
    Type: String
    NoEcho: true
    Description: 'GitHub personal access token'
  
  SlackWebhookUrl:
    Type: String
    NoEcho: true
    Description: 'Slack webhook URL for notifications'

Resources:
  # S3 Bucket for artifacts
  ArtifactsBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: !Sub 'ecommerce-pipeline-artifacts-${AWS::AccountId}-${AWS::Region}'
      VersioningConfiguration:
        Status: Enabled
      BucketEncryption:
        ServerSideEncryptionConfiguration:
          - ServerSideEncryptionByDefault:
              SSEAlgorithm: AES256
      PublicAccessBlockConfiguration:
        BlockPublicAcls: true
        BlockPublicPolicy: true
        IgnorePublicAcls: true
        RestrictPublicBuckets: true
      LifecycleConfiguration:
        Rules:
          - Id: DeleteOldArtifacts
            Status: Enabled
            ExpirationInDays: 30

  # CodePipeline Service Role
  CodePipelineServiceRole:
    Type: AWS::IAM::Role
    Properties:
      RoleName: !Sub 'CodePipelineServiceRole-${AWS::StackName}'
      AssumeRolePolicyDocument:
        Statement:
          - Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: CodePipelineServiceRolePolicy
          PolicyDocument:
            Statement:
              - Effect: Allow
                Action:
                  - s3:GetBucketVersioning
                  - s3:GetObject
                  - s3:GetObjectVersion
                  - s3:PutObject
                Resource:
                  - !Sub '${ArtifactsBucket}/*'
                  - !Ref ArtifactsBucket
              - Effect: Allow
                Action:
                  - codebuild:BatchGetBuilds
                  - codebuild:StartBuild
                Resource: '*'
              - Effect: Allow
                Action:
                  - cloudformation:CreateStack
                  - cloudformation:DeleteStack
                  - cloudformation:DescribeStacks
                  - cloudformation:UpdateStack
                  - cloudformation:CreateChangeSet
                  - cloudformation:DeleteChangeSet
                  - cloudformation:DescribeChangeSet
                  - cloudformation:ExecuteChangeSet
                  - cloudformation:SetStackPolicy
                  - cloudformation:ValidateTemplate
                Resource: '*'
              - Effect: Allow
                Action:
                  - iam:PassRole
                Resource: '*'
              - Effect: Allow
                Action:
                  - ecs:DescribeServices
                  - ecs:DescribeTaskDefinition
                  - ecs:DescribeTasks
                  - ecs:ListTasks
                  - ecs:RegisterTaskDefinition
                  - ecs:UpdateService
                Resource: '*'
              - Effect: Allow
                Action:
                  - sns:Publish
                Resource: '*'

  # CodeBuild Projects
  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub 'ecommerce-build-${AWS::StackName}'
      Description: 'Build project for E-commerce application'
      ServiceRole: !GetAtt CodeBuildServiceRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_MEDIUM
        Image: aws/codebuild/amazonlinux2-x86_64-standard:3.0
        PrivilegedMode: true
        EnvironmentVariables:
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
          - Name: AWS_ACCOUNT_ID
            Value: !Ref AWS::AccountId
          - Name: IMAGE_REPO_NAME
            Value: ecommerce-webapi
          - Name: ENVIRONMENT
            Value: development
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          phases:
            pre_build:
              commands:
                - echo "Starting pre-build phase"
                - aws ecr get-login-password --region $AWS_DEFAULT_REGION | docker login --username AWS --password-stdin $AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com
                - REPOSITORY_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com/$IMAGE_REPO_NAME
                - COMMIT_HASH=$(echo $CODEBUILD_RESOLVED_SOURCE_VERSION | cut -c 1-7)
                - IMAGE_TAG=${COMMIT_HASH:=latest}
            build:
              commands:
                - echo "Starting build phase"
                - docker build -t $IMAGE_REPO_NAME:$IMAGE_TAG .
                - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:$IMAGE_TAG
                - docker tag $IMAGE_REPO_NAME:$IMAGE_TAG $REPOSITORY_URI:latest
            post_build:
              commands:
                - echo "Starting post-build phase"
                - docker push $REPOSITORY_URI:$IMAGE_TAG
                - docker push $REPOSITORY_URI:latest
                - printf '[{"name":"ecommerce-webapi","imageUri":"%s"}]' $REPOSITORY_URI:$IMAGE_TAG > imagedefinitions.json
          artifacts:
            files:
              - imagedefinitions.json
              - infrastructure/**/*

  # Deploy to Development Environment
  DeployDevProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: !Sub 'ecommerce-deploy-dev-${AWS::StackName}'
      Description: 'Deploy to development environment'
      ServiceRole: !GetAtt CodeBuildServiceRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_SMALL
        Image: aws/codebuild/amazonlinux2-x86_64-standard:3.0
        EnvironmentVariables:
          - Name: ENVIRONMENT
            Value: development
          - Name: AWS_DEFAULT_REGION
            Value: !Ref AWS::Region
      Source:
        Type: CODEPIPELINE
        BuildSpec: |
          version: 0.2
          phases:
            pre_build:
              commands:
                - echo "Preparing deployment to development environment"
            build:
              commands:
                - echo "Deploying CloudFormation stack"
                - aws cloudformation deploy
                    --template-file infrastructure/cloudformation/ecs-service.yml
                    --stack-name ecommerce-dev-stack
                    --parameter-overrides Environment=development
                    --capabilities CAPABILITY_IAM
                    --region $AWS_DEFAULT_REGION
            post_build:
              commands:
                - echo "Deployment completed"
                - aws ecs wait services-stable --cluster ecommerce-cluster-development --services ecommerce-webapi-development

  # CodePipeline
  Pipeline:
    Type: AWS::CodePipeline::Pipeline
    Properties:
      Name: !Sub 'ecommerce-pipeline-${AWS::StackName}'
      RoleArn: !GetAtt CodePipelineServiceRole.Arn
      ArtifactStore:
        Type: S3
        Location: !Ref ArtifactsBucket
      Stages:
        # Source Stage
        - Name: Source
          Actions:
            - Name: SourceAction
              ActionTypeId:
                Category: Source
                Owner: ThirdParty
                Provider: GitHub
                Version: '1'
              Configuration:
                Owner: !Ref GitHubOwner
                Repo: !Ref GitHubRepo
                Branch: main
                OAuthToken: !Ref GitHubToken
                PollForSourceChanges: false
              OutputArtifacts:
                - Name: SourceOutput

        # Build Stage
        - Name: Build
          Actions:
            - Name: BuildAction
              ActionTypeI…