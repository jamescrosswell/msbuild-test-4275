# MSBuild Test Project

This project demonstrates how to use MSBuild properties to detect .NET framework versions in a multi-targeting project. It provides a minimal test case for understanding how framework version detection works in MSBuild.

## Project Structure

- `Directory.Build.props`: Contains framework version detection properties that are evaluated during the initial property evaluation phase
- `Directory.Build.targets`: Contains framework version detection properties that are evaluated during the target execution phase
- `TestLibrary`: A class library project targeting .NET 8.0 and 9.0
- `TestConsole`: A console application targeting .NET 8.0 and 9.0 that references TestLibrary

## Framework Version Detection

The project demonstrates two approaches to framework version detection:

1. Using `Directory.Build.props` (Property Evaluation Phase):
```xml
<PropertyGroup>
  <IsNet8OrGreater Condition="'$(TargetFrameworkVersion)' >= 'v8.0'">true</IsNet8OrGreater>
  <IsNet9OrGreater Condition="'$(TargetFrameworkVersion)' >= 'v9.0'">true</IsNet9OrGreater>
</PropertyGroup>
```

2. Using `Directory.Build.targets` (Target Execution Phase):
```xml
<PropertyGroup>
  <IsNet8OrGreaterTarget Condition="'$(TargetFrameworkVersionTarget)' >= 'v8.0'">true</IsNet8OrGreaterTarget>
  <IsNet9OrGreaterTarget Condition="'$(TargetFrameworkVersionTarget)' >= 'v9.0'">true</IsNet9OrGreaterTarget>
</PropertyGroup>
```

## Test Results

The build log shows that both approaches work correctly:

1. Properties in `Directory.Build.props` are evaluated during the initial property evaluation phase:
```
Property reassignment: $(IsNet8OrGreater)="true" (previous value: "false")
Property reassignment: $(IsNet9OrGreater)="true" (previous value: "false")
```

2. Properties in `Directory.Build.targets` are evaluated during the target execution phase:
```
Property reassignment: $(IsNet8OrGreaterTarget)="true" (previous value: "false")
Property reassignment: $(IsNet9OrGreaterTarget)="true" (previous value: "false")
```

## Key Findings

1. Both approaches successfully detect the framework version
2. The properties are available at different phases of the build:
   - `Directory.Build.props` properties are available during property evaluation
   - `Directory.Build.targets` properties are available during target execution
3. The `TargetFrameworkVersion` property is correctly set to "v8.0" or "v9.0" depending on the target framework
4. No runtime constants are needed - the build properties are sufficient for build-time conditions

## Usage

To use these properties in your code, you can reference them in MSBuild conditions:

```xml
<PropertyGroup Condition="'$(IsNet8OrGreater)' == 'true'">
  <!-- .NET 8.0 or greater specific settings -->
</PropertyGroup>

<PropertyGroup Condition="'$(IsNet9OrGreater)' == 'true'">
  <!-- .NET 9.0 or greater specific settings -->
</PropertyGroup>
```

## Building

To build the project with detailed logging:

```bash
dotnet build -v detailed > build.log 2>&1
```

This will generate a detailed build log that shows the property evaluations and target executions. 