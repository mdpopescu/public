using Microsoft.Extensions.Options;
using YouNews.Core.Briefing;
using YouNews.Core.Model;
using YouNews.Infrastructure.LanguageModels;

namespace YouNews.Infrastructure.Tests.LanguageModels;

[TestClass]
[TestCategory("Live")]
public sealed class BriefingLiveTests
{
    [TestInitialize]
    public void Initialize()
    {
        var options = new YouNewsOptions { LanguageModel = { BaseUrl = "http://192.168.0.176:8080/v1", Model = "gemma4-26b", MaxTokens = 16000, TimeoutMinutes = 15 } };
        var languageModel = new OpenAiCompatibleLanguageModel(new HttpClient(), Options.Create(options), TimeProvider.System);
        sut = new BriefingWriter(languageModel, TimeProvider.System);
    }

    [TestMethod]
    public async Task AnalysisOfFictionalLaunch()
    {
        var candidates = new[] { EARLIER_ENTRY, UNRELATED_ENTRY };

        var result = await sut.AnalyzeAsync(TITLE, CHANNEL, CreateTranscript(), candidates, CancellationToken.None);

        Console.WriteLine("HEADLINE: " + result.Headline);
        Console.WriteLine("TAGS: " + string.Join(", ", result.Tags));
        Console.WriteLine("FRAMES: " + string.Join(", ", result.FrameTimes));
        Console.WriteLine("RELATED: " + string.Join(", ", result.RelatedEntryIds));
        Console.WriteLine("QUERIES:\n" + string.Join("\n", result.SearchQueries));
        CollectionAssert.Contains(result.Tags.ToList(), EntryTags.LOCAL_MODELS);
        CollectionAssert.Contains(result.RelatedEntryIds.ToList(), EARLIER_ENTRY.Id);
        CollectionAssert.DoesNotContain(result.RelatedEntryIds.ToList(), UNRELATED_ENTRY.Id);
        Assert.IsNotEmpty(result.SearchQueries);
    }

    [TestMethod]
    public async Task SummaryOfFictionalLaunch()
    {
        var transcript = CreateTranscript();

        var result = await sut.SummarizeAsync(TITLE, transcript, CancellationToken.None);

        var words = result.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        Console.WriteLine($"SUMMARY ({words} words, transcript {transcript.WordCount} words):\n{result}");
        Assert.IsInRange(40, 160, words, "The target is 80 words: " + result);
        Assert.DoesNotContain("**", result, result);
    }

    [TestMethod]
    public async Task OpinionOfFictionalLaunch()
    {
        var request = new OpinionRequest(TITLE, CHANNEL, CreateTranscript(), SETUP, INTERESTS, [EARLIER_ENTRY], SEARCH_RESULTS);

        var result = await sut.WriteOpinionAsync(request, CancellationToken.None);

        Console.WriteLine("OPINION:\n" + result);
        var paragraphs = result.Split("\n\n", StringSplitOptions.RemoveEmptyEntries).Length;
        Assert.IsGreaterThanOrEqualTo(3, paragraphs, result);
        StringAssert.Contains(result, "5060", "The opinion must name the reader's hardware: " + result);
    }

    [TestMethod]
    public async Task OpinionWithoutSetupOrSources()
    {
        var request = new OpinionRequest(TITLE, CHANNEL, CreateTranscript(), null, INTERESTS, [], []);

        var result = await sut.WriteOpinionAsync(request, CancellationToken.None);

        Console.WriteLine("OPINION WITHOUT SOURCES:\n" + result);
        StringAssert.Contains(result, "unknown", StringComparison.OrdinalIgnoreCase, "It must say the setup is unknown: " + result);
    }

    //

    private const string TITLE = "This FREE Model DESTROYS GPT-5.5 On Your Gaming PC!!";
    private const string CHANNEL = "Local AI Lab";

    private const string TRANSCRIPT_TEXT = """
        What is going on, guys, welcome back to the channel. Today is honestly one of the craziest days I have covered in local AI, because a brand new model just dropped and it is called Halcyon 14B, from a startup called Meridian Compute. And when I say this thing is insane, I mean it. It has fourteen billion parameters, it is completely free, and according to the numbers I am about to show you it destroys GPT 5.5 on your own gaming PC. Before we get into it, a huge thank you to today's sponsor, Meridian Cloud, who gave me the GPU credits to make this video. If you do not have a big graphics card you can rent one through the link in the description and use my code for twenty percent off.

        Okay, so let me pull up the chart here. As you can see on this graph from the Meridian launch blog, Halcyon 14B scores ninety one percent on SWE-bench Verified. Ninety one. GPT 5.5 sits at eighty four on the same chart, and Qwen 3.8 27B is way down at seventy eight. So a fourteen billion parameter model is beating a frontier model on real software engineering tasks. That is the headline. Meridian says they did this with a new training technique they call recursive distillation, where the model basically teaches itself from its own reasoning traces. They say a paper is coming soon. I have not read it because it is not out yet, but the idea makes total sense to me and I think this could change everything.

        Now speed, because this is where it gets really wild. Meridian says that at their Q2 quantization, Halcyon runs at one hundred and forty tokens per second on an eight gigabyte graphics card. Eight gigabytes. And they claim there is zero quality loss at Q2 compared to the full model, because of a new thing they call adaptive ternary weights. The file is only about three and a half gigabytes, so you can see here on the Hugging Face page, it fits with a ton of room left. They also say it has a one million token context window on just twelve gigabytes of video memory, which, if you think about it, is more context than almost anyone is offering in the cloud.

        So I went and tested it myself. I rented a machine through Meridian Cloud, with an RTX 5090 in it, and I asked it to write a snake game in Python, and look at this, it did it on the first try. Then I asked it for a to-do app in React and that worked too. I asked it the classic question about how many R's are in strawberry and it got that right. So in my testing it is every bit as good as they say. I did not run the SWE-bench myself, because that takes a really long time, but honestly after seeing this demo I believe the numbers. Speed on the 5090 was about two hundred tokens per second, so right in line with what they are saying.

        Now, the license. It is open weights, you can download it right now, and it is free for you and me. There is a line in there about commercial use over a certain revenue, but I would not worry about that, that is for the big companies. Meridian raised eighty million dollars in September, by the way, so these are serious people. The CEO used to be at a big lab, and he told me on a call last week that a thirty billion parameter version is coming before the end of the year and that it will, quote, make paid APIs obsolete.

        So what does this mean for you? It means you can cancel your ChatGPT subscription. Honestly. If you have a gaming PC with eight gigs of video memory, you now have something better than GPT 5.5 sitting on your desk, for free, forever. And this also tells you something about the hardware market, because if models keep getting this efficient nobody is going to need those expensive sixteen and twenty four gigabyte cards any more. So I would hold off on buying a new graphics card right now, prices are definitely going to crash in the next couple of months.

        Let me know in the comments what you are going to build with Halcyon 14B. If you liked this video, hit like and subscribe, and I will see you in the next one. And remember, link to Meridian Cloud is down in the description, use my code for twenty percent off. Peace.
        """;

    private static readonly SetupSnapshot SETUP = new(
        [
            "agents-a1", "arch-router-q6-k", "gemma-3-12b-it-q4-k-m", "gemma4-12b-256k", "gemma4-26b", "glm-4.7-flash", "gpt-oss-20b-q4-k-m-128k",
            "muse-glimmer-30b-ud-iq3-m", "nemotron", "neohorse-1-9b-q8-0", "occamy-1-0-q4-k-m", "ornith-1-5-35b-a3b-q4-k-m", "qwen3-8-27b-ud-iq4-xs",
            "qwen3-8-9b-q8-0", "qwen3.6-moe", "qwen36-mtp", "recursivesi-flint-q4-k-m", "swift-qwen3-8-27b-iq4-xs", "tiel-coder-35b-a3b-mtp-ud-q4-k-xl", "whisper-large-v3-turbo",
        ],
        "NVIDIA GeForce RTX 5060 Ti, 16 GB; NVIDIA GeForce RTX 2060 SUPER, 8 GB; AMD Ryzen 9 3900X; 62 GB RAM",
        new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero),
        null);

    private static readonly string[] INTERESTS =
    [
        "Are there new local models better than what is on my server?",
        "New AI legislation or rumors of it",
        "RAM and video card prices",
        "AI research that matters, such as ternary models and dedicated inference cards",
    ];

    private static readonly EntryDigest EARLIER_ENTRY = new(
        812,
        new DateOnly(2026, 10, 14),
        "Meridian Compute announces Halcyon 14B and claims 91% on SWE-bench Verified",
        [EntryTags.LOCAL_MODELS, EntryTags.INDUSTRY],
        "Meridian Compute announced Halcyon 14B on its blog, claiming 91 percent on SWE-bench Verified and 140 tokens per second on an 8 GB card. The presenter read the launch post aloud and noted that weights were promised for later in the month. No third party had tested it at that point.");

    private static readonly EntryDigest UNRELATED_ENTRY = new(
        790,
        new DateOnly(2026, 10, 9),
        "DRAM contract prices rise 12 percent in the third quarter",
        [EntryTags.HARDWARE_MARKET],
        "Contract DRAM prices rose 12 percent quarter on quarter according to the presenter, who cited a market research note.");

    private static readonly WebResult[] SEARCH_RESULTS =
    [
        new(
            "Halcyon 14B tested: not close to the launch numbers : r/LocalLLaMA",
            "https://www.reddit.com/r/LocalLLaMA/comments/example/halcyon_14b_tested",
            "Ran the Q2 GGUF on a 3060 12GB and a 4060 8GB in llama.cpp. 38 t/s on the 4060, not 140. Meridian's number seems to come from a 5090 with a tiny prompt. Quality at Q2 is visibly worse than Q4, lots of broken imports in generated code. Q4 is 7.1 GB."),
        new(
            "Independent SWE-bench Verified leaderboard, October 2026",
            "https://example.org/swebench-leaderboard",
            "Community re-runs: Qwen 3.8 27B (Q4) 71.2 percent; Halcyon 14B (Q4) 58.4 percent, 12 of 500 tasks timed out; Gemma 4 26B 66.9 percent. Vendor-submitted scores are listed separately and not verified."),
        new(
            "Halcyon Community License 1.0 - Meridian Compute",
            "https://example.org/meridian/halcyon-license",
            "Free for individuals and for organizations with annual revenue below 1 million USD. Outputs may not be used to train other models. Meridian may terminate this license with 30 days notice."),
    ];

    private BriefingWriter sut = null!;

    private static Transcript CreateTranscript()
    {
        var segments = TRANSCRIPT_TEXT
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Chunk(12)
            .Select((words, index) => new TranscriptSegment(TimeSpan.FromSeconds(index * 4.8), string.Join(' ', words)))
            .ToList();

        return new Transcript(segments);
    }
}