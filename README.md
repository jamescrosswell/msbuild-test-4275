# MSBuild Property Availability Test

This repository contains a test solution to investigate the behavior of MSBuild properties in a multi-targeting scenario, specifically focusing on when `$(TargetFramework)` and derived properties are available during the build process.

## Problem Statement

When using properties derived from `$(TargetFramework)` in `Directory.Build.props`, there was a concern that these properties might not be available early enough in the build process, particularly when:
- Multi-targeting via `$(TargetFrameworks)`
- Using the properties in other `PropertyGroup`s
- Using the properties in project files that are packed (`Pack="true"`)

## Test Setup

The solution contains:
- A library project (`TestLibrary`) targeting both `net8.0` and `net9.0`
- A console app (`TestConsole`) also targeting both frameworks
- Two sets of framework detection properties:
  - One set in `Directory.Build.props`
  - A parallel set in `Directory.Build.targets` (with `Target` suffix)

The properties being tested:
```xml
<PropertyGroup>
  <TargetFrameworkVersion>$([MSBuild]::GetTargetFrameworkVersion($(TargetFramework)))</TargetFrameworkVersion>
  <IsNet8OrGreater>false</IsNet8OrGreater>
  <IsNet8OrGreater Condition="$([MSBuild]::VersionGreaterThanOrEquals($(TargetFrameworkVersion), 8.0))">true</IsNet8OrGreater>
  <IsNet9OrGreater>false</IsNet9OrGreater>
  <IsNet9OrGreater Condition="$([MSBuild]::VersionGreaterThanOrEquals($(TargetFrameworkVersion), 9.0))">true</IsNet9OrGreater>
</PropertyGroup>
```

## Key Findings

### 1. Property Behavior During Framework-Specific Builds
Both sets of properties (from `.props` and `.targets`) behave identically during the main build phases for each framework target. They correctly:
- Identify the framework version
- Set the appropriate `IsNet8OrGreater` and `IsNet9OrGreater` flags
- Maintain consistent values throughout the build process

Example from the build log:
```
[TestLibrary] [PrepareForBuild] TargetFramework: net8.0, TargetFrameworkVersion: 8.0, IsNet8OrGreater: true, IsNet9OrGreater: false
[TestLibrary] [PrepareForBuild] TargetFramework: net8.0, TargetFrameworkVersionTarget: 8.0, IsNet8OrGreaterTarget: true, IsNet9OrGreaterTarget: false
```

### 2. Final Build Step Anomaly
Both sets of properties show empty values in the final build step:
```
[TestLibrary] [BeforeBuild] TargetFramework: , TargetFrameworkVersion: 0.0, IsNet8OrGreater: false, IsNet9OrGreater: false
[TestLibrary] [BeforeBuild] TargetFramework: , TargetFrameworkVersionTarget: 0.0, IsNet8OrGreaterTarget: false, IsNet9OrGreaterTarget: false
```

### 3. Property Availability
The test confirms that `$(TargetFramework)` might not be available early in the build process. However, moving the properties to `Directory.Build.targets` doesn't solve the issue, as both sets of properties show the same behavior. This suggests the problem is with the availability of the base `TargetFramework` property itself, not with where we define our derived properties.

## Conclusion

1. The properties work correctly during the main build phases for each framework target
2. There appears to be a final build step where the properties become empty
3. Moving the properties to `Directory.Build.targets` doesn't resolve the issue
4. The root cause appears to be the availability of `$(TargetFramework)` itself, not with where we define our derived properties

## Next Steps

We might want to:
1. Investigate why the final build step shows empty values
2. Look for alternative ways to detect the framework version that might be available earlier
3. Consider if we need to handle the case where these properties might be empty

## Build Log

The full build log is available in [build.log](build.log). It was generated using:
```bash
dotnet build -v detailed > build.log
``` 