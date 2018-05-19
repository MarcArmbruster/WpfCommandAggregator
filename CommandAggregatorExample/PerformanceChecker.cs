using System;
using System.Diagnostics;

namespace CommandAggregatorExample
{
    /// <summary>
    /// Performance Checker class.
    /// </summary>   
    /// <remarks>
    ///   <para><b>History</b></para>
    ///   <list type="table">
    ///   <item>
    ///   <term><b>Author:</b></term>
    ///   <description>See Josh Smith's implementation!</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Date:</b></term>
    ///   <description>Dec/23/2014</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    internal class PerformanceChecker
    {
        /// <summary>
        /// The description for the checker instance.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// The stop watch.
        /// </summary>
        private readonly Stopwatch watch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceChecker"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public PerformanceChecker(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// Starts the checker.
        /// </summary>
        public void Start()
        {
            watch.Reset();
            watch.Start();
        }

        /// <summary>
        /// The watch reference.
        /// </summary>
        public Stopwatch StopWatch => this.watch;

        /// <summary>
        /// Stops the checker.
        /// </summary>
        public void Stop()
        {
            watch.Stop();
            Console.WriteLine(string.Format("--> {0} ran for {1} ms", this.description, watch.ElapsedMilliseconds));            
        }
    }
}
