﻿using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit.Implementation;
using BenchmarkDotNet.Toolchains.Results;

namespace BenchmarkDotNet.Toolchains.InProcess.Emit
{
    public class InProcessEmitGenerator : IGenerator
    {
        public GenerateResult GenerateProject(
            BuildPartition buildPartition,
            ILogger logger,
            string rootArtifactsFolderPath)
        {
            var artifactsPaths = ArtifactsPaths.Empty;
            try
            {
                artifactsPaths = GetArtifactsPaths(buildPartition, rootArtifactsFolderPath);

                return GenerateResult.Success(artifactsPaths, new List<string>());
            }
            catch (Exception ex)
            {
                logger.WriteLineError($"Failed to generate partition: {ex}");
                return GenerateResult.Failure(artifactsPaths, new List<string>());
            }
        }

        private string GetBinariesDirectoryPath(string buildArtifactsDirectoryPath, string configuration) =>
            buildArtifactsDirectoryPath;

        private string GetExecutableExtension() => ".dll";

        private string GetBuildArtifactsDirectoryPath(
            BuildPartition buildPartition, string programName) =>
            Path.GetDirectoryName(buildPartition.AssemblyLocation);

        private ArtifactsPaths GetArtifactsPaths(BuildPartition buildPartition, string rootArtifactsFolderPath)
        {
            string programName = buildPartition.ProgramName + RunnableConstants.DynamicAssemblySuffix;
            string buildArtifactsDirectoryPath = GetBuildArtifactsDirectoryPath(buildPartition, programName);
            string binariesDirectoryPath =
                GetBinariesDirectoryPath(buildArtifactsDirectoryPath, buildPartition.BuildConfiguration);
            string executablePath = Path.Combine(binariesDirectoryPath, $"{programName}{GetExecutableExtension()}");

            return new ArtifactsPaths(
                rootArtifactsFolderPath: rootArtifactsFolderPath,
                buildArtifactsDirectoryPath: buildArtifactsDirectoryPath,
                binariesDirectoryPath: binariesDirectoryPath,
                programCodePath: null,
                appConfigPath: null,
                nugetConfigPath: null,
                projectFilePath: null,
                buildScriptFilePath: null,
                executablePath: executablePath,
                programName: programName,
                packagesDirectoryName: null);
        }
    }
}