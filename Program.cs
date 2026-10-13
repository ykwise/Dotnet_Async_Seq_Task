using System.Diagnostics;


int[] jobTimes = { 1000, 1500, 2000, 800, 1200 };


Console.WriteLine("=== Part 1: One-by-one vs all-together (no time limit) ===");


await RunAndTimeIt("One-by-one", token => DoJobsOneByOne(jobTimes, token), CancellationToken.None);


await RunAndTimeIt("All-together", token => DoJobsTogether(jobTimes, token), CancellationToken.None);


Console.WriteLine();
Console.WriteLine("=== Part 2: Same jobs, but with a 3 second time limit ===");


using (var alarm = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
{
    await RunAndTimeIt("One-by-one", token => DoJobsOneByOne(jobTimes, token), alarm.Token);
}

using (var alarm = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
{
    await RunAndTimeIt("All-together", token => DoJobsTogether(jobTimes, token), alarm.Token);
}


static async Task DoOneSlowJob(int jobNumber, int howLongMs, CancellationToken token)
{
    Console.WriteLine($"  Job {jobNumber} started (needs {howLongMs} ms)");

    
    await Task.Delay(howLongMs, token);

    Console.WriteLine($"  Job {jobNumber} finished");
}


static async Task DoJobsOneByOne(int[] jobTimes, CancellationToken token)
{
    for (int i = 0; i < jobTimes.Length; i++)
    {
        await DoOneSlowJob(i + 1, jobTimes[i], token);
    }
}


static async Task DoJobsTogether(int[] jobTimes, CancellationToken token)
{
    var runningJobs = jobTimes
        .Select((time, i) => DoOneSlowJob(i + 1, time, token))
        .ToList();

    await Task.WhenAll(runningJobs);
}


static async Task RunAndTimeIt(string name, Func<CancellationToken, Task> work, CancellationToken token)
{
    Console.WriteLine($"{name} run:");
    var stopwatch = Stopwatch.StartNew();

    try
    {
        await work(token);
        stopwatch.Stop();
        Console.WriteLine($"{name} finished in {stopwatch.ElapsedMilliseconds} ms\n");
    }
    catch (OperationCanceledException)
    {
        
        stopwatch.Stop();
        Console.WriteLine($"{name} was CANCELLED after {stopwatch.ElapsedMilliseconds} ms\n");
    }
}
