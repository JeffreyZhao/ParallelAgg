namespace ParallelAgg.Benchmark {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using ParallelAgg.Aggregation;
    using ParallelAgg.Metadata;
    using ParallelAggregationService = ParallelAgg.Aggregation.Parallel.AggregationService;
    using SerialAggregationService = ParallelAgg.Aggregation.Serial.AggregationService;

    class Program
    {
        private static IEnumerable<int[]> GetKeys(int keyCount, int keySize, int[] current, int level) {
            for (var i = 0; i < keySize; i++) {
                current[level] = i;

                if (level == keyCount - 1) {
                    yield return (int[])current.Clone();
                }
                else {
                    foreach (var keys in GetKeys(keyCount, keySize, current, level + 1)) {
                        yield return keys;
                    }
                }
            }
        }

        private static IEnumerable<int[]> InfiniteKeys(int keyCount, int keySize) {
            while (true) {
                foreach (var keys in GetKeys(keyCount, keySize, new int[keyCount], 0)) {
                    yield return keys;
                }
            }
// ReSharper disable FunctionNeverReturns
        } 
// ReSharper restore FunctionNeverReturns

        private static AggregationConfig CreateAggregationConfig(EntityMetadata metadata) {
            var aggregators = new List<PropertyAggregatorConfig>();

            foreach (var property in metadata.Properties) {
                aggregators.Add(new SumPropertyAggregatorConfig(aggregators.Count, property));
                aggregators.Add(new AvgPropertyAggregatorConfig(aggregators.Count, property));
            }

            foreach (var valueProperty in metadata.Properties) {
                foreach (var weightProperty in metadata.Properties) {
                    if (valueProperty == weightProperty) continue;

                    aggregators.Add(new WgtAvgPropertyAggregatorConfig(aggregators.Count, valueProperty, weightProperty));
                }
            }

            return new AggregationConfig(metadata, aggregators);
        }

        private static void Dump(EntitySet set, EntityMetadata metadata, string filepath) {
            using (var writer = new StreamWriter(File.Create(filepath))) {
                writer.Write(String.Join("", Enumerable.Range(0, metadata.KeyCount).Select(i => "Key" + i + ",")));
                writer.WriteLine(String.Join(",", metadata.Properties.Select(p => p.Name)));

                foreach (var entity in set) {
// ReSharper disable AccessToForEachVariableInClosure
                    writer.Write(String.Join("", Enumerable.Range(0, metadata.KeyCount).Select(i => entity.GetKey(i) + ",")));
                    writer.WriteLine(String.Join(",", metadata.Properties.Select(p => entity.Get(p).ToString(""))));
// ReSharper restore AccessToForEachVariableInClosure
                }
            }
        }

        private static void Dump(TextWriter writer, IAggregationResult result, AggregationConfig config, int[] keys, int level) {
            for (var i = 0; i < keys.Length; i++) {
                if (i < level) {
                    writer.Write(keys[i] + ",");
                }
                else {
                    writer.Write("-,");
                }
            }

            writer.WriteLine(String.Join(",", config.Aggregators.Select(a => result.Get(a).ToString(""))));

            foreach (var groupKey in result.Keys.OrderBy(k => k)) {
                keys[level] = groupKey;
                Dump(writer, result.Get(groupKey), config, keys, level + 1);
            }
        }

        private static void Dump(IAggregationResult result, EntityMetadata metadata, AggregationConfig config, string filepath) {
            using (var writer = new StreamWriter(File.Create(filepath))) {
                writer.Write(String.Join("", Enumerable.Range(0, metadata.KeyCount).Select(i => "Key" + i + ",")));
                writer.WriteLine(String.Join(",", config.Aggregators.Select(a => "\"" + a.Name + "\"")));

                var keys = new int[metadata.KeyCount];
                Dump(writer, result, config, keys, 0);
            }
        }

        private static void PrintMessage(int total, double current) {
            Console.Write("\r{0:0.00} iterations ({1:0.00}%) processed.", current, current * 100 / total);
        }

        static void Main(string[] args)
        {
            if (args.Length < 6) {
                Console.WriteLine("Usage:\tBenchmark.exe <s or p> <key number> <key size> <property number> <entity number> <iteration> <seed>");
                Console.WriteLine("Sample:\tBenchmark.exe s 5 3 10 100000 100 231347");
                return;
            }

            var isParallel = args[0] == "p";
            var keyCount = Int32.Parse(args[1]);
            var keySize = Int32.Parse(args[2]);
            var propertyCount = Int32.Parse(args[3]);
            var entityCount = Int32.Parse(args[4]);
            var iteration = Int32.Parse(args[5]);
            var seed = args.Length < 7 ? DateTime.Now.Millisecond : Int32.Parse(args[6]);

            Console.WriteLine("Parallel: " + isParallel);
            Console.WriteLine("Key number: " + keyCount);
            Console.WriteLine("Key size: " + keySize);
            Console.WriteLine("Property number: " + propertyCount);
            Console.WriteLine("Entity number: " + entityCount);
            Console.WriteLine("Iteration: " + iteration);
            Console.WriteLine("Seed: " + seed);
            Console.WriteLine();

            var random = new Random(seed);

            var metadata = new EntityMetadata(keyCount, propertyCount);
            var config = CreateAggregationConfig(metadata);

            var set = new EntitySet();
            foreach (var keys in InfiniteKeys(keyCount, keySize).Take(entityCount)) {
                set.Add(metadata.CreateEntity(keys));
            }

            Console.Write("Preparing...");

            var watch = new Stopwatch();
            watch.Start();

            var service = isParallel ? (IAggregationService)new ParallelAggregationService() : new SerialAggregationService();
            var root = service.Aggregate(set, config, metadata);
            root.Start();

            Console.WriteLine("Done");

            PrintMessage(iteration, 0);

            for (var i = 0; i < iteration; i++) {
                var num = 0;

                foreach (var entity in set) {
                    foreach (var property in metadata.Properties) {
                        entity.Set(property, (decimal)(random.NextDouble() * 1000000));
                    }

                    if (++num % 1000 == 0) {
                        PrintMessage(iteration, i + num * 1.0 / entityCount);
                    }
                }

                PrintMessage(iteration, i + 1);
            }

            Console.WriteLine();
            Console.WriteLine("Waiting for completion.");
            
            while (root.Running) {
                Thread.Sleep(100);
            }

            Console.WriteLine("Time: " + watch.Elapsed);

            var postfix = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            Dump(set, metadata, "Source-" + postfix + ".csv");
            Dump(root.Result, metadata, config, "Result-" + postfix + ".csv");
        }
    }
}