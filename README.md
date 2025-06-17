# MSBuild Test Project

This project investigates the behavior of MSBuild properties for detecting .NET framework versions in a multi-targeting project. It provides a minimal test case for understanding how framework version detection works in MSBuild and highlights some important limitations.

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

The build log reveals several important findings:

1. Properties in `Directory.Build.props`:
   - Work correctly during framework-specific builds:
     ```
     [TestLibrary] [BeforeBuild] TargetFramework: net9.0, TargetFrameworkVersion: 9.0, IsNet8OrGreater: true, IsNet9OrGreater: true
     [TestLibrary] [BeforeBuild] TargetFramework: net8.0, TargetFrameworkVersion: 8.0, IsNet8OrGreater: true, IsNet9OrGreater: false
     ```
   - Become empty/false in some build steps:
     ```
     [TestLibrary] [BeforeBuild] TargetFramework: , TargetFrameworkVersion: 0.0, IsNet8OrGreater: false, IsNet9OrGreater: false
     ```

     References:
        - https://github.com/jamescrosswell/msbuild-test-4275/blob/4056d039ea5a74e01f95c84bda01bdbcef234676/build.log#L13008
        -  https://github.com/jamescrosswell/msbuild-test-4275/blob/4056d039ea5a74e01f95c84bda01bdbcef234676/build.log#L13110
        - https://github.com/jamescrosswell/msbuild-test-4275/blob/4056d039ea5a74e01f95c84bda01bdbcef234676/build.log#L13149

2. Properties in `Directory.Build.targets`:
   - Are consistently empty throughout the build:
     ```
     [TestLibrary] [BeforeBuild] TargetFramework: net9.0, TargetFrameworkVersionTarget: , IsNet8OrGreaterTarget: , IsNet9OrGreaterTarget: 
     [TestLibrary] [BeforeBuild] TargetFramework: net8.0, TargetFrameworkVersionTarget: , IsNet8OrGreaterTarget: , IsNet9OrGreaterTarget: 
     ```

## Key Findings

1. The `Directory.Build.props` approach works partially:
   - Properties are correctly set during framework-specific builds
   - Properties become empty/false in certain build steps
   - Perhaps we avoid setting these properties if/when the `$(TargetFramework)` is empty

2. The `Directory.Build.targets` approach does not work:
   - Properties remain empty throughout the build
   - This suggests the target execution phase is not the right place for these properties

## Building

To reproduce, you can build solution with detailed logging:

```bash
dotnet build -v detailed > build.log 2>&1
```

This will generate a detailed build log that shows the property evaluations and target executions. 