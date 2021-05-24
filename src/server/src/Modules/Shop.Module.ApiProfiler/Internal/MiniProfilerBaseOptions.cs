using Shop.Module.ApiProfiler.Helpers;
using Shop.Module.ApiProfiler.Storage;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shop.Module.ApiProfiler.Internal
{
    /// <summary>
    /// Various configuration properties for MiniProfiler.
    /// </summary>
    public class MiniProfilerBaseOptions
    {
        /// <summary>
        /// Assembly version of this dank MiniProfiler.
        /// </summary>
        public static Version Version { get; } = typeof(MiniProfilerBaseOptions).GetTypeInfo().Assembly.GetName().Version;

        /// <summary>
        /// The hash to use for file cache breaking, this is automatically calculated.
        /// </summary>
        public virtual string VersionHash { get; set; } = typeof(MiniProfilerBaseOptions).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? Version.ToString();

        /// <summary>
        /// When <see cref="IAsyncProfilerProvider.Start(string, MiniProfilerBaseOptions)"/> is called, if the current request URL contains any items in this property,
        /// no profiler will be instantiated and no results will be displayed.
        /// Default value is { "/content/", "/scripts/", "/favicon.ico" }.
        /// </summary>
        public HashSet<string> IgnoredPaths { get; } = new HashSet<string> {
            "/content/",
            "/scripts/",
            "/favicon.ico"
        };

        /// <summary>
        /// The maximum number of unviewed profiler sessions (set this low cause we don't want to blow up headers)
        /// </summary>
        public int MaxUnviewedProfiles { get; set; } = 20;

        /// <summary>
        /// Any Timing step with a duration less than or equal to this will be hidden by default in the UI; defaults to 2.0 ms.
        /// </summary>
        public decimal TrivialDurationThresholdMilliseconds { get; set; } = 2.0M;

        /// <summary>
        /// Dictates if the "time with children" column is displayed by default, defaults to false.
        /// For a per-page override you can use .RenderIncludes(showTimeWithChildren: true/false)
        /// </summary>
        public bool PopupShowTimeWithChildren { get; set; } = false;

        /// <summary>
        /// Dictates if trivial timings are displayed by default, defaults to false.
        /// For a per-page override you can use .RenderIncludes(showTrivial: true/false)
        /// </summary>
        public bool PopupShowTrivial { get; set; } = false;

        /// <summary>
        /// Determines how many traces to show before removing the oldest; defaults to 15.
        /// For a per-page override you can use .RenderIncludes(maxTracesToShow: 10)
        /// </summary>
        public int PopupMaxTracesToShow { get; set; } = 15;

        /// <summary>
        /// Dictates on which side of the page the profiler popup button is displayed; defaults to left.
        /// For a per-page override you can use .RenderIncludes(position: RenderPosition.Left/Right)
        /// </summary>
        public RenderPosition PopupRenderPosition { get; set; } = RenderPosition.Left;

        /// <summary>
        /// Allows showing/hiding of popup results buttons via keyboard.
        /// </summary>
        public string PopupToggleKeyboardShortcut { get; set; } = "Alt+P";

        /// <summary>
        /// When true, results buttons will not initially be shown, requiring keyboard activation via <see cref="PopupToggleKeyboardShortcut"/>.
        /// </summary>
        public bool PopupStartHidden { get; set; } = false;

        /// <summary>
        /// Determines if min-max, clear, etc are rendered; defaults to false.
        /// For a per-page override you can use .RenderIncludes(showControls: true/false)
        /// </summary>
        public bool ShowControls { get; set; } = false;

        /// <summary>
        /// Custom timing ExecuteTypes to ignore as duplicates in the UI.
        /// </summary>
        public HashSet<string> IgnoredDuplicateExecuteTypes { get; } = new HashSet<string>()
        {
            "Open",
            "OpenAsync",
            "Close",
            "CloseAsync" // RelationalDiagnosticListener
        };

        /// <summary>
        /// Understands how to save and load MiniProfilers. Used for caching between when
        /// a profiling session ends and results can be fetched to the client, and for showing shared, full-page results.
        /// </summary>
        /// <remarks>
        /// The normal profiling session life-cycle is as follows:
        /// 1) request begins
        /// 2) profiler is started
        /// 3) normal page/controller/request execution
        /// 4) profiler is stopped
        /// 5) profiler is cached with <see cref="Storage"/>'s implementation of <see cref="IAsyncStorage.Save"/>
        /// 6) request ends
        /// 7) page is displayed and profiling results are AJAX-fetched down, pulling cached results from 
        ///    <see cref="Storage"/>'s implementation of <see cref="IAsyncStorage.Load"/>
        /// </remarks>
        public IAsyncStorage Storage { get; set; }

        /// <summary>
        /// The <see cref="IAsyncProfilerProvider"/> class that is used to run MiniProfiler
        /// </summary>
        /// <remarks>
        /// If not set explicitly, will default to <see cref="DefaultProfilerProvider"/>
        /// </remarks>
        public IAsyncProfilerProvider ProfilerProvider { get; set; } = new DefaultProfilerProvider();

        /// <summary>
        /// Allows switching out stopwatches for unit testing.
        /// </summary>
        public Func<IStopwatch> StopwatchProvider { get; set; } = StopwatchWrapper.StartNew;

        /// <summary>
        /// Starts a new MiniProfiler from the <see cref="ProfilerProvider"/>.
        /// Shortcut for Options.ProfilerProvider.Start.
        /// </summary>
        /// <param name="profilerName">
        /// Allows explicit naming of the new profiling session; when null, an appropriate default will be used, e.g. for
        /// a web request, the URL will be used for the overall session name.
        /// </param>
        public MiniProfiler StartProfiler(string profilerName = null) => ProfilerProvider.Start(profilerName, this);

        /// <summary>
        /// Called when passed to <see cref="MiniProfiler.Configure{T}(T)"/>.
        /// </summary>
        protected virtual void OnConfigure() { }

        internal void Configure() => OnConfigure();
    }
}
