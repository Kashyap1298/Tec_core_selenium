using System;
using System.IO;

namespace CustomerImportAutomation
{
    public static class CounterManager
    {
        private static readonly string CounterDirectory = "Counters";

        static CounterManager()
        {
            // Ensure the counters directory exists
            if (!Directory.Exists(CounterDirectory))
            {
                Directory.CreateDirectory(CounterDirectory);
            }
        }

        public static int GetAndIncrementCounter(string counterName, int defaultValue = 1, int increment = 1)
        {
            string filePath = Path.Combine(CounterDirectory, $"{counterName}.txt");
            
            try
            {
                int currentValue;
                
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath).Trim();
                    if (int.TryParse(content, out currentValue))
                    {
                        // Use existing value
                    }
                    else
                    {
                        currentValue = defaultValue;
                    }
                }
                else
                {
                    currentValue = defaultValue;
                }

                // Write incremented value for next run
                int nextValue = currentValue + increment;
                File.WriteAllText(filePath, nextValue.ToString());
                
                Console.WriteLine($"[DEBUG] Counter '{counterName}': Current={currentValue}, Next={nextValue}");
                
                return currentValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Error managing counter '{counterName}': {ex.Message}");
                return defaultValue;
            }
        }

        public static void ResetCounter(string counterName, int value = 1)
        {
            string filePath = Path.Combine(CounterDirectory, $"{counterName}.txt");
            
            try
            {
                File.WriteAllText(filePath, value.ToString());
                Console.WriteLine($"[INFO] Counter '{counterName}' reset to {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to reset counter '{counterName}': {ex.Message}");
            }
        }

        public static int GetCurrentValue(string counterName, int defaultValue = 1)
        {
            string filePath = Path.Combine(CounterDirectory, $"{counterName}.txt");
            
            try
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath).Trim();
                    if (int.TryParse(content, out int currentValue))
                    {
                        return currentValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Error reading counter '{counterName}': {ex.Message}");
            }
            
            return defaultValue;
        }
    }
}