using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Base.Contracts;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using metrics.Serialization.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;


namespace metrics.Serialization.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(BenchmarkRunner.Run<SerializerBenchmark>());
        }
    }

    public class ManualConfigNonOptimized : ManualConfig
    {
        public ManualConfigNonOptimized()
        {
            AddValidator(JitOptimizationsValidator.DontFailOnError);
            WithOption(ConfigOptions.DisableOptimizationsValidator, true);
        }
    }
    
    [Config(typeof(ManualConfigNonOptimized))]
    public class SerializerBenchmark
    {
        private readonly IJsonSerializer _jsonSerializer;
        private string strings;

        public SerializerBenchmark()
        {
            _jsonSerializer = new JsonSerializer(new JsonSerializerOptionsProvider());
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "testobjectvk.txt");
            strings = File.ReadAllText(path);
            JsonConvert.DeserializeObject<VkResponse<IEnumerable<VkMessage>>>(strings);
        }
        
        [Benchmark]
        public VkResponse<IEnumerable<VkMessage>> TestTextJson()
        {
            
            return _jsonSerializer.Deserialize<VkResponse<IEnumerable<VkMessage>>>(strings);
        }

        [Benchmark]
        public VkResponse<IEnumerable<VkMessage>> TestNewtonsoft()
        {
            return JsonConvert.DeserializeObject<VkResponse<IEnumerable<VkMessage>>>(strings, new JsonSerializerSettings
            {
            });
        }
    }
}